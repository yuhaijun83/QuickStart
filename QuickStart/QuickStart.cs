using Microsoft.Win32;
using QuickStart.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QuickStart
{
    public partial class mainForm : Form
    {
        // 35, 48
        //private static string BUILD_VERSION_FW = "35";
        private static string BUILD_VERSION_FW = "48";

        private const int BUTTON_MAX_SIZE = 40;

        private const string INI_FILE_NAME = "QuickStart.ini";

        private const string System_Info = "System_Info";
        private const string MainForm_Opacity = "MainForm_Opacity";
        private const string MainForm_Language = "MainForm_Language";
        private const string MainForm_Timer = "MainForm_Timer";
        private const string MainForm_AutoRun = "MainForm_AutoRun";

        private const string Button_Name = "Button_Name";
        private const string Button_Visible = "Button_Visible";
        private const string Button_Enable = "Button_Enable";
        private const string Button_BackColor = "Button_BackColor";
        private const string Button_TextColor = "Button_TextColor";
        private const string Program_Name = "Program_Name";
        private const string Program_Type = "Program_Type";
        private const string Program_Param = "Program_Param";

        private const int WM_SYSCOMMAND = 0x112;
        private const int SC_CLOSE = 0xF060;
        private const int SC_MINIMIZE = 0xF020;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_NORMAL = 0xF120;

        private const string Process_RegItemName = "QuickStart";
        //private const string Process_HandlerName_35 = "QuickStart_FW35";
        //private const string Process_HandlerName_48 = "QuickStart_FW48";

        private int iStopMode = 0; // 0:NO 1:X 2:Y 3:T
        private int iStopMode_T = 2;
        private int iStopMode_XY = 5;

        private static string strCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private string strIniFilePath = strCurrentDirectory + "\\" + INI_FILE_NAME;
        private string win_Language = MultilingualConfig.Language_en_US;

        private static List<IniConfig> lstConfig = new List<IniConfig>();

        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            this.toCheckIsNotExistIniFile();
            this.InitLanguage();
            this.toCheckIsNotExistCurrentProcess();
            this.InitOpacity();
            this.InitConfig();
            this.InitForm();
            this.InitButton();
            this.InitTimer();
            this.InitAutoRun();
        }

        private void button_01_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[0]);
        }

        private void button_02_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[1]);
        }

        private void button_03_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[2]);
        }

        private void button_04_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[3]);
        }

        private void button_05_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[4]);
        }

        private void button_06_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[5]);
        }

        private void button_07_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[6]);
        }

        private void button_08_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[7]);
        }

        private void button_09_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[8]);
        }

        private void button_10_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[9]);
        }

        private void button_11_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[10]);
        }

        private void button_12_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[11]);
        }

        private void button_13_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[12]);
        }

        private void button_14_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[13]);
        }

        private void button_15_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[14]);
        }

        private void button_16_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[15]);
        }

        private void button_17_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[16]);
        }

        private void button_18_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[17]);
        }

        private void button_19_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[18]);
        }

        private void button_20_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[19]);
        }

        private void button_21_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[20]);
        }

        private void button_22_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[21]);
        }

        private void button_23_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[22]);
        }

        private void button_24_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[23]);
        }

        private void button_25_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[24]);
        }

        private void button_26_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[25]);
        }

        private void button_27_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[26]);
        }

        private void button_28_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[27]);
        }

        private void button_29_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[28]);
        }

        private void button_30_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[29]);
        }

        private void button_31_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[30]);
        }

        private void button_32_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[31]);
        }

        private void button_33_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[32]);
        }

        private void button_34_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[33]);
        }

        private void button_35_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[34]);
        }

        private void button_36_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[35]);
        }

        private void button_37_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[36]);
        }

        private void button_38_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[37]);
        }

        private void button_39_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[38]);
        }

        private void button_40_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[39]);
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string strTitle = "【Warn】";
            string strQuestion = "Do you really want to exit the program?!";
            switch (this.win_Language)
            {
                case MultilingualConfig.Language_zh_CN:
                    strTitle = "【警告】";
                    strQuestion = "你丫真的要退出程序吗？！";
                    break;

                case MultilingualConfig.Language_zh_TW:
                    strTitle = "【警告】";
                    strQuestion = "你丫真的要退出程序嗎？！";
                    break;

                case MultilingualConfig.Language_en_US:
                    strTitle = "【Warn】";
                    strQuestion = "Do you really want to exit the program?!";
                    break;

                case MultilingualConfig.Language_ja_JP:
                    strTitle = "【警告】";
                    strQuestion = "本当にプログラムを終了しますか？！";
                    break;

                case MultilingualConfig.Language_ko_KR:
                    strTitle = "【경고하다】";
                    strQuestion = "프로그램을 종료하시겠습니까？！";
                    break;

                default:
                    break;
            }

            DialogResult dialogResult =
                MessageBox.Show(strQuestion, strTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dialogResult == DialogResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            string strTitle = "【Warn】";
            string strQuestion = "Do you really want to exit the program?!";
            switch (this.win_Language)
            {
                case MultilingualConfig.Language_zh_CN:
                    strTitle = "【警告】";
                    strQuestion = "你丫真的要退出程序吗？！";
                    break;

                case MultilingualConfig.Language_zh_TW:
                    strTitle = "【警告】";
                    strQuestion = "你丫真的要退出程序嗎？！";
                    break;

                case MultilingualConfig.Language_en_US:
                    strTitle = "【Warn】";
                    strQuestion = "Do you really want to exit the program?!";
                    break;

                case MultilingualConfig.Language_ja_JP:
                    strTitle = "【警告】";
                    strQuestion = "本当にプログラムを終了しますか？！";
                    break;

                case MultilingualConfig.Language_ko_KR:
                    strTitle = "【경고하다】";
                    strQuestion = "프로그램을 종료하시겠습니까？！";
                    break;

                default:
                    break;
            }

            DialogResult dialogResult =
                MessageBox.Show(strQuestion, strTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dialogResult == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        private void toolStripMenuItem_About_Click(object sender, EventArgs e)
        {
            aboutForm objAboutForm = new aboutForm(this.win_Language);
            objAboutForm.ShowDialog();
        }

        private void mainForm_Move(object sender, EventArgs e)
        {
            this.iStopMode = 0;
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            this.toSetStopMode();
        }

        private void toolStripMenuItem_Display_Click(object sender, EventArgs e)
        {
            this.toShowMainForm();
        }

        private void notifyIconMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.toShowMainForm();
        }

        private void toolStripMenuItem_Opacity_10_Click(object sender, EventArgs e)
        {
            this.toSetOpacity("10");
        }

        private void toolStripMenuItem_Opacity_20_Click(object sender, EventArgs e)
        {
            this.toSetOpacity("20");
        }

        private void toolStripMenuItem_Opacity_30_Click(object sender, EventArgs e)
        {
            this.toSetOpacity("30");
        }

        private void toolStripMenuItem_Opacity_40_Click(object sender, EventArgs e)
        {
            this.toSetOpacity("40");
        }

        private void toolStripMenuItem_Opacity_50_Click(object sender, EventArgs e)
        {
            this.toSetOpacity("50");
        }

        private void toolStripMenuItem_Opacity_60_Click(object sender, EventArgs e)
        {
            this.toSetOpacity("60");
        }

        private void toolStripMenuItem_Opacity_70_Click(object sender, EventArgs e)
        {
            this.toSetOpacity("70");
        }

        private void toolStripMenuItem_Opacity_80_Click(object sender, EventArgs e)
        {
            this.toSetOpacity("80");
        }

        private void toolStripMenuItem_Opacity_90_Click(object sender, EventArgs e)
        {
            this.toSetOpacity("90");
        }

        private void toolStripMenuItem_Opacity_100_Click(object sender, EventArgs e)
        {
            this.toSetOpacity("100");
        }

        private void toolStripMenuItem_LanguageChineseSimplified_Click(object sender, EventArgs e)
        {
            this.toolStripMenuItem_LanguageChineseSimplified.Checked = true;
            this.toolStripMenuItem_LanguageChineseTraditional.Checked = false;
            this.toolStripMenuItem_LanguageEnglish.Checked = false;
            this.toolStripMenuItem_LanguageJapanese.Checked = false;
            this.toolStripMenuItem_LanguageKorean.Checked = false;

            this.win_Language = MultilingualConfig.Language_zh_CN;
            OperateIniFile.WriteIniData(System_Info, MainForm_Language, this.win_Language, strIniFilePath);
            this.setMainFormLanguage();
        }

        private void toolStripMenuItem_LanguageChineseTraditional_Click(object sender, EventArgs e)
        {
            this.toolStripMenuItem_LanguageChineseSimplified.Checked = false;
            this.toolStripMenuItem_LanguageChineseTraditional.Checked = true;
            this.toolStripMenuItem_LanguageEnglish.Checked = false;
            this.toolStripMenuItem_LanguageJapanese.Checked = false;
            this.toolStripMenuItem_LanguageKorean.Checked = false;

            this.win_Language = MultilingualConfig.Language_zh_TW;
            OperateIniFile.WriteIniData(System_Info, MainForm_Language, this.win_Language, strIniFilePath);
            this.setMainFormLanguage();
        }

        private void toolStripMenuItem_LanguageEnglish_Click(object sender, EventArgs e)
        {
            this.toolStripMenuItem_LanguageChineseSimplified.Checked = false;
            this.toolStripMenuItem_LanguageChineseTraditional.Checked = false;
            this.toolStripMenuItem_LanguageEnglish.Checked = true;
            this.toolStripMenuItem_LanguageJapanese.Checked = false;
            this.toolStripMenuItem_LanguageKorean.Checked = false;

            this.win_Language = MultilingualConfig.Language_en_US;
            OperateIniFile.WriteIniData(System_Info, MainForm_Language, this.win_Language, strIniFilePath);
            this.setMainFormLanguage();
        }

        private void toolStripMenuItem_LanguageJapanese_Click(object sender, EventArgs e)
        {
            this.toolStripMenuItem_LanguageChineseSimplified.Checked = false;
            this.toolStripMenuItem_LanguageChineseTraditional.Checked = false;
            this.toolStripMenuItem_LanguageEnglish.Checked = false;
            this.toolStripMenuItem_LanguageJapanese.Checked = true;
            this.toolStripMenuItem_LanguageKorean.Checked = false;

            this.win_Language = MultilingualConfig.Language_ja_JP;
            OperateIniFile.WriteIniData(System_Info, MainForm_Language, this.win_Language, strIniFilePath);
            this.setMainFormLanguage();
        }

        private void toolStripMenuItem_LanguageKorean_Click(object sender, EventArgs e)
        {
            this.toolStripMenuItem_LanguageChineseSimplified.Checked = false;
            this.toolStripMenuItem_LanguageChineseTraditional.Checked = false;
            this.toolStripMenuItem_LanguageEnglish.Checked = false;
            this.toolStripMenuItem_LanguageJapanese.Checked = false;
            this.toolStripMenuItem_LanguageKorean.Checked = true;

            this.win_Language = MultilingualConfig.Language_ko_KR;
            OperateIniFile.WriteIniData(System_Info, MainForm_Language, this.win_Language, strIniFilePath);
            this.setMainFormLanguage();
        }

        private void toolStripMenuItem_Timer_On_Click(object sender, EventArgs e)
        {
            this.toolStripMenuItem_Timer_On.Checked = true;
            this.toolStripMenuItem_Timer_Off.Checked = false;
            this.timerMain.Enabled = true;
            OperateIniFile.WriteIniData(System_Info, MainForm_Timer, "On", strIniFilePath);
        }

        private void toolStripMenuItem_Timer_Off_Click(object sender, EventArgs e)
        {
            this.toolStripMenuItem_Timer_On.Checked = false;
            this.toolStripMenuItem_Timer_Off.Checked = true;
            this.timerMain.Enabled = false;
            OperateIniFile.WriteIniData(System_Info, MainForm_Timer, "Off", strIniFilePath);
        }
        private void toolStripMenuItem_AutoRun_On_Click(object sender, EventArgs e)
        {
            this.toolStripMenuItem_AutoRun_On.Checked = true;
            this.toolStripMenuItem_AutoRun_Off.Checked = false;
            OperateIniFile.WriteIniData(System_Info, MainForm_AutoRun, "On", strIniFilePath);
            this.toSetAutoRun(true);
        }

        private void toolStripMenuItem_AutoRun_Off_Click(object sender, EventArgs e)
        {
            this.toolStripMenuItem_AutoRun_On.Checked = false;
            this.toolStripMenuItem_AutoRun_Off.Checked = true;
            OperateIniFile.WriteIniData(System_Info, MainForm_AutoRun, "Off", strIniFilePath);
            this.toSetAutoRun(false);
        }

        #region Me

        private void toCheckIsNotExistIniFile()
        {
            if (!File.Exists(strIniFilePath))
            {
                string strTitle = "【Error】";
                string strMessage = "File is Not Found! \n[ " + strIniFilePath + " ]";
                
                MessageBox.Show(strMessage, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(0);
            }
        }

        private void toCheckIsNotExistCurrentProcess()
        {
            int iCount = 0;
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName == Process_RegItemName)
                {
                    iCount = iCount + 1;
                }
            }

            if (iCount > 1)
            {
                string strTitle = "【Warn】";
                string strMessage = "The Program is runing...";
                switch (this.win_Language)
                {
                    case MultilingualConfig.Language_zh_CN:
                        strTitle = "【警告】";
                        strMessage = "该程序已经在运行中...";
                        break;

                    case MultilingualConfig.Language_zh_TW:
                        strTitle = "【警告】";
                        strMessage = "該程序已經在運行中...";
                        break;

                    case MultilingualConfig.Language_en_US:
                        strTitle = "【Warn】";
                        strMessage = "The Program is runing...";
                        break;

                    case MultilingualConfig.Language_ja_JP:
                        strTitle = "【警告】";
                        strMessage = "当プログラムは実行中...";
                        break;

                    case MultilingualConfig.Language_ko_KR:
                        strTitle = "【경고하다】";
                        strMessage = "프로그램이 실행 중입니다...";
                        break;

                    default:
                        break;
                }

                MessageBox.Show(strMessage, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                System.Environment.Exit(0);
            }
        }

        private void InitLanguage()
        {
            this.win_Language = OperateIniFile.ReadIniData(System_Info, MainForm_Language, "", strIniFilePath);

            if (!MultilingualConfig.Language_zh_CN.Equals(this.win_Language) && !MultilingualConfig.Language_zh_TW.Equals(this.win_Language)
                && !MultilingualConfig.Language_en_US.Equals(this.win_Language) && !MultilingualConfig.Language_ja_JP.Equals(this.win_Language)
                && !MultilingualConfig.Language_ko_KR.Equals(this.win_Language))
            {
                this.win_Language = MultilingualConfig.Language_en_US;
                OperateIniFile.WriteIniData(System_Info, MainForm_Language, this.win_Language, strIniFilePath);
            }
        }

        private void InitOpacity()
        {
            string strMainForm_Opacity = OperateIniFile.ReadIniData(System_Info, MainForm_Opacity, "", strIniFilePath);
            double dMainForm_Opacity = 1D;

            if ("".Equals(strMainForm_Opacity))
            {
                this.toSetOpacity("100");
            } 
            else
            {
                try
                {
                    dMainForm_Opacity = double.Parse(strMainForm_Opacity) / 100;
                    if (dMainForm_Opacity < 0D)
                    {

                        this.toSetOpacity("10");
                    }
                    else if (dMainForm_Opacity > 1D)
                    {

                        this.toSetOpacity("100");
                    } else
                    {
                        this.Opacity = dMainForm_Opacity;
                    }
                }
                catch
                {
                    this.toSetOpacity("100");
                }
            }

            if (dMainForm_Opacity <= 0.1D)
            {
                this.toolStripMenuItem_Opacity_10.Checked = true;
            }
            else if (dMainForm_Opacity <= 0.2D)
            {
                this.toolStripMenuItem_Opacity_20.Checked = true;
            }
            else if (dMainForm_Opacity <= 0.3D)
            {
                this.toolStripMenuItem_Opacity_30.Checked = true;
            }
            else if (dMainForm_Opacity <= 0.4D)
            {
                this.toolStripMenuItem_Opacity_40.Checked = true;
            }
            else if (dMainForm_Opacity <= 0.5D)
            {
                this.toolStripMenuItem_Opacity_50.Checked = true;
            }
            else if (dMainForm_Opacity <= 0.6D)
            {
                this.toolStripMenuItem_Opacity_60.Checked = true;
            }
            else if (dMainForm_Opacity <= 0.7D)
            {
                this.toolStripMenuItem_Opacity_70.Checked = true;
            }
            else if (dMainForm_Opacity <= 0.8D)
            {
                this.toolStripMenuItem_Opacity_80.Checked = true;
            }
            else if (dMainForm_Opacity <= 0.9D)
            {
                this.toolStripMenuItem_Opacity_90.Checked = true;
            }
            else if (dMainForm_Opacity <= 1D)
            {
                this.toolStripMenuItem_Opacity_100.Checked = true;
            }
        }

        private void InitTimer()
        {
            string strMainForm_Timer = OperateIniFile.ReadIniData(System_Info, MainForm_Timer, "", strIniFilePath);
            if (strMainForm_Timer == "")
            {
                strMainForm_Timer = "On";
                OperateIniFile.WriteIniData(System_Info, MainForm_Timer, strMainForm_Timer, strIniFilePath);
            }

            switch (strMainForm_Timer.ToUpper())
            {
                case "ON":
                    this.toolStripMenuItem_Timer_On.Checked = true;
                    this.toolStripMenuItem_Timer_Off.Checked = false;
                    this.timerMain.Enabled = true;
                    break;

                case "OFF":
                    this.toolStripMenuItem_Timer_On.Checked = false;
                    this.toolStripMenuItem_Timer_Off.Checked = true;
                    this.timerMain.Enabled = false;
                    break;

                default:
                    break;
            }
        }

        private void InitAutoRun()
        {
            string strMainForm_AutoRun = OperateIniFile.ReadIniData(System_Info, MainForm_AutoRun, "", strIniFilePath);
            if (strMainForm_AutoRun == "")
            {
                strMainForm_AutoRun = "On";
                OperateIniFile.WriteIniData(System_Info, MainForm_AutoRun, strMainForm_AutoRun, strIniFilePath);
            }

            switch (strMainForm_AutoRun.ToUpper())
            {
                case "ON":
                    this.toolStripMenuItem_AutoRun_On.Checked = true;
                    this.toolStripMenuItem_AutoRun_Off.Checked = false;
                    this.toSetAutoRun(true);
                    break;

                case "OFF":
                    this.toolStripMenuItem_AutoRun_On.Checked = false;
                    this.toolStripMenuItem_AutoRun_Off.Checked = true;
                    this.toSetAutoRun(false);
                    break;

                default:
                    break;
            }
        }

        private void InitConfig()
        {
            string strSection = null;
            IniConfig ini = null;

            StreamReader streamReader = new StreamReader(strIniFilePath, Encoding.UTF8);
            string content;
            while ((content = streamReader.ReadLine()) != null)
            {
                if (content.StartsWith("[Button_"))
                {
                    ini = new IniConfig();
                    strSection = content.Replace("[", "").Replace("]", "");

                    ini.Button_Name = OperateIniFile.ReadIniData(strSection, Button_Name, "", strIniFilePath, "utf-8", 1024);
                    ini.Button_Visible = OperateIniFile.ReadIniData(strSection, Button_Visible, "0", strIniFilePath);
                    if (!"1".Equals(ini.Button_Visible)) { continue; }
                    ini.Button_Enable = OperateIniFile.ReadIniData(strSection, Button_Enable, "0", strIniFilePath);
                    ini.Button_BackColor = OperateIniFile.ReadIniData(strSection, Button_BackColor, "", strIniFilePath);
                    ini.Button_TextColor = OperateIniFile.ReadIniData(strSection, Button_TextColor, "", strIniFilePath);
                    ini.Program_Name = OperateIniFile.ReadIniData(strSection, Program_Name, "", strIniFilePath, "utf-8", 1024);
                    ini.Program_Type = OperateIniFile.ReadIniData(strSection, Program_Type, "exe", strIniFilePath).ToLower();
                    if ("".Equals(ini.Program_Type)) { ini.Program_Type = "exe"; }
                    ini.Program_Param = OperateIniFile.ReadIniData(strSection, Program_Param, "", strIniFilePath, "utf-8", 1024);

                    lstConfig.Add(ini);
                }
            }

            content = null;
            try
            {
                if (null != streamReader)
                {
                    streamReader.Close();
                }
            } catch
            {
                ;
            }

            streamReader = null;
            strSection = null; 
            ini = null;
        }

        private void InitForm()
        {
            this.Text = "QuickStart  " + Properties.Resources.Program_Build_Version_Text;

            int isize = lstConfig.Count();

            if (BUILD_VERSION_FW.Equals("35"))
            {
                int iWidth = 769;
                if (isize <= 8)
                {
                    this.Size = new Size(iWidth, 104);
                }
                else if (9 <= isize && isize <= 16)
                {
                    this.Size = new Size(iWidth, 174);
                }
                else if (17 <= isize && isize <= 24)
                {
                    this.Size = new Size(iWidth, 243);
                }
                else if (25 <= isize && isize <= 32)
                {
                    this.Size = new Size(iWidth, 312);
                }
                else if (33 <= isize && isize <= 40)
                {
                    this.Size = new Size(iWidth, 381);
                }
                else
                {
                    this.Size = new Size(iWidth, 104);
                }

            } else if (BUILD_VERSION_FW.Equals("48"))
            {
                int iWidth = 779;

                if (isize <= 8)
                {
                    this.Size = new Size(iWidth, 115);
                }
                else if (9 <= isize && isize <= 16)
                {
                    this.Size = new Size(iWidth, 183);
                }
                else if (17 <= isize && isize <= 24)
                {
                    this.Size = new Size(iWidth, 252);
                }
                else if (25 <= isize && isize <= 32)
                {
                    this.Size = new Size(iWidth, 321);
                }
                else if (33 <= isize && isize <= 40)
                {
                    this.Size = new Size(iWidth, 389);
                }
                else
                {
                    this.Size = new Size(iWidth, 115);
                }
            }

            switch (this.win_Language)
            {
                case MultilingualConfig.Language_zh_CN:
                    this.toolStripMenuItem_LanguageChineseSimplified.Checked = true;
                    break;

                case MultilingualConfig.Language_zh_TW:
                    this.toolStripMenuItem_LanguageChineseTraditional.Checked = true;
                    break;

                case MultilingualConfig.Language_en_US:
                    this.toolStripMenuItem_LanguageEnglish.Checked = true;
                    break;

                case MultilingualConfig.Language_ja_JP:
                    this.toolStripMenuItem_LanguageJapanese.Checked = true;
                    break;

                case MultilingualConfig.Language_ko_KR:
                    this.toolStripMenuItem_LanguageKorean.Checked = true;
                    break;

                default:
                    break;
            }

            this.setMainFormLanguage();
            this.toSetMainFormLocation();
        }

        private void InitButton()
        {
            for (int i = 1; i <= lstConfig.Count(); i++)
            {
                switch (i)
                {
                    case 1:
                        this.button_01.Text = lstConfig[i - 1].Button_Name;
                        this.button_01.Visible = true;
                        this.button_01.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_01.BackColor =  Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_01.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 2:
                        this.button_02.Text = lstConfig[i - 1].Button_Name;
                        this.button_02.Visible = true;
                        this.button_02.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_02.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_02.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 3:
                        this.button_03.Text = lstConfig[i - 1].Button_Name;
                        this.button_03.Visible = true;
                        this.button_03.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_03.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_03.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 4:
                        this.button_04.Text = lstConfig[i - 1].Button_Name;
                        this.button_04.Visible = true;
                        this.button_04.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_04.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_04.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 5:
                        this.button_05.Text = lstConfig[i - 1].Button_Name;
                        this.button_05.Visible = true;
                        this.button_05.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_05.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_05.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 6:
                        this.button_06.Text = lstConfig[i - 1].Button_Name;
                        this.button_06.Visible = true;
                        this.button_06.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_06.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_06.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 7:
                        this.button_07.Text = lstConfig[i - 1].Button_Name;
                        this.button_07.Visible = true;
                        this.button_07.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_07.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_07.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 8:
                        this.button_08.Text = lstConfig[i - 1].Button_Name;
                        this.button_08.Visible = true;
                        this.button_08.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_08.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_08.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;

                    case 9:
                        this.button_09.Text = lstConfig[i - 1].Button_Name;
                        this.button_09.Visible = true;
                        this.button_09.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_09.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_09.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 10:
                        this.button_10.Text = lstConfig[i - 1].Button_Name;
                        this.button_10.Visible = true;
                        this.button_10.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_10.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_10.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 11:
                        this.button_11.Text = lstConfig[i - 1].Button_Name;
                        this.button_11.Visible = true;
                        this.button_11.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_11.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_11.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 12:
                        this.button_12.Text = lstConfig[i - 1].Button_Name;
                        this.button_12.Visible = true;
                        this.button_12.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_12.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_12.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 13:
                        this.button_13.Text = lstConfig[i - 1].Button_Name;
                        this.button_13.Visible = true;
                        this.button_13.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_13.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_13.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 14:
                        this.button_14.Text = lstConfig[i - 1].Button_Name;
                        this.button_14.Visible = true;
                        this.button_14.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_14.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_14.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 15:
                        this.button_15.Text = lstConfig[i - 1].Button_Name;
                        this.button_15.Visible = true;
                        this.button_15.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_15.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_15.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 16:
                        this.button_16.Text = lstConfig[i - 1].Button_Name;
                        this.button_16.Visible = true;
                        this.button_16.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_16.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_16.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;

                    case 17:
                        this.button_17.Text = lstConfig[i - 1].Button_Name;
                        this.button_17.Visible = true;
                        this.button_17.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_17.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_17.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 18:
                        this.button_18.Text = lstConfig[i - 1].Button_Name;
                        this.button_18.Visible = true;
                        this.button_18.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_18.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_18.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 19:
                        this.button_19.Text = lstConfig[i - 1].Button_Name;
                        this.button_19.Visible = true;
                        this.button_19.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_19.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_19.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 20:
                        this.button_20.Text = lstConfig[i - 1].Button_Name;
                        this.button_20.Visible = true;
                        this.button_20.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_20.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_20.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 21:
                        this.button_21.Text = lstConfig[i - 1].Button_Name;
                        this.button_21.Visible = true;
                        this.button_21.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_21.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_21.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 22:
                        this.button_22.Text = lstConfig[i - 1].Button_Name;
                        this.button_22.Visible = true;
                        this.button_22.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_22.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_22.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 23:
                        this.button_23.Text = lstConfig[i - 1].Button_Name;
                        this.button_23.Visible = true;
                        this.button_23.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_23.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_23.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 24:
                        this.button_24.Text = lstConfig[i - 1].Button_Name;
                        this.button_24.Visible = true;
                        this.button_24.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_24.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_24.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;

                    case 25:
                        this.button_25.Text = lstConfig[i - 1].Button_Name;
                        this.button_25.Visible = true;
                        this.button_25.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_25.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_25.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 26:
                        this.button_26.Text = lstConfig[i - 1].Button_Name;
                        this.button_26.Visible = true;
                        this.button_26.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_26.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_26.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 27:
                        this.button_27.Text = lstConfig[i - 1].Button_Name;
                        this.button_27.Visible = true;
                        this.button_27.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_27.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_27.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 28:
                        this.button_28.Text = lstConfig[i - 1].Button_Name;
                        this.button_28.Visible = true;
                        this.button_28.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_28.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_28.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 29:
                        this.button_29.Text = lstConfig[i - 1].Button_Name;
                        this.button_29.Visible = true;
                        this.button_29.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_29.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_29.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 30:
                        this.button_30.Text = lstConfig[i - 1].Button_Name;
                        this.button_30.Visible = true;
                        this.button_30.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_30.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_30.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 31:
                        this.button_31.Text = lstConfig[i - 1].Button_Name;
                        this.button_31.Visible = true;
                        this.button_31.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_31.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_31.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 32:
                        this.button_32.Text = lstConfig[i - 1].Button_Name;
                        this.button_32.Visible = true;
                        this.button_32.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_32.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_32.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;

                    case 33:
                        this.button_33.Text = lstConfig[i - 1].Button_Name;
                        this.button_33.Visible = true;
                        this.button_33.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_33.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_33.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 34:
                        this.button_34.Text = lstConfig[i - 1].Button_Name;
                        this.button_34.Visible = true;
                        this.button_34.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_34.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_34.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 35:
                        this.button_35.Text = lstConfig[i - 1].Button_Name;
                        this.button_35.Visible = true;
                        this.button_35.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_35.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_35.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 36:
                        this.button_36.Text = lstConfig[i - 1].Button_Name;
                        this.button_36.Visible = true;
                        this.button_36.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_36.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_36.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 37:
                        this.button_37.Text = lstConfig[i - 1].Button_Name;
                        this.button_37.Visible = true;
                        this.button_37.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_37.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_37.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 38:
                        this.button_38.Text = lstConfig[i - 1].Button_Name;
                        this.button_38.Visible = true;
                        this.button_38.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_38.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_38.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 39:
                        this.button_39.Text = lstConfig[i - 1].Button_Name;
                        this.button_39.Visible = true;
                        this.button_39.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_39.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_39.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;
                    case 40:
                        this.button_40.Text = lstConfig[i - 1].Button_Name;
                        this.button_40.Visible = true;
                        this.button_40.Enabled = lstConfig[i - 1].Button_Enable == "1" ? true : false;
                        if (lstConfig[i - 1].Button_BackColor != "")
                        {
                            this.button_40.BackColor = Color.FromName(lstConfig[i - 1].Button_BackColor);
                        }
                        if (lstConfig[i - 1].Button_TextColor != "")
                        {
                            this.button_40.ForeColor = Color.FromName(lstConfig[i - 1].Button_TextColor);
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void button_common(IniConfig ini)
        {
            string strTitle = "【Error】";
            string strQuestion = "The Program Startup Type Configuration Error！";
            switch (this.win_Language)
            {
                case MultilingualConfig.Language_zh_CN:
                    strTitle = "【错误】";
                    strQuestion = "程序启动类型配置错误！";
                    break;

                case MultilingualConfig.Language_zh_TW:
                    strTitle = "【錯誤】";
                    strQuestion = "程序啓動類型配置錯誤！";
                    break;

                case MultilingualConfig.Language_en_US:
                    strTitle = "【Error】";
                    strQuestion = "The Program Startup Type Configuration Error！";
                    break;

                case MultilingualConfig.Language_ja_JP:
                    strTitle = "【異常】";
                    strQuestion = "プログラム起動タイプ設定エラー！";
                    break;

                case MultilingualConfig.Language_ko_KR:
                    strTitle = "【경고하다】";
                    strQuestion = "프로그램 시작 유형 구성 오류！";
                    break;

                default:
                    break;
            }

            if ("exe".Equals(ini.Program_Type))
            {
                try
                {
                    RunCommand.RunProgram(ini.Program_Name, ini.Program_Param);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ini.Program_Name + " " + ini.Program_Param + "\n\n" + ex.Message, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if ("cmd".Equals(ini.Program_Type))
            {
                try
                {
                    RunCommand.RunCmd(ini.Program_Name, ini.Program_Param);
                } catch (Exception ex)
                {
                    MessageBox.Show(ini.Program_Name + " " + ini.Program_Param + "\n\n" + ex.Message, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(strQuestion + "\n【" + ini.Program_Type + "】", strTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toSetStopMode()
        {

            if (MouseButtons == MouseButtons.Left)
            {
                return;
            }

            int x = MousePosition.X, y = MousePosition.Y;

            if (x > (this.Location.X - iStopMode_XY)
                &&
                x < (this.Location.X + this.Width + iStopMode_XY)
                &&
                y > (this.Location.Y - iStopMode_XY)
                &&
                y < (this.Location.Y + this.Height + iStopMode_XY))
            {

                if (this.iStopMode == 1)
                {
                    this.Location = new Point(iStopMode_T, this.Location.Y);
                    this.TopMost = true;
                }
                else if (this.iStopMode == 2)
                {
                    this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width - iStopMode_T, this.Location.Y);
                    this.TopMost = true;
                }
                else if (this.iStopMode == 3)
                {
                    this.Location = new Point(this.Location.X, iStopMode_T);
                    this.TopMost = true;
                }
                else
                {
                    this.TopMost = false;
                }
            }
            else
            {
                if (this.Location.Y <= iStopMode_T)
                {
                    this.Location = new Point(this.Location.X, iStopMode_T - this.Height);
                    this.iStopMode = 3;
                }
                else if (this.Location.X <= iStopMode_T)
                {
                    this.Location = new Point(iStopMode_T - this.Width, this.Location.Y);
                    this.iStopMode = 1;
                }
                else if (this.Location.X >= Screen.PrimaryScreen.WorkingArea.Width - this.Width - iStopMode_T)
                {
                    this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - iStopMode_T, this.Location.Y);
                    this.iStopMode = 2;
                }
                else
                {
                    this.iStopMode = 0;
                }
            }

        }

        private void toSetOpacity(string strPersent)
        {
            this.toolStripMenuItem_Opacity_10.Checked = false;
            this.toolStripMenuItem_Opacity_20.Checked = false;
            this.toolStripMenuItem_Opacity_30.Checked = false;
            this.toolStripMenuItem_Opacity_40.Checked = false;
            this.toolStripMenuItem_Opacity_50.Checked = false;
            this.toolStripMenuItem_Opacity_60.Checked = false;
            this.toolStripMenuItem_Opacity_70.Checked = false;
            this.toolStripMenuItem_Opacity_80.Checked = false;
            this.toolStripMenuItem_Opacity_90.Checked = false;
            this.toolStripMenuItem_Opacity_100.Checked = false;

            switch (strPersent)
            {
                case "10":
                    this.Opacity = 0.1D;
                    this.toolStripMenuItem_Opacity_10.Checked = true;
                    break;

                case "20":
                    this.Opacity = 0.2D;
                    this.toolStripMenuItem_Opacity_20.Checked = true;
                    break;

                case "30":
                    this.Opacity = 0.3D;
                    this.toolStripMenuItem_Opacity_30.Checked = true;
                    break;

                case "40":
                    this.Opacity = 0.4D;
                    this.toolStripMenuItem_Opacity_40.Checked = true;
                    break;

                case "50":
                    this.Opacity = 0.5D;
                    this.toolStripMenuItem_Opacity_50.Checked = true;
                    break;

                case "60":
                    this.Opacity = 0.6D;
                    this.toolStripMenuItem_Opacity_60.Checked = true;
                    break;

                case "70":
                    this.Opacity = 0.7D;
                    this.toolStripMenuItem_Opacity_70.Checked = true;
                    break;

                case "80":
                    this.Opacity = 0.8D;
                    this.toolStripMenuItem_Opacity_80.Checked = true;
                    break;

                case "90":
                    this.Opacity = 0.9D;
                    this.toolStripMenuItem_Opacity_90.Checked = true;
                    break;

                case "100":
                    this.Opacity = 1D;
                    this.toolStripMenuItem_Opacity_100.Checked = true;
                    break;

                default:
                    break;
            }

            OperateIniFile.WriteIniData(System_Info, MainForm_Opacity, strPersent, strIniFilePath);
        }

        private void setMainFormLanguage()
        {
            switch (this.win_Language)
            {
                case MultilingualConfig.Language_zh_CN:
                    this.toolStripMenuItem_Display.Text = Resources.Resource_zh_CN.toolStripMenuItem_Display_Text;
                    this.toolStripMenuItem_Timer.Text = Resources.Resource_zh_CN.toolStripMenuItem_Timer_Text;
                    this.toolStripMenuItem_Timer_On.Text = Resources.Resource_zh_CN.toolStripMenuItem_TimerOn_Text;
                    this.toolStripMenuItem_Timer_Off.Text = Resources.Resource_zh_CN.toolStripMenuItem_TimerOff_Text;
                    this.toolStripMenuItem_Opacity.Text = Resources.Resource_zh_CN.toolStripMenuItem_Opacity_Text;
                    this.toolStripMenuItem_Language.Text = Resources.Resource_zh_CN.toolStripMenuItem_Language_Text;
                    this.toolStripMenuItem_LanguageChineseSimplified.Text = Resources.Resource_zh_CN.toolStripMenuItem_LanguageChineseSimplified_Text;
                    this.toolStripMenuItem_LanguageChineseTraditional.Text = Resources.Resource_zh_CN.toolStripMenuItem_LanguageChineseTraditional_Text;
                    this.toolStripMenuItem_LanguageEnglish.Text = Resources.Resource_zh_CN.toolStripMenuItem_LanguageEnglish_Text;
                    this.toolStripMenuItem_LanguageJapanese.Text = Resources.Resource_zh_CN.toolStripMenuItem_LanguageJapanese_Text;
                    this.toolStripMenuItem_LanguageKorean.Text = Resources.Resource_zh_CN.toolStripMenuItem_LanguageKorean_Text;
                    this.toolStripMenuItem_About.Text = Resources.Resource_zh_CN.toolStripMenuItem_About_Text;
                    this.toolStripMenuItem_Exit.Text = Resources.Resource_zh_CN.toolStripMenuItem_Exit_Text;
                    this.toolStripMenuItem_AutoRun.Text = Resources.Resource_zh_CN.toolStripMenuItem_AutoRun_Text;
                    this.toolStripMenuItem_AutoRun_On.Text = Resources.Resource_zh_CN.toolStripMenuItem_AutoRun_On_Text;
                    this.toolStripMenuItem_AutoRun_Off.Text = Resources.Resource_zh_CN.toolStripMenuItem_AutoRun_Off_Text;
                    break;

                case MultilingualConfig.Language_zh_TW:
                    this.toolStripMenuItem_Display.Text = Resources.Resource_zh_TW.toolStripMenuItem_Display_Text;
                    this.toolStripMenuItem_Timer.Text = Resources.Resource_zh_TW.toolStripMenuItem_Timer_Text;
                    this.toolStripMenuItem_Timer_On.Text = Resources.Resource_zh_TW.toolStripMenuItem_TimerOn_Text;
                    this.toolStripMenuItem_Timer_Off.Text = Resources.Resource_zh_TW.toolStripMenuItem_TimerOff_Text;
                    this.toolStripMenuItem_Opacity.Text = Resources.Resource_zh_TW.toolStripMenuItem_Opacity_Text;
                    this.toolStripMenuItem_Language.Text = Resources.Resource_zh_TW.toolStripMenuItem_Language_Text;
                    this.toolStripMenuItem_LanguageChineseSimplified.Text = Resources.Resource_zh_TW.toolStripMenuItem_LanguageChineseSimplified_Text;
                    this.toolStripMenuItem_LanguageChineseTraditional.Text = Resources.Resource_zh_TW.toolStripMenuItem_LanguageChineseTraditional_Text;
                    this.toolStripMenuItem_LanguageEnglish.Text = Resources.Resource_zh_TW.toolStripMenuItem_LanguageEnglish_Text;
                    this.toolStripMenuItem_LanguageJapanese.Text = Resources.Resource_zh_TW.toolStripMenuItem_LanguageJapanese_Text;
                    this.toolStripMenuItem_LanguageKorean.Text = Resources.Resource_zh_TW.toolStripMenuItem_LanguageKorean_Text;
                    this.toolStripMenuItem_About.Text = Resources.Resource_zh_TW.toolStripMenuItem_About_Text;
                    this.toolStripMenuItem_Exit.Text = Resources.Resource_zh_TW.toolStripMenuItem_Exit_Text;
                    this.toolStripMenuItem_AutoRun.Text = Resources.Resource_zh_TW.toolStripMenuItem_AutoRun_Text;
                    this.toolStripMenuItem_AutoRun_On.Text = Resources.Resource_zh_TW.toolStripMenuItem_AutoRun_On_Text;
                    this.toolStripMenuItem_AutoRun_Off.Text = Resources.Resource_zh_TW.toolStripMenuItem_AutoRun_Off_Text;
                    break;

                case MultilingualConfig.Language_en_US:
                    this.toolStripMenuItem_Display.Text = Resources.Resource_en_US.toolStripMenuItem_Display_Text;
                    this.toolStripMenuItem_Timer.Text = Resources.Resource_en_US.toolStripMenuItem_Timer_Text;
                    this.toolStripMenuItem_Timer_On.Text = Resources.Resource_en_US.toolStripMenuItem_TimerOn_Text;
                    this.toolStripMenuItem_Timer_Off.Text = Resources.Resource_en_US.toolStripMenuItem_TimerOff_Text;
                    this.toolStripMenuItem_Opacity.Text = Resources.Resource_en_US.toolStripMenuItem_Opacity_Text;
                    this.toolStripMenuItem_Language.Text = Resources.Resource_en_US.toolStripMenuItem_Language_Text;
                    this.toolStripMenuItem_LanguageChineseSimplified.Text = Resources.Resource_en_US.toolStripMenuItem_LanguageChineseSimplified_Text;
                    this.toolStripMenuItem_LanguageChineseTraditional.Text = Resources.Resource_en_US.toolStripMenuItem_LanguageChineseTraditional_Text;
                    this.toolStripMenuItem_LanguageEnglish.Text = Resources.Resource_en_US.toolStripMenuItem_LanguageEnglish_Text;
                    this.toolStripMenuItem_LanguageJapanese.Text = Resources.Resource_en_US.toolStripMenuItem_LanguageJapanese_Text;
                    this.toolStripMenuItem_LanguageKorean.Text = Resources.Resource_en_US.toolStripMenuItem_LanguageKorean_Text;
                    this.toolStripMenuItem_About.Text = Resources.Resource_en_US.toolStripMenuItem_About_Text;
                    this.toolStripMenuItem_Exit.Text = Resources.Resource_en_US.toolStripMenuItem_Exit_Text;
                    this.toolStripMenuItem_AutoRun.Text = Resources.Resource_en_US.toolStripMenuItem_AutoRun_Text;
                    this.toolStripMenuItem_AutoRun_On.Text = Resources.Resource_en_US.toolStripMenuItem_AutoRun_On_Text;
                    this.toolStripMenuItem_AutoRun_Off.Text = Resources.Resource_en_US.toolStripMenuItem_AutoRun_Off_Text;
                    break;

                case MultilingualConfig.Language_ja_JP:
                    this.toolStripMenuItem_Display.Text = Resources.Resource_ja_JP.toolStripMenuItem_Display_Text;
                    this.toolStripMenuItem_Timer.Text = Resources.Resource_ja_JP.toolStripMenuItem_Timer_Text;
                    this.toolStripMenuItem_Timer_On.Text = Resources.Resource_ja_JP.toolStripMenuItem_TimerOn_Text;
                    this.toolStripMenuItem_Timer_Off.Text = Resources.Resource_ja_JP.toolStripMenuItem_TimerOff_Text;
                    this.toolStripMenuItem_Opacity.Text = Resources.Resource_ja_JP.toolStripMenuItem_Opacity_Text;
                    this.toolStripMenuItem_Language.Text = Resources.Resource_ja_JP.toolStripMenuItem_Language_Text;
                    this.toolStripMenuItem_LanguageChineseSimplified.Text = Resources.Resource_ja_JP.toolStripMenuItem_LanguageChineseSimplified_Text;
                    this.toolStripMenuItem_LanguageChineseTraditional.Text = Resources.Resource_ja_JP.toolStripMenuItem_LanguageChineseTraditional_Text;
                    this.toolStripMenuItem_LanguageEnglish.Text = Resources.Resource_ja_JP.toolStripMenuItem_LanguageEnglish_Text;
                    this.toolStripMenuItem_LanguageJapanese.Text = Resources.Resource_ja_JP.toolStripMenuItem_LanguageJapanese_Text;
                    this.toolStripMenuItem_LanguageKorean.Text = Resources.Resource_ja_JP.toolStripMenuItem_LanguageKorean_Text;
                    this.toolStripMenuItem_About.Text = Resources.Resource_ja_JP.toolStripMenuItem_About_Text;
                    this.toolStripMenuItem_Exit.Text = Resources.Resource_ja_JP.toolStripMenuItem_Exit_Text;
                    this.toolStripMenuItem_AutoRun.Text = Resources.Resource_ja_JP.toolStripMenuItem_AutoRun_Text;
                    this.toolStripMenuItem_AutoRun_On.Text = Resources.Resource_ja_JP.toolStripMenuItem_AutoRun_On_Text;
                    this.toolStripMenuItem_AutoRun_Off.Text = Resources.Resource_ja_JP.toolStripMenuItem_AutoRun_Off_Text;
                    break;

                case MultilingualConfig.Language_ko_KR:
                    this.toolStripMenuItem_Display.Text = Resources.Resource_ko_KR.toolStripMenuItem_Display_Text;
                    this.toolStripMenuItem_Timer.Text = Resources.Resource_ko_KR.toolStripMenuItem_Timer_Text;
                    this.toolStripMenuItem_Timer_On.Text = Resources.Resource_ko_KR.toolStripMenuItem_TimerOn_Text;
                    this.toolStripMenuItem_Timer_Off.Text = Resources.Resource_ko_KR.toolStripMenuItem_TimerOff_Text;
                    this.toolStripMenuItem_Opacity.Text = Resources.Resource_ko_KR.toolStripMenuItem_Opacity_Text;
                    this.toolStripMenuItem_Language.Text = Resources.Resource_ko_KR.toolStripMenuItem_Language_Text;
                    this.toolStripMenuItem_LanguageChineseSimplified.Text = Resources.Resource_ko_KR.toolStripMenuItem_LanguageChineseSimplified_Text;
                    this.toolStripMenuItem_LanguageChineseTraditional.Text = Resources.Resource_ko_KR.toolStripMenuItem_LanguageChineseTraditional_Text;
                    this.toolStripMenuItem_LanguageEnglish.Text = Resources.Resource_ko_KR.toolStripMenuItem_LanguageEnglish_Text;
                    this.toolStripMenuItem_LanguageJapanese.Text = Resources.Resource_ko_KR.toolStripMenuItem_LanguageJapanese_Text;
                    this.toolStripMenuItem_LanguageKorean.Text = Resources.Resource_ko_KR.toolStripMenuItem_LanguageKorean_Text;
                    this.toolStripMenuItem_About.Text = Resources.Resource_ko_KR.toolStripMenuItem_About_Text;
                    this.toolStripMenuItem_Exit.Text = Resources.Resource_ko_KR.toolStripMenuItem_Exit_Text;
                    this.toolStripMenuItem_AutoRun.Text = Resources.Resource_ko_KR.toolStripMenuItem_AutoRun_Text;
                    this.toolStripMenuItem_AutoRun_On.Text = Resources.Resource_ko_KR.toolStripMenuItem_AutoRun_On_Text;
                    this.toolStripMenuItem_AutoRun_Off.Text = Resources.Resource_ko_KR.toolStripMenuItem_AutoRun_Off_Text;
                    break;

                default:
                    break;
            }
        }

        private void toShowMainForm()
        {
            this.WindowState = FormWindowState.Normal;
            this.TopMost = false;

            this.toSetMainFormLocation();
            this.Activate();
        }

        private void toSetMainFormLocation()
        {
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width / 4, Screen.PrimaryScreen.WorkingArea.Height / 4);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SYSCOMMAND)
            {
                if (m.WParam.ToInt32() == SC_MINIMIZE)
                {
                    ;
                }
                else if (m.WParam.ToInt32() == SC_NORMAL)
                {
                    this.toShowMainForm();
                }
                else if (m.WParam.ToInt32() == SC_MAXIMIZE)
                {
                    ;
                }

            }
            base.WndProc(ref m);
        }

        private void toSetAutoRun(bool bAutoRunFlg)
        {
            RegistryKey registryKey = OperateRegistry.GetRegistryKeyByHelper(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (bAutoRunFlg == true)
            {
                registryKey.SetValue(Process_RegItemName, "\"" + Process.GetCurrentProcess().MainModule.FileName + "\"", 
                    RegistryValueKind.String);
                registryKey.Close();
            }
            else
            {
                if (OperateRegistry.isExistItemKeyName(registryKey, Process_RegItemName) == true)
                {
                    registryKey.DeleteValue(Process_RegItemName);
                    registryKey.Close();
                }
            }
        }

        #endregion Me

    }
}
