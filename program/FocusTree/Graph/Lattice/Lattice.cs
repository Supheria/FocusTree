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
        public static Rectangle DrawRect 
        {
            get => drawRect;
            set
            {
                drawRect = value;
                ColNumber = DrawRect.Width / LatticeCell.Length;
                if (DrawRect.Width % LatticeCell.Length == 0 && (DrawRect.Left - OriginLeft) % LatticeCell.Length != 0) 
                { 
                    ColNumber++; 
                }
                else
                { 
                    ColNumber++;
                    if (DrawRect.Width % LatticeCell.Length > LatticeCell.Length / 2 && (DrawRect.Left - OriginLeft) % LatticeCell.Length != 0) 
                    { 
                        ColNumber++;
                    }
                }
                RowNumber = DrawRect.Height / LatticeCell.Length;
                if (DrawRect.Height % LatticeCell.Length > 0) { RowNumber++; }
            }
        }
        static Rectangle drawRect;
        static int ColNumber;
        static int RowNumber;
        /// <summary>
        /// 栅格坐标系原点 x 坐标
        /// </summary>
        public static int OriginLeft { get; set; }
        /// <summary>
        /// 栅格坐标系原点 y 坐标
        /// </summary>
        public static int OriginTop { get; set; }
        static (int ,int) LatticedPointWithinCol;
        static (int, int) LatticedPointWithinRow;

        #endregion

        #region ==== 绘图工具 ====

        /// <summary>
        /// 绘制委托列表（三层绘制）
        /// </summary>
        public static DrawLayers Drawing = new(3);
        /// <summary>
        /// 格元边框宽度
        /// </summary>
        public static int CellBorderWidth = 1;
        /// <summary>
        /// 节点边框宽度
        /// </summary>
        public static int NodeBorderWidth = 1;
        /// <summary>
        /// 格元边框宽度
        /// </summary>
        public static Color CellBorderCoolor = Color.FromArgb(200, Color.AliceBlue);
        /// <summary>
        /// 节点边框宽度
        /// </summary>
        public static Color NodeBorderColor = Color.FromArgb(150, Color.Orange);
        /// <summary>
        /// 坐标辅助线绘制用笔
        /// </summary>
        public static Pen GuidePen = new Pen(Color.FromArgb(200, Color.Red), 1.75f);

        #endregion

        #region ==== 指示器 ====

        /// <summary>
        /// 是否绘制背景栅格
        /// </summary>
        public static bool DrawBackLattice = false;

        #endregion

        #region ==== 绘制栅格 ====

        /// <summary>
        /// 绘制无限制栅格，并调用绘制委托列表
        /// </summary>
        /// <param name="image"></param>
        public static void Draw(Image image)
        {
            //if (DrawBackLattice)
            {
                var g = Graphics.FromImage(image);
                DrawLatticeCells(g);
                //
                // draw guide line
                //
                g.DrawLine(GuidePen, new(OriginLeft, DrawRect.Top), new(OriginLeft, DrawRect.Bottom));
                g.DrawLine(GuidePen, new(DrawRect.Left, OriginTop), new(DrawRect.Right, OriginTop));
                g.Flush(); g.Dispose();
            }
            Drawing.Invoke((Bitmap)image);

            DrawRect = DrawRect;

            var diffLeft = DrawRect.Left - OriginLeft;
            LatticedPointWithinCol.Item1 = diffLeft / LatticeCell.Length;
            if (diffLeft < 0 && diffLeft % LatticeCell.Length != 0) { LatticedPointWithinCol.Item1--; }
            LatticedPointWithinCol.Item2 = LatticedPointWithinCol.Item1 + ColNumber - 1;
            Program.testInfo.InfoText += $"col min {LatticedPointWithinCol.Item1} max {LatticedPointWithinCol.Item2}";
            var diffTop = DrawRect.Top - OriginTop;
            LatticedPointWithinRow.Item1 = diffTop / LatticeCell.Length;
            if (diffTop % LatticeCell.Length != 0) { LatticedPointWithinRow.Item1--; }
            LatticedPointWithinRow.Item2 = LatticedPointWithinRow.Item1 + RowNumber;
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
            var colNum = ColNumber + 1;
            var rowNum = RowNumber + 1;
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
                    if (CrossLineWithin(new(cellLeft, cellBottom), new(cellRight, cellBottom), CellBorderWidth, out var lineRect))
                    {
                        g.FillRectangle(new SolidBrush(CellBorderCoolor), lineRect);
                    }
                    if(CrossLineWithin(new(cellRight, cellTop), new(cellRight, cellBottom), CellBorderWidth, out lineRect))
                    {
                        g.FillRectangle(new SolidBrush(CellBorderCoolor), lineRect);
                    }
                    var nodeLeft = cellLeft + LatticeCell.NodePaddingWidth;
                    var nodeTop = cellTop + LatticeCell.NodePaddingHeight;
                    //
                    // draw node
                    //
                    if (CrossLineWithin(new(nodeLeft, nodeTop), new(cellRight, nodeTop), NodeBorderWidth, out lineRect))
                    {
                        g.FillRectangle(new SolidBrush(NodeBorderColor), lineRect);
                    }
                    if (CrossLineWithin(new(nodeLeft, nodeTop), new(cellLeft, cellBottom), NodeBorderWidth, out lineRect))
                    {
                        g.FillRectangle(new SolidBrush(NodeBorderColor), lineRect);
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
        public static bool CrossLineWithin(Point p1, Point p2, int lineWidth, out Rectangle lineRect)
        {
            lineRect = Rectangle.Empty;
            var halfLineWidth = lineWidth / 2;
            if (p1.Y == p2.Y)
            {
                if (!CrossLineWithin(p1.Y, DrawRect.Top, DrawRect.Bottom, (p1.X, p2.X), DrawRect.Left, DrawRect.Right, out var xMin, out var xMax)) { return false; }
                lineRect = new(xMin, p1.Y - halfLineWidth, xMax - xMin + halfLineWidth, lineWidth);
            }
            else
            {
                if (!CrossLineWithin(p1.X, DrawRect.Left, DrawRect.Right, (p1.Y, p2.Y), DrawRect.Top, DrawRect.Bottom, out var yMin, out var yMax)) { return false; }
                lineRect = new(p1.X - halfLineWidth, yMin, lineWidth, yMax - yMin + halfLineWidth);
            }
            return true;
        }
        /// <summary>
        /// 获取给定横纵直线在栅格绘图区域内的矩形
        /// </summary>
        /// <param name="p1">直线的端点</param>
        /// <param name="p2">直线的另一端点</param>
        /// <param name="lineWidth">直线的宽度</param>
        /// <param name="lineRect">在绘图区域内的横纵直线的矩形</param>
        /// <param name="isHorizon">判断是否是水平线（否则是竖直线）</param>
        /// <returns></returns>
        public static bool CrossLineWithin(Point p1, Point p2, int lineWidth, out Rectangle lineRect, out bool isHorizon)
        {
            lineRect = Rectangle.Empty;
            var halfLineWidth = lineWidth / 2;
            if (p1.Y == p2.Y)
            {
                isHorizon = true;
                if (!CrossLineWithin(p1.Y, DrawRect.Top, DrawRect.Bottom, (p1.X, p2.X), DrawRect.Left, DrawRect.Right, out var xMin, out var xMax)) { return false; }
                lineRect = new(xMin, p1.Y - halfLineWidth, xMax - xMin + halfLineWidth, lineWidth);
            }
            else
            {
                isHorizon = false;
                if (!CrossLineWithin(p1.X, DrawRect.Left, DrawRect.Right, (p1.Y, p2.Y), DrawRect.Top, DrawRect.Bottom, out var yMin, out var yMax)) { return false; }
                lineRect = new(p1.X - halfLineWidth, yMin, lineWidth, yMax - yMin + halfLineWidth);
            }
            return true;
        }
        private static bool CrossLineWithin(int theSame, int theSameLimitMin, int theSameLimitMax, (int, int) ends, int endLimitMin, int endLimitMax, out int endMin, out int endMax)
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
        public static Rectangle RectWithin(Rectangle rect)
        {
            var left = rect.Left;
            var right = rect.Right;
            var top = rect.Top;
            var bottom = rect.Bottom;
            var drRight = DrawRect.Right;
            var drBottom = DrawRect.Bottom;
            if (left < DrawRect.Left) { left = DrawRect.Left; }
            if (right > DrawRect.Right) { right = drRight; }
            if (top < DrawRect.Top) { top = DrawRect.Top; }
            if (bottom > DrawRect.Bottom) { bottom = drBottom; }
            return new(left, top, right - left, bottom - top);
        }
        public static bool LatticedPointWithin(LatticedPoint point)
        {
            if (point.Col < LatticedPointWithinCol.Item1) { return false; }
            if (point.Col > LatticedPointWithinCol.Item2) { return false; }
            if (point.Row < LatticedPointWithinRow.Item1) { return false; }
            if (point.Row > LatticedPointWithinRow.Item2) { return false; }
            return true;
        }

        #endregion
    }
}
