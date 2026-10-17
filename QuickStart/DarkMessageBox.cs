using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace QuickStart
{
    // 暗色模式消息框：与 MessageBox.Show 接口语义对齐的自绘对话框（无边框深灰底 + 浅色文字 + 扁平按钮），
    // 仅在暗色主题下替代系统原生亮色 MessageBox；随 owner 所在屏幕与 DPI 缩放
    class DarkMessageBox : Form
    {
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int HTCAPTION = 0x2;

        private static readonly Color ColorBack = Color.FromArgb(32, 32, 32);
        private static readonly Color ColorText = Color.FromArgb(240, 240, 240);
        private static readonly Color ColorButtonBack = Color.FromArgb(62, 62, 62);
        private static readonly Color ColorButtonBorder = Color.FromArgb(100, 100, 100);
        private static readonly Color ColorBorder = Color.FromArgb(80, 80, 80);

        private Font fontCaption = null;
        private Font fontBody = null;
        private Bitmap bitmapIcon = null; // 图标位图（SystemIcons 是共享句柄，ToBitmap 副本需自行释放）

        // 按应用界面语言映射按钮文字（与系统原生弹框的本地化行为对齐）
        private static string toButtonText(string key, string language)
        {
            if ("zh-CN".Equals(language) || "zh-TW".Equals(language))
            {
                if ("OK".Equals(key)) { return "确定"; }
                return "Yes".Equals(key) ? "是" : "否";
            }
            if ("ja-JP".Equals(language))
            {
                if ("OK".Equals(key)) { return "OK"; }
                return "Yes".Equals(key) ? "はい" : "いいえ";
            }
            if ("ko-KR".Equals(language))
            {
                if ("OK".Equals(key)) { return "확인"; }
                return "Yes".Equals(key) ? "예" : "아니요";
            }
            return key;
        }

        private DarkMessageBox(Form owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, string language)
        {
            float f = (owner != null && owner.DeviceDpi > 0 ? owner.DeviceDpi : 96) / 96f;

            this.fontBody = owner != null && owner.Font != null ? owner.Font : SystemFonts.MessageBoxFont;
            this.fontCaption = new Font(this.fontBody, FontStyle.Bold);

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = ColorBack;
            this.ForeColor = ColorText;
            this.KeyPreview = true;
            this.AutoScaleMode = AutoScaleMode.None;
            this.DoubleBuffered = true;
            this.Font = this.fontBody; // 按钮等子控件继承按 DPI 缩放后的字体

            int pad = (int)Math.Round(18 * f);
            int gap = (int)Math.Round(14 * f);

            // 标题
            Label lblCaption = new Label();
            lblCaption.Text = caption;
            lblCaption.Font = this.fontCaption;
            lblCaption.AutoSize = true;
            lblCaption.MaximumSize = new Size((int)Math.Round(460 * f), 0);
            lblCaption.ForeColor = ColorText;
            lblCaption.BackColor = ColorBack;
            Size captionSize = TextRenderer.MeasureText(caption, this.fontCaption, new Size((int)Math.Round(460 * f), 0), TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);

            // 正文（限宽自动换行）
            Label lblText = new Label();
            lblText.Text = text;
            lblText.Font = this.fontBody;
            lblText.AutoSize = true;
            lblText.MaximumSize = new Size((int)Math.Round(460 * f), 0);
            lblText.ForeColor = ColorText;
            lblText.BackColor = ColorBack;
            Size textSize = TextRenderer.MeasureText(text, this.fontBody, new Size((int)Math.Round(460 * f), 0), TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);

            // 图标
            int iconSize = (int)Math.Round(32 * f);
            PictureBox picIcon = null;
            Icon sysIcon = toMapIcon(icon);
            if (sysIcon != null)
            {
                picIcon = new PictureBox();
                picIcon.Size = new Size(iconSize, iconSize);
                picIcon.SizeMode = PictureBoxSizeMode.CenterImage;
                this.bitmapIcon = sysIcon.ToBitmap();
                picIcon.Image = this.bitmapIcon;
                picIcon.BackColor = ColorBack;
            }

            int bodyWidth = textSize.Width + (picIcon != null ? iconSize + gap : 0);
            int bodyHeight = Math.Max(textSize.Height, picIcon != null ? iconSize : 0);

            // 按钮
            int btnW = (int)Math.Round(88 * f);
            int btnH = (int)Math.Round(28 * f);
            int btnGap = (int)Math.Round(8 * f);

            Button btnOk = null, btnYes = null, btnNo = null;
            if (buttons == MessageBoxButtons.OK)
            {
                btnOk = toMakeButton(toButtonText("OK", language), btnW, btnH);
                btnOk.DialogResult = DialogResult.OK;
            }
            else
            {
                btnYes = toMakeButton(toButtonText("Yes", language), btnW, btnH);
                btnYes.DialogResult = DialogResult.Yes;
                btnNo = toMakeButton(toButtonText("No", language), btnW, btnH);
                btnNo.DialogResult = DialogResult.No;
            }

            int buttonCount = buttons == MessageBoxButtons.OK ? 1 : 2;
            int buttonsWidth = buttonCount * btnW + (buttonCount - 1) * btnGap;

            int clientWidth = pad + Math.Max(bodyWidth, buttonsWidth) + pad;
            int clientHeight = pad + captionSize.Height + gap + bodyHeight + (int)Math.Round(20 * f) + btnH + pad;
            this.ClientSize = new Size(clientWidth, clientHeight);

            int bodyY = pad + captionSize.Height + gap;
            lblCaption.Location = new Point(pad, pad);
            if (picIcon != null)
            {
                picIcon.Location = new Point(pad, bodyY);
            }
            lblText.Location = new Point(pad + (picIcon != null ? iconSize + gap : 0), bodyY);

            int btnX = clientWidth - pad - buttonsWidth;
            int btnY = clientHeight - pad - btnH;
            if (btnOk != null)
            {
                btnOk.Location = new Point(btnX, btnY);
                this.Controls.Add(btnOk);
                this.AcceptButton = btnOk;
                this.CancelButton = btnOk;
            }
            if (btnYes != null)
            {
                btnYes.Location = new Point(btnX, btnY);
                btnNo.Location = new Point(btnX + btnW + btnGap, btnY);
                this.Controls.Add(btnYes);
                this.Controls.Add(btnNo);
                this.CancelButton = btnNo;
                if (defaultButton == MessageBoxDefaultButton.Button2)
                {
                    // 与原生 MessageBox 一致：默认按钮为 No（Enter/焦点都在 No 上，避免误回车退出）
                    this.AcceptButton = btnNo;
                    this.ActiveControl = btnNo;
                }
                else
                {
                    this.AcceptButton = btnYes;
                }
            }

            this.Controls.Add(lblCaption);
            this.Controls.Add(lblText);
            if (picIcon != null)
            {
                this.Controls.Add(picIcon);
            }

            // 居中于 owner（owner 不可用或已最小化到托盘时居中其所在屏幕工作区；最小化窗口的 Bounds 在屏幕外）
            Rectangle bounds = owner != null && owner.IsHandleCreated && owner.Visible && owner.WindowState != FormWindowState.Minimized
                ? owner.Bounds
                : Screen.FromControl(owner ?? this).WorkingArea;
            this.Location = new Point(
                bounds.Left + (bounds.Width - clientWidth) / 2,
                bounds.Top + (bounds.Height - clientHeight) / 2);

            // 无边框窗口用鼠标拖动
            this.MouseDown += delegate
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
            };
        }

        private static Button toMakeButton(string text, int w, int h)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Size = new Size(w, h);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = ColorButtonBorder;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(82, 82, 82);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(96, 96, 96);
            btn.BackColor = ColorButtonBack;
            btn.ForeColor = ColorText;
            btn.UseVisualStyleBackColor = false;
            btn.TabStop = true;
            return btn;
        }

        private static Icon toMapIcon(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Warning: return SystemIcons.Warning;
                case MessageBoxIcon.Error: return SystemIcons.Error;
                case MessageBoxIcon.Information: return SystemIcons.Information;
                case MessageBoxIcon.Question: return SystemIcons.Question;
                default: return null;
            }
        }

        // 1px 外描边，无边框窗口与桌面区分
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen pen = new Pen(ColorBorder))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (this.fontCaption != null) { this.fontCaption.Dispose(); this.fontCaption = null; }
            if (this.bitmapIcon != null) { this.bitmapIcon.Dispose(); this.bitmapIcon = null; }
            base.OnFormClosed(e);
        }

        public static DialogResult Show(Form owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, string language)
        {
            using (DarkMessageBox box = new DarkMessageBox(owner, text, caption, buttons, icon, defaultButton, language))
            {
                return box.ShowDialog(owner);
            }
        }
    }
}
