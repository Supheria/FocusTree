using FocusTree.Data.Focus;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace FocusTree.UI.Graph
{
    /// <summary>
    /// 国策树绘图工具
    /// </summary>
    public static class GraphDrawer
    {
        /// <summary>
        /// 节点字体
        /// </summary>
        public static string NodeFont { get; private set; } = "黑体";
        /// <summary>
        /// 节点字体样式
        /// </summary>
        public static StringFormat NodeFontFormat { get; private set; } = new()
        { 
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        /// <summary>
        /// 节点文字颜色
        /// </summary>
        public static SolidBrush NodeFG { get; private set; } = new(Color.FromArgb(175, Color.DarkBlue));
        /// <summary>
        /// 默认节点背景颜色
        /// </summary>
        public static SolidBrush NodeBG_Normal { get; private set; } = new(Color.FromArgb(80, Color.Aqua));
        /// <summary>
        /// 冲突节点的背景颜色
        /// </summary>
        public static SolidBrush NodeBG_Conflicted { get; private set; } = new(Color.FromArgb(80, Color.Red));
        /// <summary>
        /// 先前选中节点背景颜色
        /// </summary>
        public static SolidBrush NodeBG_Selected { get; private set; } = new(Color.FromArgb(80, Color.DarkOrange));
        /// <summary>
        /// 选中节点背景颜色
        /// </summary>
        public static SolidBrush NodeBG_Selecting { get; private set; } = new(Color.FromArgb(80, Color.BlueViolet));
        /// <summary>
        /// 节点连接线条（每个依赖组使用单独的颜色）
        /// </summary>
        public static Pen[] NodeRequire { get; private set; } = new Pen[]{
            new Pen(Color.FromArgb(100, Color.Cyan), 2),
            new Pen(Color.FromArgb(100, Color.Yellow), 2),
            new Pen(Color.FromArgb(100, Color.Green), 2),
            new Pen(Color.FromArgb(100, Color.Orange), 2),
            new Pen(Color.FromArgb(100, Color.Purple), 2)
        };

        /// <summary>
        /// 节点绘制委托列表
        /// </summary>
        public static Dictionary<int, CellDrawer> NodeDrawerCatalog { get; private set; } = new();
        static Dictionary<(int, int), CellDrawer> LineDrawerCatalog = new();
        /// <summary>
        /// 将节点绘制上载到栅格绘图委托（要更新栅格放置区域，应该先更新再调用此方法，因为使用了裁剪超出绘图区域的绘图方法）
        /// </summary>
        public static void UploadNodeMap(FocusData focus)
        {
            var id = focus.ID; 
            LatticeCell cell = new(focus);
            if (NodeDrawerCatalog.TryGetValue(id, out var drawer))
            {
                Lattice.Drawing -= drawer;
            }
            Lattice.Drawing += NodeDrawerCatalog[id] = (g) => DrawNode(g, focus, NodeBG_Normal);

        }
        /// <summary>
        /// 将节点关系线绘制到栅格绘图委托
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="startLoc"></param>
        /// <param name="endLoc"></param>
        public static void UploadRequireLine(Pen pen, FocusData start, FocusData end)
        {
            (int, int) ID = (start.ID, end.ID);
            if (LineDrawerCatalog.TryGetValue(ID, out var drawer))
            {
                Lattice.Drawing -= drawer;
            }
            Lattice.Drawing += LineDrawerCatalog[ID] = (g) => DrawLines(g, pen, start.LatticedPoint, end.LatticedPoint);

        }
        public static void DrawLines(Graphics g, Pen pen , Point startLoc, Point endLoc)
        {
            var widthDiff = endLoc.X - startLoc.X;
            var heightDiff = startLoc.Y - endLoc.Y;
            LatticeCell cell = new(startLoc.X, startLoc.Y);
            var paddingHeight = LatticeCell.NodePaddingHeight;
            var nodeWidth = LatticeCell.NodeWidth;
            //
            // 竖线1
            //
            var y1 = cell.RealTop + paddingHeight;
            int halfHeight = heightDiff / 2;
            cell.LatticedTop -= halfHeight;
            var y2 = cell.RealTop + paddingHeight / 2;
            var x = cell.NodeRealLeft + nodeWidth / 2;
            if (Lattice.LineWithin(x, (y1, y2), pen.Width, out var saveLine))
            {
                var line = saveLine;
                g.DrawLine(pen, line.Item1, line.Item2);
            }
            //
            // 横线
            //
            if (Math.Abs(widthDiff) > 0)
            {
                cell.LatticedLeft += widthDiff;
                var x2 = cell.NodeRealLeft + nodeWidth / 2;
                if (Lattice.LineWithin((x, x2), y2, pen.Width, out saveLine))
                {
                    var line = saveLine;
                    g.DrawLine(pen, line.Item1, line.Item2);
                }
            }
            //
            // 竖线2
            //
            y1 = y2;
            cell.LatticedTop -= heightDiff - halfHeight - 1;
            y2 = cell.RealTop;
            x = cell.NodeRealLeft + nodeWidth / 2;
            if (Lattice.LineWithin(x, (y1, y2), pen.Width, out saveLine))
            {
                var line = saveLine;
                g.DrawLine(pen, line.Item1, line.Item2);
            }
            g.Flush();
        }
        /// <summary>
        /// 绘制节点
        /// </summary>
        /// <param name="g"></param>
        /// <param name="id"></param>
        /// <param name="brush"></param>
        public static void DrawNode(Graphics g, FocusData focus, SolidBrush brush)
        {
            LatticeCell cell = new(focus);
            var rect = cell.InnerPartRealRects[LatticeCell.Parts.Node];
            var testRect = cell.RealRect;
            if (!Lattice.RectWithin(rect, out var saveRect)) { return; }
            rect = saveRect;
            g.FillRectangle(new SolidBrush(Color.White), rect);
            g.FillRectangle(brush, rect);
            //g.Flush();
            var name = focus.Name;
            var fontHeight = name.Length / 3;
            if (fontHeight == 1 && name.Length % 3 != 0) { fontHeight++; }
            else if (fontHeight == 0) { fontHeight++; }
            var fontWidth = name.Length / fontHeight;
            var fontSizeH = 0.7f * rect.Height / fontHeight;
            var fontSizeW = 0.7f * rect.Width / fontWidth;
            var fontSize = Math.Min(fontSizeH, fontSizeW);
            if (fontSize <= 0) { return; }
            string sName = name;
            if (fontHeight > 1)
            {
                sName = string.Empty;
                for (int i = 0; i < fontHeight; i++)
                {
                    sName += $"{name.Substring(i * fontWidth, fontWidth)}\n";
                }
                sName = sName[..^1];
            }
            var font = new Font(NodeFont, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(sName, font, NodeFG, rect, NodeFontFormat);
            g.Flush();
        }
    }
}
