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
        /// 栅格放置区域（绘图区域应该调用 ToDrawRect）
        /// </summary>
        public static Rectangle Bounds
        {
            set
            {
                ColNumber = value.Width / LatticeCell.Width;
                RowWidth = ColNumber * LatticeCell.Width;
                RowNumber = value.Height / LatticeCell.Height;
                ColHeight = RowNumber * LatticeCell.Height;
                var deviOfDiffInWidth = (int)((float)(value.Width - RowWidth) * 0.5f);
                var deviOfDiffInHeight = (int)((float)(value.Height - ColHeight) * 0.5f);
                ToDrawRect = new Rectangle(
                    value.X + deviOfDiffInWidth,
                    value.Y + deviOfDiffInHeight,
                    RowWidth,
                    ColHeight
                    );
            }
        }
        /// <summary>
        /// 栅格绘图区域（根据给定放置区域、列数、行数自动生成，并在给定放置区域内居中）
        /// </summary>
        public static Rectangle ToDrawRect { get; private set; }
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
                CellOffsetLeft = (value - ToDrawRect.X) % LatticeCell.Width;
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
                CellOffsetTop = (value - ToDrawRect.Y) % LatticeCell.Height;
            }
        }
        static int originTop;
        /// <summary>
        /// 格元横坐标偏移量，对栅格坐标系原点相对于 ToDrawRect 的左上角的偏移量，在格元大小内实施相似偏移量
        /// </summary>
        public static int CellOffsetLeft;
        /// <summary>
        /// 格元纵坐标偏移量，对栅格坐标系原点相对于 ToDrawRect 的左上角的偏移量，在格元大小内实施相似偏移量
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
            if (left < ToDrawRect.Left) { left += ToDrawRect.Width; }
            else if (left > ToDrawRect.Right) { left -= ToDrawRect.Width; }
            var realWidth = ToDrawRect.Right - left;
            if (realWidth >= width)
            {
                return new (int, int)[] { (left, left + width) };
            }
            else
            {
                return new (int, int)[]
                {
                    (left, left + realWidth),
                    (ToDrawRect.Left, ToDrawRect.Left + width - realWidth)
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
            if (top < ToDrawRect.Top) { top += ToDrawRect.Height; }
            else if (top > ToDrawRect.Bottom) { top -= ToDrawRect.Height; }
            var realHeight = ToDrawRect.Bottom - top;
            if (realHeight >= height)
            {
                return new (int, int)[] { (top, top + height) };
            }
            else
            {
                return new (int, int)[]
                {
                    (top, top + realHeight),
                    (ToDrawRect.Top, ToDrawRect.Top + height - realHeight)
                };
            }
        }
    }
}
