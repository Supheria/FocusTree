using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.UI.Graph
{
    /// <summary>
    /// 格元
    /// </summary>
    struct LatticeCell
    {
        /// <summary>
        /// 格元真实左边界
        /// </summary>
        public static int RealLeft;
        /// <summary>
        /// 格元真实上边界
        /// </summary>
        public static int RealTop;
        /// <summary>
        /// 节点真实左边界
        /// </summary>
        public static int NodeRealLeft { get => RealLeft + NodePaddingWidth; }
        /// <summary>
        /// 节点真实上边界
        /// </summary>
        public static int NodeRealTop { get => RealTop + NodePaddingHeight; }
        /// <summary>
        /// 格元宽（限制最小值和最大值）
        /// </summary>
        public static int Width 
        {
            get => width = width < WidthMinimum ? WidthMinimum : width > WidthMaximum ? WidthMaximum : width;
            set => width = value < WidthMinimum ? WidthMinimum : value > WidthMaximum ? WidthMaximum : value;
        }
        static int width;
        /// <summary>
        /// 格元高（限制最小值和最大值）
        /// </summary>
        public static int Height
        {
            get => height = height < HeightMinimum ? HeightMinimum : height > HeightMaximum ? HeightMaximum : height;
            set => height = value < HeightMinimum ? HeightMinimum : value > HeightMaximum ? HeightMaximum : value;
        }
        static int height;
        /// <summary>
        /// 格元宽最小值
        /// </summary>
        public static int WidthMinimum = 50;
        /// <summary>
        /// 格元宽最小值
        /// </summary>
        public static int WidthMaximum = 200;
        /// <summary>
        /// 格元高最小值
        /// </summary>
        public static int HeightMinimum = 30;
        /// <summary>
        /// 格元高最大值
        /// </summary>
        public static int HeightMaximum = 100;
        /// <summary>
        /// 节点宽
        /// </summary>
        public static int NodeWidth { get => Width - NodePaddingWidth; }
        /// <summary>
        /// 节点高
        /// </summary>
        public static int NodeHeight { get => Height - NodePaddingHeight; }
        /// <summary>
        /// 节点 Left 到格元 Left 的空隙
        /// </summary>
        public static int NodePaddingWidth { get => (int)(width * 0.3f); }
        /// <summary>
        /// 节点 Top 到格元 Top 的空隙
        /// </summary>
        public static int NodePaddingHeight { get => (int)(height * 0.3f); }
        
        ///// <summary>
        ///// ===绘制栅格时调用===
        ///// 栅格绘图区域内格元左边界(起点x, 终点x)（如果分裂则返回右、左顺序的两个数对）
        ///// </summary>
        //public static (int, int)[] DrawLeftLine()
        //{
        //    return Lattice.LineToDrawInHorizon(RealLeftTop.X, Width);
        //}
        ///// <summary>
        ///// ===绘制栅格时调用===
        ///// 栅格绘图区域内节点左边界(起点x, 终点x)（如果分裂则返回右、左顺序的两个数对）
        ///// </summary>
        //public static (int, int)[] NodeDrawLeftLine()
        //{
        //    return Lattice.LineToDrawInHorizon(RealLeftTop.X + NodePaddingWidth, Width - NodePaddingWidth);
        //}
        ///// <summary>
        ///// ===绘制栅格时调用===
        ///// 栅格绘图区域内格元上边界(起点y, 终点y)（如果分裂则返回下、上顺序的两个数对）
        ///// </summary>
        //public static (int, int)[] DrawTopLine()
        //{
        //    return Lattice.LineToDrawInVertical(RealLeftTop.Y, Height);
        //}
        ///// <summary>
        ///// ===绘制栅格时调用===
        ///// 栅格绘图区域内节点上边界(起点y, 终点y)（如果分裂则返回下、上顺序的两个数对）
        ///// </summary>
        //public static (int, int)[] NodeDrawTopLine()
        //{
        //    return Lattice.LineToDrawInVertical(RealLeftTop.Y + NodePaddingHeight, Height - NodePaddingHeight);
        //}
        
        /// <summary>
        /// 在给定的格元矩形中获得其中的节点矩形
        /// </summary>
        /// <returns></returns>
        public static Rectangle NodeRectInCellRect(int left, int top, int width, int height)
        {
            return Lattice.RectWithinDrawRect(
                left + NodePaddingWidth,
                top + NodePaddingHeight,
                width - NodePaddingWidth,
                height - NodePaddingHeight
                );
        }
        /// <summary>
        /// 以格元为单位，根据给定坐标对栅格坐标系原点的偏移量设置格元的真实左上角坐标
        /// </summary>
        /// <param name="point">给定坐标</param>
        /// <returns>以格元为单位，格元左上角坐标对栅格原点的偏移量</returns>
        public static Point SetRealLeftTopWithPointOffsetToLatticeOrigin(Point point)
        {
            var widthDiff = point.X - Lattice.OriginLeft;
            var heightDiff = point.Y - Lattice.OriginTop;
            var widthCells = widthDiff / Width;
            var heightCells = heightDiff / Height;
            widthCells = widthDiff < 0 ? widthCells - 1 : widthCells;
            heightCells = heightDiff < 0 ? heightCells - 1 : heightCells;
            RealLeft = Width * widthCells + Lattice.CellOffsetLeft;
            RealTop = Height * heightCells + Lattice.CellOffsetTop;
            return new(widthCells, heightCells);
        }
        public static void Draw(Graphics g, Point? cellLeftTop = null)
        {
            if (cellLeftTop == null) { cellLeftTop = new Point(RealLeft, RealTop); }
            var point = cellLeftTop.Value;
            //
            // drawLeftRight
            //
            if (point.Y > Lattice.DrawRect.Top && point.Y < Lattice.DrawRect.Bottom)
            {
                var right = point.X + Width;
                g.DrawLine(Lattice.CellPen,
                    new(point.X < Lattice.DrawRect.Left ? Lattice.DrawRect.Left : point.X, point.Y),
                    new(right > Lattice.DrawRect.Right ? Lattice.DrawRect.Right : right, point.Y)
                    );
            }
            if (point.X > Lattice.DrawRect.Left && point.X < Lattice.DrawRect.Right)
            {
                var bottom = point.Y + Height;
                g.DrawLine(Lattice.CellPen,
                    new(point.X, point.Y < Lattice.DrawRect.Top ? Lattice.DrawRect.Top : point.Y),
                    new(point.X, bottom > Lattice.DrawRect.Bottom ? Lattice.DrawRect.Bottom : bottom)
                    );
            }
        }
    }
}
