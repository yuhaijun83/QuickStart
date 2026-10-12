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
        private const int BUTTON_MAX_SIZE = 48;

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

        // 停靠功能相关
        private int iStopMode = 0; // 0:NO 1:X 2:Y 3:T
        private int iStopMode_T = 2;
        private int iStopMode_XY = 5;
        // 已停靠时记录所停靠屏幕的工作区，保持粘性，避免多屏之间来回跳动
        private Rectangle stopWorkingArea = Rectangle.Empty;

        // 编辑模式（拖拽排序）相关
        private bool bEditMode = false;
        private List<Button> buttonList = new List<Button>();
        private List<Color> buttonDefaultBackColors = new List<Color>();
        private List<Color> buttonDefaultForeColors = new List<Color>();
        private List<IniSection> lstSections = new List<IniSection>();
        private Button dragSourceButton = null;
        private Point dragStartPoint = Point.Empty;
        private Button dragHighlightButton = null;
        private Color dragHighlightOriginalColor = Color.Empty;
        private const int DRAG_START_THRESHOLD = 4;

        // 编辑模式按钮抖动动画（类似 iOS 编辑主屏）
        private System.Windows.Forms.Timer jiggleTimer = null;
        private int jiggleTick = 0;
        private int jiggleBaseDpi = 96;
        private Dictionary<Button, Point> jiggleOriginalLocations = new Dictionary<Button, Point>();

        private static string strCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private string strIniFilePath = strCurrentDirectory + "\\" + INI_FILE_NAME;
        private string win_Language = MultilingualConfig.Language_en_US;

        private static List<IniConfig> lstConfig = new List<IniConfig>();

        public mainForm()
        {
            InitializeComponent();
            this.InitButtonList();
        }

        private void InitButtonList()
        {
            this.buttonList.Add(this.button_01); this.buttonList.Add(this.button_02);
            this.buttonList.Add(this.button_03); this.buttonList.Add(this.button_04);
            this.buttonList.Add(this.button_05); this.buttonList.Add(this.button_06);
            this.buttonList.Add(this.button_07); this.buttonList.Add(this.button_08);
            this.buttonList.Add(this.button_09); this.buttonList.Add(this.button_10);
            this.buttonList.Add(this.button_11); this.buttonList.Add(this.button_12);
            this.buttonList.Add(this.button_13); this.buttonList.Add(this.button_14);
            this.buttonList.Add(this.button_15); this.buttonList.Add(this.button_16);
            this.buttonList.Add(this.button_17); this.buttonList.Add(this.button_18);
            this.buttonList.Add(this.button_19); this.buttonList.Add(this.button_20);
            this.buttonList.Add(this.button_21); this.buttonList.Add(this.button_22);
            this.buttonList.Add(this.button_23); this.buttonList.Add(this.button_24);
            this.buttonList.Add(this.button_25); this.buttonList.Add(this.button_26);
            this.buttonList.Add(this.button_27); this.buttonList.Add(this.button_28);
            this.buttonList.Add(this.button_29); this.buttonList.Add(this.button_30);
            this.buttonList.Add(this.button_31); this.buttonList.Add(this.button_32);
            this.buttonList.Add(this.button_33); this.buttonList.Add(this.button_34);
            this.buttonList.Add(this.button_35); this.buttonList.Add(this.button_36);
            this.buttonList.Add(this.button_37); this.buttonList.Add(this.button_38);
            this.buttonList.Add(this.button_39); this.buttonList.Add(this.button_40);
            this.buttonList.Add(this.button_41); this.buttonList.Add(this.button_42);
            this.buttonList.Add(this.button_43); this.buttonList.Add(this.button_44);
            this.buttonList.Add(this.button_45); this.buttonList.Add(this.button_46);
            this.buttonList.Add(this.button_47); this.buttonList.Add(this.button_48);

            foreach (Button btn in this.buttonList)
            {
                // 记录设计器默认颜色，配置为空时用于还原
                this.buttonDefaultBackColors.Add(btn.BackColor);
                this.buttonDefaultForeColors.Add(btn.ForeColor);

                btn.MouseDown += new MouseEventHandler(this.button_MouseDown);
                btn.MouseMove += new MouseEventHandler(this.button_MouseMove);
                btn.MouseUp += new MouseEventHandler(this.button_MouseUp);
                btn.DragEnter += new DragEventHandler(this.button_DragEnter);
                btn.DragLeave += new EventHandler(this.button_DragLeave);
                btn.DragDrop += new DragEventHandler(this.button_DragDrop);
            }
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

        private void button_41_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[40]);
        }

        private void button_42_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[41]);
        }

        private void button_43_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[42]);
        }

        private void button_44_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[43]);
        }

        private void button_45_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[44]);
        }

        private void button_46_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[45]);
        }

        private void button_47_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[46]);
        }

        private void button_48_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[47]);
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
            this.stopWorkingArea = Rectangle.Empty;
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

        private void toolStripMenuItem_EditMode_On_Click(object sender, EventArgs e)
        {
            this.SetEditMode(true);
        }

        private void toolStripMenuItem_EditMode_Off_Click(object sender, EventArgs e)
        {
            this.SetEditMode(false);
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
                    ini.Button_Enable = OperateIniFile.ReadIniData(strSection, Button_Enable, "0", strIniFilePath);
                    ini.Button_BackColor = OperateIniFile.ReadIniData(strSection, Button_BackColor, "", strIniFilePath);
                    ini.Button_TextColor = OperateIniFile.ReadIniData(strSection, Button_TextColor, "", strIniFilePath);
                    ini.Program_Name = OperateIniFile.ReadIniData(strSection, Program_Name, "", strIniFilePath, "utf-8", 1024);
                    ini.Program_Type = OperateIniFile.ReadIniData(strSection, Program_Type, "exe", strIniFilePath).ToLower();
                    if ("".Equals(ini.Program_Type)) { ini.Program_Type = "exe"; }
                    ini.Program_Param = OperateIniFile.ReadIniData(strSection, Program_Param, "", strIniFilePath, "utf-8", 1024);

                    // 记录全部段（含不可视段），拖拽排序回写 ini 时用于保持隐藏段不动
                    IniSection section = new IniSection();
                    section.SectionName = strSection;
                    section.Config = ini;
                    section.Visible = "1".Equals(ini.Button_Visible);
                    lstSections.Add(section);

                    if (section.Visible)
                    {
                        lstConfig.Add(ini);
                    }
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

            // 旧版 ini 只有 40 个按钮段，补齐缺失的 [Button_XX] 段（默认隐藏）并回写文件
            // 注意：必须在 streamReader 关闭之后回写，否则文件被占用导致替换失败
            bool bNeedAppend = false;
            for (int i = 1; i <= BUTTON_MAX_SIZE; i++)
            {
                string strName = "Button_" + i.ToString("D2");
                bool bFound = false;
                foreach (IniSection s in lstSections)
                {
                    if (strName.Equals(s.SectionName, StringComparison.OrdinalIgnoreCase))
                    {
                        bFound = true;
                        break;
                    }
                }

                if (!bFound)
                {
                    IniConfig def = new IniConfig();
                    def.Button_Name = "Btn_" + i.ToString("D2");
                    def.Button_Visible = "0";
                    def.Button_Enable = "1";
                    def.Button_BackColor = "";
                    def.Button_TextColor = "";
                    def.Program_Name = "";
                    def.Program_Type = "exe";
                    def.Program_Param = "";

                    IniSection section = new IniSection();
                    section.SectionName = strName;
                    section.Config = def;
                    section.Visible = false;
                    lstSections.Add(section);
                    bNeedAppend = true;
                }
            }

            if (bNeedAppend)
            {
                IniFileRewriter.RewriteButtonSections(this.strIniFilePath, this.lstSections);
            }
        }

        private void InitForm()
        {
            this.Text = "QuickStart  " + Properties.Resources.Program_Build_Version_Text;

            int isize = lstConfig.Count();

            // 宽度固定按每行 8 个按钮计算（与原版一致），高度按实际按钮行数；
            // 边距取已随 DPI 缩放的按钮左/上边距，保证四周间隔一致（原先 +7 为未缩放常量，高分屏下右边距偏窄）
            int iButtonCount = isize < BUTTON_MAX_SIZE ? isize : BUTTON_MAX_SIZE;

            int iRight = this.buttonList[7].Right;
            int iBottom = this.buttonList[0].Bottom;

            for (int i = 0; i < iButtonCount; i++)
            {
                if (this.buttonList[i].Bottom > iBottom)
                {
                    iBottom = this.buttonList[i].Bottom;
                }
            }

            int iMarginX = this.buttonList[0].Left;
            int iMarginY = this.buttonList[0].Top;

            this.ClientSize = new Size(iRight + iMarginX, iBottom + iMarginY);

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
            this.RenderButtons();
        }

        private void RenderButtons()
        {
            for (int i = 0; i < BUTTON_MAX_SIZE; i++)
            {
                Button btn = this.buttonList[i];

                if (i < lstConfig.Count())
                {
                    IniConfig ini = lstConfig[i];

                    btn.Text = ini.Button_Name;
                    btn.Visible = true;
                    btn.Enabled = this.bEditMode || "1".Equals(ini.Button_Enable);
                    btn.AllowDrop = this.bEditMode;

                    if (ini.Button_BackColor != "")
                    {
                        btn.BackColor = Color.FromName(ini.Button_BackColor);
                    }
                    else
                    {
                        btn.BackColor = this.buttonDefaultBackColors[i];
                    }

                    if (ini.Button_TextColor != "")
                    {
                        btn.ForeColor = Color.FromName(ini.Button_TextColor);
                    }
                    else
                    {
                        btn.ForeColor = this.buttonDefaultForeColors[i];
                    }
                }
                else
                {
                    btn.Visible = false;
                    btn.AllowDrop = false;
                }
            }
        }

        private void SetEditMode(bool bOn)
        {
            this.bEditMode = bOn;
            this.toolStripMenuItem_EditMode_On.Checked = bOn;
            this.toolStripMenuItem_EditMode_Off.Checked = !bOn;

            this.toClearHighlight();
            this.RenderButtons();

            if (bOn)
            {
                this.toStartJiggle();
                this.Text = "QuickStart  " + Properties.Resources.Program_Build_Version_Text + "  [" + this.toolStripMenuItem_EditMode.Text + "]";
            }
            else
            {
                this.toStopJiggle();
                this.Text = "QuickStart  " + Properties.Resources.Program_Build_Version_Text;
            }
        }

        private void toStartJiggle()
        {
            if (null == this.jiggleTimer)
            {
                this.jiggleTimer = new System.Windows.Forms.Timer();
                this.jiggleTimer.Interval = 60;
                this.jiggleTimer.Tick += new EventHandler(this.jiggleTimer_Tick);
            }

            this.jiggleOriginalLocations.Clear();
            foreach (Button btn in this.buttonList)
            {
                this.jiggleOriginalLocations[btn] = btn.Location;
            }

            this.jiggleTick = 0;
            this.jiggleBaseDpi = this.DeviceDpi;
            this.jiggleTimer.Start();
        }

        private void toStopJiggle()
        {
            if (null != this.jiggleTimer)
            {
                this.jiggleTimer.Stop();
            }

            foreach (KeyValuePair<Button, Point> kv in this.jiggleOriginalLocations)
            {
                kv.Key.Location = kv.Value;
            }
            this.jiggleOriginalLocations.Clear();
        }

        private void jiggleTimer_Tick(object sender, EventArgs e)
        {
            // 中途切换显示器导致 DPI 变化时，以当前（已重缩放的）位置重新建立基准
            if (this.DeviceDpi != this.jiggleBaseDpi)
            {
                this.jiggleOriginalLocations.Clear();
                foreach (Button btn in this.buttonList)
                {
                    this.jiggleOriginalLocations[btn] = btn.Location;
                }
                this.jiggleBaseDpi = this.DeviceDpi;
                return;
            }

            this.jiggleTick++;

            // 振幅随 DPI 缩放（96dpi 下约 2.5 像素），各按钮相位错开形成自然抖动
            double amp = 2.5 * this.DeviceDpi / 96.0;
            int i = 0;

            foreach (Button btn in this.buttonList)
            {
                if (btn.Visible && this.jiggleOriginalLocations.ContainsKey(btn))
                {
                    Point orig = this.jiggleOriginalLocations[btn];
                    double phase = this.jiggleTick * 0.45 + i * 0.9;
                    int dx = (int)Math.Round(amp * Math.Sin(phase));
                    int dy = (int)Math.Round(amp * 0.6 * Math.Sin(phase * 1.3 + 1.0));
                    btn.Location = new Point(orig.X + dx, orig.Y + dy);
                }
                i++;
            }
        }

        private void button_MouseDown(object sender, MouseEventArgs e)
        {
            if (!this.bEditMode || e.Button != MouseButtons.Left)
            {
                return;
            }

            this.dragSourceButton = (Button)sender;
            this.dragStartPoint = e.Location;
        }

        private void button_MouseMove(object sender, MouseEventArgs e)
        {
            if (null == this.dragSourceButton)
            {
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                this.dragSourceButton = null;
                return;
            }

            if (Math.Abs(e.X - this.dragStartPoint.X) > DRAG_START_THRESHOLD
                || Math.Abs(e.Y - this.dragStartPoint.Y) > DRAG_START_THRESHOLD)
            {
                Button source = this.dragSourceButton;
                this.dragSourceButton = null;
                source.DoDragDrop(source, DragDropEffects.Move);
            }
        }

        private void button_MouseUp(object sender, MouseEventArgs e)
        {
            this.dragSourceButton = null;
        }

        private void button_DragEnter(object sender, DragEventArgs e)
        {
            if (!this.bEditMode || !e.Data.GetDataPresent(typeof(Button)))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            Button source = (Button)e.Data.GetData(typeof(Button));
            Button target = (Button)sender;

            if (source == target)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Move;
            this.toHighlightTarget(target);
        }

        private void button_DragLeave(object sender, EventArgs e)
        {
            this.toClearHighlight();
        }

        private void button_DragDrop(object sender, DragEventArgs e)
        {
            this.toClearHighlight();

            if (!this.bEditMode || !e.Data.GetDataPresent(typeof(Button)))
            {
                return;
            }

            Button source = (Button)e.Data.GetData(typeof(Button));
            Button target = (Button)sender;

            int iFrom = this.buttonList.IndexOf(source);
            int iTo = this.buttonList.IndexOf(target);

            if (iFrom < 0 || iTo < 0 || iFrom == iTo
                || iFrom >= lstConfig.Count() || iTo >= lstConfig.Count())
            {
                return;
            }

            IniConfig moved = lstConfig[iFrom];
            lstConfig.RemoveAt(iFrom);
            lstConfig.Insert(iTo, moved);

            this.RenderButtons();
            this.toSaveButtonOrder();
        }

        private void toHighlightTarget(Button target)
        {
            if (this.dragHighlightButton == target)
            {
                return;
            }

            this.toClearHighlight();
            this.dragHighlightButton = target;
            this.dragHighlightOriginalColor = target.BackColor;
            target.BackColor = Color.Khaki;
        }

        private void toClearHighlight()
        {
            if (null != this.dragHighlightButton)
            {
                this.dragHighlightButton.BackColor = this.dragHighlightOriginalColor;
                this.dragHighlightButton = null;
            }
        }

        private void toSaveButtonOrder()
        {
            int i = 0;
            foreach (IniSection section in this.lstSections)
            {
                if (section.Visible && i < lstConfig.Count())
                {
                    section.Config = lstConfig[i];
                    i++;
                }
            }

            IniFileRewriter.RewriteButtonSections(this.strIniFilePath, this.lstSections);
        }


        private void button_common(IniConfig ini)
        {
            if (this.bEditMode)
            {
                return;
            }

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

            // 阈值随 DPI 缩放（设计值基于 96dpi）
            int t = Math.Max(1, (int)Math.Round(iStopMode_T * this.DeviceDpi / 96.0));
            int xy = Math.Max(1, (int)Math.Round(iStopMode_XY * this.DeviceDpi / 96.0));

            int x = MousePosition.X, y = MousePosition.Y;

            if (x > (this.Location.X - xy)
                &&
                x < (this.Location.X + this.Width + xy)
                &&
                y > (this.Location.Y - xy)
                &&
                y < (this.Location.Y + this.Height + xy))
            {
                // 鼠标移回窗口上：从停靠状态滑出（使用停靠时记录的屏幕，保证多屏下位置正确）
                if (this.iStopMode == 1)
                {
                    this.Location = new Point(this.stopWorkingArea.Left + t, this.Location.Y);
                    this.TopMost = true;
                }
                else if (this.iStopMode == 2)
                {
                    this.Location = new Point(this.stopWorkingArea.Right - this.Width - t, this.Location.Y);
                    this.TopMost = true;
                }
                else if (this.iStopMode == 3)
                {
                    this.Location = new Point(this.Location.X, this.stopWorkingArea.Top + t);
                    this.TopMost = true;
                }
                else
                {
                    this.TopMost = false;
                }
            }
            else
            {
                if (this.iStopMode != 0 && !this.stopWorkingArea.IsEmpty)
                {
                    // 已停靠：保持停靠在该屏幕边缘，不重新扫描其他屏幕（防止多屏间来回跳）
                    this.toSnapToEdge(this.iStopMode, this.stopWorkingArea, t);
                    return;
                }

                // 未停靠：以窗口中心最接近的屏幕为停靠屏（多屏下各屏独立停靠；
                // 中心在屏幕外时 FromPoint 返回最近屏幕，允许把窗口甩出屏幕外停靠）
                Point center = new Point(this.Location.X + this.Width / 2, this.Location.Y + this.Height / 2);

                Screen scr = Screen.FromPoint(center);
                Rectangle wa = scr.WorkingArea;

                int dTop = this.Location.Y - wa.Top;
                int dLeft = this.Location.X - wa.Left;
                int dRight = wa.Right - (this.Location.X + this.Width);

                if (dTop <= t && dTop <= dLeft && dTop <= dRight)
                {
                    this.iStopMode = 3;
                    this.stopWorkingArea = wa;
                    this.toSnapToEdge(3, wa, t);
                }
                else if (dLeft <= t && dLeft <= dRight)
                {
                    this.iStopMode = 1;
                    this.stopWorkingArea = wa;
                    this.toSnapToEdge(1, wa, t);
                }
                else if (dRight <= t)
                {
                    this.iStopMode = 2;
                    this.stopWorkingArea = wa;
                    this.toSnapToEdge(2, wa, t);
                }
                else
                {
                    this.iStopMode = 0;
                    this.stopWorkingArea = Rectangle.Empty;
                }
            }

        }

        // 按停靠模式把窗口吸附到指定屏幕工作区的边缘（只露出 t 像素）
        private void toSnapToEdge(int mode, Rectangle wa, int t)
        {
            if (mode == 1)
            {
                this.Location = new Point(wa.Left + t - this.Width, this.Location.Y);
            }
            else if (mode == 2)
            {
                this.Location = new Point(wa.Right - t, this.Location.Y);
            }
            else if (mode == 3)
            {
                this.Location = new Point(this.Location.X, wa.Top + t - this.Height);
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
                    this.toolStripMenuItem_EditMode.Text = Resources.Resource_zh_CN.toolStripMenuItem_EditMode_Text;
                    this.toolStripMenuItem_EditMode_On.Text = Resources.Resource_zh_CN.toolStripMenuItem_EditMode_On_Text;
                    this.toolStripMenuItem_EditMode_Off.Text = Resources.Resource_zh_CN.toolStripMenuItem_EditMode_Off_Text;
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
                    this.toolStripMenuItem_EditMode.Text = Resources.Resource_zh_TW.toolStripMenuItem_EditMode_Text;
                    this.toolStripMenuItem_EditMode_On.Text = Resources.Resource_zh_TW.toolStripMenuItem_EditMode_On_Text;
                    this.toolStripMenuItem_EditMode_Off.Text = Resources.Resource_zh_TW.toolStripMenuItem_EditMode_Off_Text;
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
                    this.toolStripMenuItem_EditMode.Text = Resources.Resource_en_US.toolStripMenuItem_EditMode_Text;
                    this.toolStripMenuItem_EditMode_On.Text = Resources.Resource_en_US.toolStripMenuItem_EditMode_On_Text;
                    this.toolStripMenuItem_EditMode_Off.Text = Resources.Resource_en_US.toolStripMenuItem_EditMode_Off_Text;
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
                    this.toolStripMenuItem_EditMode.Text = Resources.Resource_ja_JP.toolStripMenuItem_EditMode_Text;
                    this.toolStripMenuItem_EditMode_On.Text = Resources.Resource_ja_JP.toolStripMenuItem_EditMode_On_Text;
                    this.toolStripMenuItem_EditMode_Off.Text = Resources.Resource_ja_JP.toolStripMenuItem_EditMode_Off_Text;
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
                    this.toolStripMenuItem_EditMode.Text = Resources.Resource_ko_KR.toolStripMenuItem_EditMode_Text;
                    this.toolStripMenuItem_EditMode_On.Text = Resources.Resource_ko_KR.toolStripMenuItem_EditMode_On_Text;
                    this.toolStripMenuItem_EditMode_Off.Text = Resources.Resource_ko_KR.toolStripMenuItem_EditMode_Off_Text;
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
