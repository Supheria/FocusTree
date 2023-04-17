using FocusTree.UI.test;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.UI.Graph
{
    /// <summary>
    /// 栅格
    /// </summary>
    static class Lattice
    {
        #region ==== 设置列行数 ====

        /// <summary>
        /// 栅格行数（根据格元高自动生成）
        /// </summary>
        public static int RowNumber { get; private set; }
        /// <summary>
        /// 栅格列数（根据格元宽自动生成）
        /// </summary>
        public static int ColNumber { get; private set; }

        #endregion

        #region ==== 设置边界区域 ====

        /// <summary>
        /// 栅格放置区域（绘图区域应该调用 DrawRect）
        /// </summary>
        public static Rectangle Bounds
        {
            set
            {
                ColNumber = value.Width / LatticeCell.Width;
                RowWidth = ColNumber * LatticeCell.Width;
                RowNumber = value.Height / LatticeCell.Height;
                ColHeight = RowNumber * LatticeCell.Height;
                DeviDiffInDrawRectWidth = (int)((float)(value.Width - RowWidth) * 0.5f);
                DeviDiffInDrawRectHeight = (int)((float)(value.Height - ColHeight) * 0.5f);
                DrawRect = new Rectangle(
                    value.X + DeviDiffInDrawRectWidth,
                    value.Y + DeviDiffInDrawRectHeight,
                    RowWidth,
                    ColHeight
                    );
            }
        }
        /// <summary>
        /// 栅格绘图区域与放置区域的宽的差值的一半
        /// </summary>
        static int DeviDiffInDrawRectWidth;
        /// <summary>
        /// 栅格绘图区域与放置区域的高的差值的一半
        /// </summary>
        static int DeviDiffInDrawRectHeight;
        /// <summary>
        /// 栅格绘图区域（根据给定放置区域、列数、行数自动生成，并在给定放置区域内居中）
        /// </summary>
        public static Rectangle DrawRect { get; private set; }
        /// <summary>
        /// 栅格总列宽
        /// </summary>
        public static int RowWidth;
        /// <summary>
        /// 栅格总行高
        /// </summary>
        public static int ColHeight;

        #endregion

        #region ==== 设置栅格坐标系原点 ====

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

        /// <summary>
        /// 
        /// </summary>
        public static int ScaleFactor = 1;

        #region ==== 绘制栅格 ====

        /// <summary>
        /// 格元边界绘制用笔
        /// </summary>
        public static Pen CellPen = new(Color.AliceBlue, 1.5f);
        /// <summary>
        /// 节点边界绘制用笔
        /// </summary>
        public static Pen NodePen = new(Color.Orange, 1.5f);
        /// <summary>
        /// ===绘制栅格时调用===
        /// 从左向右的循环横坐标
        /// </summary>
        /// <param name="left">左端点</param>
        /// <param name="width">循环单位宽度</param>
        /// <returns>返回间隔循环单位宽度的起点横坐标和终点横坐标的数对。如果起点或终点超出绘图区域，则返回数组里有一个额外的分割剩余线段的坐标数对。</returns>
        public static (int, int)[] LoopInHorizon(int left, int width)
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
        /// 从上向下的循环纵坐标
        /// </summary>
        /// <param name="top">上端点</param>
        /// <param name="height">循环单位高度</param>
        /// <returns>返回从上向下的垂直线的起点坐标和终点坐标的数对。如果线需要分割，则返回数组里有一个额外的分割剩余线段的坐标数对。</returns>
        public static (int, int)[] LoopInVertical(int top, int height)
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
        /// <summary>
        /// 需要单独绘制的格元列表
        /// </summary>
        public static List<LatticeCell> CellsToDraw = new();
        public static List<(LatticeCell, Rectangle, SolidBrush)> CellsToFill = new();
        /// <summary>
        /// 绘制无限制栅格
        /// </summary>
        /// <param name="g"></param>
        public static void Draw(Graphics g)
        {
            foreach(var cell in CellsToDraw)
            {

            }
            for (int i = 0; i < ColNumber; i++)
            {
                for (int j = 0; j < RowNumber; j++)
                {
                    //DrawLoopCell(g,
                    //    i * LatticeCell.Width + (OriginLeft - DrawRect.X) % LatticeCell.Width + DeviDiffInDrawRectWidth,
                    //    j * LatticeCell.Height + (OriginTop - DrawRect.Y) % LatticeCell.Height + DeviDiffInDrawRectHeight
                    //    );
                }
            }
        }
        /// <summary>
        /// 循环往复地在栅格绘图区域内绘制格元
        /// </summary>
        /// <param name="g"></param>
        private static void DrawLoopCell(Graphics g, int cellLeft, int cellTop)
        {
            var cellLeftLine = LoopInHorizon(cellLeft, LatticeCell.Width);
            var cellTopLine = LoopInVertical(cellTop, LatticeCell.Height);
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
            var nodeLeftLine = LoopInHorizon(nodeLeft, LatticeCell.NodeWidth);
            var nodeTopLine = LoopInVertical(nodeTop, LatticeCell.NodeHeight);
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
        /// 重绘格元
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cell"></param>
        public static void ReDrawCell(Graphics g, LatticeCell cell)
        {
            g.FillRectangle(new SolidBrush(Color.White), cell.RealRect);
            ReDrawCell(g, CellPen, 
                new(cell.RealLeft, cell.RealTop),
                new(LatticeCell.Width, LatticeCell.Height)
                );
            ReDrawCell(g, NodePen, 
                new(cell.NodeRealLeft, cell.NodeRealTop), 
                new(LatticeCell.NodeWidth, LatticeCell.NodeHeight)
                );
        }
        static TestInfo test = new();
        /// <summary>
        /// 在栅格绘图区域内绘制左下-左上-上右的七形线
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen"></param>
        /// <param name="LeftTop">七形线左上角坐标</param>
        /// <param name="size">七形线尺寸</param>
        static void ReDrawCell(Graphics g, Pen pen, Point LeftTop, Size size)
        {
            test.Show();
            var left = LeftTop.X;
            var top = LeftTop.Y;
            if (top > DrawRect.Top && top < DrawRect.Bottom)
            {
                
                var right = left + size.Width;
                test.InfoText = $"raw left: {left}, raw right: {right}\nrect left: {DrawRect.Left}, right: {DrawRect.Right}, width: {DrawRect.Width}\n";
                //
                if (left < DrawRect.Left)
                {
                    left = DrawRect.Width + (left % DrawRect.Width);
                    if (left < DrawRect.Left) { left += DrawRect.Width; }
                    test.InfoText += $"mode right left: {left % DrawRect.Right}\n";
                    test.InfoText += $"mode width left: {left % DrawRect.Width}\n";
                    var cutWidth = DrawRect.Right - left;
                    if (cutWidth > size.Width) { cutWidth = size.Width; }
                    g.DrawLine(pen,
                        new(DrawRect.Left, top),
                        new(DrawRect.Left + size.Width - cutWidth, top)
                        );
                    // IF append
                    g.DrawLine(pen,
                        new(left, top),
                        new(left + cutWidth, top)
                        );
                    //return;
                }
                else if (right > DrawRect.Right)
                {
                    test.InfoText += $"mode right to Right: {right % DrawRect.Right}, to Width{right % DrawRect.Width}";
                    var testRight = right % DrawRect.Width;
                    if (testRight >= DrawRect.Left) { right = testRight; }
                    var cutWidth = right - DrawRect.Left;
                    if (cutWidth > size.Width) { cutWidth = size.Width; }
                    g.DrawLine(pen,
                        new(DrawRect.Right - (size.Width - cutWidth), top),
                        new(DrawRect.Right, top)
                        );
                    // IF append
                    g.DrawLine(pen,
                        new(right - cutWidth, top),
                        new(right, top)
                        );
                }
                    //g.DrawLine(pen,
                    //    new(DrawRect.Left, top),
                    //    new(right, top)
                    //    );
                    //g.DrawLine(pen,
                    //    new(left + DrawRect.Width, top),
                    //    new(DrawRect.Right, top)
                    //    );
                    //return; 
                //g.DrawLine(pen,
                //    new(left < DrawRect.Left ? DrawRect.Left : left, top),
                //    new(right > DrawRect.Right ? DrawRect.Right : right, top)
                //    );
            }
            //if (left > DrawRect.Left && left < DrawRect.Right)
            //{
            //    var bottom = top + size.Height;
            //    if (top > DrawRect.Bottom || bottom < DrawRect.Top) {  return; }
            //    g.DrawLine(pen,
            //        new(left, top < DrawRect.Top ? DrawRect.Top : top),
            //        new(left, bottom > DrawRect.Bottom ? DrawRect.Bottom : bottom)
            //        );
            //}
        }
    }
}
