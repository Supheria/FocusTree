#define VERTIC
using FocusTree.UI.test;

namespace FocusTree.UI.Graph
{
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
        public static int RowWidth;
        /// <summary>
        /// 栅格总行高
        /// </summary>
        public static int ColHeight;
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

        #region ==== 绘制栅格 ====

        /// <summary>
        /// 格元边界绘制用笔
        /// </summary>
        public static Pen CellPen = new(Color.AliceBlue, 1.5f);
        /// <summary>
        /// 节点边界绘制用笔
        /// </summary>
        public static Pen NodePen = new(Color.Orange, 1.5f);
        public delegate Point CellDrawer(Graphics g);
        public static CellDrawer DrawCell;
        /// <summary>
        /// 需要单独绘制的格元队列
        /// </summary>
        public static List<CellDrawer> DrawCellQueue = new();
        /// <summary>
        /// 上一次绘制栅格时的原点位置
        /// </summary>
        static Point LastOrigin = new();
        /// <summary>
        /// 绘制无限制栅格
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds">栅格放置区域</param>
        /// <param name="cursor">新的光标位置</param>
        public static void Draw(Graphics g, Rectangle bounds)
        {
            g.Clear(Color.White);
            //DrawCell += delegate (Graphics g) { DrawCellQueue.Clear(); };
            SetBounds(bounds);

            Dictionary<int, HashSet<int>> skipColRow = new();
            foreach(var drawer in  DrawCellQueue)
            {
                var index = drawer(g);
                if (skipColRow.TryAdd(index.X, new() { index.Y }) == false)
                {
                    skipColRow[index.X].Add(index.Y);
                }
            }
            DrawCellQueue.Clear();

            for (int i = 0; i < ColNumber; i++)
            {
                var skip = skipColRow.TryGetValue(i, out var skipCols);
                for (int j = 0; j < RowNumber; j++)
                {
                    //if (skip && skipCols.TryGetValue(j, out var value) && value == j)
                    //{
                    //    //if (i == ColNumber - 1 || i == 0 || )
                    //    //DrawLoopCell(g, i, j, true, false, true, true);
                    //    continue;
                    //}
                    DrawLoopCell(g, i, j);
                }
            }
            LastOrigin = new(OriginLeft, OriginTop);
        }
        /// <summary>
        /// 设置边界及绘图参数
        /// </summary>
        /// <param name="bounds"></param>
        private static void SetBounds(Rectangle bounds)
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
        }
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
            var cellLeftLine = LineToDrawInHorizon(cellLeft, LatticeCell.Width);
            var cellTopLine = LineToDrawInVertical(cellTop, LatticeCell.Height);
            //
            // cell main: LeftBottom -> LeftTop -> TopRight
            //
            g.DrawLines(CellPen, new Point[]
            {
                        new(cellLeftLine[0].Item1, cellTopLine[0].Item2),
                        new(cellLeftLine[0].Item1, cellTopLine[0].Item1),
                        new(cellLeftLine[0].Item2, cellTopLine[0].Item1)
            });
            //
            // cell append
            //
            if (cellLeftLine.Length > 1)
            {
                g.DrawLine(CellPen,
                    new(cellLeftLine[1].Item1, cellTopLine[0].Item1),
                    new(cellLeftLine[1].Item2, cellTopLine[0].Item1)
                    );
            }
            if (cellTopLine.Length > 1)
            {
                g.DrawLine(CellPen,
                    new(cellLeftLine[0].Item1, cellTopLine[1].Item1),
                    new(cellLeftLine[0].Item1, cellTopLine[1].Item2)
                    );
            }
            var nodeLeft = cellLeft + LatticeCell.NodePaddingWidth;
            var nodeTop = cellTop + LatticeCell.NodePaddingHeight;
            var nodeLeftLine = LineToDrawInHorizon(nodeLeft, LatticeCell.NodeWidth);
            var nodeTopLine = LineToDrawInVertical(nodeTop, LatticeCell.NodeHeight);
            //
            // node main: LeftBottom -> LeftTop -> TopRight
            //
            g.DrawLines(NodePen, new Point[]
            {
                        new(nodeLeftLine[0].Item1, nodeTopLine[0].Item2),
                        new(nodeLeftLine[0].Item1, nodeTopLine[0].Item1),
                        new(nodeLeftLine[0].Item2, nodeTopLine[0].Item1)
            });
            //
            // node append
            //
            if (nodeLeftLine.Length > 1)
            {
                g.DrawLine(NodePen,
                    new(nodeLeftLine[1].Item1, nodeTopLine[0].Item1),
                    new(nodeLeftLine[1].Item2, nodeTopLine[0].Item1)
                    );
            }
            if (nodeTopLine.Length > 1)
            {
                g.DrawLine(NodePen,
                    new(nodeLeftLine[0].Item1, nodeTopLine[1].Item1),
                    new(nodeLeftLine[0].Item1, nodeTopLine[1].Item2)
                    );
            }
        }
        /// <summary>
        /// ===绘制栅格时调用===
        /// 从左向右的水平线在栅格绘图区域内的绘制
        /// </summary>
        /// <param name="left">左端点</param>
        /// <param name="width">线长</param>
        /// <returns>返回从左向右的水平线的起点坐标和终点坐标的数对。如果线需要分割，则返回数组里有一个额外的分割剩余线段的坐标数对。</returns>
        public static (int, int)[] LineToDrawInHorizon(int left, int width)
        {
            if (left < DrawRect.Left) { left += DrawRect.Width; }
            else if (left > DrawRect.Right) { left -= DrawRect.Width; }
            var realWidth = DrawRect.Right - left;
            if (realWidth >= width)
            {
                return new (int, int)[] { (left, left + width) };
            }
            else
            {
                return new (int, int)[]
                {
                    (left, left + realWidth),
                    (DrawRect.Left, DrawRect.Left + width - realWidth)
                };
            }
        }
        /// <summary>
        /// ===绘制栅格时调用===
        /// 从上向下的垂直线在栅格绘图区域内的绘制
        /// </summary>
        /// <param name="top">上端点</param>
        /// <param name="height">线高</param>
        /// <returns>返回从上向下的垂直线的起点坐标和终点坐标的数对。如果线需要分割，则返回数组里有一个额外的分割剩余线段的坐标数对。</returns>
        public static (int, int)[] LineToDrawInVertical(int top, int height)
        {
            if (top < DrawRect.Top) { top += DrawRect.Height; }
            else if (top > DrawRect.Bottom) { top -= DrawRect.Height; }
            var realHeight = DrawRect.Bottom - top;
            if (realHeight >= height)
            {
                return new (int, int)[] { (top, top + height) };
            }
            else
            {
                return new (int, int)[]
                {
                    (top, top + realHeight),
                    (DrawRect.Top, DrawRect.Top + height - realHeight)
                };
            }
        }

        #endregion

        #region ==== 格元绘制核心 ====

        /// <summary>
        /// 在栅格绘图区域内绘制左下-左上-上右的七形线
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen"></param>
        /// <param name="LeftTop">七形线左上角坐标</param>
        /// <param name="size">七形线尺寸</param>
        /// <param name="drawMain">是否绘制未超出栅格绘图区域的部分</param>
        /// <param name="drawAppend">是否补绘超出栅格绘图区域的部分。补绘方式形如：以栅格坐标系原点为方向参照中心，若横线向左超出绘图区域左边界，则超出部分补绘到绘图区域的右边界向左。向其他方向的超出各个边界同理</param>
        static void DrawCellLine(Graphics g, Pen pen, Point LeftTop, Size size, bool drawMain, bool drawAppend)
        {
            var dr = DrawRect;
            var pairLeftRight = DrawCellLeftLine((LeftTop.X, LeftTop.Y), size.Width, (dr.Left, dr.Right), (dr.Top, dr.Bottom), (dr.Width, dr.Height), drawMain, drawAppend);
            var pairTopBottom = DrawCellLeftLine((LeftTop.Y, LeftTop.X), size.Height, (dr.Top, dr.Bottom), (dr.Left, dr.Right), (dr.Height, dr.Width), drawMain, drawAppend);
            foreach (var pair in pairLeftRight.Value)
            {
                var key = pairLeftRight.Key;
                g.DrawLine(pen, new(pair.Item1, key), new(pair.Item2, key));
            }
#if VERTIC
            foreach (var pair in pairTopBottom.Value)
            {
                var key = pairTopBottom.Key;
                g.DrawLine(pen, new(key, pair.Item1), new(key, pair.Item2));
            }
#endif
        }
        /// <summary>
        /// 得到绘制格元左边界（上边界）线的端点坐标（变量命名以绘制横线为例）
        /// </summary>
        /// <param name="left_top">格元左上角坐标组成的坐标数对。数对前者最终返回画线的起点、终点坐标中变化的值，数对后者最终则返回不变值（eg.画横线时，横坐标变，纵坐标不变，则数对前者是left，后者是top）</param>
        /// <param name="width">画线的长度（横线传入width，竖线传入height）</param>
        /// <param name="drLR">栅格绘图区域与线平行的方向，用于判断和限制坐标数对的前者范围（eg.画横线传入绘图区域的Left和Right）</param>
        /// <param name="drTB">栅格绘图区域与线垂直的方向，用于判断和限制坐标数对的后者范围（eg.画横线传入绘图区域的Top和Bottom）</param>
        /// <param name="drSize">栅格绘图区域的宽高组成的数对，前者用于判断和限制坐标数对的前者，后者则用于做标数对后者（eg.画横线传入绘图区域的(width, height)数对，画竖线则相反）</param>
        /// <param name="drawMain">是否绘制未超出栅格绘图区域的部分</param>
        /// <param name="drawAppend">是否补绘超出栅格绘图区域的部分。补绘方式形如：以栅格坐标系原点为方向参照中心，若横线向左超出绘图区域左边界，则超出部分补绘到绘图区域的右边界向左。向其他方向的超出各个边界同理</param>
        /// <returns>键值是约束和转化后的坐标数对前者，键值中的每个数对是绘制线时作为起点和终点的坐标数对的前者。假定drawMain = drawAppend = true，那么键值中最少一个数对，此时绘制线位于栅格绘图区域内部，只需绘制一次；最多两个数对，此时绘制线有超出部分，其中分别是未超出部分和超出部分，需要绘制两次</returns>
        static KeyValuePair<int, List<(int, int)>> DrawCellLeftLine((int, int) left_top, int width, (int, int) drLR, (int, int) drTB, (int, int) drSize, bool drawMain, bool drawAppend)
        {
            var left = left_top.Item1; var top = left_top.Item2;
            var drLeft = drLR.Item1; var drRight = drLR.Item2;
            var drTop = drTB.Item1; var drBottom = drTB.Item2;
            var drWidth = drSize.Item1; var drHeight = drSize.Item2;

            var rawTop = top;
            if (drawAppend)
            {
                if (top <= drTop)
                {
                    top = (top % drHeight) + drHeight;
                    if (top <= drTop) { top += drHeight; }
                }
                if (top >= drBottom) { top %= drHeight; }
            }
            else
            {
                if (top < drTop || top > drBottom) { return new(top, new()); }
            }
            var right = left + width;
            if (left >= drLeft && right <= drRight)
            {
                if (drawMain || (drawAppend && (rawTop < drTop || rawTop > drBottom)))
                {
                    return new(top, new() { (left, right) });
                    //g.DrawLine(pen, new(left, top), new(right, top));
                }
            }
            else if (left < drLeft)
            {
                return CellLeftBeyondDRLeft(left, top, width, drLeft, drRight, drWidth, drawMain, drawAppend);
            }
            else if (right > drRight)
            {
                return CellRightBeyondDRRight(right, top, width, drLeft, drRight, drWidth, drawMain, drawAppend);
            }
            return new(top, new());
        }
        /// <summary>
        /// 格元左边界超出栅格绘图区域左边界
        /// </summary>
        static KeyValuePair<int, List<(int, int)>> CellLeftBeyondDRLeft(int left, int top, int width, int drLeft, int drRight, int drWidth, bool drawMain, bool drawAppend)
        {
            KeyValuePair<int, List<(int, int)>> result = new(top, new());
            left = drWidth + (left % drWidth);
            if (left <= drLeft) { left += drWidth; }
            var cutWidth = drRight - left;
            if (cutWidth > width) { cutWidth = width; }
            var saveWidth = width - cutWidth;
            if (drawMain)
            {
                result.Value.Add((drLeft, drLeft + saveWidth));
                //g.DrawLine(pen, new(drLeft, top), new(drLeft + saveWidth, top));
            }
            if (drawAppend)
            {
                result.Value.Add((left, left + cutWidth));
                //g.DrawLine(pen, new(left, top), new(left + cutWidth, top));
            }
            return result;
        }
        /// <summary>
        /// 格元右边界超出栅格绘图区域右边界
        /// </summary>
        static KeyValuePair<int, List<(int, int)>> CellRightBeyondDRRight(int right, int top, int width, int drLeft, int drRight, int drWidth, bool drawMain, bool drawAppend)
        {
            KeyValuePair<int, List<(int, int)>> result = new(top, new());
            var testRight = right % drWidth;
            if (testRight > drLeft) { right = testRight; }
            var cutWidth = right - drLeft;
            if (cutWidth > width) { cutWidth = width; }
            var saveWidth = width - cutWidth;
            if (drawMain)
            {
                result.Value.Add((drRight - saveWidth, drRight));
                //g.DrawLine(pen, new(drRight - saveWidth, top), new(drRight, top));
            }
            if (drawAppend)
            {
                result.Value.Add((right - cutWidth, right));
                //g.DrawLine(pen, new(right - cutWidth, top), new(right, top));
            }
            return result;
        }

#endregion

        /// <summary>
        /// 获取一个在栅格绘图区域内的矩形
        /// </summary>
        /// <param name="left">预设矩形左边界</param>
        /// <param name="top">预设矩形上边界</param>
        /// <param name="width">预设矩形宽</param>
        /// <param name="height">预设矩形高</param>
        /// <returns>在绘图区域内的可能被裁剪过的矩形</returns>
        public static Rectangle RectWithinDrawRect(Rectangle rect)
        {
            var left = rect.Left;
            var top = rect.Top;
            var width = rect.Width;
            var height = rect.Height;
            if (left < DrawRect.Left)
            {
                width -= DrawRect.Left - left;
                left = DrawRect.Left;
            }
            if (top < DrawRect.Top)
            {
                height -= DrawRect.Top - top;
                top = DrawRect.Top;
            }
            return new(left, top,
                left + width > DrawRect.Right ? DrawRect.Right - left : width,
                top + height > DrawRect.Bottom ? DrawRect.Bottom - top : height
                );
        }
        /// <summary>
        /// 重绘格元（停止绘制超出部分）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cell"></param>
        public static void ReDrawCell(Graphics g, LatticeCell cell)
        {
            //CellDrawer drawer = () =>
            {
                g.FillRectangle(new SolidBrush(Color.White), cell.RealRect);
                DrawCellLine(g, CellPen,
                    new(cell.RealLeft, cell.RealTop),
                    new(LatticeCell.Width, LatticeCell.Height),
                    true, false);
                DrawCellLine(g, NodePen,
                    new(cell.NodeRealLeft, cell.NodeRealTop),
                    new(LatticeCell.NodeWidth, LatticeCell.NodeHeight),
                    true, false);
            };

        }
        ////static TestInfo test = new();
        ///// <summary>
        ///// 添加格元及其重绘方法添加到格元绘制队列
        ///// </summary>
        ///// <param name="cell"></param>
        ///// <param name="cellDrawer"></param>
        //public static void AddCellToDrawQueue(LatticeCell cell, CellDrawer cellDrawer)
        //{
        //    //test.Show();
        //    //var ColRowIndex = cell.GetRealColRowIndex();
        //    DrawCellQueue[cell] = cellDrawer;
        //    //test.InfoText = $"ColRowIndex: {ColRowIndex}\n" +
        //    //    $"Real LeftTop: {new Point(cell.RealLeft, cell.RealTop)}\n" +
        //    //    $"latticed again: {new Point(ColRowIndex.Item1 * LatticeCell.Width, ColRowIndex.Item2 * LatticeCell.Height)}";
        //}
        ///// <summary>
        ///// 从格元绘制队列里移除可能存在的格元
        ///// </summary>
        ///// <param name="cell"></param>
        //public static void CancelCellFromDrawQueue(LatticeCell cell)
        //{
        //    DrawCellQueue.Remove(cell);
        //}
    }
}
