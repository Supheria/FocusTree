#define PointBmp
using FocusTree.Data.Focus;
using FocusTree.UI.test;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FocusTree.UI.Graph
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
        public static int NodeLineWidth = 3;
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

        #endregion

        #region ==== 绘制委托列表 ====

        /// <summary>
        /// 节点绘制委托列表
        /// </summary>
        public static Dictionary<int, CellDrawer> NodeDrawerCatalog { get; private set; } = new();

        #endregion

        #region ==== 加载背景 ====

        /// <summary>
        /// 获取背景图片在给定尺寸下的缓存，如果为null或尺寸不同则获取后返回
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private static Image GetBackImageCacher(Size size)
        {
            if (BackImageCache == null || size != BackImageCache.Size) { SetBackImageCacher(size); }
            return BackImageCache;
        }
        /// <summary>
        /// 根据给定尺寸设置背景图片缓存
        /// </summary>
        private static void SetBackImageCacher(Size size)
        {
            var Width = size.Width;
            var Height = size.Height;
            if (!File.Exists(BackImagePath))
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
            var sourceImage = (Bitmap)Image.FromFile(BackImagePath);
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
            LatticeCell cell = new(focus);
            Lattice.Drawing += NodeDrawerCatalog[focus.ID] = (image) => DrawNode(image, focus);
        }
        /// <summary>
        /// 将节点关系线绘制到栅格绘图委托
        /// </summary>
        /// <param name="penIndex">笔颜色</param>
        /// <param name="start">起始国策</param>
        /// <param name="end">结束国策</param>
        public static void UploadDrawerRequireLine(int penIndex, FocusData start, FocusData end)
        {
            Lattice.Drawing += (image) => DrawRequireLine(image, start.LatticedPoint, end.LatticedPoint);
        }

        #endregion

        #region ==== 绘制节点 ====

        public static void DrawNode(Bitmap image, FocusData focus)
        {
            LatticeCell cell = new(focus);
            var nodeRect = cell.InnerPartRealRects[LatticeCell.Parts.Node];
            if (!Lattice.RectWithin(nodeRect, out nodeRect)) { return; }
            var cellRect = cell.RealRect;
            if (cellRect.Width < LatticeCell.SizeMax.Width / 2 || cellRect.Height < LatticeCell.SizeMax.Height / 2)
            {
                DrawBlankNode(image, nodeRect);
            }
            else { DrawStringNode(image, nodeRect, focus.Name); }
        }

        #endregion

        #region ==== 节点绘制方法 ====

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
                    var A = pixel.A;
                    var R = pixel.R;
                    var G = pixel.G;
                    var B = pixel.B;
                    pImage.SetPixel(x, y, Color.FromArgb(A, 255 - R, 255 - G, 255 - B));
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
                    var A = pixel.A;
                    var R = pixel.R;
                    var G = pixel.G;
                    var B = pixel.B;
                    pImage.SetPixel(x, y, Color.FromArgb(A, 255 - R, 255 - G, 255 - B));
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
                    var A = pixel.A;
                    var R = pixel.R;
                    var G = pixel.G;
                    var B = pixel.B;
                    pImage.SetPixel(x, y, Color.FromArgb(A, 255 - R, 255 - G, 255 - B));
                }
            }
            // bottom
            for (int i = NodeBorderWidth; i < nodeRect.Width - NodeBorderWidth; i++)
            {
                for (int j = nodeRect.Height - NodeBorderWidth; j < nodeRect.Height; j++)
                {
                    if (j <= 0) { break; }
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var pixel = pCache.GetPixel(x, y);
                    var A = pixel.A;
                    var R = pixel.R;
                    var G = pixel.G;
                    var B = pixel.B;
                    pImage.SetPixel(x, y, Color.FromArgb(A, 255 - R, 255 - G, 255 - B));
                }
            }
            pCache.UnlockBits();
            pImage.UnlockBits();
        }
        /// <summary>
        /// 绘制有文字节点 - 确定区域内的像素分布（为选择字的颜色），并用黑、白纯色区分出文字和底纹的区别，好为下一步扣出字形
        /// </summary>
        /// <param name="image"></param>
        /// <param name="nodeRect"></param>
        /// <param name="name"></param>
        private static void DrawStringNode(Bitmap image, Rectangle nodeRect, string name)
        {
            int black = 0;
            int white = 0;

            PointBitmap pImage = new(image);
            pImage.LockBits();
            for (int i = 0; i < nodeRect.Width; i++)
            {
                for (int j = 0; j < nodeRect.Height; j++)
                {
                    var bkPixel = pImage.GetPixel(nodeRect.Left + i, nodeRect.Top + j);
                    if (bkPixel.R < 123 && bkPixel.G < 123 && bkPixel.B < 123)
                    {
                        black++;
                    }
                    else { white++; }
                    // set rect to white blank
                    pImage.SetPixel(nodeRect.Left + i, nodeRect.Top + j, Color.White);
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

            DrawStringNode(image, nodeRect, white, black);
        }
        /// <summary>
        /// 根据字黑底白绘制文字部分和底纹部分
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rect"></param>
        /// <param name="white"></param>
        /// <param name="black"></param>
        private static void DrawStringNode(Bitmap image, Rectangle rect, int white, int black)
        {
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pCache = new(BackImageCache);
            pCache.LockBits();
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j < rect.Height; j++)
                {
                    var x = rect.Left + i;
                    var y = rect.Top + j;
                    var bkPixel = pCache.GetPixel(x, y);
                    var pixel = pImage.GetPixel(x, y);
                    if (pixel.R != 255/* || pixel.G != 255 || pixel.B != 255*/)
                    {
                        if (black < white)
                        {
                            pImage.SetPixel(x, y, NodeFGDark);
                        }
                        else
                        {
                            pImage.SetPixel(x, y, NodeFGBright);
                        }
                        continue;
                    }
                    if (i <= NodeBorderWidth || i >= rect.Width - NodeBorderWidth || j <= NodeBorderWidth || j >= rect.Height - NodeBorderWidth)
                    {
                        var A = pixel.A;
                        var R = pixel.R;
                        var G = pixel.G;
                        var B = pixel.B;
                        pImage.SetPixel(x, y, Color.FromArgb(A, 255 - R, 255 - G, 255 - B));
                    }
                    else { pImage.SetPixel(x, y, bkPixel); }
                }
            }
            pCache.UnlockBits();
            pImage.UnlockBits();
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
        public static void DrawRequireLine(Bitmap image, Point startLoc, Point endLoc)
        {
            var widthDiff = endLoc.X - startLoc.X;
            var heightDiff = startLoc.Y - endLoc.Y;
            LatticeCell cell = new(startLoc.X, startLoc.Y);
            var paddingHeight = LatticeCell.NodePaddingHeight;
            var nodeWidth = LatticeCell.NodeWidth;
            var drLeft = Lattice.DrawRect.Left;
            var drRight = Lattice.DrawRect.Right;
            //
            // 竖线1
            //
            var halfHeightDiff = heightDiff / 2;
            var x = cell.NodeRealLeft + nodeWidth / 2;
            var y = cell.RealTop + paddingHeight;
            cell.LatticedTop -= halfHeightDiff;
            var y2 = cell.RealTop + paddingHeight / 2;
            if (x >= drLeft && x <= drRight)
            {
                DrawLine(image, new(x, y), new(x, y2), false);
            }
            //
            // 横线
            //
            if (Math.Abs(widthDiff) > 0)
            {
                cell.LatticedLeft += widthDiff;
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
                var leaveHeight = heightDiff - halfHeightDiff - 1;
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

        /// <summary>
        /// 重绘绘制过的格元
        /// </summary>
        /// <param name="image"></param>
        public static void RedrawDrawnCells(Image image)
        {
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(BackImageCache, 0, 0);
            g.Flush(); g.Dispose();
        }
        /// <summary>
        /// 用背景图片缓存填满 image 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rect"></param>
        public static void DrawFillBackImage(Image image, Rectangle rect)
        {
            Graphics g = Graphics.FromImage(image);
            GetBackImageCacher(rect.Size);
            g.DrawImage(BackImageCache, rect, rect, GraphicsUnit.Pixel);
            g.Flush(); g.Dispose();
        }
        /// <summary>
        /// 根据给定的矩形截取背景图片缓存并填充到 Image，或填充白色到给定矩形
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rect"></param>
        public static void DrawRectWithBackImage(Image image, Rectangle rect)
        {
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(BackImageCache, rect, rect, GraphicsUnit.Pixel);
            g.Flush(); g.Dispose();
        }
    }
}
