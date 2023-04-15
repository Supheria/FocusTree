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
        public static Point ColRowInLattice
        {
            get => colRowInLattice;
            set
            {
                colRowInLattice = value;
                RealLeftTop.X = Width * value.X + Lattice.CellOffsetLeft;
                RealLeftTop.Y = Height * value.Y + Lattice.CellOffsetTop;
            }
        }
        static Point colRowInLattice;
        /// <summary>
        /// 格元真实左上角坐标
        /// </summary>
        public static Point RealLeftTop;
        /// <summary>
        /// 格元宽长（根据给定放置区域和给定列数自动生成）
        /// </summary>
        public static int Width 
        {
            get => width;
            set
            {
                width = value;
                NodePaddingWidth = (int)(value * 0.3f);
            }
        }
        static int width;
        /// <summary>
        /// 格元高长（根据给定放置区域和给定行数自动生成）
        /// </summary>
        public static int Height
        {
            get => height;
            set
            {
                height = value;
                NodePaddingHeight = (int)(Height * 0.3f);
            }
        }
        static int height;
        /// <summary>
        /// 节点 Left 到格元 Left 的空隙
        /// </summary>
        public static int NodePaddingWidth { get; private set; }
        /// <summary>
        /// 节点 Top 到格元 Top 的空隙
        /// </summary>
        public static int NodePaddingHeight { get; private set; }
        /// <summary>
        /// 栅格区域内格元绘图左边界
        /// </summary>
        public static (int, int)[] ToDrawLeftRight()
        {
            return Lattice.LineToDrawInHorizon(RealLeftTop.X, Width);
        }
        /// <summary>
        /// 节点栅格区域内绘图上边界(起点x, 终点x)（如果分裂则返回右、左顺序的两个数对）
        /// </summary>
        public static (int, int)[] NodeToDrawLeftRight()
        {
            return Lattice.LineToDrawInHorizon(RealLeftTop.X + NodePaddingWidth, Width - NodePaddingWidth);
        }
        /// <summary>
        /// 格元元左上边界（绘图应该调用 ToDrawTop）
        /// <summary>
        /// 栅格区域内格元绘图上边界
        /// </summary>
        public static (int, int)[] ToDrawTopBottom()
        {
            return Lattice.LineToDrawInVertical(RealLeftTop.Y, Height);
        }
        /// <summary>
        /// 节点栅格区域内绘图上边界(起点y, 终点y)（如果分裂则返回下、上顺序的两个数对）
        /// </summary>
        public static (int, int)[] NodeToDrawTopBottom()
        {
            return Lattice.LineToDrawInVertical(RealLeftTop.Y + NodePaddingHeight, Height - NodePaddingHeight);
        }
    }
}
