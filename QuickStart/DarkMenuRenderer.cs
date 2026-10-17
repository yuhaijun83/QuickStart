using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QuickStart
{
    // 暗色托盘菜单配色表：与主窗体/暗色弹框同一套深灰系配色
    class DarkMenuColorTable : ProfessionalColorTable
    {
        internal static readonly Color ColorBack = Color.FromArgb(43, 43, 43);
        internal static readonly Color ColorHover = Color.FromArgb(66, 66, 66);
        private static readonly Color ColorBorder = Color.FromArgb(80, 80, 80);

        public override Color MenuBorder { get { return ColorBorder; } }
        public override Color MenuItemBorder { get { return Color.FromArgb(100, 100, 100); } }

        public override Color MenuItemSelected { get { return ColorHover; } }
        public override Color MenuItemSelectedGradientBegin { get { return ColorHover; } }
        public override Color MenuItemSelectedGradientEnd { get { return ColorHover; } }
        public override Color MenuItemPressedGradientBegin { get { return ColorBack; } }
        public override Color MenuItemPressedGradientMiddle { get { return ColorBack; } }
        public override Color MenuItemPressedGradientEnd { get { return ColorBack; } }

        public override Color ToolStripDropDownBackground { get { return ColorBack; } }
        public override Color ToolStripGradientBegin { get { return ColorBack; } }
        public override Color ToolStripGradientMiddle { get { return ColorBack; } }
        public override Color ToolStripGradientEnd { get { return ColorBack; } }
        public override Color ToolStripBorder { get { return ColorBorder; } }

        public override Color ImageMarginGradientBegin { get { return ColorBack; } }
        public override Color ImageMarginGradientMiddle { get { return ColorBack; } }
        public override Color ImageMarginGradientEnd { get { return ColorBack; } }
        public override Color ImageMarginRevealedGradientBegin { get { return ColorBack; } }
        public override Color ImageMarginRevealedGradientMiddle { get { return ColorBack; } }
        public override Color ImageMarginRevealedGradientEnd { get { return ColorBack; } }

        public override Color SeparatorDark { get { return Color.FromArgb(25, 25, 25); } }
        public override Color SeparatorLight { get { return Color.FromArgb(62, 62, 62); } }

        public override Color CheckBackground { get { return ColorBack; } }
        public override Color CheckPressedBackground { get { return ColorHover; } }
        public override Color CheckSelectedBackground { get { return ColorHover; } }
    }

    // 暗色托盘菜单渲染器：默认专业渲染器 + 暗色配色表 + 浅色对勾/子菜单箭头（默认是深色，暗底上看不见）
    class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        public DarkMenuRenderer()
            : base(new DarkMenuColorTable())
        {
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            Rectangle r = e.ImageRectangle;
            if (r.Width <= 0 || r.Height <= 0)
            {
                return;
            }

            SmoothingMode old = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen pen = new Pen(Color.FromArgb(240, 240, 240), 2f))
            {
                int x = r.Left + r.Width / 5;
                int y = r.Top + r.Height / 2;
                e.Graphics.DrawLines(pen, new Point[]
                {
                    new Point(x, y),
                    new Point(x + r.Width / 6, y + r.Height / 4),
                    new Point(x + r.Width / 2, y - r.Height / 4),
                });
            }
            e.Graphics.SmoothingMode = old;
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            Rectangle r = e.ArrowRectangle;
            if (r.Width <= 0 || r.Height <= 0)
            {
                return;
            }

            SmoothingMode old = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(200, 200, 200)))
            {
                Point[] pts;
                if (e.Direction == ArrowDirection.Right)
                {
                    pts = new Point[]
                    {
                        new Point(r.Left + r.Width / 4, r.Top + r.Height / 4),
                        new Point(r.Left + r.Width / 4, r.Bottom - r.Height / 4),
                        new Point(r.Right - r.Width / 4, r.Top + r.Height / 2),
                    };
                }
                else if (e.Direction == ArrowDirection.Left)
                {
                    pts = new Point[]
                    {
                        new Point(r.Right - r.Width / 4, r.Top + r.Height / 4),
                        new Point(r.Right - r.Width / 4, r.Bottom - r.Height / 4),
                        new Point(r.Left + r.Width / 4, r.Top + r.Height / 2),
                    };
                }
                else if (e.Direction == ArrowDirection.Up)
                {
                    pts = new Point[]
                    {
                        new Point(r.Left + r.Width / 4, r.Bottom - r.Height / 4),
                        new Point(r.Right - r.Width / 4, r.Bottom - r.Height / 4),
                        new Point(r.Left + r.Width / 2, r.Top + r.Height / 4),
                    };
                }
                else
                {
                    pts = new Point[]
                    {
                        new Point(r.Left + r.Width / 4, r.Top + r.Height / 4),
                        new Point(r.Right - r.Width / 4, r.Top + r.Height / 4),
                        new Point(r.Left + r.Width / 2, r.Bottom - r.Height / 4),
                    };
                }
                e.Graphics.FillPolygon(brush, pts);
            }
            e.Graphics.SmoothingMode = old;
        }
    }
}
