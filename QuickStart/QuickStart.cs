﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        private const int BUTTON_MAX_SIZE = 40;

        private const string INI_FILE_NAME = "QuickStart.ini";

        private const string System_Info = "System_Info";
        private const string MainForm_Opacity = "MainForm_Opacity";

        private const string Button_Name = "Button_Name";
        private const string Button_Visible = "Button_Visible";
        private const string Program_Name = "Program_Name";
        private const string Program_Type = "Program_Type";
        private const string Program_Param = "Program_Param";

        private int iStopMode = 0; // 0:NO 1:X 2:Y 3:T
        private int iStopMode_T = 2;
        private int iStopMode_XY = 5;

        private static List<IniConfig> lstConfig = new List<IniConfig>();

        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            this.InitConfig();
            this.InitForm();
            this.InitButton();
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
            DialogResult dialogResult =
                MessageBox.Show("Do you really want to quit?", "Warn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
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
            DialogResult dialogResult =
                MessageBox.Show("Do you really want to quit?", "Warn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dialogResult == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        private void toolStripMenuItem_About_Click(object sender, EventArgs e)
        {
            aboutForm objAboutForm = new aboutForm();
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

        #region Me

        private void InitConfig()
        {
            string strIniFilePath = System.Environment.CurrentDirectory + "\\" + INI_FILE_NAME;

            string strSection = null;
            IniConfig ini = null;

            for (int i = 1; i <= BUTTON_MAX_SIZE; i++)
            {
                ini = new IniConfig();

                if (i < 10)
                {
                    strSection = "Button_0" + i.ToString();
                }
                else
                {
                    strSection = "Button_" + i.ToString();
                }

                ini.Button_Name = OperateIniFile.ReadIniData(strSection, Button_Name, "", strIniFilePath);
                ini.Button_Visible = OperateIniFile.ReadIniData(strSection, Button_Visible, "0", strIniFilePath);
                if (!"1".Equals(ini.Button_Visible)) { continue; }
                ini.Program_Name = OperateIniFile.ReadIniData(strSection, Program_Name, "", strIniFilePath);
                ini.Program_Type = OperateIniFile.ReadIniData(strSection, Program_Type, "exe", strIniFilePath);
                if ("".Equals(ini.Program_Type)) { ini.Program_Type = "exe"; }
                ini.Program_Param = OperateIniFile.ReadIniData(strSection, Program_Param, "", strIniFilePath);

                lstConfig.Add(ini);
            }

        }

        private void InitForm()
        {
            string strIniFilePath = System.Environment.CurrentDirectory + "\\" + INI_FILE_NAME;
            String strMainForm_Opacity = OperateIniFile.ReadIniData(System_Info, MainForm_Opacity, "", strIniFilePath);
            double dMainForm_Opacity = 1;
            if (!"".Equals(strMainForm_Opacity))
            {
                try
                {
                    dMainForm_Opacity = double.Parse(strMainForm_Opacity);
                } catch
                {
                    ;
                }
            }
            this.Opacity = dMainForm_Opacity;

            // 12, 64 13, 64 13, 64 ,13 = 243
            int isize = lstConfig.Count();
            if (isize <= 8)
            {
                this.Size = new Size(769, 104);
            } else if (9 <= isize && isize  <= 16)
            {
                this.Size = new Size(769, 174);
            } else if (17 <= isize && isize <= 24)
            {
                this.Size = new Size(769, 243);
            } else if (25 <= isize && isize <= 32)
            {
                this.Size = new Size(769, 312);
            } else if (33 <= isize && isize <= 40)
            {
                this.Size = new Size(769, 381);
            } else
            {
                this.Size = new Size(769, 104);
            }
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
                        break;
                    case 2:
                        this.button_02.Text = lstConfig[i - 1].Button_Name;
                        this.button_02.Visible = true;
                        break;
                    case 3:
                        this.button_03.Text = lstConfig[i - 1].Button_Name;
                        this.button_03.Visible = true;
                        break;
                    case 4:
                        this.button_04.Text = lstConfig[i - 1].Button_Name;
                        this.button_04.Visible = true;
                        break;
                    case 5:
                        this.button_05.Text = lstConfig[i - 1].Button_Name;
                        this.button_05.Visible = true;
                        break;
                    case 6:
                        this.button_06.Text = lstConfig[i - 1].Button_Name;
                        this.button_06.Visible = true;
                        break;
                    case 7:
                        this.button_07.Text = lstConfig[i - 1].Button_Name;
                        this.button_07.Visible = true;
                        break;
                    case 8:
                        this.button_08.Text = lstConfig[i - 1].Button_Name;
                        this.button_08.Visible = true;
                        break;

                    case 9:
                        this.button_09.Text = lstConfig[i - 1].Button_Name;
                        this.button_09.Visible = true;
                        break;
                    case 10:
                        this.button_10.Text = lstConfig[i - 1].Button_Name;
                        this.button_10.Visible = true;
                        break;
                    case 11:
                        this.button_11.Text = lstConfig[i - 1].Button_Name;
                        this.button_11.Visible = true;
                        break;
                    case 12:
                        this.button_12.Text = lstConfig[i - 1].Button_Name;
                        this.button_12.Visible = true;
                        break;
                    case 13:
                        this.button_13.Text = lstConfig[i - 1].Button_Name;
                        this.button_13.Visible = true;
                        break;
                    case 14:
                        this.button_14.Text = lstConfig[i - 1].Button_Name;
                        this.button_14.Visible = true;
                        break;
                    case 15:
                        this.button_15.Text = lstConfig[i - 1].Button_Name;
                        this.button_15.Visible = true;
                        break;
                    case 16:
                        this.button_16.Text = lstConfig[i - 1].Button_Name;
                        this.button_16.Visible = true;
                        break;

                    case 17:
                        this.button_17.Text = lstConfig[i - 1].Button_Name;
                        this.button_17.Visible = true;
                        break;
                    case 18:
                        this.button_18.Text = lstConfig[i - 1].Button_Name;
                        this.button_18.Visible = true;
                        break;
                    case 19:
                        this.button_19.Text = lstConfig[i - 1].Button_Name;
                        this.button_19.Visible = true;
                        break;
                    case 20:
                        this.button_20.Text = lstConfig[i - 1].Button_Name;
                        this.button_20.Visible = true;
                        break;
                    case 21:
                        this.button_21.Text = lstConfig[i - 1].Button_Name;
                        this.button_21.Visible = true;
                        break;
                    case 22:
                        this.button_22.Text = lstConfig[i - 1].Button_Name;
                        this.button_22.Visible = true;
                        break;
                    case 23:
                        this.button_23.Text = lstConfig[i - 1].Button_Name;
                        this.button_23.Visible = true;
                        break;
                    case 24:
                        this.button_24.Text = lstConfig[i - 1].Button_Name;
                        this.button_24.Visible = true;
                        break;

                    case 25:
                        this.button_25.Text = lstConfig[i - 1].Button_Name;
                        this.button_25.Visible = true;
                        break;
                    case 26:
                        this.button_26.Text = lstConfig[i - 1].Button_Name;
                        this.button_26.Visible = true;
                        break;
                    case 27:
                        this.button_27.Text = lstConfig[i - 1].Button_Name;
                        this.button_27.Visible = true;
                        break;
                    case 28:
                        this.button_28.Text = lstConfig[i - 1].Button_Name;
                        this.button_28.Visible = true;
                        break;
                    case 29:
                        this.button_29.Text = lstConfig[i - 1].Button_Name;
                        this.button_29.Visible = true;
                        break;
                    case 30:
                        this.button_30.Text = lstConfig[i - 1].Button_Name;
                        this.button_30.Visible = true;
                        break;
                    case 31:
                        this.button_31.Text = lstConfig[i - 1].Button_Name;
                        this.button_31.Visible = true;
                        break;
                    case 32:
                        this.button_32.Text = lstConfig[i - 1].Button_Name;
                        this.button_32.Visible = true;
                        break;

                    case 33:
                        this.button_33.Text = lstConfig[i - 1].Button_Name;
                        this.button_33.Visible = true;
                        break;
                    case 34:
                        this.button_34.Text = lstConfig[i - 1].Button_Name;
                        this.button_34.Visible = true;
                        break;
                    case 35:
                        this.button_35.Text = lstConfig[i - 1].Button_Name;
                        this.button_35.Visible = true;
                        break;
                    case 36:
                        this.button_36.Text = lstConfig[i - 1].Button_Name;
                        this.button_36.Visible = true;
                        break;
                    case 37:
                        this.button_37.Text = lstConfig[i - 1].Button_Name;
                        this.button_37.Visible = true;
                        break;
                    case 38:
                        this.button_38.Text = lstConfig[i - 1].Button_Name;
                        this.button_38.Visible = true;
                        break;
                    case 39:
                        this.button_39.Text = lstConfig[i - 1].Button_Name;
                        this.button_39.Visible = true;
                        break;
                    case 40:
                        this.button_40.Text = lstConfig[i - 1].Button_Name;
                        this.button_40.Visible = true;
                        break;

                    default:
                        break;
                }
            }
        }

        private void button_common(IniConfig ini)
        {
            if ("exe".Equals(ini.Program_Type))
            {
                try
                {
                    RunCommand.RunProgram(ini.Program_Name, ini.Program_Param);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ini.Program_Name + " " + ini.Program_Param + "\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if ("cmd".Equals(ini.Program_Type))
            {
                try
                {
                    RunCommand.RunCmd(ini.Program_Name, ini.Program_Param);
                } catch (Exception ex)
                {
                    MessageBox.Show(ini.Program_Name + " " + ini.Program_Param + "\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Program Start Type Error！\n【" + ini.Program_Type + "】", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                }
                else if (this.iStopMode == 2)
                {
                    this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width - iStopMode_T, this.Location.Y);
                }
                else if (this.iStopMode == 3)
                {
                    this.Location = new Point(this.Location.X, iStopMode_T);
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

        #endregion Me
    }
}
