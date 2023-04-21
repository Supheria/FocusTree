﻿#define DEBUG

using FocusTree.UI.test;

namespace FocusTree.UI.Graph
{
    /// <summary>
    /// 绘制栅格时，绘制非循环节点的委托类型
    /// </summary>
    /// <param name="g">传入栅格的 GDI</param>
    public delegate void CellDrawer(Graphics g);
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
        static int DeviDiffInDrawRectWidth;
        /// <summary>
        /// 栅格绘图区域与放置区域的高的差值的一半
        /// </summary>
        static int DeviDiffInDrawRectHeight;
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
        public static Pen NodePen = new(Color.Orange, 1.5f);
        /// <summary>
        /// 需要单独绘制的格元委托列表
        /// </summary>
        public static event CellDrawer Drawing;

        #endregion

        #region ==== 绘制栅格 ====

        /// <summary>
        /// 设置栅格放置区域，绘制栅格（首次绘制或重置列行数）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds">放置区域</param>
        public static void Draw(Graphics g, Rectangle bounds)
        {
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
            Draw(g);
        }
        static TestInfo test = new();
        /// <summary>
        /// 绘制无限制栅格，并调用绘制格元的委托
        /// </summary>
        /// <param name="g"></param>
        public static void Draw(Graphics g)
        {
            test.Show();
            g.Clear(Color.White);

            Drawing?.Invoke(g);

            for (int i = 0; i < ColNumber; i++)
            {
                for (int j = 0; j < RowNumber; j++)
                {
                    DrawLoopCell(g, i, j);
                }
            }
#if DEBUG
            var testPen = new Pen(Color.Red, 0.5f);
            g.DrawLine(testPen, new(OriginLeft, DrawRect.Top), new(OriginLeft, DrawRect.Bottom));
            g.DrawLine(testPen, new(DrawRect.Left, OriginTop), new(DrawRect.Right, OriginTop));
#endif
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

        #region ==== 非循环格元绘制方法 ====


        /// <summary>
        /// 重绘格元（裁去超出部分）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cell"></param>
        public static void ReDrawCell(Graphics g, LatticeCell cell)
        {
            g.FillRectangle(new SolidBrush(Color.White), cell.RealRect);
            DrawCellLines(g, CellPen, new(cell.RealLeft, cell.RealTop), new(LatticeCell.Width, LatticeCell.Height));
            DrawCellLines(g, NodePen, new(cell.NodeRealLeft, cell.NodeRealTop), new(LatticeCell.NodeWidth, LatticeCell.NodeHeight));
        }
        /// <summary>
        /// 在栅格绘图区域内绘制左下-左上-上右的七形线
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen"></param>
        /// <param name="LeftTop">七形线左上角坐标</param>
        /// <param name="size">七形线尺寸</param>
        static void DrawCellLines(Graphics g, Pen pen, Point LeftTop, Size size)
        {
            var left = LeftTop.X;
            var top = LeftTop.Y;
            if (top > DrawRect.Top && top < DrawRect.Bottom)
            {
                var right = left + size.Width;
                if (left > DrawRect.Right || right < DrawRect.Left) { return; }
                g.DrawLine(pen,
                    new(left < DrawRect.Left ? DrawRect.Left : left, top),
                    new(right > DrawRect.Right ? DrawRect.Right : right, top)
                    );
            }
            if (left > DrawRect.Left && left < DrawRect.Right)
            {
                var bottom = top + size.Height;
                if (top > DrawRect.Bottom || bottom < DrawRect.Top) { return; }
                g.DrawLine(pen,
                    new(left, top < DrawRect.Top ? DrawRect.Top : top),
                    new(left, bottom > DrawRect.Bottom ? DrawRect.Bottom : bottom)
                    );
            }
        }

        #endregion

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
            var top = rect.Top;
            var width = rect.Width;
            var height = rect.Height;
            if (left < DrawRect.Left)
            {
                width -= DrawRect.Left - left;
                if (width <= 0) { return false; }
                left = DrawRect.Left;
            }
            if (top < DrawRect.Top)
            {
                height -= DrawRect.Top - top;
                if (height <= 0) { return false; }
                top = DrawRect.Top;
            }
            saveRect = new(left, top,
                left + width > DrawRect.Right ? DrawRect.Right - left : width,
                top + height > DrawRect.Bottom ? DrawRect.Bottom - top : height
                );
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
        /// <summary>
        /// 绘制栅格时绘制水平线
        /// </summary>
        /// <param name="x">端点的横坐标数对</param>
        /// <param name="y">端点的纵坐标</param>
        /// <param name="pen"></param>
        public static void DrawLineWhileDrawing((int, int) x, int y, Pen pen)
        {
            if (LineWithin(x, y, pen.Width, out var saveLine))
            {
                var line = saveLine;
                Drawing += (g) => g.DrawLine(pen, saveLine.Item1, saveLine.Item2);
            }
        }
        /// <summary>
        /// 绘制栅格时绘制垂直线
        /// </summary>
        /// <param name="x">端点的横坐标</param>
        /// <param name="y">端点的纵坐标数对</param>
        /// <param name="pen"></param>
        public static void DrawLineWhileDrawing(int x, (int, int)y, Pen pen)
        {
            if (LineWithin(x, y, pen.Width, out var saveLine))
            {
                var line = saveLine;
                Drawing += (g) => g.DrawLine(pen, line.Item1, line.Item2);
            }
        }
        /// <summary>
        /// 绘制栅格时填充矩形
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="brush"></param>
        public static void DrawFillWhileDrawing(Rectangle rect, Brush brush)
        {
            if (RectWithin(rect, out var saveRect))
            {
                rect = saveRect;
                Drawing += (g) => { g.FillRectangle(brush, rect); };
            }
        }
        /// <summary>
        /// 清空绘制委托
        /// </summary>
        public static void DrawingClear()
        {
            Drawing = null;
        }
    }
}
