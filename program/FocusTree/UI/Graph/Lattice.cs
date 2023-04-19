#define DEBUG
using FocusTree.UI.test;
using static System.Formats.Asn1.AsnWriter;

namespace FocusTree.UI.Graph
{
    /// <summary>
    /// 栅格
    /// </summary>
    public class Lattice
    {
        #region ==== 基本参数 ====

        /// <summary>
        /// 栅格行数（根据格元高自动生成）
        /// </summary>
        int RowNumber;
        /// <summary>
        /// 栅格列数（根据格元宽自动生成）
        /// </summary>
        int ColNumber;
        /// <summary>
        /// 栅格总列宽
        /// </summary>
        public int RowWidth;
        /// <summary>
        /// 栅格总行高
        /// </summary>
        public int ColHeight;
        /// <summary>
        /// 栅格绘图区域（根据给定放置区域、列数、行数自动生成，并在给定放置区域内居中）
        /// </summary>
        public Rectangle DrawRect { get; private set; }
        /// <summary>
        /// 栅格绘图区域与放置区域的宽的差值的一半
        /// </summary>
        int DeviDiffInDrawRectWidth;
        /// <summary>
        /// 栅格绘图区域与放置区域的高的差值的一半
        /// </summary>
        int DeviDiffInDrawRectHeight;
        /// <summary>
        /// 栅格坐标系原点 x 坐标
        /// </summary>
        public int OriginLeft;
        /// <summary>
        /// 栅格坐标系原点 y 坐标
        /// </summary>
        public int OriginTop;
        /// <summary>
        /// 格元横坐标偏移量，对栅格坐标系原点相对于 DrawRect 的左上角的偏移量，在格元大小内实施相似偏移量
        /// </summary>
        public int CellOffsetLeft { get => OriginLeft - DrawRect.X + DeviDiffInDrawRectWidth; }
        /// <summary>
        /// 格元纵坐标偏移量，对栅格坐标系原点相对于 DrawRect 的左上角的偏移量，在格元大小内实施相似偏移量
        /// </summary>
        public int CellOffsetTop { get => OriginTop - DrawRect.Y + DeviDiffInDrawRectHeight; }

        #endregion

        #region ==== 绘制参数 ====

        /// <summary>
        /// 格元边界绘制用笔
        /// </summary>
        public static Pen CellPen = new(Color.AliceBlue, 1.5f);
        /// <summary>
        /// 节点边界绘制用笔
        /// </summary>
        public static Pen NodePen = new(Color.Orange, 1.5f);
        /// <summary>
        /// 绘制栅格时，绘制非循环节点的委托类型
        /// </summary>
        /// <param name="g">传入栅格的 GDI</param>
        public delegate void CellDrawer(Graphics g);
        /// <summary>
        /// 需要单独绘制的格元委托
        /// </summary>
        public CellDrawer DrawCell 
        { 
            get => drawCell ??= (g) => { }; 
            set => drawCell = value;
        }
        CellDrawer drawCell;

        #endregion

        #region ==== 绘制栅格 ====

        /// <summary>
        /// 绘制时设置栅格放置区域（首次调用绘图或更改绘图尺寸）
        /// </summary>
        /// <param name="bounds">放置区域</param>
        public void Draw(Graphics g, Rectangle bounds)
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
        /// <summary>
        /// 绘制（重绘）栅格及加入格元绘制委托的格元
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds">栅格放置区域</param>
        /// <param name="cursor">新的光标位置</param>
        public void Draw(Graphics g)
        {
            g.Clear(Color.White);

            DrawCell(g);
            var delArray = DrawCell.GetInvocationList();
            foreach ( var del in delArray )
            {
                DrawCell -= del as CellDrawer;
            }

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

        #region ==== 循环节点绘图方法 ====

        /// <summary>
        /// 绘制循环格元（格元左上角坐标与栅格坐标系中心偏移量近似投射在一个格元大小范围内）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="col">循环到的列数</param>
        /// <param name="row">循环到的行数</param>
        /// <param name="drawMain">是否绘制未超出栅格绘图区域的部分</param>
        /// <param name="drawAppend">是否补绘超出栅格绘图区域的部分</param>
        private void DrawLoopCell(Graphics g, int col, int row)
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
        private void DrawLoopCellLine(Graphics g, Pen pen, Point LeftTop, Size size)
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

        #region ==== 非循环节点绘图方法 ====

        /// <summary>
        /// 重绘单个格元（裁去超出栅格绘图区域的部分）
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cell"></param>
        public void ReDrawCell(Graphics g, LatticeCell cell)
        {
            g.FillRectangle(new SolidBrush(Color.White), cell.RealRect(this));
            DrawCellLines(g, CellPen, new(cell.RealLeft(this), cell.RealTop(this)), new(LatticeCell.Width, LatticeCell.Height));
            DrawCellLines(g, NodePen, new(cell.NodeRealLeft(this), cell.NodeRealTop(this)), new(LatticeCell.NodeWidth, LatticeCell.NodeHeight));
        }
        /// <summary>
        /// 在栅格绘图区域内绘制左下-左上-上右的七形线
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen"></param>
        /// <param name="LeftTop">七形线左上角坐标</param>
        /// <param name="size">七形线尺寸</param>
        private void DrawCellLines(Graphics g, Pen pen, Point LeftTop, Size size)
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
        public bool RectWithinDrawRect(Rectangle rect, out Rectangle saveRect)
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
            saveRect =  new(left, top,
                left + width > DrawRect.Right ? DrawRect.Right - left : width,
                top + height > DrawRect.Bottom ? DrawRect.Bottom - top : height
                );
            return true;
        }
        /// <summary>
        /// 高亮格元被选中的部分，并在重绘栅格时绘制高亮
        /// </summary>
        /// <param name="cell">选中的格元</param>
        /// <param name="cursor">选中坐标</param>
        /// <returns></returns>
        public LatticeCell.InnerParts HighlightCellSelectionWithinRedraw(LatticeCell cell, Point cursor)
        {
            var rects = cell.InnerPartRealRects(this);
            foreach (var pair in rects) 
            {
                var rect = pair.Value.Item1;
                if (!rect.Contains(cursor)) { continue; }
                var part = pair.Key;
                if (part != cell.LastTouchPart)
                {
                    cell.LastTouchPart = part;
                    if (RectWithinDrawRect(rect, out var saveRect))
                    {
                        DrawCell += (g) => { g.FillRectangle(pair.Value.Item2, saveRect); };
                    }
                }
                return part;
            }

            return LatticeCell.InnerParts.Leave;
        }
        /// <summary>
        /// 立即绘制高亮光标进入格元后所处其中的部分，或取消高亮光标刚离开的部分
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lattice">关联的栅格</param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public LatticeCell.InnerParts HightLightCellSelectionInstantly(Graphics g, LatticeCell cell, Point cursor)
        {
            var rects = cell.InnerPartRealRects(this);
            foreach (var pair in rects) 
            {
                var rect = pair.Value.Item1;
                if (!rect.Contains(cursor)) { continue; }
                var part = pair.Key;
                if (part != cell.LastTouchPart)
                {
                    ReDrawCell(g, cell);
                    cell.LastTouchPart = part;
                    if (RectWithinDrawRect(rect, out var saveRect))
                    {
                        g.FillRectangle(pair.Value.Item2, saveRect);
                    }
                }
                return part;
            }

            ReDrawCell(g, cell);
            return LatticeCell.InnerParts.Leave;
        }
    }
}
