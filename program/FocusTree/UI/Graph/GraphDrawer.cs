using FocusTree.Data.Focus;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        /// 展示信息字体
        /// </summary>
        public static string InfoFont { get; private set; } = "仿宋";
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
        public static SolidBrush NodeBG_Normal { get; private set; } = new(Color.FromArgb(100, Color.Cyan));
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
        public static int NodeShadowLength = 2;
        public static int PenWidth = 4;
        /// <summary>
        /// 背景图片
        /// </summary>
        public static readonly Bitmap BackImage;
        private static Bitmap BkInverseColor;
        /// <summary>
        /// 背景图片在当前 Client Rect 范围中的缓存
        /// </summary>
        static Bitmap BackImageCacher;
        static Bitmap BkInverseColorCacher;
        /// <summary>
        /// 是否显示背景图片
        /// </summary>
        public static readonly bool ShowBackGroung = false;
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
        static GraphDrawer()
        {
            if (!File.Exists("Background.jpg")) { return; }
            BackImage = (Bitmap)Image.FromFile("Background.jpg");
            ShowBackGroung = true;

            if (File.Exists("Background_Inverse.jpg"))
            {
                BkInverseColor = (Bitmap)Image.FromFile("Background_Inverse.jpg");
                return;
            }
            var width = BackImage.Width;
            var height = BackImage.Height;
            BkInverseColor = new Bitmap(width, height);//初始化一个记录处理后的图片的对象
            int x, y, resultR, resultG, resultB;
            Color pixel;

            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = BackImage.GetPixel(x, y);//获取当前坐标的像素值
                    resultR = 255 - pixel.R;//反红
                    resultG = 255 - pixel.G;//反绿
                    resultB = 255 - pixel.B;//反蓝
                    BkInverseColor.SetPixel(x, y, Color.FromArgb(resultR, resultG, resultB));//绘图
                }
            }
            BkInverseColor.Save("Background_Inverse.jpg");
        }

        /// <summary>
        /// 节点绘制委托列表
        /// </summary>
        public static Dictionary<int, CellDrawer> NodeDrawerCatalog { get; private set; } = new();
        static Dictionary<(int, int), CellDrawer> LineDrawerCatalog = new();
        /// <summary>
        /// 根据当前 Client Rect 大小设置背景图片缓存
        /// </summary>
        public static void SetBackImageCacher(Size size)
        {
            var Width = size.Width;
            var Height = size.Height;
            var bkWidth = Width;
            var bkHeight = Height;
            float sourceRatio = (float)GraphDrawer.BackImage.Width / (float)GraphDrawer.BackImage.Height;
            float clientRatio = (float)Width / (float)Height;
            if (sourceRatio < clientRatio)
            {
                bkWidth = Width;
                bkHeight = (int)(Width / sourceRatio);
            }
            else if (sourceRatio > clientRatio)
            {
                bkHeight = Height;
                bkWidth = (int)(Height * sourceRatio);
            }
            if (BackImageCacher != null && bkWidth == BackImageCacher.Width && bkHeight == BackImageCacher.Height) { return; }
            BackImageCacher?.Dispose();

            BackImageCacher = new Bitmap(bkWidth, bkHeight);
            var g = Graphics.FromImage(BackImageCacher);
            g.DrawImage(BackImage, 0, 0, bkWidth, bkHeight);
            g.Flush();

            BkInverseColorCacher = new Bitmap(bkWidth, bkHeight);
            g = Graphics.FromImage(BkInverseColorCacher);
            g.DrawImage(BkInverseColor, 0, 0, bkWidth, bkHeight);
            g.Flush(); g.Dispose();
        }
        public static Image GetBackImageCacher(Size size)
        {
            if (BackImageCacher == null) { SetBackImageCacher(size); }
            return BackImageCacher;
        }
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
            if (ShowBackGroung)
            {
                Lattice.Drawing += NodeDrawerCatalog[id] = (g) => DrawNode(g, focus, BackImageCacher);
            }
            else { Lattice.Drawing += NodeDrawerCatalog[id] = (g) => DrawNode(g, focus, NodeBG_Normal); }
            

        }
        /// <summary>
        /// 将节点关系线绘制到栅格绘图委托
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="startLoc"></param>
        /// <param name="endLoc"></param>
        public static void UploadRequireLine(int penIndex, FocusData start, FocusData end)
        {
            (int, int) ID = (start.ID, end.ID);
            if (LineDrawerCatalog.TryGetValue(ID, out var drawer))
            {
                Lattice.Drawing -= drawer;
            }
            Lattice.Drawing += LineDrawerCatalog[ID] = (g) => DrawLines(g, NodeRequire[penIndex], start.LatticedPoint, end.LatticedPoint);

        }
        public static void DrawLines(Graphics g, Pen pen, Point startLoc, Point endLoc)
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
            DrawLine(g, x, (y1, y2), pen);
            //
            // 横线
            //
            if (Math.Abs(widthDiff) > 0)
            {
                cell.LatticedLeft += widthDiff;
                var x2 = cell.NodeRealLeft + nodeWidth / 2;
                DrawLine(g, (x, x2), y2, pen);
            }
            //
            // 竖线2
            //
            y1 = y2;
            cell.LatticedTop -= heightDiff - halfHeight - 1;
            y2 = cell.RealTop;
            x = cell.NodeRealLeft + nodeWidth / 2;
            DrawLine(g, x, (y1, y2), pen);
        }
        private static void DrawLine(Graphics g, int x, (int, int) y, Pen pen)
        {
            Rectangle lineRect = new(x - PenWidth / 2, Math.Min(y.Item1, y.Item2), PenWidth, Math.Abs(y.Item1 - y.Item2));
            if (!Lattice.RectWithin(lineRect, out var saveRect)) { return; }
            var rect = saveRect;
            if (ShowBackGroung)
            {
                g.DrawImage(BkInverseColorCacher, rect, rect, GraphicsUnit.Pixel);
                rect = new(rect.X + 1, rect.Y, rect.Width - 2, rect.Height);
                g.DrawImage(BackImageCacher, rect, rect, GraphicsUnit.Pixel);
            }
            else
            {
                if (Lattice.LineWithin(x, y, pen.Width, out var saveLine))
                {
                    var line = saveLine;
                    g.DrawLine(pen, line.Item1, line.Item2);
                }
            }
            g.Flush();
        }
        private static void DrawLine(Graphics g, (int, int) x, int y, Pen pen)
        {
            Rectangle lineRect = new(Math.Min(x.Item1, x.Item2), y - PenWidth / 2, Math.Abs(x.Item1 - x.Item2), PenWidth);
            if (!Lattice.RectWithin(lineRect, out var saveRect)) { return; }
            var rect = saveRect;
            if (ShowBackGroung)
            {
                g.DrawImage(BkInverseColorCacher, rect, rect, GraphicsUnit.Pixel);
                rect = new(rect.X, rect.Y + 1, rect.Width, rect.Height - 2);
                g.DrawImage(BackImageCacher, rect, rect, GraphicsUnit.Pixel);
            }
            else
            {
                if (Lattice.LineWithin(x, y, pen.Width, out var saveLine))
                {
                    var line = saveLine;
                    g.DrawLine(pen, line.Item1, line.Item2);
                }
            }
            g.Flush();
        }
        public static void DrawNode(Graphics g, FocusData focus, Image imageFiller)
        {
            LatticeCell cell = new(focus);
            var rect = cell.InnerPartRealRects[LatticeCell.Parts.Node];
            Rectangle shadding = new(rect.Left - NodeShadowLength, rect.Top - NodeShadowLength, rect.Width + NodeShadowLength * 2, rect.Height + NodeShadowLength * 2);
            if (!Lattice.RectWithin(shadding, out var saveRect)) { return; }
            rect = saveRect;
            g.DrawImage(BkInverseColorCacher, rect, rect, GraphicsUnit.Pixel);
            rect = new(rect.Left + NodeShadowLength, rect.Top + NodeShadowLength, rect.Width - NodeShadowLength * 2, rect.Height - NodeShadowLength * 2);

            var testRect = cell.RealRect;
            if (testRect.Width < LatticeCell.SizeMax.Width / 2 || testRect.Height < LatticeCell.SizeMax.Height / 2)
            {
                g.DrawImage(imageFiller, rect, rect, GraphicsUnit.Pixel);
                return; 
            }

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
            var strBmp = DrawString(sName, font, rect);
            g.DrawImage(strBmp, rect, new Rectangle(0, 0, rect.Width, rect.Height), GraphicsUnit.Pixel);
            g.Flush();
        }

        private static Bitmap DrawString(string str, Font font, Rectangle rect)
        {
            Bitmap strBmp = new Bitmap(rect.Width, rect.Height);
            var g = Graphics.FromImage(strBmp);
            g.Clear(Color.White);
            g.DrawString(str, font, new SolidBrush(Color.Black), new Rectangle(0, 0, rect.Width, rect.Height), NodeFontFormat);
            g.Flush();
            g.Dispose();

            int black = 0;
            int white = 0;
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j <  rect.Height; j++)
                {
                    var pixel = strBmp.GetPixel(i, j);//获取当前坐标的像素值
                    var bkPixel = BackImageCacher.GetPixel(rect.Left + i, rect.Top + j);
                    if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
                    {

                        //strBmp.SetPixel(i, j, bkPixel);
                    }
                    //{
                    //    //var RGB = BackImageCacher.GetPixel(rect.Left + i, rect.Top + j);
                    //    //strBmp.SetPixel(i, j, Color.FromArgb(RGB.R, RGB.G, RGB.B));
                    //}
                    else
                    {
                        if (bkPixel.R < 123 && bkPixel.G < 123 && bkPixel.B < 123)
                        {
                            black++;
                            //strBmp.SetPixel(i, j, Color.White);
                        }
                        else
                        {
                            white++;
                            //strBmp.SetPixel(i, j, Color.Black);
                        }
                        //var RGB = BkInverseColorCacher.GetPixel(rect.Left + i, rect.Top + j);
                        //strBmp.SetPixel(i, j, Color.FromArgb(RGB.R, RGB.G, RGB.B));
                    }
                }
            }
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j < rect.Height; j++)
                {
                    var pixel = strBmp.GetPixel(i, j);//获取当前坐标的像素值
                    var bkPixel = BackImageCacher.GetPixel(rect.Left + i, rect.Top + j);
                    if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
                    {
                        strBmp.SetPixel(i, j, bkPixel);
                    }
                    else
                    {
                        if (black < white)
                        {
                            strBmp.SetPixel(i, j, Color.Black);
                        }
                        else
                        {
                            strBmp.SetPixel(i, j, Color.White);
                        }
                    }
                }
            }
            return strBmp;
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
            Rectangle shadowRect = new(rect.Left + NodeShadowLength, rect.Top + NodeShadowLength, rect.Width, rect.Height);
            g.FillRectangle(new SolidBrush(Color.White), shadowRect);
            g.FillRectangle(brush, rect);
            var testRect = cell.RealRect;
            if (testRect.Width < LatticeCell.SizeMax.Width / 2 || testRect.Height < LatticeCell.SizeMax.Height / 2) { return; }

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
            var font = new Font(NodeFont, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(sName, font, NodeFG, rect, NodeFontFormat);
            g.Flush();
        }
    }
}
