using Microsoft.Win32;
using QuickStart.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QuickStart
{
    public partial class mainForm : Form
    {
        private const int BUTTON_MAX_SIZE = 64;

        private const string INI_FILE_NAME = "QuickStart.ini";

        private const string System_Info = "System_Info";
        private const string MainForm_Opacity = "MainForm_Opacity";
        private const string MainForm_Language = "MainForm_Language";
        private const string MainForm_Timer = "MainForm_Timer";
        private const string MainForm_AutoRun = "MainForm_AutoRun";
        private const string MainForm_ShowProgramIcon = "MainForm_ShowProgramIcon";
        private const string MainForm_Theme = "MainForm_Theme";

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

        // 按下反馈：记录处于按下态的按钮，Paint 时绘制凹陷效果
        private HashSet<Button> pressedButtons = new HashSet<Button>();

        // 按钮程序图标映射：按钮 → 配置程序的系统图标（仅引用，所有权归 iconCacheByFile；后台提取完成后在 UI 线程整体替换）
        private Dictionary<Button, Bitmap> buttonIcons = new Dictionary<Button, Bitmap>();

        // 跨重建图标缓存：键 = 解析后路径|最后写入时间|大小（程序文件变化后自动重新提取）；Bitmap 统一由该缓存持有，窗体退出时释放
        private static readonly Dictionary<string, Bitmap> iconCacheByFile = new Dictionary<string, Bitmap>(StringComparer.OrdinalIgnoreCase);
        private static readonly object iconCacheLock = new object();

        // 图标重建代数：后台提取完成时若代数已变化（ini 连续多次修改），丢弃过期结果
        private int iconGeneration = 0;

        // 窗体正在退出：置位后拒绝一切图标结果回写（防止 FormClosing 释放缓存后，
        // 排队中的 BeginInvoke 回调把已释放的 Bitmap 重新塞回按钮映射导致绘制崩溃）
        private volatile bool bIconsExiting = false;

        // 主题：ini [System_Info] MainForm_Theme = Auto/Light/Dark（默认 Auto 跟随系统）
        private string strThemeConfig = "Auto";
        private bool bDarkMode = false;
        private Color colorLightFormBack = Color.Empty; // 设计器亮色窗体背景，切回亮色时还原
        private ToolStripRenderer rendererMenuDefault = null; // 托盘菜单默认渲染器/颜色，切回亮色时还原
        private Color colorMenuBack = Color.Empty;
        private Color colorMenuFore = Color.Empty;
        private DarkMenuRenderer darkMenuRenderer = null;

        // 标题栏暗色模式（Win10 1809+）实现已提取到 ThemeHelper（About 窗体共用）

        // 鼠标悬停时文字浮动动画
        private Button hoverAnimButton = null;
        private System.Windows.Forms.Timer hoverAnimTimer = null;

        // 左键长按 3 秒进入编辑模式
        private Button longPressButton = null;
        private System.Windows.Forms.Timer longPressTimer = null;

        // 编辑模式下 8 秒无按钮顺序变动自动退出
        private System.Windows.Forms.Timer editIdleTimer = null;
        private bool bDragDropActive = false;

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
            this.buttonList.Add(this.button_49); this.buttonList.Add(this.button_50);
            this.buttonList.Add(this.button_51); this.buttonList.Add(this.button_52);
            this.buttonList.Add(this.button_53); this.buttonList.Add(this.button_54);
            this.buttonList.Add(this.button_55); this.buttonList.Add(this.button_56);
            this.buttonList.Add(this.button_57); this.buttonList.Add(this.button_58);
            this.buttonList.Add(this.button_59); this.buttonList.Add(this.button_60);
            this.buttonList.Add(this.button_61); this.buttonList.Add(this.button_62);
            this.buttonList.Add(this.button_63); this.buttonList.Add(this.button_64);

            foreach (Button btn in this.buttonList)
            {
                // 按钮未显式设色时会继承窗体的淡黄背景，立体渐变会发黄，这里显式给淡绿色
                if (btn.BackColor.Equals(this.BackColor))
                {
                    btn.BackColor = Color.White; // 默认纯白（渐变已调淡，不再发灰）
                }

                // 记录设计器默认颜色，配置为空时用于还原
                this.buttonDefaultBackColors.Add(btn.BackColor);
                this.buttonDefaultForeColors.Add(btn.ForeColor);

                btn.MouseDown += new MouseEventHandler(this.button_MouseDown);
                btn.MouseMove += new MouseEventHandler(this.button_MouseMove);
                btn.MouseUp += new MouseEventHandler(this.button_MouseUp);
                btn.DragEnter += new DragEventHandler(this.button_DragEnter);
                btn.DragLeave += new EventHandler(this.button_DragLeave);
                btn.DragDrop += new DragEventHandler(this.button_DragDrop);
                btn.MouseLeave += new EventHandler(this.button_MouseLeave3D);
                btn.MouseEnter += new EventHandler(this.button_MouseEnter3D);
                btn.MouseCaptureChanged += new EventHandler(this.button_MouseCaptureChanged);

                // 立体感：自绘渐变面与高光/阴影边缘（见 button_Paint3D）
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Paint += new PaintEventHandler(this.button_Paint3D);
            }
        }

        // 立体感绘制：卡片风格 —— 纯色圆角面 + 外侧柔和投影（多层半透明圆角矩形叠加模拟模糊）。
        // 按下时投影收缩、面轻微变暗，形成按压感。颜色始终取按钮当前 BackColor，ini 自定义色与拖拽高亮自动适配。
        private void button_Paint3D(object sender, PaintEventArgs e)
        {
            Button btn = (Button)sender;
            Rectangle r = btn.ClientRectangle;
            if (r.Width < 6 || r.Height < 6)
            {
                return;
            }

            Graphics g = e.Graphics;
            Color baseColor = btn.BackColor;
            if (this.bDarkMode)
            {
                baseColor = toDarkButtonColor(baseColor);
            }
            bool pressed = this.pressedButtons.Contains(btn);

            // 侧壁以外的区域用窗体背景盖掉按钮自绘的方角底色
            using (SolidBrush bg = new SolidBrush(this.BackColor))
            {
                g.FillRectangle(bg, r);
            }

            float dpi = btn.DeviceDpi / 96f;
            int radius = Math.Max(4, (int)Math.Round(6 * dpi));

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 键帽式立体：顶面下方露出实心灰色侧壁形成物理厚度；按下时顶面下沉、侧壁消失、面变暗
            int thickness = pressed ? 1 : Math.Max(2, (int)Math.Round(3 * dpi));
            int faceY = pressed ? 1 : 0;

            Rectangle sideRect = new Rectangle(r.X, r.Y + thickness, r.Width - 1, r.Height - 1 - thickness);
            using (GraphicsPath side = RoundedRect(sideRect, radius))
            {
                using (SolidBrush sb = new SolidBrush(ControlPaint.Dark(baseColor, 0.20f)))
                {
                    g.FillPath(sb, side);
                }
                using (Pen p = new Pen(ControlPaint.Dark(baseColor, 0.30f)))
                {
                    g.DrawPath(p, side);
                }
            }

            Rectangle faceRect = new Rectangle(r.X, r.Y + faceY, r.Width - 1, r.Height - 1 - thickness);
            using (GraphicsPath face = RoundedRect(faceRect, radius))
            {
                Color faceColor = pressed ? ControlPaint.Dark(baseColor, 0.06f) : baseColor;
                using (SolidBrush fb = new SolidBrush(faceColor))
                {
                    g.FillPath(fb, face);
                }
                using (Pen p = new Pen(ControlPaint.Dark(baseColor, 0.25f)))
                {
                    g.DrawPath(p, face);
                }
            }
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

            // 程序图标：按钮左上角显示配置程序（exe/cmd 等）的系统图标，提取失败则不显示
            Bitmap iconBmp;
            if (this.buttonIcons.TryGetValue(btn, out iconBmp) && iconBmp != null)
            {
                int iconSize = Math.Max(14, (int)Math.Round(20 * dpi));
                int iconMargin = Math.Max(2, (int)Math.Round(4 * dpi));
                Rectangle iconRect = new Rectangle(r.X + iconMargin, r.Y + faceY + iconMargin, iconSize, iconSize);

                System.Drawing.Drawing2D.InterpolationMode oldInterp = g.InterpolationMode;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(iconBmp, iconRect);
                g.InterpolationMode = oldInterp;
            }

            // Paint 在按钮自绘之后触发，会盖掉原文字，这里按原样式重绘；按下时文字右下移 1px 增强按压感；
            // 悬停时文字做 ±2px 上下正弦浮动
            Color textColor = btn.Enabled ? btn.ForeColor : ControlPaint.Dark(btn.ForeColor, 0.25f);
            if (this.bDarkMode)
            {
                textColor = btn.Enabled
                    ? toDarkTextColor(textColor)
                    : toBlend(toDarkTextColor(btn.ForeColor), Color.Black, 0.25f);
            }
            Rectangle textRect;
            if (pressed)
            {
                textRect = new Rectangle(r.X + 1, r.Y + 1, r.Width, r.Height);
            }
            else
            {
                int bobY = ReferenceEquals(this.hoverAnimButton, btn)
                    ? (int)Math.Round(2 * dpi * Math.Sin(Environment.TickCount * 0.006))
                    : 0;
                textRect = new Rectangle(r.X, r.Y + bobY, r.Width, r.Height);
            }
            // 自绘文字：从第一个字符开始排版，一行放不下就换到下一行，行数超过按钮高度时最后一行截断加省略号
            toDrawButtonText(g, btn.Text, btn.Font, textRect, textColor);
        }

        // 按按钮宽度逐行排版文字：优先在空格处断行，无空格的超长串按字符硬断；
        // 整块放得下时垂直居中，放不下时从顶部开始画、并在最后可见行末尾加省略号
        private static void toDrawButtonText(Graphics g, string text, Font font, Rectangle rect, Color color)
        {
            if (string.IsNullOrEmpty(text) || rect.Width < 8 || rect.Height < 8)
            {
                return;
            }

            TextFormatFlags measureFlags = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine;
            int lineHeight = TextRenderer.MeasureText(g, "Wg", font, new Size(int.MaxValue, int.MaxValue), measureFlags).Height;
            if (lineHeight < 6)
            {
                lineHeight = Math.Max(6, font.Height);
            }

            List<string> lines = new List<string>();
            foreach (string logical in text.Replace("\r\n", "\n").Split('\n'))
            {
                toWrapOneLine(g, logical, font, rect.Width, measureFlags, lines);
            }

            int maxRows = rect.Height / lineHeight;
            if (maxRows < 1)
            {
                return;
            }
            bool overflow = lines.Count > maxRows;
            int rows = overflow ? maxRows : lines.Count;
            int y = overflow ? rect.Y : rect.Y + (rect.Height - rows * lineHeight) / 2;
            TextFormatFlags lineFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.Top
                | TextFormatFlags.NoPadding | TextFormatFlags.SingleLine;

            for (int i = 0; i < rows; i++)
            {
                string line = lines[i];
                if (overflow && i == rows - 1)
                {
                    line = toClipWithEllipsis(g, line, font, rect.Width, measureFlags);
                }
                if (line.Length > 0)
                {
                    TextRenderer.DrawText(g, line, font, new Rectangle(rect.X, y, rect.Width, lineHeight), color, lineFlags);
                }
                y += lineHeight;
            }
        }

        // 把一行文字按 maxWidth 拆成若干物理行：能在空格处断就在最后一个空格处断，否则按字符硬断
        private static void toWrapOneLine(Graphics g, string line, Font font, int maxWidth, TextFormatFlags measureFlags, List<string> result)
        {
            if (line.Length == 0)
            {
                result.Add(string.Empty);
                return;
            }
            if (toTextWidth(g, line, font, measureFlags) <= maxWidth)
            {
                result.Add(line);
                return;
            }

            int start = 0;
            while (start < line.Length)
            {
                // 二分找出从 start 起能放进 maxWidth 的最长前缀
                int lo = start + 1;
                int hi = line.Length;
                while (lo < hi)
                {
                    int mid = (lo + hi + 1) / 2;
                    if (toTextWidth(g, line.Substring(start, mid - start), font, measureFlags) <= maxWidth)
                    {
                        lo = mid;
                    }
                    else
                    {
                        hi = mid - 1;
                    }
                }
                int end = lo;

                int space = line.LastIndexOf(' ', end - 1);
                if (space > start)
                {
                    // 在前缀范围内找最后一个空格断行，空格本身不保留
                    result.Add(line.Substring(start, space - start));
                    start = space + 1;
                }
                else
                {
                    result.Add(line.Substring(start, end - start));
                    start = end;
                }
            }
        }

        private static string toClipWithEllipsis(Graphics g, string line, Font font, int maxWidth, TextFormatFlags measureFlags)
        {
            const string ellipsis = "...";
            if (toTextWidth(g, line + ellipsis, font, measureFlags) <= maxWidth)
            {
                return line + ellipsis;
            }
            int lo = 0;
            int hi = line.Length;
            while (lo < hi)
            {
                int mid = (lo + hi + 1) / 2;
                if (toTextWidth(g, line.Substring(0, mid) + ellipsis, font, measureFlags) <= maxWidth)
                {
                    lo = mid;
                }
                else
                {
                    hi = mid - 1;
                }
            }
            return line.Substring(0, lo) + ellipsis;
        }

        private static int toTextWidth(Graphics g, string text, Font font, TextFormatFlags measureFlags)
        {
            return TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), measureFlags).Width;
        }

        private static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            this.toCheckIsNotExistIniFile();
            this.toEnsureSystemInfoSection();
            this.InitTheme(); // 提前读取主题配置，保证启动阶段的弹框（重复实例警告等）也能按主题选色
            this.InitLanguage();
            this.toCheckIsNotExistCurrentProcess();
            this.InitOpacity();
            this.InitShowProgramIcon();
            this.InitConfig();
            this.RebuildButtonIcons();
            this.InitForm();
            this.InitButton();
            this.ApplyTheme();
            this.InitTimer();
            this.InitAutoRun();
            this.toStartIniWatcher();

            // 跨显示器/系统缩放比例变化导致控件重新缩放后，同样强制重绘，避免残留陈旧画面
            this.DpiChanged += (s, ev) => this.toInvalidateButtons();
        }

        // 保险：窗体显示后强制重绘一次按钮。
        // 首帧可能在按钮完成 DPI 缩放/布局之前绘制，文字按当时的宽度排版（长文字不换行、居中后被裁成中间一段），
        // 之后若无任何事件触发重绘，这个陈旧画面会一直留在屏幕上（鼠标悬停/遮挡才会恢复）
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.toInvalidateButtons();
        }

        private void toInvalidateButtons()
        {
            foreach (Button btn in this.buttonList)
            {
                if (btn.Visible)
                {
                    btn.Invalidate();
                }
            }
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

        private void button_49_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[48]);
        }

        private void button_50_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[49]);
        }

        private void button_51_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[50]);
        }

        private void button_52_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[51]);
        }

        private void button_53_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[52]);
        }

        private void button_54_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[53]);
        }

        private void button_55_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[54]);
        }

        private void button_56_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[55]);
        }

        private void button_57_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[56]);
        }

        private void button_58_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[57]);
        }

        private void button_59_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[58]);
        }

        private void button_60_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[59]);
        }

        private void button_61_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[60]);
        }

        private void button_62_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[61]);
        }

        private void button_63_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[62]);
        }

        private void button_64_Click(object sender, EventArgs e)
        {
            this.button_common(lstConfig[63]);
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
                toShowMessageBox(strQuestion, strTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dialogResult == DialogResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }

            // 确认退出时释放图标缓存持有的全部 Bitmap（iconCacheByFile 统一持有所有权）
            if (e.Cancel == false)
            {
                this.bIconsExiting = true;
                this.buttonIcons.Clear();
                lock (iconCacheLock)
                {
                    foreach (Bitmap bmp in iconCacheByFile.Values)
                    {
                        if (bmp != null)
                        {
                            bmp.Dispose();
                        }
                    }
                    iconCacheByFile.Clear();
                }
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
                toShowMessageBox(strQuestion, strTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dialogResult == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        private void toolStripMenuItem_About_Click(object sender, EventArgs e)
        {
            aboutForm objAboutForm = new aboutForm(this.win_Language, toIsDarkModeByConfig());
            objAboutForm.ShowDialog(this);
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

        private void toolStripMenuItem_ProgramIcon_On_Click(object sender, EventArgs e)
        {
            this.toolStripMenuItem_ProgramIcon_On.Checked = true;
            this.toolStripMenuItem_ProgramIcon_Off.Checked = false;
            OperateIniFile.WriteIniData(System_Info, MainForm_ShowProgramIcon, "On", strIniFilePath);
            this.RebuildButtonIcons();
        }

        private void toolStripMenuItem_ProgramIcon_Off_Click(object sender, EventArgs e)
        {
            this.toolStripMenuItem_ProgramIcon_On.Checked = false;
            this.toolStripMenuItem_ProgramIcon_Off.Checked = true;
            OperateIniFile.WriteIniData(System_Info, MainForm_ShowProgramIcon, "Off", strIniFilePath);
            this.RebuildButtonIcons();
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
                
                toShowMessageBox(strMessage, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                System.Environment.Exit(0);
            }
        }

        // 提取行中的段名（] 之后的内容忽略，与 Win32 profile API 行为一致）；非段行返回 null
        private static string toSectionNameOfLine(string line)
        {
            string t = line.Trim();
            if (t.Length < 2 || t[0] != '[')
            {
                return null;
            }

            int iEnd = t.IndexOf(']');
            if (iEnd < 1)
            {
                return null;
            }

            return t.Substring(1, iEnd - 1).Trim();
        }

        // ini 缺失 [System_Info] 段时，在文件第一行插入与模板完全一致的完整块：
        // "System Config" 注释横幅 + [System_Info] 段 + 5 个键（固定顺序），原有内容整体下移并保持不变；
        // 横幅注释行天然接住 UTF-8 BOM（BOM 粘在注释上无害），避免 BOM 粘进段名导致 Win32 API 找不到段；
        // 段已存在但位于第一行且文件带 BOM 时（API 认不出段名），仅在其上方补插横幅注释；其余情况不做任何修改；
        // 文件不是合法 UTF-8（如 ANSI/GBK）或只读/被占用时跳过处理，避免损坏原内容或启动崩溃
        private void toEnsureSystemInfoSection()
        {
            try
            {
                if (!File.Exists(this.strIniFilePath))
                {
                    return;
                }

                byte[] raw = File.ReadAllBytes(this.strIniFilePath);
                bool bHasBom = raw.Length >= 3 && raw[0] == 0xEF && raw[1] == 0xBB && raw[2] == 0xBF;

                // 严格 UTF-8 解码：失败说明是 ANSI/GBK 等其他编码，跳过（宽松解码产生的替换符重写文件会损坏中文内容）
                string text;
                try
                {
                    text = new UTF8Encoding(false, true).GetString(raw, bHasBom ? 3 : 0, raw.Length - (bHasBom ? 3 : 0));
                }
                catch (DecoderFallbackException)
                {
                    return;
                }

                string[] lines = text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

                int iSection = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if ("System_Info".Equals(toSectionNameOfLine(lines[i]), StringComparison.OrdinalIgnoreCase))
                    {
                        iSection = i;
                        break;
                    }
                }

                string banner = new string('#', 90);

                if (iSection >= 0)
                {
                    // BOM 陷阱：[System_Info] 位于第一行且文件带 BOM 时，API 把 BOM 字节读进段名而找不到该段，
                    // 后续所有 API 写入都会变成在文件末尾追加重复段；在段上方补插横幅注释行接住 BOM
                    if (iSection == 0 && bHasBom)
                    {
                        List<string> bannerLines = new List<string>(lines);
                        bannerLines.InsertRange(0, new string[] { banner, "#    System Config", banner });
                        File.WriteAllText(this.strIniFilePath, string.Join("\r\n", bannerLines.ToArray()), new UTF8Encoding(true));
                    }
                    return;
                }

                List<string> block = new List<string>();
                block.Add(banner);
                block.Add("#    System Config");
                block.Add(banner);
                block.Add("[System_Info]");
                block.Add(MainForm_Opacity + "=100");
                block.Add(MainForm_Language + "=en-US");
                block.Add(MainForm_Timer + "=On");
                block.Add(MainForm_AutoRun + "=On");
                block.Add(MainForm_ShowProgramIcon + "=On");
                block.Add(MainForm_Theme + "=Auto");
                if (lines.Length == 0 || lines[0].Trim().Length != 0)
                {
                    block.Add(string.Empty); // 与模板一致：块后空一行（原文件首行已是空行则不重复加）
                }

                List<string> outLines = new List<string>(lines);
                outLines.InsertRange(0, block);

                File.WriteAllText(this.strIniFilePath, string.Join("\r\n", outLines.ToArray()), new UTF8Encoding(bHasBom));
            }
            catch (Exception)
            {
                // 文件只读或被其他进程占用：跳过补全，后续 API 写入按原行为处理
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

                toShowMessageBox(strMessage, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
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

        // 旧版 ini 缺失 MainForm_ShowProgramIcon 键时自动补写默认值 On（与 Timer/AutoRun 惯例一致；仅启动时写回，热重载不写文件）
        private void InitShowProgramIcon()
        {
            string strShowIcon = OperateIniFile.ReadIniData(System_Info, MainForm_ShowProgramIcon, "", strIniFilePath);
            if (strShowIcon == "")
            {
                OperateIniFile.WriteIniData(System_Info, MainForm_ShowProgramIcon, "On", strIniFilePath);
            }
            this.toUpdateProgramIconMenuCheck();
        }

        // 按 ini 当前值同步托盘菜单"程序图标"的勾选状态（启动与热重载时调用；空/非法值按 On 处理）
        private void toUpdateProgramIconMenuCheck()
        {
            string strShowIcon = OperateIniFile.ReadIniData(System_Info, MainForm_ShowProgramIcon, "On", strIniFilePath).Trim().ToUpper();
            if ("OFF".Equals(strShowIcon))
            {
                this.toolStripMenuItem_ProgramIcon_On.Checked = false;
                this.toolStripMenuItem_ProgramIcon_Off.Checked = true;
            }
            else
            {
                this.toolStripMenuItem_ProgramIcon_On.Checked = true;
                this.toolStripMenuItem_ProgramIcon_Off.Checked = false;
            }
        }

        // 旧版 ini 缺失 MainForm_Theme 键时自动补写默认值 Auto（与 Timer/AutoRun 惯例一致；仅启动时写回，热重载不写文件）
        private void InitTheme()
        {
            string strTheme = OperateIniFile.ReadIniData(System_Info, MainForm_Theme, "", strIniFilePath);
            if (strTheme == "")
            {
                strTheme = "Auto";
                OperateIniFile.WriteIniData(System_Info, MainForm_Theme, "Auto", strIniFilePath);
            }
            this.strThemeConfig = strTheme.Trim();
        }

        // 应用主题：解析 Auto/Light/Dark → 计算生效明暗 → 窗体背景 + 标题栏 + 按钮重绘 + 菜单勾选
        private void ApplyTheme()
        {
            if (this.colorLightFormBack.IsEmpty)
            {
                this.colorLightFormBack = this.BackColor; // 首次记录设计器亮色背景
            }

            bool dark = toIsDarkModeByConfig();

            this.bDarkMode = dark;
            this.BackColor = dark ? Color.FromArgb(32, 32, 32) : this.colorLightFormBack;
            this.toApplyTitleBarTheme(dark);
            this.toApplyMenuTheme(dark);
            this.toUpdateThemeMenuCheck();
            this.toInvalidateButtons();
        }

        // 托盘菜单明暗：WinForms 菜单是自绘的（非系统原生），暗色时换暗色渲染器，亮色时还原默认
        private void toApplyMenuTheme(bool dark)
        {
            if (this.rendererMenuDefault == null)
            {
                this.rendererMenuDefault = this.contextMenuStripMain.Renderer;
                this.colorMenuBack = this.contextMenuStripMain.BackColor;
                this.colorMenuFore = this.contextMenuStripMain.ForeColor;
            }

            if (dark)
            {
                if (this.darkMenuRenderer == null)
                {
                    this.darkMenuRenderer = new DarkMenuRenderer();
                }
                this.contextMenuStripMain.BackColor = Color.FromArgb(43, 43, 43);
                this.contextMenuStripMain.ForeColor = Color.FromArgb(240, 240, 240);
                this.contextMenuStripMain.Renderer = this.darkMenuRenderer;
            }
            else
            {
                this.contextMenuStripMain.Renderer = this.rendererMenuDefault;
                this.contextMenuStripMain.BackColor = this.colorMenuBack;
                this.contextMenuStripMain.ForeColor = this.colorMenuFore;
            }
        }

        // 按当前主题配置计算是否暗色（Auto/未加载时跟随系统）；不依赖控件状态，ApplyTheme 之前也可安全调用
        private bool toIsDarkModeByConfig()
        {
            string cfg = (this.strThemeConfig ?? "Auto").Trim();
            if ("Light".Equals(cfg, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if ("Dark".Equals(cfg, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return ThemeHelper.IsSystemDarkMode(); // Auto（含非法值）跟随系统
        }

        // 标题栏明暗（实现见 ThemeHelper，About 窗体共用）
        private void toApplyTitleBarTheme(bool dark)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }
            ThemeHelper.ApplyTitleBarTheme(this.Handle, dark);
        }

        private void toUpdateThemeMenuCheck()
        {
            string cfg = (this.strThemeConfig ?? "Auto").Trim();
            this.toolStripMenuItem_Theme_Auto.Checked = !"Light".Equals(cfg, StringComparison.OrdinalIgnoreCase) && !"Dark".Equals(cfg, StringComparison.OrdinalIgnoreCase);
            this.toolStripMenuItem_Theme_Light.Checked = "Light".Equals(cfg, StringComparison.OrdinalIgnoreCase);
            this.toolStripMenuItem_Theme_Dark.Checked = "Dark".Equals(cfg, StringComparison.OrdinalIgnoreCase);
        }

        // 托盘菜单切换主题：写回 ini（热重载会再次读到同值，无冲突）并立即应用
        private void toSetTheme(string strMode)
        {
            this.strThemeConfig = strMode;
            OperateIniFile.WriteIniData(System_Info, MainForm_Theme, strMode, strIniFilePath);
            this.ApplyTheme();
        }

        private void toolStripMenuItem_Theme_Auto_Click(object sender, EventArgs e)
        {
            this.toSetTheme("Auto");
        }

        private void toolStripMenuItem_Theme_Light_Click(object sender, EventArgs e)
        {
            this.toSetTheme("Light");
        }

        private void toolStripMenuItem_Theme_Dark_Click(object sender, EventArgs e)
        {
            this.toSetTheme("Dark");
        }

        // 颜色混合：ratioB 为 b 的占比（0~1）
        private static Color toBlend(Color a, Color b, float ratioB)
        {
            int r = (int)Math.Round(a.R + (b.R - a.R) * ratioB);
            int g = (int)Math.Round(a.G + (b.G - a.G) * ratioB);
            int bl = (int)Math.Round(a.B + (b.B - a.B) * ratioB);
            return Color.FromArgb(a.A, r, g, bl);
        }

        // 暗色模式按钮底色：默认白色 → 深灰面板；用户显式配置色 → 与深灰混合保留色相
        private static Color toDarkButtonColor(Color baseColor)
        {
            if (baseColor.ToArgb() == Color.White.ToArgb())
            {
                return Color.FromArgb(62, 62, 62);
            }
            return toBlend(baseColor, Color.FromArgb(48, 48, 48), 0.72f);
        }

        // 暗色模式文字色：默认黑 → 浅灰白；用户显式配置色 → 向白提亮保留色相
        private static Color toDarkTextColor(Color textColor)
        {
            if (textColor.ToArgb() == SystemColors.ControlText.ToArgb())
            {
                return Color.FromArgb(240, 240, 240);
            }
            return toBlend(textColor, Color.FromArgb(250, 250, 250), 0.62f);
        }

        // 按当前主题弹框：亮色用系统原生 MessageBox，暗色用自绘暗色对话框
        // 用配置实时计算（而非 bDarkMode），保证启动阶段 ApplyTheme 之前的弹框也按主题选色
        private DialogResult toShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            if (toIsDarkModeByConfig())
            {
                return DarkMessageBox.Show(this, text, caption, buttons, icon, defaultButton, this.win_Language);
            }
            return MessageBox.Show(this, text, caption, buttons, icon, defaultButton);
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

        // ini 热重载：外部修改 ini 后自动重新解析并刷新按钮，无需重启
        private FileSystemWatcher iniWatcher = null;
        private System.Windows.Forms.Timer iniReloadTimer = null;

        private void InitConfig()
        {
            this.toParseIni();

            // 旧版 ini 按钮段少于 BUTTON_MAX_SIZE（如 40/48 个），补齐缺失的 [Button_XX] 段（默认隐藏）并回写文件
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

        // 解析 ini 全部按钮段到 lstSections/lstConfig（启动与热重载共用）
        private void toParseIni()
        {
            // 先解析到局部列表，全部成功后才整体替换：解析中途抛异常（文件被编辑器占用等）时保持原有配置与按钮不变，
            // 避免 lstConfig 被清空而按钮仍在 → 点击触发 lstConfig 越界崩溃
            List<IniSection> newSections = new List<IniSection>();
            List<IniConfig> newConfig = new List<IniConfig>();

            string strSection = null;
            IniConfig ini = null;

            using (StreamReader streamReader = new StreamReader(strIniFilePath, Encoding.UTF8))
            {
            string content;
            while ((content = streamReader.ReadLine()) != null)
            {
                string strLineSection = toSectionNameOfLine(content);
                if (strLineSection != null && strLineSection.StartsWith("Button_", StringComparison.OrdinalIgnoreCase))
                {
                    ini = new IniConfig();
                    strSection = strLineSection;

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
                    newSections.Add(section);

                    if (section.Visible)
                    {
                        newConfig.Add(ini);
                    }
                }
            }

            }

            // 解析结果为空但此前有配置：极可能读到截断/半写状态的文件（编辑器保存瞬间），
            // 视为可疑读取，抛异常让调用方保持原配置并稍后重试，避免按钮全部消失
            if (newSections.Count == 0 && lstSections.Count > 0)
            {
                throw new IOException("ini parse found no button sections (suspected partial read)");
            }

            // 解析全程无异常，整体替换生效
            this.lstSections.Clear();
            this.lstSections.AddRange(newSections);
            lstConfig.Clear();
            lstConfig.AddRange(newConfig);

            strSection = null;
            ini = null;
        }

        // 启动 ini 文件监视：外部修改后防抖 600ms 自动重载；重载只读不写文件，不会自激循环
        private void toStartIniWatcher()
        {
            try
            {
                this.iniWatcher = new FileSystemWatcher();
                this.iniWatcher.Path = Path.GetDirectoryName(this.strIniFilePath);
                this.iniWatcher.Filter = Path.GetFileName(this.strIniFilePath);
                this.iniWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;
                this.iniWatcher.Changed += new FileSystemEventHandler(this.iniFile_Changed);
                this.iniWatcher.Created += new FileSystemEventHandler(this.iniFile_Changed);
                this.iniWatcher.Renamed += (s, ev) => this.iniFile_Changed(s, ev);
                this.iniWatcher.EnableRaisingEvents = true;
            }
            catch (Exception)
            {
                // 无法建立监视时退回旧行为（仅启动时读取）
            }
        }

        private void iniFile_Changed(object sender, FileSystemEventArgs e)
        {
            // watcher 回调在非 UI 线程，切回 UI 线程操作定时器
            if (!this.IsHandleCreated)
            {
                return;
            }

            try
            {
                this.BeginInvoke(new Action(() =>
                {
                    if (this.iniReloadTimer == null)
                    {
                        this.iniReloadTimer = new System.Windows.Forms.Timer();
                        this.iniReloadTimer.Interval = 600;
                        this.iniReloadTimer.Tick += new EventHandler(this.iniReloadTimer_Tick);
                    }
                    this.iniReloadTimer.Stop();
                    this.iniReloadTimer.Start();
                }));
            }
            catch (Exception)
            {
                // 关窗瞬间句柄可能已销毁（InvalidOperationException），忽略即可
            }
        }

        private void iniReloadTimer_Tick(object sender, EventArgs e)
        {
            this.iniReloadTimer.Stop();

            // 文件被删除/改名移走：保持界面现状，停止重试。
            // 若继续运行，后续托盘设置(透明度/语言等)的 ini 写入会新建一个不含按钮段的空 ini，导致配置丢失
            if (!File.Exists(this.strIniFilePath))
            {
                return;
            }

            // 拖拽换位进行中暂不重载，稍后再试
            if (this.bDragDropActive)
            {
                this.iniReloadTimer.Start();
                return;
            }

            try
            {
                this.toReloadConfig();
            }
            catch (Exception)
            {
                // 文件可能仍被编辑器占用，稍后重试
                this.iniReloadTimer.Start();
            }
        }

        private void toReloadConfig()
        {
            this.toParseIni();

            // ini 里改了主题配置则同步应用（Auto/Light/Dark 热切换）
            string strTheme = OperateIniFile.ReadIniData(System_Info, MainForm_Theme, "Auto", strIniFilePath).Trim();
            if (!strTheme.Equals(this.strThemeConfig ?? "Auto", StringComparison.OrdinalIgnoreCase))
            {
                this.strThemeConfig = strTheme;
                this.ApplyTheme();
            }

            this.RebuildButtonIcons();
            this.toUpdateProgramIconMenuCheck();
            this.RenderButtons();
            this.toRecalcClientSize();
        }

        private void InitForm()
        {
            this.Text = "QuickStart  " + Properties.Resources.Program_Build_Version_Text;

            this.toRecalcClientSize();

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

        // 按当前可视按钮数量重算窗口客户区大小（启动与热重载共用）
        private void toRecalcClientSize()
        {
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
        }

        private void InitButton()
        {
            this.RenderButtons();
        }

        // 重建按钮程序图标（启动与 ini 热重载时调用）：提取在后台线程执行，不阻塞窗体显示，
        // 窗体先出现、图标随后数十毫秒内浮现；结果按代数校验，过期结果自动丢弃
        private void RebuildButtonIcons()
        {
            // ini 开关：[System_Info] 段 MainForm_ShowProgramIcon=Off 时不显示按钮程序图标（空/非法值按 On 处理）
            string strShowIcon = OperateIniFile.ReadIniData(System_Info, MainForm_ShowProgramIcon, "On", strIniFilePath).Trim().ToUpper();

            // 快照当前按钮→程序配置（lstConfig 是 UI 线程状态，后台线程不得直接读取）
            List<Button> snapshotButtons = new List<Button>();
            List<string> snapshotPrograms = new List<string>();
            if (!"OFF".Equals(strShowIcon))
            {
                int iButtonCount = lstConfig.Count < BUTTON_MAX_SIZE ? lstConfig.Count : BUTTON_MAX_SIZE;
                for (int i = 0; i < iButtonCount; i++)
                {
                    snapshotButtons.Add(this.buttonList[i]);
                    snapshotPrograms.Add(lstConfig[i].Program_Name);
                }
            }

            this.iconGeneration++;
            int iGen = this.iconGeneration;

            // 分工原则（实测结论）：SHGetFileInfo 的图标句柄在后台线程获取是安全的（纯 shell32，耗时大头），
            // 但 Icon/GDI+ 对象的创建与 Dispose 在后台线程会触发 AccessViolationException（与 UI 线程消息泵存在隐性耦合），
            // 因此句柄→Bitmap 的转换统一回到 UI 线程执行；重复重建时旧线程结果由代数校验丢弃
            System.Threading.Thread extractThread = new System.Threading.Thread(() =>
            {
                List<PendingIcon> pending = new List<PendingIcon>();
                try
                {
                    Dictionary<Button, Bitmap> extracted = new Dictionary<Button, Bitmap>();
                    for (int i = 0; i < snapshotButtons.Count; i++)
                    {
                        string strPath = ProgramIcon.ResolveProgramPath(snapshotPrograms[i]);
                        string strKey = toGetIconCacheKey(strPath);
                        if (strKey == null)
                        {
                            continue;
                        }

                        // 先查跨重建缓存（热重载时图标直接复用，零提取）
                        Bitmap cached;
                        lock (iconCacheLock)
                        {
                            iconCacheByFile.TryGetValue(strKey, out cached);
                        }
                        if (cached != null)
                        {
                            extracted[snapshotButtons[i]] = cached;
                            continue;
                        }

                        IntPtr hIcon = ProgramIcon.ExtractIconHandle(strPath);
                        if (hIcon != IntPtr.Zero)
                        {
                            pending.Add(new PendingIcon(snapshotButtons[i], hIcon, strKey));
                        }
                    }

                    bool posted = false;
                    if (this.IsHandleCreated)
                    {
                        try
                        {
                            this.BeginInvoke(new Action(() => this.toConvertAndApplyIcons(extracted, pending, iGen)));
                            posted = true;
                        }
                        catch (Exception)
                        {
                            // 关窗瞬间句柄可能已销毁，走下方统一释放
                        }
                    }

                    if (!posted)
                    {
                        foreach (PendingIcon p in pending)
                        {
                            ProgramIcon.ReleaseIconHandle(p.hIcon);
                        }
                        pending.Clear();
                    }
                }
                catch (Exception)
                {
                    // 后台提取的任何异常都不允许影响主程序（结果丢弃即可）
                    foreach (PendingIcon p in pending)
                    {
                        ProgramIcon.ReleaseIconHandle(p.hIcon);
                    }
                }
            });
            extractThread.IsBackground = true;
            extractThread.SetApartmentState(System.Threading.ApartmentState.STA);
            extractThread.Start();
        }

        // 后台线程提取的中间结果：按钮 + 待转换的 shell 图标句柄 + 缓存键
        private sealed class PendingIcon
        {
            public Button btn;
            public IntPtr hIcon;
            public string strKey;

            public PendingIcon(Button btn_, IntPtr hIcon_, string strKey_)
            {
                this.btn = btn_;
                this.hIcon = hIcon_;
                this.strKey = strKey_;
            }
        }

        // 缓存键含文件写入时间与大小，程序更新后图标自动重新提取；路径无效返回 null
        private static string toGetIconCacheKey(string strPath)
        {
            if (string.IsNullOrEmpty(strPath))
            {
                return null;
            }

            try
            {
                FileInfo fi = new FileInfo(strPath);
                return strPath + "|" + fi.LastWriteTimeUtc.Ticks + "|" + fi.Length;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // UI 线程：把后台取回的图标句柄转换为 Bitmap（GDI+ 只允许在 UI 线程触碰）、回填缓存并应用
        private void toConvertAndApplyIcons(Dictionary<Button, Bitmap> extracted, List<PendingIcon> pending, int iGen)
        {
            // 窗体退出中：只释放句柄，不再触碰已释放的缓存与按钮映射
            if (this.bIconsExiting)
            {
                foreach (PendingIcon p in pending)
                {
                    ProgramIcon.ReleaseIconHandle(p.hIcon);
                }
                return;
            }

            foreach (PendingIcon p in pending)
            {
                Bitmap bmp = ProgramIcon.IconHandleToBitmap(p.hIcon);
                ProgramIcon.ReleaseIconHandle(p.hIcon);

                if (bmp != null)
                {
                    lock (iconCacheLock)
                    {
                        // 并发重建可能对同一文件各提取了一份：保留先入缓存的，丢弃重复的，避免孤儿 Bitmap
                        Bitmap existing;
                        if (iconCacheByFile.TryGetValue(p.strKey, out existing) && existing != null)
                        {
                            bmp.Dispose();
                            bmp = existing;
                        }
                        else
                        {
                            iconCacheByFile[p.strKey] = bmp;
                        }
                    }
                    extracted[p.btn] = bmp;
                }
            }

            this.toApplyButtonIcons(extracted, iGen);
        }

        // UI 线程：应用后台提取结果（代数已变或窗体退出中则丢弃过期结果）；整体替换映射后强制重绘按钮
        private void toApplyButtonIcons(Dictionary<Button, Bitmap> extracted, int iGen)
        {
            if (this.bIconsExiting || iGen != this.iconGeneration)
            {
                return;
            }

            this.buttonIcons = extracted;
            this.toInvalidateButtons();
        }

        private void RenderButtons()
        {
            for (int i = 0; i < BUTTON_MAX_SIZE; i++)
            {
                Button btn = this.buttonList[i];

                if (i < lstConfig.Count())
                {
                    IniConfig ini = lstConfig[i];

                    btn.Text = this.toDisplayText(ini.Button_Name);
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

        // 按钮文字手动换行：ini 里写 </br>、<br>、<br/> 或字面 \n（反斜杠+n）都会转成真实换行显示
        private string toDisplayText(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            return name.Replace("</br>", "\n").Replace("<br/>", "\n").Replace("<br>", "\n").Replace(@"\n", "\n");
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
                this.toRestartEditIdle();
                this.Text = "QuickStart  " + Properties.Resources.Program_Build_Version_Text + "  [" + this.toolStripMenuItem_EditMode.Text + "]";
            }
            else
            {
                this.toStopJiggle();
                this.toStopEditIdle();
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
            // 编辑模式下右键任意处（含按钮）立即退出编辑模式
            if (this.bEditMode && e.Button == MouseButtons.Right)
            {
                this.toExitEditModeByRightClick((Button)sender);
                return;
            }

            // 按下反馈：记录按下态并重绘
            if (e.Button == MouseButtons.Left)
            {
                this.pressedButtons.Add((Button)sender);
                ((Button)sender).Invalidate();

                // 非编辑模式下开始长按计时，满 3 秒自动进入编辑模式
                if (!this.bEditMode)
                {
                    this.toStartLongPress((Button)sender);
                }
            }

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

                // 拖拽期间 MouseUp 会被拖拽循环吞掉，这里清掉按下态避免卡住
                if (this.pressedButtons.Remove(source))
                {
                    source.Invalidate();
                }
                if (ReferenceEquals(this.hoverAnimButton, source))
                {
                    this.hoverAnimButton = null;
                    if (this.hoverAnimTimer != null)
                    {
                        this.hoverAnimTimer.Stop();
                    }
                }

                this.bDragDropActive = true;
                source.DoDragDrop(source, DragDropEffects.Move);
                this.bDragDropActive = false;
            }
        }

        private void button_MouseUp(object sender, MouseEventArgs e)
        {
            this.toCancelLongPress();

            Button btn = (Button)sender;
            if (this.pressedButtons.Remove(btn))
            {
                btn.Invalidate();
            }
            this.dragSourceButton = null;
        }

        // 丢失鼠标捕获（如按下左键时 Alt+Tab 切走）不会产生 MouseUp/MouseLeave，这里兜底清除按下态。
        // 该事件在「获得」捕获时也会触发（此时 Capture 为 true），需跳过，否则会在按下瞬间把按下反馈清掉
        private void button_MouseCaptureChanged(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Capture)
            {
                return;
            }

            this.toCancelLongPress();
            if (this.pressedButtons.Remove(btn))
            {
                btn.Invalidate();
            }
        }

        // 编辑模式下右键窗体内任意位置立即退出编辑模式（无需等 8 秒自动退出）
        private void mainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.bEditMode && e.Button == MouseButtons.Right)
            {
                this.toExitEditModeByRightClick(null);
            }
        }

        // 右键退出编辑模式：先清掉按钮按下态，避免退出后卡在凹陷效果
        private void toExitEditModeByRightClick(Button btn)
        {
            if (null != btn && this.pressedButtons.Remove(btn))
            {
                btn.Invalidate();
            }
            foreach (Button b in this.pressedButtons)
            {
                b.Invalidate();
            }
            this.pressedButtons.Clear();

            this.SetEditMode(false);
        }

        // 按下后指针移出按钮（或在按钮外松开）时清除按下态，避免卡在凹陷效果
        private void button_MouseLeave3D(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (ReferenceEquals(this.longPressButton, btn))
            {
                this.toCancelLongPress();
            }
            if (ReferenceEquals(this.hoverAnimButton, btn))
            {
                this.hoverAnimButton = null;
                if (this.hoverAnimTimer != null)
                {
                    this.hoverAnimTimer.Stop();
                }
            }
            if (this.pressedButtons.Remove(btn))
            {
                btn.Invalidate();
            }
        }

        // 悬停开始：启动定时器持续重绘，Paint 里按 TickCount 计算文字浮动偏移
        private void button_MouseEnter3D(object sender, EventArgs e)
        {
            this.hoverAnimButton = (Button)sender;
            if (this.hoverAnimTimer == null)
            {
                this.hoverAnimTimer = new System.Windows.Forms.Timer();
                this.hoverAnimTimer.Interval = 30;
                this.hoverAnimTimer.Tick += (s, ev) =>
                {
                    if (this.hoverAnimButton == null)
                    {
                        this.hoverAnimTimer.Stop();
                    }
                    else
                    {
                        this.hoverAnimButton.Invalidate();
                    }
                };
            }
            this.hoverAnimTimer.Start();
        }

        // 长按进入编辑模式：3 秒计时，期间松开/移出按钮则取消
        private void toStartLongPress(Button btn)
        {
            this.longPressButton = btn;
            if (this.longPressTimer == null)
            {
                this.longPressTimer = new System.Windows.Forms.Timer();
                this.longPressTimer.Interval = 3000;
                this.longPressTimer.Tick += (s, ev) =>
                {
                    this.longPressTimer.Stop();
                    if (this.longPressButton != null && this.pressedButtons.Contains(this.longPressButton))
                    {
                        this.longPressButton = null;
                        this.SetEditMode(true);
                    }
                };
            }
            this.longPressTimer.Start();
        }

        private void toCancelLongPress()
        {
            this.longPressButton = null;
            if (this.longPressTimer != null)
            {
                this.longPressTimer.Stop();
            }
        }

        // 编辑模式闲置计时：8 秒无按钮顺序变动则退出；每次换位后重置
        private void toRestartEditIdle()
        {
            if (this.editIdleTimer == null)
            {
                this.editIdleTimer = new System.Windows.Forms.Timer();
                this.editIdleTimer.Interval = 8000;
                this.editIdleTimer.Tick += (s, ev) =>
                {
                    if (this.bDragDropActive)
                    {
                        this.editIdleTimer.Start();
                        return;
                    }
                    this.SetEditMode(false);
                };
            }
            this.editIdleTimer.Stop();
            this.editIdleTimer.Start();
        }

        private void toStopEditIdle()
        {
            if (this.editIdleTimer != null)
            {
                this.editIdleTimer.Stop();
            }
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

            this.toRemapButtonIconsAfterReorder(iFrom, iTo);

            this.RenderButtons();
            this.toSaveButtonOrder();

            // 顺序有变动，重置 8 秒自动退出计时
            this.toRestartEditIdle();
        }

        // 拖拽换位后同步重排图标映射：buttonIcons 以按钮控件为键，若不随 lstConfig 一起换位，
        // 图标会留在旧位置，直到 ini 热重载（约 600ms 防抖）才被纠正，视觉上拖拽结果"停顿一下"；
        // 纯字典换位零提取开销，图标与文字同帧更新。同时递增代数，丢弃换位前在途的后台提取结果，
        // 避免过期结果回填旧映射
        private void toRemapButtonIconsAfterReorder(int iFrom, int iTo)
        {
            this.iconGeneration++;

            if (this.buttonIcons.Count == 0)
            {
                return;
            }

            List<Bitmap> iconsByIndex = new List<Bitmap>();
            foreach (Button btn in this.buttonList)
            {
                Bitmap bmp;
                this.buttonIcons.TryGetValue(btn, out bmp);
                iconsByIndex.Add(bmp);
            }

            Bitmap movedIcon = iconsByIndex[iFrom];
            iconsByIndex.RemoveAt(iFrom);
            iconsByIndex.Insert(iTo, movedIcon);

            Dictionary<Button, Bitmap> remapped = new Dictionary<Button, Bitmap>();
            for (int i = 0; i < this.buttonList.Count; i++)
            {
                if (iconsByIndex[i] != null)
                {
                    remapped[this.buttonList[i]] = iconsByIndex[i];
                }
            }

            this.buttonIcons = remapped;
            this.toInvalidateButtons();
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
                    toShowMessageBox(ini.Program_Name + " " + ini.Program_Param + "\n\n" + ex.Message, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            else if ("cmd".Equals(ini.Program_Type))
            {
                try
                {
                    RunCommand.RunCmd(ini.Program_Name, ini.Program_Param);
                } catch (Exception ex)
                {
                    toShowMessageBox(ini.Program_Name + " " + ini.Program_Param + "\n\n" + ex.Message, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                toShowMessageBox(strQuestion + "\n【" + ini.Program_Type + "】", strTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
                    this.toolStripMenuItem_Theme.Text = Resources.Resource_zh_CN.toolStripMenuItem_Theme_Text;
                    this.toolStripMenuItem_Theme_Auto.Text = Resources.Resource_zh_CN.toolStripMenuItem_Theme_Auto_Text;
                    this.toolStripMenuItem_Theme_Light.Text = Resources.Resource_zh_CN.toolStripMenuItem_Theme_Light_Text;
                    this.toolStripMenuItem_Theme_Dark.Text = Resources.Resource_zh_CN.toolStripMenuItem_Theme_Dark_Text;
                    this.toolStripMenuItem_ProgramIcon.Text = Resources.Resource_zh_CN.toolStripMenuItem_ProgramIcon_Text;
                    this.toolStripMenuItem_ProgramIcon_On.Text = Resources.Resource_zh_CN.toolStripMenuItem_ProgramIcon_On_Text;
                    this.toolStripMenuItem_ProgramIcon_Off.Text = Resources.Resource_zh_CN.toolStripMenuItem_ProgramIcon_Off_Text;
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
                    this.toolStripMenuItem_Theme.Text = Resources.Resource_zh_TW.toolStripMenuItem_Theme_Text;
                    this.toolStripMenuItem_Theme_Auto.Text = Resources.Resource_zh_TW.toolStripMenuItem_Theme_Auto_Text;
                    this.toolStripMenuItem_Theme_Light.Text = Resources.Resource_zh_TW.toolStripMenuItem_Theme_Light_Text;
                    this.toolStripMenuItem_Theme_Dark.Text = Resources.Resource_zh_TW.toolStripMenuItem_Theme_Dark_Text;
                    this.toolStripMenuItem_ProgramIcon.Text = Resources.Resource_zh_TW.toolStripMenuItem_ProgramIcon_Text;
                    this.toolStripMenuItem_ProgramIcon_On.Text = Resources.Resource_zh_TW.toolStripMenuItem_ProgramIcon_On_Text;
                    this.toolStripMenuItem_ProgramIcon_Off.Text = Resources.Resource_zh_TW.toolStripMenuItem_ProgramIcon_Off_Text;
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
                    this.toolStripMenuItem_Theme.Text = Resources.Resource_en_US.toolStripMenuItem_Theme_Text;
                    this.toolStripMenuItem_Theme_Auto.Text = Resources.Resource_en_US.toolStripMenuItem_Theme_Auto_Text;
                    this.toolStripMenuItem_Theme_Light.Text = Resources.Resource_en_US.toolStripMenuItem_Theme_Light_Text;
                    this.toolStripMenuItem_Theme_Dark.Text = Resources.Resource_en_US.toolStripMenuItem_Theme_Dark_Text;
                    this.toolStripMenuItem_ProgramIcon.Text = Resources.Resource_en_US.toolStripMenuItem_ProgramIcon_Text;
                    this.toolStripMenuItem_ProgramIcon_On.Text = Resources.Resource_en_US.toolStripMenuItem_ProgramIcon_On_Text;
                    this.toolStripMenuItem_ProgramIcon_Off.Text = Resources.Resource_en_US.toolStripMenuItem_ProgramIcon_Off_Text;
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
                    this.toolStripMenuItem_Theme.Text = Resources.Resource_ja_JP.toolStripMenuItem_Theme_Text;
                    this.toolStripMenuItem_Theme_Auto.Text = Resources.Resource_ja_JP.toolStripMenuItem_Theme_Auto_Text;
                    this.toolStripMenuItem_Theme_Light.Text = Resources.Resource_ja_JP.toolStripMenuItem_Theme_Light_Text;
                    this.toolStripMenuItem_Theme_Dark.Text = Resources.Resource_ja_JP.toolStripMenuItem_Theme_Dark_Text;
                    this.toolStripMenuItem_ProgramIcon.Text = Resources.Resource_ja_JP.toolStripMenuItem_ProgramIcon_Text;
                    this.toolStripMenuItem_ProgramIcon_On.Text = Resources.Resource_ja_JP.toolStripMenuItem_ProgramIcon_On_Text;
                    this.toolStripMenuItem_ProgramIcon_Off.Text = Resources.Resource_ja_JP.toolStripMenuItem_ProgramIcon_Off_Text;
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
                    this.toolStripMenuItem_Theme.Text = Resources.Resource_ko_KR.toolStripMenuItem_Theme_Text;
                    this.toolStripMenuItem_Theme_Auto.Text = Resources.Resource_ko_KR.toolStripMenuItem_Theme_Auto_Text;
                    this.toolStripMenuItem_Theme_Light.Text = Resources.Resource_ko_KR.toolStripMenuItem_Theme_Light_Text;
                    this.toolStripMenuItem_Theme_Dark.Text = Resources.Resource_ko_KR.toolStripMenuItem_Theme_Dark_Text;
                    this.toolStripMenuItem_ProgramIcon.Text = Resources.Resource_ko_KR.toolStripMenuItem_ProgramIcon_Text;
                    this.toolStripMenuItem_ProgramIcon_On.Text = Resources.Resource_ko_KR.toolStripMenuItem_ProgramIcon_On_Text;
                    this.toolStripMenuItem_ProgramIcon_Off.Text = Resources.Resource_ko_KR.toolStripMenuItem_ProgramIcon_Off_Text;
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

            // 系统明暗主题实时切换：设置变更广播 WM_SETTINGCHANGE(ImmersiveColorSet) 时立即重应用（Auto 模式下生效）
            const int WM_SETTINGCHANGE = 0x001A;
            if (m.Msg == WM_SETTINGCHANGE && m.LParam != IntPtr.Zero)
            {
                try
                {
                    string section = Marshal.PtrToStringAuto(m.LParam);
                    if ("ImmersiveColorSet".Equals(section))
                    {
                        this.ApplyTheme();
                    }
                }
                catch (Exception)
                {
                }
            }

            base.WndProc(ref m);
        }

        private void toSetAutoRun(bool bAutoRunFlg)
        {
            using (RegistryKey registryKey = OperateRegistry.GetRegistryKeyByHelper(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (null == registryKey)
                {
                    return;
                }

                if (bAutoRunFlg == true)
                {
                    registryKey.SetValue(Process_RegItemName, "\"" + Process.GetCurrentProcess().MainModule.FileName + "\"",
                        RegistryValueKind.String);
                }
                else
                {
                    if (OperateRegistry.isExistItemKeyName(registryKey, Process_RegItemName) == true)
                    {
                        registryKey.DeleteValue(Process_RegItemName);
                    }
                }
            }
        }

        #endregion Me

    }
}
