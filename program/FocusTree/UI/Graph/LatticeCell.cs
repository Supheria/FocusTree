
using static System.Formats.Asn1.AsnWriter;

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

        #region ==== 坐标 ====

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
        /// 默认构造函数：(0, 0)
        /// </summary>
        public LatticeCell()
        {
            LatticedLeft = 0;
            LatticedTop = 0;
        }
        /// <summary>
        /// 使用坐标位置创建，将坐标自动转换为栅格化坐标
        /// </summary>
        /// <param name="cursor"></param>
        public LatticeCell(Point cursor)
        {
            var widthDiff = cursor.X - Lattice.OriginLeft;
            var heightDiff = cursor.Y - Lattice.OriginTop;
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

        #endregion

        /// <summary>
        /// 判断两个格元的左边界和上边界是否同时相等
        /// </summary>
        /// <param name="lhd"></param>
        /// <param name="rhd"></param>
        /// <returns></returns>
        public static bool operator ==(LatticeCell lhd, LatticeCell rhd)
        {
            return lhd.LatticedLeft == rhd.LatticedLeft && lhd.LatticedTop == rhd.LatticedTop;
        }
        /// <summary>
        /// 判断两个格元的左边界或上边界是否不相等
        /// </summary>
        /// <param name="lhd"></param>
        /// <param name="rhd"></param>
        /// <returns></returns>
        public static bool operator !=(LatticeCell lhd, LatticeCell rhd)
        {
            return lhd.LatticedLeft != rhd.LatticedLeft || lhd.LatticedTop != rhd.LatticedTop;
        }

        public override bool Equals(object obj)
        {
            var cell = (LatticeCell)obj;
            return LatticedLeft == cell.LatticedLeft && LatticedTop == cell.LatticedTop;
        }
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
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
        /// <summary>
        /// 格元内部各个区域的真实矩形
        /// </summary>
        /// <param name="lattice"></param>
        /// <returns></returns>
        //public Dictionary<InnerParts, (Rectangle, SolidBrush)> InnerPartRealRects
        //{
        //    get => new()
        //    {
        //        [InnerParts.Left] = (new(RealLeft, NodeRealTop, Width - NodeWidth, Height - NodePaddingHeight), new(InnerPartColor_Left)),
        //        [InnerParts.Top] = (new(NodeRealLeft, RealTop, Width - NodePaddingWidth, Height - NodeHeight), new(InnerPartColor_Top)),
        //        [InnerParts.LeftTop] = (new(RealLeft, RealTop, Width - NodeWidth, Height - NodeHeight), new(InnerPartColor_LeftTop)),
        //        [InnerParts.Node] = (NodeRealRect, new(InnerPartColor_Node))
        //    };
        //}
        public Dictionary<InnerParts, Rectangle> InnerPartRealRects
        {
            get => new()
            {
                [InnerParts.Left] = new(RealLeft, NodeRealTop, Width - NodeWidth, Height - NodePaddingHeight),
                [InnerParts.Top] = new(NodeRealLeft, RealTop, Width - NodePaddingWidth, Height - NodeHeight),
                [InnerParts.LeftTop] = new(RealLeft, RealTop, Width - NodeWidth, Height - NodeHeight),
                [InnerParts.Node] = NodeRealRect
            };
        }
        /// <summary>
        /// 光标进入格元后高亮光标所处其中的部分，或取消高亮光标刚离开的部分
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cell"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public InnerParts HightLightCursorTouchOn(Graphics g, Point cursor)
        {
            //foreach (var pair in InnerPartRealRects)
            //{
            //    var rect = pair.Value.Item1;
            //    if (!rect.Contains(cursor)) { continue; }
            //    if (rect != LastTouchRect)
            //    {
            //        Lattice.ReDrawCell(g, this);
            //        LastTouchRect = rect;
            //        if (Lattice.RectWithinDrawRect(rect, out var saveRect))
            //        {
            //            g.FillRectangle(pair.Value.Item2, saveRect);
            //        }
            //    }
            //    return pair.Key;
            //}
            //Lattice.ReDrawCell(g, this);
            return InnerParts.Leave;
        }
        /// <summary>
        /// 获取光标在格元上所处的部分
        /// </summary>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public InnerParts GetInnerPartCursorOn(Point cursor)
        {
            foreach (var pair in InnerPartRealRects)
            {
                var rect = pair.Value;
                if (rect.Contains(cursor))
                {
                    return pair.Key;
                }
            }
            return InnerParts.Leave;
        }
        /// <summary>
        /// 高亮格元被选中的部分，并在重绘栅格时绘制高亮
        /// </summary>
        /// <param name="cursor">选中坐标</param>
        /// <returns>格元选中的部分</returns>
        public InnerParts HighlightSelection(Point cursor)
        {
            //foreach (var pair in InnerPartRealRects)
            //{
            //    var rect = pair.Value.Item1;
            //    if (!rect.Contains(cursor)) { continue; }
            //    if (rect != LastTouchRect)
            //    {
            //        LastTouchRect = rect;
            //        if (Lattice.RectWithinDrawRect(rect, out var saveRect))
            //        {
            //            Lattice.DrawCell += (g) => { g.FillRectangle(pair.Value.Item2, saveRect); };
            //        }
            //    }
            //    return pair.Key;
            //}
            return InnerParts.Leave;
        }
    }
}
