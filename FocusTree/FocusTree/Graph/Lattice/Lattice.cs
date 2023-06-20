#define DEBUG


namespace FocusTree.Graph
{
    /// <summary>
    /// 栅格
    /// </summary>
    static class Lattice
    {
        #region ==== 基本参数 ====

        /// <summary>
        /// 栅格绘图区域矩形
        /// </summary>
        public static Rectangle DrawRect { get; set; }
        /// <summary>
        /// 栅格坐标系原点 x 坐标
        /// </summary>
        public static int OriginLeft { get; set; }
        /// <summary>
        /// 栅格坐标系原点 y 坐标
        /// </summary>
        public static int OriginTop { get; set; }

        #endregion

        #region ==== 绘图工具 ====

        /// <summary>
        /// 格元边框宽度
        /// </summary>
        public static Pen CellPen = new(Color.FromArgb(200, Color.AliceBlue), 1.5f);
        /// <summary>
        /// 节点边框宽度
        /// </summary>
        public static Pen NodePen = new(Color.FromArgb(150, Color.Orange), 1.75f);
        /// <summary>
        /// 坐标辅助线绘制用笔
        /// </summary>
        public static Pen GuidePen = new Pen(Color.FromArgb(200, Color.Red), 1.75f);

        #endregion

        #region ==== 指示器 ====


        #endregion

        #region ==== 绘制栅格 ====

        /// <summary>
        /// 绘制无限制栅格，并调用绘制委托列表
        /// </summary>
        /// <param name="image"></param>
        public static void Draw(Image image)
        {
            var g = Graphics.FromImage(image);
            DrawLatticeCells(g);
            //
            // draw guide line
            //
            g.DrawLine(GuidePen, new(OriginLeft, DrawRect.Top), new(OriginLeft, DrawRect.Bottom));
            g.DrawLine(GuidePen, new(DrawRect.Left, OriginTop), new(DrawRect.Right, OriginTop));
            g.Flush(); g.Dispose();
            //Program.testInfo.InfoText = $"{new Point(ColNumber, RowNumber)}";
            //Program.testInfo.InfoText = $"{Drawing.MethodNumber()}\n" +
            //    $"1. {Drawing.MethodNumber(0)}, 2. {Drawing.MethodNumber(1)}, 3. {Drawing.MethodNumber(2)}";
        }

        /// <summary>
        /// 绘制循环格元（格元左上角坐标与栅格坐标系中心偏移量近似投射在一个格元大小范围内）
        /// </summary>
        /// <param name="g"></param>
        private static void DrawLatticeCells(Graphics g)
        {
            var offsetLeft = (OriginLeft - DrawRect.Left) % LatticeCell.Length;
            var offSetTop = (OriginTop - DrawRect.Top) % LatticeCell.Length;
            if (offsetLeft > 0) { offsetLeft -= LatticeCell.Length; }
            if (offSetTop > 0) { offSetTop -= LatticeCell.Length; }
            offsetLeft += DrawRect.Left;
            offSetTop += DrawRect.Top;
            var colNum = DrawRect.Width / LatticeCell.Length + 2;
            var rowNum = DrawRect.Height / LatticeCell.Length + 2;
            for (int i = 0; i < colNum; i++)
            {
                for (int j = 0; j < rowNum; j++)
                {
                    var cellLeft = offsetLeft + i * LatticeCell.Length;
                    var cellTop = offSetTop + j * LatticeCell.Length;
                    var cellRight = cellLeft + LatticeCell.Length;
                    var cellBottom = cellTop + LatticeCell.Length;
                    //
                    // draw cell
                    //
                    if (CrossLineWithin(new(cellLeft, cellBottom), new(cellRight, cellBottom), CellPen.Width, out var p1, out var p2))
                    {
                        g.DrawLine(CellPen, p1, p2);
                    }
                    if (CrossLineWithin(new(cellRight, cellTop), new(cellRight, cellBottom), CellPen.Width, out p1, out p2))
                    {
                        g.DrawLine(CellPen, p1, p2);
                    }
                    var nodeLeft = cellLeft + LatticeCell.NodePaddingWidth;
                    var nodeTop = cellTop + LatticeCell.NodePaddingHeight;
                    //
                    // draw node
                    //
                    if (CrossLineWithin(new(nodeLeft, nodeTop), new(cellRight, nodeTop), NodePen.Width, out p1, out p2))
                    {
                        g.DrawLine(NodePen, p1, p2);
                    }
                    if (CrossLineWithin(new(nodeLeft, nodeTop), new(cellLeft, cellBottom), NodePen.Width, out p1, out p2))
                    {
                        g.DrawLine(NodePen, p1, p2);
                    }
                }
            }
        }

        #endregion

        #region ==== 范围裁剪 ====

        /// <summary>
        /// 获取给定横纵直线在栅格绘图区域内的矩形
        /// </summary>
        /// <param name="p1">直线的端点</param>
        /// <param name="p2">直线的另一端点</param>
        /// <param name="lineWidth">直线的宽度</param>
        /// <param name="lineRect">在绘图区域内的横纵直线的矩形</param>
        /// <returns></returns>
        public static bool CrossLineWithin(PointF p1, PointF p2, float lineWidth, out PointF endMin, out PointF endMax)
        {
            endMin = endMax = PointF.Empty;
            var halfLineWidth = (lineWidth / 2);
            if (p1.Y == p2.Y)
            {
                if (!CrossLineWithin(p1.Y, DrawRect.Top, DrawRect.Bottom, (p1.X + halfLineWidth, p2.X + halfLineWidth), DrawRect.Left, DrawRect.Right, out var xMin, out var xMax)) { return false; }
                endMin = new(xMin, p1.Y);
                endMax = new(xMax, p1.Y);
            }
            else
            {
                if (!CrossLineWithin(p1.X, DrawRect.Left, DrawRect.Right, (p1.Y + halfLineWidth, p2.Y + halfLineWidth), DrawRect.Top, DrawRect.Bottom, out var yMin, out var yMax)) { return false; }
                endMin = new(p1.X, yMin);
                endMax = new(p1.X, yMax);
            }
            return true;
        }
        private static bool CrossLineWithin(float theSame, float theSameLimitMin, float theSameLimitMax, (float, float) ends, float endLimitMin, float endLimitMax, out float endMin, out float endMax)
        {
            endMin = endMax = 0;
            if (ends.Item1 == ends.Item2 || theSame < theSameLimitMin || theSame > theSameLimitMax) { return false; }
            endMin = Math.Min(ends.Item1, ends.Item2);
            endMax = Math.Max(ends.Item1, ends.Item2);
            if (endMin >= endLimitMax) { return false; }
            if (endMax <= endLimitMin) { return false; }
            if (endMin < endLimitMin) { endMin = endLimitMin; }
            if (endMax > endLimitMax) { endMax = endLimitMax; }
            return true;
        }
        /// <summary>
        /// 获取给定的矩形在栅格绘图区域内的矩形
        /// </summary>
        /// <param name="rect">给定的矩形</param>
        /// <param name="saveRect">在绘图区域内的可能被裁剪过的矩形（默认为 empty）</param>
        /// <returns>如果给定的矩形完全超出了绘图区域，返回false；否则返回true</returns>
        public static bool RectWithin(Rectangle rect, out Rectangle saveRect)
        {
            saveRect = Rectangle.Empty;
            var left = rect.Left;
            var right = rect.Right;
            var top = rect.Top;
            var bottom = rect.Bottom;
            var drRight = DrawRect.Right;
            var drBottom = DrawRect.Bottom;
            if (left < DrawRect.Left)
            {
                if (right <= DrawRect.Left) { return false; }
                left = DrawRect.Left;
            }
            if (right > drRight)
            {
                if (left >= drRight) { return false; }
                right = drRight;
            }
            if (top < DrawRect.Top)
            {
                if (bottom <= DrawRect.Top) { return false; }
                top = DrawRect.Top;
            }
            if (bottom > DrawRect.Bottom)
            {
                if (top >= drBottom) { return false; }
                bottom = drBottom;
            }
            saveRect = new(left, top, right - left, bottom - top);
            return true;
        }

        #endregion
    }
}
