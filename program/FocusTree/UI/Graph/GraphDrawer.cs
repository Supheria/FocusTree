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
        public static string NodeFont { get; private set; } = "仿宋";
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
        public static Dictionary<Point, CellDrawer> NodeDrawerCatalog { get; private set; } = new();
        static Dictionary<Point, CellDrawer> LineDrawerCatalog = new();
        /// <summary>
        /// 将节点绘制上载到栅格绘图委托（要更新栅格放置区域，应该先更新再调用此方法，因为使用了裁剪超出绘图区域的绘图方法）
        /// </summary>
        public static void UploadNodeMap(FocusData focus, SolidBrush brush)
        {
            var point = focus.LatticedPoint;
            CellDrawer drawer = (g) => DrawNode(g, focus, brush);
            if (NodeDrawerCatalog.TryAdd(point, drawer))
            {
                NodeDrawerCatalog[point] += drawer;
            }
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
            Lattice.Drawing += (g) => DrawLines(g, pen, start.LatticedPoint, end.LatticedPoint);

        }
        private static void AddLine(int x, (int, int) y, Pen pen, Point point)
        {
            if (Lattice.LineWithin(x, y, pen.Width, out var saveLine))
            {
                var line = saveLine;
                CellDrawer drawer = (g) => g.DrawLine(pen, line.Item1, line.Item2);
                if (!NodeDrawerCatalog.TryAdd(point, drawer))
                {
                    NodeDrawerCatalog[point] = drawer;
                }
                Lattice.Drawing += drawer;
            }
        }
        private static void AddLine((int, int) x, int y, Pen pen, Point point)
        {
            if (Lattice.LineWithin(x, y, pen.Width, out var saveLine))
            {
                var line = saveLine;
                CellDrawer drawer = (g) => g.DrawLine(pen, line.Item1, line.Item2);
                if (!NodeDrawerCatalog.TryAdd(point, drawer))
                {
                    NodeDrawerCatalog[point] = drawer;
                }
                Lattice.Drawing += drawer;
            }
        }
        /// <summary>
        /// 将节点关系线绘制到栅格绘图委托
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen"></param>
        /// <param name="startLoc"></param>
        /// <param name="endLoc"></param>
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
            var y2 = cell.RealTop + paddingHeight / 2;
            var x1 = cell.NodeRealLeft + nodeWidth / 2;
            AddLine(x1, (y1, y2), pen, cell.LatticedPoint);
            int halfHeight = heightDiff / 2;
            for (int i = 0; i < halfHeight - 1; i++)
            {
                cell.LatticedTop--;
                y1 = cell.RealRect.Bottom;
                y2 = cell.RealTop;
                AddLine(x1, (y1, y2), pen, cell.LatticedPoint);
            }
            if (halfHeight > 1)
            {
                cell.LatticedTop--;
                y1 = cell.RealRect.Bottom;
                y2 = cell.RealTop + paddingHeight / 2;
                AddLine(x1, (y1, y2), pen, cell.LatticedPoint);
            }
            //
            // 横线
            //
            if (Math.Abs(widthDiff) > 0)
            {
                var x2 = cell.NodeRealLeft + nodeWidth / 2;
                AddLine((x1, x2), y2, pen, cell.LatticedPoint);
                
                cell.LatticedLeft += widthDiff;
                for (int i = 0; i < Math.Abs(widthDiff) - 1; i++)
                {
                    cell.LatticedLeft += widthDiff < 0 ? -1 : 1;
                    x1 = cell.RealLeft;
                    x2 = cell.RealRect.Right;
                    AddLine((x1, x2), y2, pen, cell.LatticedPoint);
                }
                cell.LatticedLeft += widthDiff < 0 ? -1 : 1;
                x1 = cell.NodeRealLeft + nodeWidth / 2;
                x2 = cell.RealRect.Right;
                AddLine((x1, x2), y2, pen, cell.LatticedPoint);
            }
            //
            // 竖线2
            //
            y1 = y2;
            y2 = cell.RealTop;
            AddLine(x1, (y1, y2), pen, cell.LatticedPoint);
            for (int i = 0; i < heightDiff - halfHeight - 1; i++)
            {
                cell.LatticedTop--;
                y1 = cell.RealRect.Bottom;
                y2 = cell.RealTop;
                AddLine(x1, (y1, y2), pen, cell.LatticedPoint);
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
            if (!Lattice.RectWithin(rect, out var saveRect)) { return; }
            rect = saveRect;
            g.FillRectangle(new SolidBrush(Color.White), rect);
            g.FillRectangle(brush, rect);
            var testRect = cell.RealRect;
            if (testRect.Width < LatticeCell.SizeMax.Width / 2 || testRect.Height < LatticeCell.SizeMax.Height / 2) { return; }
            var name = focus.Name;
            var fontHeight = name.Length / 4 + 1;
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
            var font = new Font(NodeFont, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(sName, font, NodeFG, rect, NodeFontFormat);
            //g.DrawString(focus.Name, font, NodeFG, rect, NodeFontFormat);
            g.Flush();
        }
    }
}
