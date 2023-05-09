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
        public static Pen CellPen = new(Color.FromArgb(200, Color.AliceBlue), 1.5f);
        /// <summary>
        /// 节点边界绘制用笔
        /// </summary>
        public static Pen NodePen = new(Color.FromArgb(150, Color.Orange), 1.5f);
        /// <summary>
        /// 坐标辅助线绘制用笔
        /// </summary>
        public static Pen GuidePen = new Pen(Color.FromArgb(50, Color.Red), 1.75f);
        /// <summary>
        /// 绘制委托列表（三层绘制）
        /// </summary>
        public static DrawLayers Drawing = new(3);

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
            DrawRect = bounds;
        }
        /// <summary>
        /// 绘制无限制栅格，并调用绘制格元的委托
        /// </summary>
        /// <param name="g"></param>
        public static void Draw(Image image)
        {
            Drawing.Invoke((Bitmap)image);
            //if (DrawBackLattice)
            {
                var g = Graphics.FromImage(image);
                DrawLatticeCells(g);
                // guide line
                g.DrawLine(GuidePen, new(OriginLeft, DrawRect.Top), new(OriginLeft, DrawRect.Bottom));
                g.DrawLine(GuidePen, new(DrawRect.Left, OriginTop), new(DrawRect.Right, OriginTop));
                g.Flush(); g.Dispose();
            }
            //Program.testInfo.Show();
            //Program.testInfo.InfoText = $"{new Point(ColNumber, RowNumber)}";
            //Program.testInfo.InfoText = $"{Drawing.MethodNumber()}\n" +
            //    $"1. {Drawing.MethodNumber(0)}, 2. {Drawing.MethodNumber(1)}, 3. {Drawing.MethodNumber(2)}";
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
        private static void DrawLatticeCells(Graphics g)
        {
            var offsetLeft = OriginLeft % LatticeCell.Width;
            var offSetTop = OriginTop % LatticeCell.Height;
            if (offsetLeft < 0) { offsetLeft += LatticeCell.Width; }
            if (offSetTop < 0) { offSetTop += LatticeCell.Height; }
            if (offsetLeft > 0) { offsetLeft -= LatticeCell.Width; }
            if (offSetTop > 0) { offSetTop -= LatticeCell.Height; }
            for (int i = 0; i < DrawRect.Width / LatticeCell.Width + 2; i++)
            {
                for (int j = 0; j < DrawRect.Height / LatticeCell.Height + 2; j++)
                {
                    var cellLeft = offsetLeft + i * LatticeCell.Width;
                    var cellTop = offSetTop + j * LatticeCell.Height;
                    var cellRight = cellLeft + LatticeCell.Width;
                    var cellBottom = cellTop + LatticeCell.Height;
                    if (CrossLineWithin(new(cellLeft, cellBottom), new(cellRight, cellBottom), out var line))
                    {
                        g.DrawLine(NodePen, line.Item1, line.Item2);
                    }
                    if(CrossLineWithin(new(cellRight, cellTop), new(cellRight, cellBottom), out line))
                    {
                        g.DrawLine(NodePen, line.Item1, line.Item2);
                    }
                }
            }
        }

        #endregion

        #region ==== 范围裁剪 ====

        public static bool CrossLineWithin(Point p1, Point p2, out (Point, Point) line)
        {
            line = new(p1, p2);
            if (p1 == p2) { throw new ArgumentException("Two points are the same."); }
            if (p1.X == p2.X)
            {
                var x = p1.X;
                if (x < DrawRect.Left || x > DrawRect.Right) { return false; }
                var yMin = Math.Min(p1.Y, p2.Y);
                var yMax = Math.Max(p1.Y, p2.Y);
                var drBottom = DrawRect.Bottom;
                if (yMax <= DrawRect.Top || yMin >= drBottom) { return false; }
                line = (new(x, yMin < DrawRect.Top ? DrawRect.Top : yMin), new(x, yMax > drBottom ? drBottom : yMax));
                return true;
            }
            if (p1.Y == p2.Y)
            {
                var y = p1.Y;
                if (y < DrawRect.Top || y > DrawRect.Bottom) { return false; }
                var xMin = Math.Min(p1.X, p2.X);
                var xMax = Math.Max(p1.X, p2.X);
                var drRight = DrawRect.Right;
                if (xMax <= DrawRect.Left || xMin >= drRight) { return false; }
                line = (new(xMin < DrawRect.Left ? DrawRect.Left : xMin, y), new(xMax > drRight ? drRight : xMax, y));
                return true;
            }
            throw new ArgumentException("Only cross line is supported.");
        }
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
