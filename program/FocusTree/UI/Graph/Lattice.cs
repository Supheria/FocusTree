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
        public static int OriginLeft
        {
            get => originLeft;
            set
            {
                originLeft = value;
                CellOffsetLeft = (value - DrawRect.X) % LatticeCell.Width + DeviDiffInDrawRectWidth;
            }
        }
        static int originLeft;
        /// <summary>
        /// 栅格坐标系原点 y 坐标
        /// </summary>
        public static int OriginTop
        {
            get => originTop;
            set
            {
                originTop = value;
                CellOffsetTop = (value - DrawRect.Y) % LatticeCell.Height + DeviDiffInDrawRectHeight;
            }
        }
        static int originTop;
        /// <summary>
        /// 格元横坐标偏移量，对栅格坐标系原点相对于 DrawRect 的左上角的偏移量，在格元大小内实施相似偏移量
        /// </summary>
        public static int CellOffsetLeft;
        /// <summary>
        /// 格元纵坐标偏移量，对栅格坐标系原点相对于 DrawRect 的左上角的偏移量，在格元大小内实施相似偏移量
        /// </summary>
        public static int CellOffsetTop;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public static int ScaleFactor = 1;

        #region ==== 绘制栅格 ====

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
        /// <summary>
        /// 格元边界绘制用笔
        /// </summary>
        public static Pen CellPen = new Pen(Color.AliceBlue, 1.5f);
        public static Pen NodePen = new Pen(Color.Orange, 1.5f);
        /// <summary>
        /// 绘制无限制栅格
        /// </summary>
        /// <param name="g"></param>
        public static void Draw(Graphics g)
        {
            for (int i = 0; i < ColNumber; i++)
            {
                for (int j = 0; j < RowNumber; j++)
                {
                    LatticeCell.RealLeft = i * LatticeCell.Width + CellOffsetLeft;
                    LatticeCell.RealTop = j * LatticeCell.Height + CellOffsetTop;
                    var cellLeftLine = LineToDrawInHorizon(LatticeCell.RealLeft, LatticeCell.Width);
                    var cellTopLine = LineToDrawInVertical(LatticeCell.RealTop, LatticeCell.Height);
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
                    var nodeLeftLine = LineToDrawInHorizon(LatticeCell.NodeRealLeft, LatticeCell.NodeWidth);
                    var nodeTopLine = LineToDrawInVertical(LatticeCell.NodeRealTop, LatticeCell.NodeHeight);
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
        public static Rectangle RectWithinDrawRect(int left, int top, int width, int height)
        {
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
        public static Point PointWithinDrawRect(Point point)
        {
            if (point.X < DrawRect.Left) { point.X = DrawRect.Left; }
            else if (point.X > DrawRect.Right) { point.X = DrawRect.Right; }
            if (point.Y < DrawRect.Top) { point.Y = DrawRect.Top; }
            else if (point.Y > DrawRect.Bottom) { point.Y = DrawRect.Bottom; }
            return point;
        }
    }
}
