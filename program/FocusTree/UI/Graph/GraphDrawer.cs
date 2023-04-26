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

        #region ==== 画笔 ====

        /// <summary>
        /// 节点边框宽度
        /// </summary>
        public static int NodeBorderWidth = 2;
        /// <summary>
        /// 节点文字颜色
        /// </summary>
        public static Color NodeFG { get; private set; } = Color.FromArgb(175, Color.DarkBlue);
        /// <summary>
        /// 节点文字颜色 - 背景图片 - 重色
        /// </summary>
        public static Color NodeFG_BkDark { get; private set; } = Color.FromArgb(175, Color.Black);
        /// <summary>
        /// 节点文字颜色 - 背景图片 - 浅色
        /// </summary>
        public static Color NodeFG_BkLight { get; private set; } = Color.FromArgb(175, Color.AliceBlue);
        /// <summary>
        /// 默认节点背景颜色
        /// </summary>
        public static Color NodeBG_Normal { get; private set; } = Color.FromArgb(100, Color.Cyan);
        /// <summary>
        /// 节点背景颜色阴影
        /// </summary>
        public static Color NodeBGShadow { get; private set; } = Color.FromArgb(100, Color.DarkBlue);
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
            new(Color.FromArgb(100, Color.Cyan), 2),
            new(Color.FromArgb(100, Color.Yellow), 2),
            new(Color.FromArgb(100, Color.Green), 2),
            new(Color.FromArgb(100, Color.Orange), 2),
            new(Color.FromArgb(100, Color.Purple), 2)
        };

        #endregion

        #region ==== 背景 ====

        /// <summary>
        /// 无图片背景
        /// </summary>
        public static Color BlankBackground = Color.AliceBlue;
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
        /// <summary>
        /// 关系线绘制委托列表
        /// </summary>
        static Dictionary<(int, int), CellDrawer> LineDrawerCatalog = new();
        //public static HashSet<Point> LastDrawnCells = new();
        public static Bitmap RedrawBuffer;

        #endregion

        #region ==== 加载背景图片 ====

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
                RedrawBuffer = new(Width, Height);
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
            if (BackImageCache == null || new Size(bkWidth, bkHeight) != BackImageCache.Size)
            {
                BackImageCache?.Dispose();
                BackImageCache = new Bitmap(bkWidth, bkHeight);
                var g = Graphics.FromImage(BackImageCache);
                g.DrawImage(sourceImage, 0, 0, bkWidth, bkHeight);
                g.Flush(); g.Dispose();
            }
            sourceImage.Dispose();
            RedrawBuffer = new(Width, Height);
        }

        #endregion

        #region ==== 上载绘制委托 ====

        /// <summary>
        /// 将节点绘制上载到栅格绘图委托
        /// </summary>
        public static void UploadDrawerNode(FocusData focus)
        {
            var id = focus.ID;
            LatticeCell cell = new(focus);
            if (NodeDrawerCatalog.TryGetValue(id, out var drawer))
            {
                Lattice.Drawing -= drawer;
            }
            Lattice.Drawing += NodeDrawerCatalog[id] = (image) => DrawNode(image, focus);
        }
        /// <summary>
        /// 将节点关系线绘制到栅格绘图委托
        /// </summary>
        /// <param name="penIndex">笔颜色</param>
        /// <param name="start">起始国策</param>
        /// <param name="end">结束国策</param>
        public static void UploadDrawerRequireLine(int penIndex, FocusData start, FocusData end)
        {
            (int, int) ID = (start.ID, end.ID);
            if (LineDrawerCatalog.TryGetValue(ID, out var drawer))
            {
                Lattice.Drawing -= drawer;
            }
            Lattice.Drawing += LineDrawerCatalog[ID] = (image) => DrawRequireLine(image, NodeRequire[penIndex], start.LatticedPoint, end.LatticedPoint);
        }

        #endregion

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
                    RedrawBuffer.SetPixel(x, y, pixel);
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
                    RedrawBuffer.SetPixel(x, y, pixel);
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
                    RedrawBuffer.SetPixel(x, y, pixel);
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
                    RedrawBuffer.SetPixel(x, y, pixel);
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
            if (fontSize <= 0) { return; }
            string sName = name;
            if (fontHeight > 1)
            {
                sName = string.Empty;
                for (int i = 0; i < fontHeight; i++)
                {
                    sName += $"{name.Substring(i * fontWidth, fontWidth > name.Length - i * fontWidth ? name.Length - i * fontWidth : fontWidth)}\n";
                }
                sName = sName[..^1];
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
                    if (pixel.R != 255 || pixel.G != 255 || pixel.B != 255)
                    {
                        RedrawBuffer.SetPixel(x, y, bkPixel);
                        if (black < white)
                        {
                            pImage.SetPixel(x, y, NodeFG_BkDark);
                        }
                        else
                        {
                            pImage.SetPixel(x, y, NodeFG_BkLight);
                        }
                        continue;
                    }
                    if (i <= NodeBorderWidth || i >= rect.Width - NodeBorderWidth || j <= NodeBorderWidth || j >= rect.Height - NodeBorderWidth)
                    {
                        RedrawBuffer.SetPixel(x, y, bkPixel);
                        var A = pixel.A;
                        var R = pixel.R;
                        var G = pixel.G;
                        var B = pixel.B;
                        pImage.SetPixel(x, y, Color.FromArgb(A, 255 - R, 255 - G, 255 - B));
                    }
                    else
                    {
                        pImage.SetPixel(x, y, bkPixel);
                    }

                }
            }
            pCache.UnlockBits();
            pImage.UnlockBits();
        }
        /// <summary>
        /// 绘制节点 - 旧的
        /// </summary>
        /// <param name="g"></param>
        /// <param name="id"></param>
        /// <param name="brush"></param>
        public static void DrawNode(Graphics g, FocusData focus, Color color)
        {
            LatticeCell cell = new(focus);
            var rect = cell.InnerPartRealRects[LatticeCell.Parts.Node];
            if (!Lattice.RectWithin(rect, out var saveRect)) { return; }
            rect = saveRect;
            Rectangle shadowRect = new(rect.Left + NodeBorderWidth, rect.Top + NodeBorderWidth, rect.Width, rect.Height);
            g.FillRectangle(new SolidBrush(Color.White), shadowRect);
            g.FillRectangle(new SolidBrush(color), rect);
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
            g.DrawString(sName, font, new SolidBrush(NodeFG), rect, NodeFontFormat);
            g.Flush();
        }
        /// <summary>
        /// 绘制关系线 - 寻找线的起点和终点
        /// </summary>
        /// <param name="image"></param>
        /// <param name="pen"></param>
        /// <param name="startLoc"></param>
        /// <param name="endLoc"></param>
        [Obsolete("这玩意可能还有bug，不过把所有节点都加入列表里了应该就可以避免，但这样做就很感觉不靠谱")]
        public static void DrawRequireLine(Bitmap image, Pen pen, Point startLoc, Point endLoc)
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
                DrawLine(image, new(x, y), new(x, y2), pen, false);
            }
            //
            // 横线
            //
            if (Math.Abs(widthDiff) > 0)
            {
                cell.LatticedLeft += widthDiff;
                var x2 = cell.NodeRealLeft + nodeWidth / 2;
                DrawLine(image, new(x, y2), new(x2, y2), pen, true);
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
                DrawLine(image, new(x, y), new(x, y2), pen, false);
            }
        }
        /// <summary>
        /// 绘制空心线，或直接画线
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pen"></param>
        private static void DrawLine(Bitmap image, Point p1, Point p2, Pen pen, bool horizon)
        {
            var halfBorder = NodeBorderWidth / 2;
            Rectangle lineRect;
            if (horizon)
            {
                lineRect = new(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y) - halfBorder, Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y) + NodeBorderWidth);
            }
            else
            {
                lineRect = new(Math.Min(p1.X, p2.X) - halfBorder, Math.Min(p1.Y, p2.Y), Math.Abs(p1.X - p2.X) + NodeBorderWidth, Math.Abs(p1.Y - p2.Y));
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
                    RedrawBuffer.SetPixel(left, top, pixel);
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
                        RedrawBuffer.SetPixel(left, bottom, pixel);
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
                RedrawBuffer.SetPixel(left, top, pixel);
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
                    RedrawBuffer.SetPixel(right, top, pixel);
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
        /// <summary>
        /// 重绘绘制过的格元
        /// </summary>
        /// <param name="image"></param>
        public static void RedrawDrawnCells(Image image)
        {
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(RedrawBuffer, 0, 0);
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
        public static void SetRedrawBuffer()
        {
            //RedrawBuffer?.Dispose();
            //RedrawBuffer = new(BackImageCache.Width, BackImageCache.Height);
        }
    }
}
