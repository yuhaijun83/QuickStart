using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuickStart
{
    /// <summary>
    /// 一个 [Button_XX] 段在 ini 文件中的记录：段名、配置、是否可视。
    /// </summary>
    class IniSection
    {
        public string SectionName;
        public IniConfig Config;
        public bool Visible;
    }

    /// <summary>
    /// 按行改写 ini 中 [Button_XX] 段的键值，使文件顺序与界面顺序一致。
    /// 注释行、空行、未知行以及 [System_Info] 等非按钮段全部原样保留。
    /// </summary>
    class IniFileRewriter
    {
        private static readonly string[] BUTTON_KEYS = new string[]
        {
            "Button_Name",
            "Button_Visible",
            "Button_Enable",
            "Button_BackColor",
            "Button_TextColor",
            "Program_Name",
            "Program_Type",
            "Program_Param"
        };

        public static bool RewriteButtonSections(string iniFilePath, List<IniSection> sectionsInFileOrder)
        {
            if (!File.Exists(iniFilePath) || null == sectionsInFileOrder)
            {
                return false;
            }

            string tempPath = iniFilePath + ".tmp";
            try
            {
                byte[] raw = File.ReadAllBytes(iniFilePath);
                bool hasBom = raw.Length >= 3 && raw[0] == 0xEF && raw[1] == 0xBB && raw[2] == 0xBF;
                string content = hasBom ? Encoding.UTF8.GetString(raw, 3, raw.Length - 3) : Encoding.UTF8.GetString(raw);
                string newline = content.Contains("\r\n") ? "\r\n" : "\n";
                string[] lines = content.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

                // 段名 -> 该段应写入的配置（可视段已被重排为新顺序）
                Dictionary<string, IniConfig> byName = new Dictionary<string, IniConfig>(StringComparer.OrdinalIgnoreCase);
                foreach (IniSection section in sectionsInFileOrder)
                {
                    byName[section.SectionName] = section.Config;
                }

                List<string> output = new List<string>(lines.Length + 16);
                bool inButtonSection = false;
                IniConfig currentConfig = null;
                HashSet<string> seenKeys = null;
                HashSet<string> seenSections = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (string line in lines)
                {
                    string trimmed = line.Trim();

                    if (trimmed.Length > 2 && trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                    {
                        AppendMissingKeys(output, inButtonSection, currentConfig, seenKeys);

                        string sectionName = trimmed.Substring(1, trimmed.Length - 2).Trim();
                        inButtonSection = sectionName.StartsWith("Button_", StringComparison.OrdinalIgnoreCase);
                        if (inButtonSection)
                        {
                            seenSections.Add(sectionName);
                            byName.TryGetValue(sectionName, out currentConfig);
                            seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        }
                        else
                        {
                            currentConfig = null;
                            seenKeys = null;
                        }

                        output.Add(line);
                        continue;
                    }

                    if (inButtonSection && null != currentConfig)
                    {
                        int eq = trimmed.IndexOf('=');
                        if (eq > 0)
                        {
                            string key = trimmed.Substring(0, eq).Trim();
                            string canonicalKey = ToCanonicalKey(key);
                            if (null != canonicalKey)
                            {
                                seenKeys.Add(canonicalKey);
                                output.Add(canonicalKey + "=" + GetConfigValue(currentConfig, canonicalKey));
                                continue;
                            }
                        }
                        // 段内的注释行/空行/未知行原样保留
                    }

                    output.Add(line);
                }

                AppendMissingKeys(output, inButtonSection, currentConfig, seenKeys);

                // 文件里不存在的按钮段（如旧 ini 升级到更多按钮）追加到文件末尾
                foreach (IniSection section in sectionsInFileOrder)
                {
                    if (!seenSections.Contains(section.SectionName))
                    {
                        output.Add("");
                        output.Add("[" + section.SectionName + "]");
                        foreach (string key in BUTTON_KEYS)
                        {
                            output.Add(key + "=" + GetConfigValue(section.Config, key));
                        }
                    }
                }

                string result = string.Join(newline, output);
                File.WriteAllText(tempPath, result, hasBom ? new UTF8Encoding(true) : new UTF8Encoding(false));

                // 原子替换：替换前原文件始终存在，替换失败也不会丢失配置文件
                try
                {
                    File.Replace(tempPath, iniFilePath, null);
                }
                catch (Exception)
                {
                    // 个别文件系统（FAT/网络盘）不支持原子替换时退回覆盖写，原文件始终保留
                    File.Copy(tempPath, iniFilePath, true);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                try
                {
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private static string ToCanonicalKey(string key)
        {
            foreach (string buttonKey in BUTTON_KEYS)
            {
                if (string.Equals(buttonKey, key, StringComparison.OrdinalIgnoreCase))
                {
                    return buttonKey;
                }
            }
            return null;
        }

        private static void AppendMissingKeys(List<string> output, bool inButtonSection, IniConfig config, HashSet<string> seenKeys)
        {
            if (!inButtonSection || null == config || null == seenKeys)
            {
                return;
            }

            foreach (string key in BUTTON_KEYS)
            {
                if (!seenKeys.Contains(key))
                {
                    output.Add(key + "=" + GetConfigValue(config, key));
                }
            }
        }

        private static string GetConfigValue(IniConfig config, string key)
        {
            switch (key)
            {
                case "Button_Name": return config.Button_Name;
                case "Button_Visible": return config.Button_Visible;
                case "Button_Enable": return config.Button_Enable;
                case "Button_BackColor": return config.Button_BackColor;
                case "Button_TextColor": return config.Button_TextColor;
                case "Program_Name": return config.Program_Name;
                case "Program_Type": return config.Program_Type;
                case "Program_Param": return config.Program_Param;
                default: return "";
            }
        }
    }
}
