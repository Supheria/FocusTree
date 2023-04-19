
using static System.Formats.Asn1.AsnWriter;

namespace FocusTree.UI.Graph
{
    /// <summary>
    /// 格元
    /// </summary>
    public struct LatticeCell
    {
        #region ==== 设置宽高和间距 ====

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
        public static int WidthMinimum = 24;
        /// <summary>
        /// 格元宽最小值
        /// </summary>
        public static int WidthMaximum = 240;
        /// <summary>
        /// 格元高最小值
        /// </summary>
        public static int HeightMinimum = 10;
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
        public static int NodePaddingWidth { get => (int)(width * NodePaddingFactor.X); }
        /// <summary>
        /// 节点 Top 到格元 Top 的空隙
        /// </summary>
        public static int NodePaddingHeight { get => (int)(height * NodePaddingFactor.Y); }
        /// <summary>
        /// 节点空隙系数（0.3 < X < 0.5, 0.3 < Y < 0.5)
        /// </summary>
        public static PointF NodePaddingFactor
        {
            get => new(npf.X < 0.3f ? 0.3f : npf.X > 0.5f ? 0.5f : npf.X, npf.Y < 0.3f ? 0.3f : npf.Y > 0.5f ? 0.5f : npf.Y);
            set => npf = new(value.X < 0.3f ? 0.3f : value.X > 0.5f ? 0.5f : value.X, value.Y < 0.3f ? 0.3f : value.Y > 0.5f ? 0.5f : value.Y);
        }
        public static PointF npf;

        #endregion

        #region ==== 栅格引用参数 ====

        /// <summary>
        /// 格元栅格化左边界
        /// </summary>
        public int LatticedLeft;
        /// <summary>
        /// 格元栅格化上边界
        /// </summary>
        public int LatticedTop;
        /// <summary>
        /// 格元真实左边界
        /// </summary>
        public int RealLeft(Lattice lattice)
        { 
            return Width * LatticedLeft + lattice.CellOffsetLeft; 
        }
        /// <summary>
        /// 格元真实上边界
        /// </summary>
        public int RealTop(Lattice lattice)
        {
           return Height * LatticedTop + lattice.CellOffsetTop;
        }
        /// <summary>
        /// 格元真实坐标矩形
        /// </summary>
        /// <returns></returns>
        public Rectangle RealRect(Lattice lattice)
        { 
            return new(RealLeft(lattice), RealTop(lattice), Width, Height);
        }
        /// <summary>
        /// 节点真实左边界
        /// </summary>
        public int NodeRealLeft(Lattice lattice)
        { 
            return RealLeft(lattice) + NodePaddingWidth; 
        }
        /// <summary>
        /// 节点真实上边界
        /// </summary>
        public int NodeRealTop(Lattice lattice)
        { 
            return RealTop(lattice) + NodePaddingHeight; 
        }
        /// <summary>
        /// 节点真实坐标矩形
        /// </summary>
        public Rectangle NodeRealRect(Lattice lattice)
        { 
            return new(NodeRealLeft(lattice), NodeRealTop(lattice), NodeWidth, NodeHeight); 
        }

        #endregion

        #region ==== 构造函数 ====

        public LatticeCell()
        {
            LatticedLeft = 0;
            LatticedTop = 0;
        }
        public LatticeCell(Lattice lattice, Point realPoint)
        {
            var widthDiff = realPoint.X - lattice.OriginLeft;
            var heightDiff = realPoint.Y - lattice.OriginTop;
            LatticedLeft = widthDiff / Width;
            LatticedTop = heightDiff / Height;
            if (widthDiff < 0) { LatticedLeft--; }
            if (heightDiff < 0) { LatticedTop--; }
        }

        #endregion

        /// <summary>
        /// 格元的内部区域
        /// </summary>
        public enum InnerParts
        {
            /// <summary>
            /// 离开格元
            /// </summary>
            Leave,
            /// <summary>
            /// 节点左侧区域
            /// </summary>
            Left,
            /// <summary>
            /// 节点上方区域
            /// </summary>
            Top,
            /// <summary>
            /// 节点左上方区域
            /// </summary>
            LeftTop,
            /// <summary>
            /// 节点区域
            /// </summary>
            Node
        }
        /// <summary>
        /// 上一次接触的格元内部区域
        /// </summary>
        public InnerParts LastTouchPart = new();
        /// <summary>
        /// 节点左侧区域填充颜色
        /// </summary>
        public Color InnerPartColor_Left = Color.FromArgb(100, Color.Gray);
        /// <summary>
        /// 节点上方区域填充颜色
        /// </summary>
        public Color InnerPartColor_Top = Color.FromArgb(100, Color.Gray);
        /// <summary>
        /// 节点左上方区域填充颜色
        /// </summary>
        public Color InnerPartColor_LeftTop = Color.FromArgb(100, Color.Gray);
        /// <summary>
        /// 节点区域填充颜色
        /// </summary>
        public Color InnerPartColor_Node = Color.FromArgb(150, Color.Orange);
        /// <summary>
        /// 格元内部各个区域的真实矩形
        /// </summary>
        /// <param name="lattice"></param>
        /// <returns></returns>
        public Dictionary<InnerParts, (Rectangle, SolidBrush)> InnerPartRealRects(Lattice lattice)
        {
            return new()
            {
                [InnerParts.Left] = (new(RealLeft(lattice), NodeRealTop(lattice), Width - NodeWidth, Height - NodePaddingHeight), new(InnerPartColor_Left)),
                [InnerParts.Top] = (new(NodeRealLeft(lattice), RealTop(lattice), Width - NodePaddingWidth, Height - NodeHeight), new(InnerPartColor_Top)),
                [InnerParts.LeftTop] = (new(RealLeft(lattice), RealTop(lattice), Width - NodeWidth, Height - NodeHeight), new(InnerPartColor_LeftTop)),
                [InnerParts.Node] = (NodeRealRect(lattice), new(InnerPartColor_Node))
            };
        }
    }
}
