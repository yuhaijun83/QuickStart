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
        private bool bDarkMode = false;

        // 与暗色弹框/主窗体一致的暗色配色
        private static readonly Color ColorDarkBack = Color.FromArgb(32, 32, 32);
        private static readonly Color ColorDarkText = Color.FromArgb(240, 240, 240);
        private static readonly Color ColorDarkButtonBack = Color.FromArgb(62, 62, 62);
        private static readonly Color ColorDarkButtonBorder = Color.FromArgb(100, 100, 100);
        private static readonly Color ColorDarkLink = Color.FromArgb(120, 175, 230);

        public aboutForm()
        {
            InitializeComponent();
        }

        public aboutForm(string strWin_Language, bool darkMode)
        {
            InitializeComponent();
            this.bDarkMode = darkMode;
            this.lbl_BuildVersion.Text = Properties.Resources.Program_Build_Version_Text;
            this.setAboutFormLanguage(strWin_Language);
            if (this.bDarkMode)
            {
                this.ApplyDarkStyle();
            }
        }

        private void button_AboutFormClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.InitLogo();
            base.OnLoad(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (this.bDarkMode)
            {
                ThemeHelper.ApplyTitleBarTheme(this.Handle, true);
            }
        }

        // 暗色风格：窗体/文字/按钮/链接配色与暗色弹框一致（亮色模式保持系统原生外观不动）
        private void ApplyDarkStyle()
        {
            this.BackColor = ColorDarkBack;
            this.ForeColor = ColorDarkText;

            this.lbl_BigTitle.BackColor = ColorDarkBack;
            this.lbl_BigTitle.ForeColor = ColorDarkText;
            this.lbl_BuildVersion.BackColor = ColorDarkBack;
            this.lbl_BuildVersion.ForeColor = ColorDarkText;
            this.pictureBox1.BackColor = ColorDarkBack;

            this.linkLabel_GitHub.BackColor = ColorDarkBack;
            this.linkLabel_GitHub.LinkColor = ColorDarkLink;
            this.linkLabel_GitHub.ActiveLinkColor = Color.FromArgb(160, 200, 240);
            this.linkLabel_GitHub.VisitedLinkColor = ColorDarkLink;

            this.button_AboutForm_Close.FlatStyle = FlatStyle.Flat;
            this.button_AboutForm_Close.FlatAppearance.BorderColor = ColorDarkButtonBorder;
            this.button_AboutForm_Close.FlatAppearance.BorderSize = 1;
            this.button_AboutForm_Close.FlatAppearance.MouseOverBackColor = Color.FromArgb(82, 82, 82);
            this.button_AboutForm_Close.FlatAppearance.MouseDownBackColor = Color.FromArgb(96, 96, 96);
            this.button_AboutForm_Close.BackColor = ColorDarkButtonBack;
            this.button_AboutForm_Close.ForeColor = ColorDarkText;
            this.button_AboutForm_Close.UseVisualStyleBackColor = false;
        }

        // pictureBox1 使用的是 BackgroundImage，而 BackgroundImageLayout 默认值为 Tile，
        // 会把图标平铺成多张。这里改为单张等比缩放，并按控件实际尺寸（已随 DPI 缩放）
        // 选取图标中最合适的帧，避免高分屏下模糊。
        private void InitLogo()
        {
            this.pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;

            Size box = this.pictureBox1.ClientSize;
            if (box.Width <= 0 || box.Height <= 0)
            {
                box = new Size(128, 128);
            }

            try
            {
                using (Icon best = new Icon(this.Icon, box))
                {
                    this.pictureBox1.BackgroundImage = best.ToBitmap();
                }
            }
            catch
            {
                ;
            }
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
