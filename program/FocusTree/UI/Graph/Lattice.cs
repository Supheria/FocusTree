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
    struct Lattice
    {
        /// <summary>
        /// 栅格行数
        /// </summary>
        public static int RowNumber { get; private set; }
        /// <summary>
        /// 栅格列数
        /// </summary>
        public static int ColNumber { get; private set; }
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
        /// <summary>
        /// 
        /// </summary>
        public static int ScaleFactor = 1;
        /// <summary>
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
            return new(
                left,
                top,
                left + width > DrawRect.Right ? DrawRect.Right - left : width,
                top + height > DrawRect.Bottom ? DrawRect.Bottom - top : height
                );
        }
    }
}
