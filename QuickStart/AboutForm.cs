using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuickStart
{
    public partial class aboutForm : Form
    {
        public aboutForm()
        {
            InitializeComponent();
        }

        public aboutForm(string strWin_Language)
        {
            InitializeComponent();
            this.lbl_BuildVersion.Text = Properties.Resources.Program_Build_Version_Text;
            this.setAboutFormLanguage(strWin_Language);
        }

        private void button_AboutFormClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel_GitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            String strTargetUrl = "https://github.com/yuhaijun83/QuickStart";
            System.Diagnostics.Process.Start("iexplore.exe", strTargetUrl);
        }

        #region Me

        private void setAboutFormLanguage(String strWin_Language)
        {
            switch (strWin_Language)
            {
                case MultilingualConfig.Language_zh_CN:
                    this.Text = Resources.Resource_zh_CN.aboutForm_Title_Text;
                    this.button_AboutForm_Close.Text = Resources.Resource_zh_CN.aboutForm_BtnClose_Text;
                    break;
                case MultilingualConfig.Language_zh_TW:
                    this.Text = Resources.Resource_zh_TW.aboutForm_Title_Text;
                    this.button_AboutForm_Close.Text = Resources.Resource_zh_TW.aboutForm_BtnClose_Text;
                    break;
                case MultilingualConfig.Language_en_US:
                    this.Text = Resources.Resource_en_US.aboutForm_Title_Text;
                    this.button_AboutForm_Close.Text = Resources.Resource_en_US.aboutForm_BtnClose_Text;
                    break;
                case MultilingualConfig.Language_ja_JP:
                    this.Text = Resources.Resource_ja_JP.aboutForm_Title_Text;
                    this.button_AboutForm_Close.Text = Resources.Resource_ja_JP.aboutForm_BtnClose_Text;
                    break;
                case MultilingualConfig.Language_ko_KR:
                    this.Text = Resources.Resource_ko_KR.aboutForm_Title_Text;
                    this.button_AboutForm_Close.Text = Resources.Resource_ko_KR.aboutForm_BtnClose_Text;
                    break;

                default:
                    break;
            }
        }

        #endregion Me

    }
}
