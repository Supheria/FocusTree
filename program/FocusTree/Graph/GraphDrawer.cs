#define PointBmp
using FocusTree.Data.Focus;
using FocusTree.UI.test;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using FocusTree.Properties;

namespace FocusTree.Graph
{
    /// <summary>
    /// 国策树绘图工具
    /// </summary>
    public static class GraphDrawer
    {
        #region ==== 字体 ====

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

        #endregion

        #region ==== 节点 ====

        /// <summary>
        /// 节点边框宽度
        /// </summary>
        public static int NodeBorderWidth = 2;
        /// <summary>
        /// 节点连接线宽度
        /// </summary>
        public static int NodeLineWidth = 2;
        /// <summary>
        /// 节点文字颜色 - 深色
        /// </summary>
        public static Color NodeFGDark { get; private set; } = Color.FromArgb(175, Color.Black);
        /// <summary>
        /// 节点文字颜色 - 浅色
        /// </summary>
        public static Color NodeFGBright { get; private set; } = Color.FromArgb(175, Color.AliceBlue);

        #endregion

        #region ==== 背景 ====

        /// <summary>
        /// 无图片背景
        /// </summary>
        public static Color BlankBackground = Color.WhiteSmoke;
        /// <summary>
        /// 背景图片文件路径
        /// </summary>
        public static string BackImagePath = "Background.jpg";
        /// <summary>
        /// 背景图片在给定尺寸下的缓存
        /// </summary>
        static Bitmap BackImageCache;
        public static Size BackImageSize 
        { 
            get
            {
                if (File.Exists(BackImagePath))
                {
                    return Image.FromFile(BackImagePath).Size;
                }
                else
                {
                    return Resources.background.Size;
                }
            }
        }
        /// <summary>
        /// 是否显示背景图片
        /// </summary>
        public static bool ShowBackImage = true;

        #endregion

        #region ==== 节点绘制委托列表 ====

        /// <summary>
        /// 节点绘制委托列表
        /// </summary>
        public static Dictionary<int, CellDrawer> NodeDrawerCatalog { get; private set; } = new();

        #endregion

        #region ==== 加载背景 ====

        /// <summary>
        /// 新键背景缓存，并重绘背景
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rect"></param>
        public static void DrawNewBackground(Image image)
        {
            SetBackImageCacher(image.Size);
            RedrawBackground(image);
        }
        /// <summary>
        /// 重绘背景（首次重绘应该使用 DrawNewBackground）
        /// </summary>
        /// <param name="image"></param>
        public static void RedrawBackground(Image image)
        {
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(BackImageCache, 0, 0);
            g.Flush(); g.Dispose();
        }
        /// <summary>
        /// 根据给定尺寸设置背景图片缓存
        /// </summary>
        private static void SetBackImageCacher(Size size)
        {
            var Width = size.Width;
            var Height = size.Height;
            if (!ShowBackImage) 
            {
                BackImageCache?.Dispose();
                BackImageCache = new Bitmap(Width, Height);
                PointBitmap pCache = new(BackImageCache);
                pCache.LockBits();
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        pCache.SetPixel(i, j, BlankBackground);
                    }
                }
                pCache.UnlockBits();
                return;
            }
            Image sourceImage;
            if (File.Exists(BackImagePath))
            {
                sourceImage = Image.FromFile(BackImagePath);
            }
            else
            {
                sourceImage = Resources.background;
            }
            var bkWidth = Width;
            var bkHeight = Height;
            float sourceRatio = (float)sourceImage.Width / (float)sourceImage.Height;
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
            Bitmap newBackImage = new(bkWidth, bkHeight);
            var g = Graphics.FromImage(newBackImage);
            g.DrawImage(sourceImage, 0, 0, bkWidth, bkHeight);
            g.Flush(); g.Dispose();
            sourceImage.Dispose();

            BackImageCache?.Dispose();
            BackImageCache = new(Width, Height);
            g = Graphics.FromImage(BackImageCache);
            Rectangle cutRect = new(0, 0, Width, Height);
            g.DrawImage(newBackImage, cutRect, cutRect, GraphicsUnit.Pixel); 
            g.Flush(); g.Dispose();
            newBackImage.Dispose();
        }

        #endregion

        #region ==== 上载绘制委托 ====

        /// <summary>
        /// 将节点绘制上载到栅格绘图委托
        /// </summary>
        public static void UploadDrawerNode(FocusData focus)
        {
            LatticeCell cell = new(focus.LatticedPoint);
            Lattice.Drawing += NodeDrawerCatalog[focus.ID] = (image) => DrawFocusNode(image, focus);
        }
        /// <summary>
        /// 将节点关系线绘制到栅格绘图委托
        /// </summary>
        /// <param name="penIndex">笔颜色</param>
        /// <param name="start">起始国策</param>
        /// <param name="end">结束国策</param>
        public static void UploadDrawerRequireLine(int penIndex, LatticedPoint start, LatticedPoint end)
        {
            Lattice.Drawing += (image) => DrawRequireLine(image, start, end);
        }

        #endregion

        #region ==== 绘制国策节点 ====

        /// <summary>
        /// 绘制无填充色的国策节点（上载绘制委托默认）
        /// </summary>
        /// <param name="image"></param>
        /// <param name="focus"></param>
        private static void DrawFocusNode(Bitmap image, FocusData focus)
        {
            LatticeCell cell = new(focus.LatticedPoint);
            var nodeRect = cell.InnerPartRealRects[LatticeCell.Parts.Node];
            if (!Lattice.RectWithin(nodeRect, out nodeRect)) { return; }
            var cellRect = cell.RealRect;
            if (cellRect.Width < LatticeCell.SizeMax.Width / 2 || cellRect.Height < LatticeCell.SizeMax.Height / 2)
            {
                DrawBlankNode(image, nodeRect);
            }
            else { DrawStringNode(image, nodeRect, focus.Name); }
        }
        /// <summary>
        /// 绘制有颜色填充国策节点
        /// </summary>
        /// <param name="image"></param>
        /// <param name="focus"></param>
        /// <param name="fillColor"></param>
        public static void DrawFocusNode(Image image, FocusData focus, Color fillColor)
        {
            DrawFocusNode((Bitmap)image, focus, fillColor);
        }
        /// <summary>
        /// 绘制有颜色填充国策节点
        /// </summary>
        /// <param name="image"></param>
        /// <param name="focus"></param>
        /// <param name="fillColor"></param>
        private static void DrawFocusNode(Bitmap image, FocusData focus, Color fillColor)
        {
            LatticeCell cell = new(focus.LatticedPoint);
            var nodeRect = cell.InnerPartRealRects[LatticeCell.Parts.Node];
            if (!Lattice.RectWithin(nodeRect, out nodeRect)) { return; }
            var cellRect = cell.RealRect;
            if (cellRect.Width < LatticeCell.SizeMax.Width / 2 || cellRect.Height < LatticeCell.SizeMax.Height / 2)
            {
                DrawBlankNode(image, nodeRect, fillColor);
            }
            else { DrawStringNode(image, nodeRect, focus.Name, fillColor); }
        }

        #endregion

        #region ==== 绘制非国策节点 ====

        public static void DrawNode(Bitmap image, LatticedPoint point, LatticeCell.Parts cellPart, Color fillColor)
        {
            LatticeCell cell = new(point);
            if (!Lattice.RectWithin(cell.InnerPartRealRects[cellPart], out var rect)) { return; }
            var g = Graphics.FromImage(image);
            g.FillRectangle(new SolidBrush(fillColor), rect);
            g.Flush(); g.Dispose();
        }

        #endregion

        #region ==== 国策节点绘制方法 ====

        /// <summary>
        /// 绘制无文字节点
        /// </summary>
        /// <param name="image"></param>
        /// <param name="nodeRect"></param>
        /// <param name="g"></param>
        private static void DrawBlankNode(Bitmap image, Rectangle nodeRect)
        {
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pCache = new(BackImageCache);
            pCache.LockBits();
            // left
            for (int i = 0; i < NodeBorderWidth; i++)
            {
                for (int j = 0; j < nodeRect.Height; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var pixel = pCache.GetPixel(x, y);
                    pImage.SetPixel(x, y, GetInverseColor(pixel));
                }
            }
            // right
            for (int i = nodeRect.Width - NodeBorderWidth; i < nodeRect.Width; i++)
            {
                if (i < 0) { break; }
                for (int j = 0; j < nodeRect.Height; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var pixel = pCache.GetPixel(x, y);
                    pImage.SetPixel(x, y, GetInverseColor(pixel));
                }
            }
            // top
            for (int i = NodeBorderWidth; i < nodeRect.Width - NodeBorderWidth; i++)
            {
                for (int j = 0; j < NodeBorderWidth; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var pixel = pCache.GetPixel(x, y);
                    pImage.SetPixel(x, y, GetInverseColor(pixel));
                }
            }
            // bottom
            for (int i = NodeBorderWidth; i < nodeRect.Width - NodeBorderWidth; i++)
            {
                for (int j = nodeRect.Height - NodeBorderWidth; j < nodeRect.Height; j++)
                {
                    //if (j <= 0) { break; }
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var pixel = pCache.GetPixel(x, y);
                    pImage.SetPixel(x, y, GetInverseColor(pixel));
                }
            }
            pCache.UnlockBits();
            pImage.UnlockBits();
        }
        private static void DrawBlankNode(Bitmap image, Rectangle nodeRect, Color fillColor)
        {
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pCache = new(BackImageCache);
            pCache.LockBits();
            for (int i = 0; i < nodeRect.Width; i++)
            {
                for (int j = 0; j < nodeRect.Height; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var bkPixel = pCache.GetPixel(x, y);
                    if (i <= NodeBorderWidth || i >= nodeRect.Width - NodeBorderWidth || j <= NodeBorderWidth || j >= nodeRect.Height - NodeBorderWidth)
                    {
                        pImage.SetPixel(x, y, GetInverseColor(bkPixel));
                    }
                    else { pImage.SetPixel(x, y, fillColor); }
                }
            }
            pCache.UnlockBits();
            pImage.UnlockBits();
        }
        /// <summary>
        /// 根据字黑底白绘制文字部分和底纹部分
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rect"></param>
        /// <param name="white"></param>
        /// <param name="black"></param>
        private static void DrawStringNode(Bitmap image, Rectangle nodeRect, string name)
        {
            var whiteMore = GetNodeNamePattern(image, nodeRect, name);
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pCache = new(BackImageCache);
            pCache.LockBits();
            if (whiteMore)
            {
                for (int i = 0; i < nodeRect.Width; i++)
                {
                    for (int j = 0; j < nodeRect.Height; j++)
                    {
                        var x = nodeRect.Left + i;
                        var y = nodeRect.Top + j;
                        var bkPixel = pCache.GetPixel(x, y);
                        var pixel = pImage.GetPixel(x, y);
                        if (pixel.R != 255/* || pixel.G != 255 || pixel.B != 255*/)
                        {
                            pImage.SetPixel(x, y, NodeFGDark);
                            continue;
                        }
                        if (i <= NodeBorderWidth || i >= nodeRect.Width - NodeBorderWidth || j <= NodeBorderWidth || j >= nodeRect.Height - NodeBorderWidth)
                        {
                            pImage.SetPixel(x, y, GetInverseColor(bkPixel));
                        }
                        else { pImage.SetPixel(x, y, bkPixel); }
                    }
                }
            }
            else
            {
                for (int i = 0; i < nodeRect.Width; i++)
                {
                    for (int j = 0; j < nodeRect.Height; j++)
                    {
                        var x = nodeRect.Left + i;
                        var y = nodeRect.Top + j;
                        var bkPixel = pCache.GetPixel(x, y);
                        var pixel = pImage.GetPixel(x, y);
                        if (pixel.R != 255/* || pixel.G != 255 || pixel.B != 255*/)
                        {
                            pImage.SetPixel(x, y, NodeFGBright);
                            continue;
                        }
                        if (i <= NodeBorderWidth || i >= nodeRect.Width - NodeBorderWidth || j <= NodeBorderWidth || j >= nodeRect.Height - NodeBorderWidth)
                        {
                            pImage.SetPixel(x, y, GetInverseColor(bkPixel));
                        }
                        else { pImage.SetPixel(x, y, bkPixel); }
                    }
                }
            }
            pCache.UnlockBits();
            pImage.UnlockBits();
        }
        /// <summary>
        /// 根据字黑底白绘制文字部分，根据给定颜色绘制底纹
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rect"></param>
        /// <param name="white"></param>
        /// <param name="black"></param>
        /// <param name="fillColor"></param>
        private static void DrawStringNode(Bitmap image, Rectangle nodeRect, string name, Color fillColor)
        {
            var whiteMore = GetNodeNamePattern(image, nodeRect, name);
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pCache = new(BackImageCache);
            pCache.LockBits();
            if (whiteMore)
            {
                for (int i = 0; i < nodeRect.Width; i++)
                {
                    for (int j = 0; j < nodeRect.Height; j++)
                    {
                        var x = nodeRect.Left + i;
                        var y = nodeRect.Top + j;
                        var bkPixel = pCache.GetPixel(x, y);
                        var pixel = pImage.GetPixel(x, y);
                        if (pixel.R != 255/* || pixel.G != 255 || pixel.B != 255*/)
                        {
                            pImage.SetPixel(x, y, NodeFGDark);
                            continue;
                        }
                        if (i <= NodeBorderWidth || i >= nodeRect.Width - NodeBorderWidth || j <= NodeBorderWidth || j >= nodeRect.Height - NodeBorderWidth)
                        {
                            pImage.SetPixel(x, y, GetInverseColor(bkPixel));
                        }
                        else { pImage.SetPixel(x, y, fillColor); }
                    }
                }
            }
            else
            {
                for (int i = 0; i < nodeRect.Width; i++)
                {
                    for (int j = 0; j < nodeRect.Height; j++)
                    {
                        var x = nodeRect.Left + i;
                        var y = nodeRect.Top + j;
                        var bkPixel = pCache.GetPixel(x, y);
                        var pixel = pImage.GetPixel(x, y);
                        if (pixel.R != 255/* || pixel.G != 255 || pixel.B != 255*/)
                        {
                            pImage.SetPixel(x, y, NodeFGBright);
                            continue;
                        }
                        if (i <= NodeBorderWidth || i >= nodeRect.Width - NodeBorderWidth || j <= NodeBorderWidth || j >= nodeRect.Height - NodeBorderWidth)
                        {
                            pImage.SetPixel(x, y, GetInverseColor(bkPixel));
                        }
                        else { pImage.SetPixel(x, y, fillColor); }
                    }
                }
            }
            pCache.UnlockBits();
            pImage.UnlockBits();
        }
        /// <summary>
        /// 确定区域内的像素亮度分布，并用黑、白纯色区分出文字和底纹的区别
        /// </summary>
        /// <param name="image"></param>
        /// <param name="nodeRect"></param>
        /// <param name="name"></param>
        private static bool GetNodeNamePattern(Bitmap image, Rectangle nodeRect, string name)
        {
            int black = 0;
            int white = 0;

            PointBitmap pImage = new(image);
            pImage.LockBits();
            for (int i = 0; i < nodeRect.Width; i++)
            {
                for (int j = 0; j < nodeRect.Height; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var pixel = pImage.GetPixel(x, y);
                    if (pixel.GetBrightness() < 0.5f)
                    {
                        black++;
                    }
                    else { white++; }
                    // set rect to white blank
                    pImage.SetPixel(x, y, Color.White);
                }
            }
            pImage.UnlockBits();

            var fontHeight = name.Length / 3;
            if (name.Length % 3 != 0) { fontHeight++; }
            if (fontHeight == 0) { fontHeight++; }
            var fontWidth = name.Length / fontHeight;
            if (name.Length % 3 != 0) { fontWidth++; }
            var fontSizeH = 0.7f * nodeRect.Height / fontHeight;
            var fontSizeW = 0.7f * nodeRect.Width / fontWidth;
            var fontSize = Math.Min(fontSizeH, fontSizeW);
            string sName = name;
            if (fontHeight > 1)
            {
                sName = string.Empty;
                for (int i = 0; i < fontHeight; i++)
                {
                    var start = i * fontWidth;
                    var leave = name.Length - start;
                    sName += $"{name.Substring(start, fontWidth > leave ? leave : fontWidth)}\n";
                }
            }
            var font = new Font(NodeFont, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);

            var g = Graphics.FromImage(image);
            g.DrawString(sName, font, new SolidBrush(Color.Black), nodeRect, NodeFontFormat);
            g.Flush(); g.Dispose();

            return black <= white;
        }
        /// <summary>
        /// 反色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Color GetInverseColor(Color color)
        {
            var R = 255 - color.R;
            var G = 255 - color.G;
            var B = 255 - color.B;
            return Color.FromArgb(color.A, R, G, B);
        }

        #endregion

        #region ==== 绘制关系线 ====

        /// <summary>
        /// 绘制关系线 - 寻找线的起点和终点
        /// </summary>
        /// <param name="image"></param>
        /// <param name="pen"></param>
        /// <param name="startLoc"></param>
        /// <param name="endLoc"></param>
        public static void DrawRequireLine(Bitmap image, LatticedPoint startLoc, LatticedPoint endLoc)
        {
            var colDiff = endLoc.Col - startLoc.Col;
            var rowDiff = startLoc.Row - endLoc.Row;
            LatticeCell cell = new(startLoc);
            var paddingHeight = LatticeCell.NodePaddingHeight;
            var nodeWidth = LatticeCell.NodeWidth;
            var drLeft = Lattice.DrawRect.Left;
            var drRight = Lattice.DrawRect.Right;
            //
            // 竖线1
            //
            var halfRowDiff = rowDiff / 2;
            var x = cell.NodeRealLeft + nodeWidth / 2;
            var y = cell.RealTop + paddingHeight;
            cell.LatticedTop -= halfRowDiff;
            var y2 = cell.RealTop + paddingHeight / 2;
            if (x >= drLeft && x <= drRight)
            {
                DrawLine(image, new(x, y), new(x, y2), false);
            }
            //
            // 横线
            //
            if (Math.Abs(colDiff) > 0)
            {
                cell.LatticedLeft += colDiff;
                var x2 = cell.NodeRealLeft + nodeWidth / 2;
                DrawLine(image, new(x, y2), new(x2, y2), true);
            }
            //
            // 竖线2
            //
            x = cell.NodeRealLeft + nodeWidth / 2;
            if (x >= drLeft && x <= drRight)
            {
                y = y2;
                var leaveHeight = rowDiff - halfRowDiff - 1;
                cell.LatticedTop -= leaveHeight;
                y2 = cell.RealTop;
                DrawLine(image, new(x, y), new(x, y2), false);
            }
        }

        #endregion

        #region ==== 绘制线方法 ====

        /// <summary>
        /// 绘制空心线，或直接画线
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pen"></param>
        private static void DrawLine(Bitmap image, Point p1, Point p2, bool horizon)
        {
            var halfLineWidth = NodeLineWidth / 2;
            Rectangle lineRect;
            if (horizon)
            {
                lineRect = new(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y) - halfLineWidth, Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y) + NodeLineWidth);
            }
            else
            {
                lineRect = new(Math.Min(p1.X, p2.X) - halfLineWidth, Math.Min(p1.Y, p2.Y), Math.Abs(p1.X - p2.X) + NodeLineWidth, Math.Abs(p1.Y - p2.Y));
            }
            if (!Lattice.RectWithin(lineRect, out lineRect)) { return; }
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pCache = new(BackImageCache);
            pCache.LockBits();
            if (horizon)
            {
                // top
                for (int i = 0; i < lineRect.Width; i++)
                {
                    var left = lineRect.Left + i;
                    var top = lineRect.Top;
                    var pixel = pCache.GetPixel(left, top);
                    var A = pixel.A;
                    var R = pixel.R;
                    var G = pixel.G;
                    var B = pixel.B;
                    pImage.SetPixel(left, top, Color.FromArgb(A, 255 - R, 255 - G, 255 - B));
                }
                // bottom
                var bottom = lineRect.Bottom;
                if (lineRect.Top < bottom)
                {
                    for (int i = 0; i < lineRect.Width; i++)
                    {
                        var left = lineRect.Left + i;
                        var pixel = pCache.GetPixel(left, bottom);
                        var A = pixel.A;
                        var R = pixel.R;
                        var G = pixel.G;
                        var B = pixel.B;
                        pImage.SetPixel(left, bottom, Color.FromArgb(A, 255 - R, 255 - G, 255 - B));
                    }
                }
            }
            // left
            for (int j = 0; j < lineRect.Height; j++)
            {
                var left = lineRect.Left;
                var top = lineRect.Top + j;
                var pixel = pCache.GetPixel(left, top);
                var A = pixel.A;
                var R = pixel.R;
                var G = pixel.G;
                var B = pixel.B;
                pImage.SetPixel(left, top, Color.FromArgb(A, 255 - R, 255 - G, 255 - B));
            }
            // right
            var right = lineRect.Right;
            if (lineRect.Left < right)
            {
                for (int j = 0; j < lineRect.Height; j++)
                {
                    var top = lineRect.Top + j;
                    var pixel = pCache.GetPixel(right, top);
                    var A = pixel.A;
                    var R = pixel.R;
                    var G = pixel.G;
                    var B = pixel.B;
                    pImage.SetPixel(right, top, Color.FromArgb(A, 255 - R, 255 - G, 255 - B));
                }
            }
            pCache.UnlockBits();
            pImage.UnlockBits();
        }

        #endregion
    }
}
