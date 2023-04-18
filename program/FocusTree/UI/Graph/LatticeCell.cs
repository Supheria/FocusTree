﻿using static FocusTree.UI.Graph.Lattice;

namespace FocusTree.UI.Graph
{
    /// <summary>
    /// 格元
    /// </summary>
    struct LatticeCell
    {
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
        public int RealLeft { get => Width * LatticedLeft + CellOffsetLeft; }
        /// <summary>
        /// 格元真实上边界
        /// </summary>
        public int RealTop { get => Height * LatticedTop + CellOffsetTop; }
        /// <summary>
        /// 格元真实列索引
        /// </summary>
        public int RealColIndex { get => RealLeft / Width; }
        /// <summary>
        /// 格元真实行索引
        /// </summary>
        public int RealRowIndex { get => RealTop / Height; }
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

        #endregion
        public LatticeCell()
        {
            LatticedLeft = 0;
            LatticedTop = 0;
        }
        public LatticeCell(Point realPoint)
        {
            var widthDiff = realPoint.X - OriginLeft;
            var heightDiff = realPoint.Y - OriginTop;
            LatticedLeft = widthDiff / Width;
            LatticedTop = heightDiff / Height;
            if (widthDiff < 0) { LatticedLeft--; }
            if (heightDiff < 0) { LatticedTop--; }
        }
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
        public enum CellParts
        {
            /// <summary>
            /// 已离开格元
            /// </summary>
            Leave,
            /// <summary>
            /// 节点左侧区域
            /// </summary>
            Left,
            /// <summary>
            /// 节点头上区域
            /// </summary>
            Top,
            /// <summary>
            /// 节点右上区域
            /// </summary>
            LeftTop,
            /// <summary>
            /// 节点内
            /// </summary>
            Node
        }
        Rectangle LastTouchInRect = new();
        Rectangle[] SideParts 
        { 
            get => new Rectangle[]
            {
                new(RealLeft, NodeRealTop, Width - NodeWidth, Height - NodePaddingHeight), // left
                new(NodeRealLeft, RealTop, Width - NodePaddingWidth, Height - NodeHeight), // top
                new(RealLeft, RealTop, Width - NodeWidth, Height - NodeHeight) // left top
            };
        }
        /// <summary>
        /// 光标进入格元后高亮光标所处部分，或取消高亮光标刚离开的部分
        /// </summary>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public CellParts HighlightCursor(Graphics g, Point cursor)
        {
            for (int i = 0; i < SideParts.Length; i++)
            {
                var part = SideParts[i];
                if (!part.Contains(cursor)) { continue; }
                if (part != LastTouchInRect)
                {
                    //ReDrawCell(g, this);
                    LastTouchInRect = part;
                    part = RectWithinDrawRect(part);
                    Point ColRow = new(RealColIndex, RealRowIndex);
                    CellDrawer drawer = (g) => 
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Gray)), part);
                        return ColRow;
                    };
                    DrawCellQueue.Add(drawer);
                }
                return i == 0 ? CellParts.Left : i == 1 ? CellParts.Top : i == 2 ? CellParts.LeftTop : CellParts.Node;
            }
            var rect = NodeRealRect;
            if (rect.Contains(cursor))
            {
                if(LastTouchInRect != rect)
                {
                    LastTouchInRect = rect;
                    rect = RectWithinDrawRect(rect);
                    Point ColRow = new(RealColIndex, RealRowIndex);
                    CellDrawer drawer = (g) =>
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(150, Color.Orange)), rect);
                        return ColRow;
                    }; 
                    DrawCellQueue.Add(drawer);
                }
                return CellParts.Node;
            }
            //ReDrawCell(g, this);
            //CancelCellFromDrawQueue(this);
            return CellParts.Leave;
        }
    }
}
