#define DEBUG

using FocusTree.UI.test;

namespace FocusTree.UI.Graph
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
        /// 栅格绘图区域与放置区域的宽的差值的一半
        /// </summary>
        public static int DeviDiffInDrawRectWidth;
        /// <summary>
        /// 栅格绘图区域与放置区域的高的差值的一半
        /// </summary>
        public static int DeviDiffInDrawRectHeight;
        /// <summary>
        /// 栅格坐标系原点 x 坐标
        /// </summary>
        public static int OriginLeft;
        /// <summary>
        /// 栅格坐标系原点 y 坐标
        /// </summary>
        public static int OriginTop;
        /// <summary>
        /// 格元横坐标偏移量，对栅格坐标系原点相对于 DrawRect 的左上角的偏移量，在格元大小内实施相似偏移量
        /// </summary>
        public static int CellOffsetLeft { get => OriginLeft - DrawRect.X + DeviDiffInDrawRectWidth; }
        /// <summary>
        /// 格元纵坐标偏移量，对栅格坐标系原点相对于 DrawRect 的左上角的偏移量，在格元大小内实施相似偏移量
        /// </summary>
        public static int CellOffsetTop { get => OriginTop - DrawRect.Y + DeviDiffInDrawRectHeight; }

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
        public static Pen GuidePen = new Pen(Color.FromArgb(200, Color.Red), 0.5f);
        /// <summary>
        /// 需要单独绘制的格元委托列表
        /// </summary>
        public static event CellDrawer Drawing;

        #endregion

        #region ==== 指示器 ====

        /// <summary>
        /// 是否绘制坐标辅助线
        /// </summary>
        public static bool DrawGuideLine = true;
        /// <summary>
        /// 是否绘制背景栅格
        /// </summary>
        public static bool DrawBackLattice = false;

        #endregion

        #region ==== 上一次绘图标志 ===

        public static int LastCellWidth = LatticeCell.Width;
        public static int LastCellHeight = LatticeCell.Height;
        public static int LastOriginLeft = OriginLeft;
        public static int LastOriginTop = OriginTop;
        public static Rectangle LastDrawRect = DrawRect;
        public static int LastDeviDiffInDrawRectWidth = DeviDiffInDrawRectWidth;
        public static int LastDeviDiffInDrawRectHeight = DeviDiffInDrawRectHeight;

        #endregion

        #region ==== 绘制栅格 ====

        static TestInfo test = new();
        /// <summary>
        /// 设置栅格放置区域（自动重置列行数）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds">放置区域</param>
        public static void SetBounds(Rectangle bounds)
        {
            test.Show();
            ColNumber = bounds.Width / LatticeCell.Width;
            RowWidth = ColNumber * LatticeCell.Width;
            RowNumber = bounds.Height / LatticeCell.Height;
            ColHeight = RowNumber * LatticeCell.Height;
            DeviDiffInDrawRectWidth = (int)((float)(bounds.Width - RowWidth) * 0.5f);
            DeviDiffInDrawRectHeight = (int)((float)(bounds.Height - ColHeight) * 0.5f);
            DrawRect = new Rectangle(
                bounds.X + DeviDiffInDrawRectWidth,
                bounds.Y + DeviDiffInDrawRectHeight,
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

            test.InfoText = $"{DrawRect}" + Drawing?.GetInvocationList().Length;

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
                g.Flush(); g.Dispose();
            }
            if (DrawGuideLine)
            {
                var g = Graphics.FromImage(image);
                g.DrawLine(GuidePen, new(OriginLeft, DrawRect.Top), new(OriginLeft, DrawRect.Bottom));
                g.DrawLine(GuidePen, new(DrawRect.Left, OriginTop), new(DrawRect.Right, OriginTop));
                g.Flush(); g.Dispose();
            }
            LastCellWidth = LatticeCell.Width;
            LastCellHeight = LatticeCell.Height;
            LastOriginLeft = OriginLeft;
            LastOriginTop = OriginTop;
            LastDrawRect = DrawRect;
            LastDeviDiffInDrawRectWidth = DeviDiffInDrawRectWidth;
            LastDeviDiffInDrawRectHeight = DeviDiffInDrawRectHeight;
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
            var cellLeft = col * LatticeCell.Width + (OriginLeft - DrawRect.X) % LatticeCell.Width + DeviDiffInDrawRectWidth;
            var cellTop = row * LatticeCell.Height + (OriginTop - DrawRect.Y) % LatticeCell.Height + DeviDiffInDrawRectHeight;
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
            //var width = rect.Width;
            //var height = rect.Height;
            if (left < DrawRect.Left)
            {
                if (right <= DrawRect.Left) { return false; }
                //width -= DrawRect.Left - left;
                left = DrawRect.Left;
            }
            if (right > DrawRect.Right)
            {
                if (left >= DrawRect.Right) { return false; }
                //width -= right - DrawRect.Right;
                right = DrawRect.Right;
            }
            if (top < DrawRect.Top)
            {
                if (bottom <= DrawRect.Top) { return false; }
                //height -= DrawRect.Top - top;
                top = DrawRect.Top;
            }
            if (bottom > DrawRect.Bottom)
            {
                if (top >= DrawRect.Bottom) { return false; }
                //height -= bottom - DrawRect.Bottom;
                bottom = DrawRect.Bottom;
            }
            saveRect = new(left, top, right - left, bottom - top);
            //if (saveRect.Height <= 0 || saveRect.Width <= 0) 
            //{ return false; }
            return true;

        }
        public static bool RectWithin(Rectangle rect)
        {
            var right = rect.Right;
            var bottom = rect.Bottom;
            if (rect.Left < DrawRect.Left)
            {
                if (right <= DrawRect.Left) { return false; }
            }
            if (right > DrawRect.Right)
            {
                if (rect.Left >= DrawRect.Right) { return false; }
            }
            if (rect.Top < DrawRect.Top)
            {
                if (bottom <= DrawRect.Top) { return false; }
            }
            if (bottom > DrawRect.Bottom)
            {
                if (rect.Top >= DrawRect.Bottom) { return false; }
            }
            return true;
        }
        /// <summary>
        /// 获取一个在栅格绘图区域内的水平线
        /// </summary>
        /// <param name="x">端点的横坐标数对</param>
        /// <param name="y">端点的纵坐标</param>
        /// <param name="saveLine">在绘图区域内可能被裁剪过的线段的两个端点坐标的数对</param>
        /// <returns>如果线有部分在绘图区域内，返回true；否则返回false</returns>
        public static bool LineWithin((int, int) x, int y, float penWidth, out (Point, Point) saveLine)
        {
            saveLine = new();
            if (y - penWidth < DrawRect.Top || y + penWidth > DrawRect.Bottom) { return false; }
            var x1 = x.Item1;
            var x2 = x.Item2;
            if (x1 == x2) { return false; }
            var Left = DrawRect.Left;
            var Right = DrawRect.Right;
            if (x1 < x2)
            {
                if (x1 > Right || x2 < Left) { return false; }
                if (x1 < Left) { x1 = Left; }
                if (x2 > Right) { x2 = Right; }
            }
            else if (x2 < x1)
            {
                if (x2 > Right || x1 < Left) { return false; }
                if (x2 < Left) { x2 = Left; }
                if (x1 > Right) { x1 = Right; }
            }
            saveLine = (new(x1, y), new(x2, y));
            return true;
        }
        /// <summary>
        /// 获取一个在栅格绘图区域内的垂直线
        /// </summary>
        /// <param name="x">端点的横坐标</param>
        /// <param name="y">端点的纵坐标数对</param>
        /// <param name="saveLine">在绘图区域内可能被裁剪过的线段的两个端点坐标的数对</param>
        /// <returns>如果线有部分在绘图区域内，返回true；否则返回false</returns>
        public static bool LineWithin(int x, (int, int) y, float penWidth, out (Point, Point) saveLine)
        {
            saveLine = new();
            if (x - penWidth < DrawRect.Left || x + penWidth > DrawRect.Right) { return false; }
            var y1 = y.Item1;
            var y2 = y.Item2;
            if (y1 == y2) { return false; }
            var Top = DrawRect.Top;
            var Bottom = DrawRect.Bottom;
            if (y1 < y2)
            {
                if (y1 > Bottom || y2 < Top) { return false; }
                if (y1 < Top) { y1 = Top; }
                if (y2 > Bottom) { y2 = Bottom; }
            }
            else if (y2 < y1)
            {
                if (y2 > Bottom || y1 < Top) { return false; }
                if (y2 < Top) { y2 = Top; }
                if (y1 > Bottom) { y1 = Bottom; }
            }
            saveLine = (new(x, y1), new(x, y2));
            return true;
        }

        #endregion
    }
}
