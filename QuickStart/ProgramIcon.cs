using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace QuickStart
{
    // 从 ini 配置的启动程序（exe/cmd/bat/msc/lnk 等）提取系统关联图标，供按钮左上角显示
    class ProgramIcon
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_LARGEICON = 0x000000000; // 取 32x32 大图标，绘制时按需缩小，画质更好

        // 把 ini 配置的 Program_Name 解析为实际存在的文件路径；解析不到返回 null
        // 支持：完整路径（含 %变量%）、裸命令名（shutdown/notepad 等，按 PATH 查找）
        public static string ResolveProgramPath(string programName)
        {
            if (string.IsNullOrEmpty(programName))
            {
                return null;
            }

            string expanded;
            try
            {
                expanded = Environment.ExpandEnvironmentVariables(programName).Trim().Trim('"');
            }
            catch (Exception)
            {
                return null;
            }

            if (expanded.Length == 0)
            {
                return null;
            }

            if (Path.IsPathRooted(expanded))
            {
                return File.Exists(expanded) ? expanded : null;
            }

            // 裸命令：在 PATH 各目录中按 name / name.exe / .cmd / .bat / .com 顺序查找
            string pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            string[] extensions = new string[] { "", ".exe", ".cmd", ".bat", ".com" };

            foreach (string dirRaw in pathEnv.Split(';'))
            {
                string dir = dirRaw.Trim().Trim('"');
                if (dir.Length == 0)
                {
                    continue;
                }

                foreach (string ext in extensions)
                {
                    string candidate;
                    try
                    {
                        candidate = Path.Combine(dir, expanded + ext);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }
                }
            }

            return null;
        }

        // 提取文件的系统关联图标句柄（仅 shell32 调用，不触碰 GDI+，可安全用于后台线程）；失败返回 IntPtr.Zero
        // 调用方用完后必须：IconHandleToBitmap 转换 + ReleaseIconHandle 释放
        public static IntPtr ExtractIconHandle(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return IntPtr.Zero;
            }

            try
            {
                SHFILEINFO fi = new SHFILEINFO();
                IntPtr res = SHGetFileInfo(filePath, 0, ref fi, (uint)Marshal.SizeOf(typeof(SHFILEINFO)), SHGFI_ICON | SHGFI_LARGEICON);

                if (res == IntPtr.Zero || fi.hIcon == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }

                return fi.hIcon;
            }
            catch (Exception)
            {
                return IntPtr.Zero;
            }
        }

        // 把 ExtractIconHandle 得到的句柄转换为独立 Bitmap（含 GDI+，必须在 UI 线程调用）；
        // 转换后仍需调用 ReleaseIconHandle 释放句柄
        public static Bitmap IconHandleToBitmap(IntPtr hIcon)
        {
            if (hIcon == IntPtr.Zero)
            {
                return null;
            }

            try
            {
                Bitmap bmp;
                using (Icon ico = Icon.FromHandle(hIcon))
                {
                    bmp = ico.ToBitmap();
                    GC.KeepAlive(ico);
                }
                return bmp;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // 释放 ExtractIconHandle 返回的 shell 图标句柄，防止 GDI 泄漏
        public static void ReleaseIconHandle(IntPtr hIcon)
        {
            if (hIcon != IntPtr.Zero)
            {
                DestroyIcon(hIcon);
            }
        }
    }
}
