#define DEBUG

using FocusTree.UI.test;

namespace FocusTree.Graph
{
    /// <summary>
    /// 绘制栅格时，绘制非循环节点的委托类型
    /// </summary>
    /// <param name="g">传入栅格的 GDI</param>
    public delegate void CellDrawer(Bitmap image);
    /// <summary>
    /// 栅格
    /// </summary>
    static class Lattice
    {
        #region ==== 基本参数 ====

        /// <summary>
        /// 栅格行数（根据格元高自动生成）
        /// </summary>
        static int RowNumber;
        /// <summary>
        /// 栅格列数（根据格元宽自动生成）
        /// </summary>
        static int ColNumber;
        /// <summary>
        /// 栅格总列宽
        /// </summary>
        static int RowWidth;
        /// <summary>
        /// 栅格总行高
        /// </summary>
        static int ColHeight;
        /// <summary>
        /// 栅格绘图区域（根据给定放置区域、列数、行数自动生成，并在给定放置区域内居中）
        /// </summary>
        public static Rectangle DrawRect { get; private set; }
        /// <summary>
        /// 栅格坐标系原点 x 坐标
        /// </summary>
        public static int OriginLeft;
        /// <summary>
        /// 栅格坐标系原点 y 坐标
        /// </summary>
        public static int OriginTop;

        #endregion

        #region ==== 绘图工具 ====

        /// <summary>
        /// 格元边界绘制用笔
        /// </summary>
        public static Pen CellPen = new(Color.AliceBlue, 1.5f);
        /// <summary>
        /// 节点边界绘制用笔
        /// </summary>
        public static Pen NodePen = new(Color.FromArgb(150, Color.Orange), 1.5f);
        /// <summary>
        /// 坐标辅助线绘制用笔
        /// </summary>
        public static Pen GuidePen = new Pen(Color.FromArgb(200, Color.Red), 1.75f);
        /// <summary>
        /// 需要单独绘制的格元委托列表
        /// </summary>
        public static event CellDrawer Drawing;

        #endregion

        #region ==== 指示器 ====

        /// <summary>
        /// 是否绘制背景栅格
        /// </summary>
        public static bool DrawBackLattice = false;

        #endregion

        #region ==== 绘制栅格 ====

        /// <summary>
        /// 设置栅格放置区域（自动重置列行数）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds">放置区域</param>
        public static void SetBounds(Rectangle bounds)
        {
            ColNumber = bounds.Width / LatticeCell.Width;
            RowWidth = ColNumber * LatticeCell.Width;
            RowNumber = bounds.Height / LatticeCell.Height;
            ColHeight = RowNumber * LatticeCell.Height;
            var deviDiffWidth = (int)((float)(bounds.Width - RowWidth) * 0.5f);
            var deviDiffHeight = (int)((float)(bounds.Height - ColHeight) * 0.5f);
            DrawRect = new Rectangle(
                bounds.X + deviDiffWidth,
                bounds.Y + deviDiffHeight,
                RowWidth,
                ColHeight
                );
        }
        /// <summary>
        /// 绘制无限制栅格，并调用绘制格元的委托
        /// </summary>
        /// <param name="g"></param>
        public static void Draw(Image image)
        {
            Drawing?.Invoke((Bitmap)image);

            if (DrawBackLattice)
            {
                var g = Graphics.FromImage(image);
                for (int i = 0; i < ColNumber; i++)
                {
                    for (int j = 0; j < RowNumber; j++)
                    {
                        DrawLoopCell(g, i, j);
                    }
                }
                // guide line
                g.DrawLine(GuidePen, new(OriginLeft, DrawRect.Top), new(OriginLeft, DrawRect.Bottom));
                g.DrawLine(GuidePen, new(DrawRect.Left, OriginTop), new(DrawRect.Right, OriginTop));
                g.Flush(); g.Dispose();
            }
            if (Drawing == null) { return; }
            var delArray = Drawing.GetInvocationList();
        }
        /// <summary>
        /// 清空绘制委托
        /// </summary>
        public static void DrawingClear()
        {
            if (Drawing == null) { return; }
            var delArray = Drawing.GetInvocationList();
            foreach (var del in delArray)
            {
                Drawing -= del as CellDrawer;
            }
        }

        #endregion

        #region ==== 循环格元绘制方法 ====

        /// <summary>
        /// 绘制循环格元（格元左上角坐标与栅格坐标系中心偏移量近似投射在一个格元大小范围内）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="col">循环到的列数</param>
        /// <param name="row">循环到的行数</param>
        /// <param name="drawMain">是否绘制未超出栅格绘图区域的部分</param>
        /// <param name="drawAppend">是否补绘超出栅格绘图区域的部分</param>
        private static void DrawLoopCell(Graphics g, int col, int row)
        {
            var cellLeft = col * LatticeCell.Width + (OriginLeft) % LatticeCell.Width;
            var cellTop = row * LatticeCell.Height + (OriginTop) % LatticeCell.Height;
            DrawLoopCellLine(g, CellPen, new(cellLeft, cellTop), new(LatticeCell.Width, LatticeCell.Height));

            var nodeLeft = cellLeft + LatticeCell.NodePaddingWidth;
            var nodeTop = cellTop + LatticeCell.NodePaddingHeight;
            DrawLoopCellLine(g, NodePen, new(nodeLeft, nodeTop), new(LatticeCell.NodeWidth, LatticeCell.NodeHeight));
        }
        /// <summary>
        /// 绘制循环格元的边界线或节点线
        /// </summary>
        /// <param name="pen">绘制格元或节点用的笔刷</param>
        /// <param name="LeftTop">格元或节点的左上角坐标</param>
        /// <param name="size">格元或节点的大小</param>
        private static void DrawLoopCellLine(Graphics g, Pen pen, Point LeftTop, Size size)
        {
            var LeftRight = GetLoopedLineEnds(LeftTop.X, size.Width, (DrawRect.Left, DrawRect.Right), DrawRect.Width);
            var TopBottom = GetLoopedLineEnds(LeftTop.Y, size.Height, (DrawRect.Top, DrawRect.Bottom), DrawRect.Height);
            var left = LeftRight[0].Item1;
            var top = TopBottom[0].Item1;
            //
            // draw main: LeftBottom -> LeftTop -> TopRight
            //
            g.DrawLines(pen, new Point[]
            {
                        new(left, TopBottom[0].Item2),
                        new(left, top),
                        new(LeftRight[0].Item2, top)
            });
            //
            // draw append
            //
            if (LeftRight.Length > 1)
            {
                g.DrawLine(pen,
                    new(LeftRight[1].Item1, top),
                    new(LeftRight[1].Item2, top)
                    );
            }
            if (TopBottom.Length > 1)
            {
                g.DrawLine(pen,
                    new(left, TopBottom[1].Item1),
                    new(left, TopBottom[1].Item2)
                    );
            }
        }
        /// <summary>
        /// 得到转换为在栅格绘图区域内循环绘制后的格元或节点边界线的端点的横（纵）坐标数对
        /// </summary>
        /// <param name="head">起点横（纵）坐标（left or top）</param>
        /// <param name="length">线段长度</param>
        /// <param name="drBounds">head在DrawRect(dr)里的限制范围</param>
        /// <param name="drLength">DrawRect(dr)的宽度（长度）</param>
        /// <returns>转换后的坐标数对，前者是起点横（纵）坐标，后者是终点的。如果线需要分割，则返回数组里有一个额外的分割后另一部分线段的坐标数对。</returns>
        private static (int, int)[] GetLoopedLineEnds(int head, int length, (int, int) drBounds, int drLength)
        {
            var left_top = drBounds.Item1;
            var right_bottom = drBounds.Item2;

            if (head < left_top) { head += drLength; }
            else if (head > right_bottom) { head -= drLength; }
            var realWidth = right_bottom - head;
            if (realWidth >= length)
            {
                return new (int, int)[] { (head, head + length) };
            }
            else
            {
                return new (int, int)[]
                {
                    (head, head + realWidth),
                    (left_top, left_top + length - realWidth)
                };
            }
        }

        #endregion

        #region ==== 范围裁剪 ====

        /// <summary>
        /// 获取一个在栅格绘图区域内的矩形
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="saveRect">在绘图区域内的可能被裁剪过的矩形（默认为 empty）</param>
        /// <returns>如果裁剪后宽度或高度小于等于0，返回false；否则返回true</returns>
        public static bool RectWithin(Rectangle rect, out Rectangle saveRect)
        {
            saveRect = Rectangle.Empty;
            var left = rect.Left;
            var right = rect.Right;
            var top = rect.Top;
            var bottom = rect.Bottom;
            if (left <= DrawRect.Left)
            {
                if (right <= DrawRect.Left) { return false; }
                left = DrawRect.Left;
            }
            if (right >= DrawRect.Right)
            {
                if (left >= DrawRect.Right) { return false; }
                right = DrawRect.Right;
            }
            if (top <= DrawRect.Top)
            {
                if (bottom <= DrawRect.Top) { return false; }
                top = DrawRect.Top;
            }
            if (bottom >= DrawRect.Bottom)
            {
                if (top >= DrawRect.Bottom) { return false; }
                bottom = DrawRect.Bottom;
            }
            saveRect = new(left, top, right - left, bottom - top);
            return true;

        }

        #endregion
    }
}
