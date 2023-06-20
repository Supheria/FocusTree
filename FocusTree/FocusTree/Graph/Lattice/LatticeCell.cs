namespace FocusTree.Graph
{
    /// <summary>
    /// 格元
    /// </summary>
    public struct LatticeCell
    {
        #region ==== 设置宽高和间距 ====

        /// <summary>
        /// 格元边长（限制最小值和最大值）
        /// </summary>
        public static int Length
        {
            get => sideLength;
            set => sideLength = value < LengthMin ? LengthMin : value > LengthMax ? LengthMax : value;
        }
        static int sideLength = 30;
        /// <summary>
        /// 最小尺寸
        /// </summary>
        public static int LengthMin { get; set; } = 25;
        /// <summary>
        /// 最大尺寸
        /// </summary>
        public static int LengthMax { get; set; } = 125;
        /// <summary>
        /// 节点宽
        /// </summary>
        public static int NodeWidth => Length - NodePaddingWidth;
        /// <summary>
        /// 节点高
        /// </summary>
        public static int NodeHeight => Length - NodePaddingHeight;
        /// <summary>
        /// 节点 Left 到格元 Left 的空隙
        /// </summary>
        public static int NodePaddingWidth => (int)(Length * NodePaddingZoomFactor.X);
        /// <summary>
        /// 节点 Top 到格元 Top 的空隙
        /// </summary>
        public static int NodePaddingHeight => (int)(Length * NodePaddingZoomFactor.Y);
        /// <summary>
        /// 节点空隙系数（0.3 < X < 0.7, 0.3 < Y < 0.7)
        /// </summary>
        public static PointF NodePaddingZoomFactor
        {
            get => nodePaddingZoomFactor;
            set => nodePaddingZoomFactor = new(value.X < 0.3f ? 0.3f : value.X > 0.7f ? 0.7f : value.X, value.Y < 0.3f ? 0.3f : value.Y > 0.7f ? 0.7f : value.Y);
        }
        static PointF nodePaddingZoomFactor = new(0.3f, 0.485f);

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
        /// <summary>
        /// 格元栅格化坐标
        /// </summary>
        public LatticedPoint LatticedPoint => new(LatticedLeft, LatticedTop);
        /// <summary>
        /// 格元真实左边界
        /// </summary>
        public int RealLeft => Length * LatticedLeft + Lattice.OriginLeft;
        /// <summary>
        /// 格元真实上边界
        /// </summary>
        public int RealTop => Length * LatticedTop + Lattice.OriginTop;
        /// <summary>
        /// 节点真实左边界
        /// </summary>
        public int NodeRealLeft => RealLeft + NodePaddingWidth;
        /// <summary>
        /// 节点真实上边界
        /// </summary>
        public int NodeRealTop => RealTop + NodePaddingHeight;
        /// <summary>
        /// 节点真实坐标矩形
        /// </summary>
        public Rectangle NodeRealRect => new(NodeRealLeft, NodeRealTop, NodeWidth, NodeHeight);

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
        /// 使用已有的栅格化坐标创建
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        public LatticeCell(LatticedPoint point)
        {
            LatticedLeft = point.Col;
            LatticedTop = point.Row;
        }

        #endregion

        #region ==== 格元区域 ====

        /// <summary>
        /// 格元的部分
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
        /// 格元各个部分的真实坐标矩形
        /// </summary>
        public Dictionary<Parts, Rectangle> CellPartsRealRect
        {
            get => new()
            {
                [Parts.Leave] = Rectangle.Empty,
                [Parts.Left] = new(RealLeft, NodeRealTop, Length - NodeWidth, Length - NodePaddingHeight),
                [Parts.Top] = new(NodeRealLeft, RealTop, Length - NodePaddingWidth, Length - NodeHeight),
                [Parts.LeftTop] = new(RealLeft, RealTop, Length - NodeWidth, Length - NodeHeight),
                [Parts.Node] = NodeRealRect
            };
        }
        /// <summary>
        /// 获取坐标在格元上所处的部分
        /// </summary>
        /// <param name="point">坐标</param>
        /// <returns></returns>
        public Parts GetPartPointOn(Point point)
        {
            foreach (var pair in CellPartsRealRect)
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