using FocusTree.Data.Focus;

namespace FocusTree.UI.Graph
{
    /// <summary>
    /// 格元
    /// </summary>
    struct LatticeCell
    {
        #region ==== 设置宽高和间距 ====

        /// <summary>
        /// 格元宽（限制最小值和最大值）
        /// </summary>
        public static int Width
        {
            get => width;
            set => width = value < SizeMin.Width ? SizeMin.Width : value > SizeMax.Width ? SizeMax.Width : value;
        }
        static int width = 30;
        /// <summary>
        /// 格元高（限制最小值和最大值）
        /// </summary>
        public static int Height
        {
            get => height;
            set => height = value < SizeMin.Height ? SizeMin.Height : value > SizeMax.Height ? SizeMax.Height : value;
        }
        static int height = 30;
        /// <summary>
        /// 最小尺寸
        /// </summary>
        public static Size SizeMin = new(25, 25);
        /// <summary>
        /// 最大尺寸
        /// </summary>
        public static Size SizeMax = new(100, 100);
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
        public static int NodePaddingWidth { get => (int)(width * NodePaddingZoomFactor.X); }
        /// <summary>
        /// 节点 Top 到格元 Top 的空隙
        /// </summary>
        public static int NodePaddingHeight { get => (int)(height * NodePaddingZoomFactor.Y); }
        /// <summary>
        /// 节点空隙系数（0.3 < X < 0.7, 0.3 < Y < 0.7)
        /// </summary>
        public static PointF NodePaddingZoomFactor
        {
            get => new(npf.X < 0.3f ? 0.3f : npf.X > 0.7f ? 0.7f : npf.X, npf.Y < 0.3f ? 0.3f : npf.Y > 0.7f ? 0.7f : npf.Y);
            set => npf = new(value.X < 0.3f ? 0.3f : value.X > 0.7f ? 0.7f : value.X, value.Y < 0.3f ? 0.3f : value.Y > 0.7f ? 0.7f : value.Y);
        }
        static PointF npf = new(0.3f, 0.5f);

        #endregion

        #region ==== 坐标 ====

        /// <summary>
        /// 格元栅格化左边界
        /// </summary>
        public int LatticedLeft { get; set; }
        /// <summary>
        /// 格元栅格化上边界
        /// </summary>
        public int LatticedTop { get; set; }
        public Point LatticedPoint { get => new(LatticedLeft, LatticedTop); }
        /// <summary>
        /// 格元真实左边界
        /// </summary>
        public int RealLeft { get => Width * LatticedLeft + Lattice.CellOffsetLeft; }
        /// <summary>
        /// 格元真实上边界
        /// </summary>
        public int RealTop { get => Height * LatticedTop + Lattice.CellOffsetTop; }
        /// <summary>
        /// 格元真实坐标矩形
        /// </summary>
        /// <returns></returns>
        public Rectangle RealRect { get => new(RealLeft, RealTop, Width, Height); }
        /// <summary>
        /// 节点真实左边界
        /// </summary>
        public int NodeRealLeft { get => RealLeft + NodePaddingWidth; }
        /// <summary>
        /// 节点真实上边界
        /// </summary>
        public int NodeRealTop { get => RealTop + NodePaddingHeight; }
        /// <summary>
        /// 节点真实坐标矩形
        /// </summary>
        public Rectangle NodeRealRect { get => new(NodeRealLeft, NodeRealTop, NodeWidth, NodeHeight); }

        #endregion

        #region ==== 构造函数 ====

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LatticeCell()
        {
            LatticedLeft = 0;
            LatticedTop = 0;
        }
        /// <summary>
        /// 使用真实坐标创建，将坐标转换为栅格化坐标
        /// </summary>
        /// <param name="cursor"></param>
        public LatticeCell(Point point)
        {
            var widthDiff = point.X - Lattice.OriginLeft;
            var heightDiff = point.Y - Lattice.OriginTop;
            LatticedLeft = widthDiff / Width;
            LatticedTop = heightDiff / Height;
            if (widthDiff < 0) { LatticedLeft--; }
            if (heightDiff < 0) { LatticedTop--; }
        }
        /// <summary>
        /// 使用已有的栅格化坐标创建
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        public LatticeCell(int col, int row)
        {
            LatticedLeft = col;
            LatticedTop = row;
        }
        /// <summary>
        /// 使用国策对象创建
        /// </summary>
        /// <param name="focus"></param>
        public LatticeCell(FocusData focus)
        {
            LatticedLeft = focus.LatticedPoint.X;
            LatticedTop = focus.LatticedPoint.Y;
        }

        #endregion

        #region ==== 格元区域 ====

        /// <summary>
        /// 格元的内部区域
        /// </summary>
        public enum Parts
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
        /// 上一次接触的区域
        /// </summary>
        public Rectangle LastTouchRect = new();
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
        public Dictionary<Parts, Rectangle> InnerPartRealRects
        {
            get => new()
            {
                [Parts.Left] = new(RealLeft, NodeRealTop, Width - NodeWidth, Height - NodePaddingHeight),
                [Parts.Top] = new(NodeRealLeft, RealTop, Width - NodePaddingWidth, Height - NodeHeight),
                [Parts.LeftTop] = new(RealLeft, RealTop, Width - NodeWidth, Height - NodeHeight),
                [Parts.Node] = NodeRealRect
            };
        }
        /// <summary>
        /// 获取坐标在格元上所处的部分
        /// </summary>
        /// <param name="point">坐标</param>
        /// <returns></returns>
        public Parts GetInnerPartPointOn(Point point)
        {
            foreach (var pair in InnerPartRealRects)
            {
                var rect = pair.Value;
                if (rect.Contains(point))
                {
                    return pair.Key;
                }
            }
            return Parts.Leave;
        }

        #endregion
    }
}
