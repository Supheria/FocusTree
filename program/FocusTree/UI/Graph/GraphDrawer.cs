#define PointBmp
using FocusTree.Data.Focus;
using FocusTree.UI.test;

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
        public static Color NodeFG { get; private set; } = Color.FromArgb(175, Color.DarkBlue);
        /// <summary>
        /// 默认节点背景颜色
        /// </summary>
        public static SolidBrush NodeBG_Normal { get; private set; } = new(Color.FromArgb(100, Color.Cyan));
        public static Color NodeBG { get; private set; } = Color.FromArgb(100, Color.Cyan);
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
        public static int NodeBorderWidth = 2;
        public static int PenWidth = 2;
        /// <summary>
        /// 背景图片
        /// </summary>
        public static readonly Bitmap BackImage;
        /// <summary>
        /// 反色背景图片
        /// </summary>
        private static Bitmap BackImageInverse;
        /// <summary>
        /// 背景图片在给定尺寸下的缓存
        /// </summary>
        static Bitmap BkCacher;
        /// <summary>
        /// 反色背景图片在给定尺寸下的缓存
        /// </summary>
        static Bitmap BkInverseCacher;
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
        /// <summary>
        /// 节点绘制委托列表
        /// </summary>
        public static Dictionary<int, CellDrawer> NodeDrawerCatalog { get; private set; } = new();
        static Dictionary<(int, int), CellDrawer> LineDrawerCatalog = new();
        public static HashSet<Point> LastDrawnCells = new();
        /// <summary>
        /// 加载背景图片并反色
        /// </summary>
        static GraphDrawer()
        {
            if (!File.Exists("Background.jpg")) { return; }
            BackImage = (Bitmap)Image.FromFile("Background.jpg");
            ShowBackGroung = true;

            if (File.Exists("Background_Inverse.jpg"))
            {
                BackImageInverse = (Bitmap)Image.FromFile("Background_Inverse.jpg");
                return;
            }
            var width = BackImage.Width;
            var height = BackImage.Height;
            BackImageInverse = new Bitmap(width, height);

            PointBitmap pImage = new(BackImage);
            pImage.LockBits();
            PointBitmap pInverse = new PointBitmap(BackImageInverse);
            pInverse.LockBits();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pixel = pImage.GetPixel(x, y);
                    pInverse.SetPixel(x, y, Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B));
                }
            }
            pInverse.UnlockBits();
            pImage.UnlockBits();
            BackImageInverse.Save("Background_Inverse.jpg");
        }
        /// <summary>
        /// 根据给定尺寸设置背景图片缓存
        /// </summary>
        public static void SetBackImageCacher(Size size)
        {
            var Width = size.Width;
            var Height = size.Height;
            var bkWidth = Width;
            var bkHeight = Height;
            float sourceRatio = (float)BackImage.Width / (float)BackImage.Height;
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
            if (BkCacher != null && bkWidth == BkCacher.Width && bkHeight == BkCacher.Height) { return; }
            BkCacher?.Dispose();
            BkInverseCacher?.Dispose();

            BkCacher = new Bitmap(bkWidth, bkHeight);
            var g = Graphics.FromImage(BkCacher);
            g.DrawImage(BackImage, 0, 0, bkWidth, bkHeight);
            g.Flush();

            BkInverseCacher = new Bitmap(bkWidth, bkHeight);
            g = Graphics.FromImage(BkInverseCacher);
            g.DrawImage(BackImageInverse, 0, 0, bkWidth, bkHeight);
            g.Flush(); g.Dispose();
        }
        /// <summary>
        /// 获取背景图片在给定尺寸下的缓存，如果为null或尺寸不同则获取后返回
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Image GetBackImageCacher(Size size)
        {
            if (BkCacher == null || size != BkCacher.Size) { SetBackImageCacher(size); }
            return BkCacher;
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
            Lattice.Drawing += NodeDrawerCatalog[id] = (image) => DrawNode(image, focus);
        }
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
            LastDrawnCells.Add(cell.LatticedPoint);
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
            if (!ShowBackGroung)
            {
                pImage.LockBits();
                for (int i = 0; i < nodeRect.Width; i++)
                {
                    for (int j = 0; j < nodeRect.Height; j++)
                    {
                        var x = nodeRect.Left + i;
                        var y = nodeRect.Top + j;
                        if (i >= nodeRect.Width - NodeBorderWidth || j >= nodeRect.Height - NodeBorderWidth)
                        {
                            pImage.SetPixel(x, y, NodeBGShadow);
                        }
                        else { pImage.SetPixel(x, y, NodeBG); }
                    }
                }
                pImage.UnlockBits();
                return;
            }
            DateTime t1 = DateTime.Now;
#if PointBmp
            pImage.LockBits();
            PointBitmap pInverseCacher = new(BkInverseCacher);
            pInverseCacher.LockBits();
            for (int i = 0; i < NodeBorderWidth; i++)
            {
                for (int j = 0; j < nodeRect.Height; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var pixel = pInverseCacher.GetPixel(x, y);
                    pImage.SetPixel(x, y, pixel);
                }
            }
            for (int i = nodeRect.Width - NodeBorderWidth; i < nodeRect.Width; i++)
            {
                for (int j = 0; j < nodeRect.Height; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var pixel = pInverseCacher.GetPixel(x, y);
                    pImage.SetPixel(x, y, pixel);
                }
            }
            for (int i = NodeBorderWidth; i < nodeRect.Width - NodeBorderWidth; i++)
            {
                for (int j = 0; j < NodeBorderWidth; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var pixel = pInverseCacher.GetPixel(x, y);
                    pImage.SetPixel(x, y, pixel);
                }
            }
            for (int i = NodeBorderWidth; i < nodeRect.Width - NodeBorderWidth; i++)
            {
                for (int j = nodeRect.Height - NodeBorderWidth; j < nodeRect.Height; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var pixel = pInverseCacher.GetPixel(x, y);
                    pImage.SetPixel(x, y, pixel);
                }
            }
            pInverseCacher.UnlockBits();
            pImage.UnlockBits();
#else
            Bitmap nodeBmp = new(nodeRect.Width, nodeRect.Height);
            for (int i = 0; i < nodeRect.Width; i++)
            {
                for (int j = 0; j < nodeRect.Height; j++)
                {
                    if (i <= NodeBorderWidth || i >= nodeRect.Width - NodeBorderWidth || j <= NodeBorderWidth || j >= nodeRect.Height - NodeBorderWidth)
                    {
                        var pixel = BkInverseCacher.GetPixel(nodeRect.Left + i, nodeRect.Top + j);
                        nodeBmp.SetPixel(i, j, Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B));
                    }
                    else
                    {
                        var pixel = BkCacher.GetPixel(nodeRect.Left + i, nodeRect.Top + j);
                        nodeBmp.SetPixel(i, j, Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B));
                    }
                }
            }
            Rectangle rect = new(0, 0, nodeRect.Width, nodeRect.Height);
            g.DrawImage(nodeBmp, nodeRect, rect, GraphicsUnit.Pixel);
            nodeBmp.Dispose();
            g.Flush();
#endif
            DateTime t2 = DateTime.Now;
            TimeSpan ts = t2 - t1;
            test.Show();
            //test.InfoText = $"{ts.TotalMilliseconds}";
        }
        static TestInfo test = new();
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
            if (ShowBackGroung)
            {
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
            }
            var fontHeight = name.Length / 3;
            if (fontHeight == 1 && name.Length % 3 != 0) { fontHeight++; }
            else if (fontHeight == 0) { fontHeight++; }
            var fontWidth = name.Length / fontHeight;
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
                    sName += $"{name.Substring(i * fontWidth, fontWidth)}\n";
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
            if (!ShowBackGroung)
            {
                pImage.LockBits();
                for (int i = 0; i < rect.Width; i++)
                {
                    for (int j = 0; j < rect.Height; j++)
                    {
                        var x = rect.Left + i;
                        var y = rect.Top + j;
                        var pixel = pImage.GetPixel(x, y);
                        if (pixel.R != 255 || pixel.G != 255 || pixel.B != 255)
                        {
                            pImage.SetPixel(x, y, NodeFG);
                            continue;
                        }
                        if (i >= rect.Width - NodeBorderWidth || j >= rect.Height - NodeBorderWidth)
                        {
                            pImage.SetPixel(x, y, NodeBGShadow);
                        }
                        else { pImage.SetPixel(x, y, NodeBG); }
                    }
                }
                pImage.UnlockBits();
                return;
            }
            pImage.LockBits();
            PointBitmap pCacher = new(BkCacher);
            pCacher.LockBits();
            PointBitmap pInverseCacher = new(BkInverseCacher);
            pInverseCacher.LockBits();
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j < rect.Height; j++)
                {
                    var x = rect.Left + i;
                    var y = rect.Top + j;
                    var pixel = pImage.GetPixel(x, y);
                    var bkPixel = pCacher.GetPixel(x, y);
                    if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
                    {
                        if (i <= NodeBorderWidth || i >= rect.Width - NodeBorderWidth || j <= NodeBorderWidth || j >= rect.Height - NodeBorderWidth)
                        {
                            bkPixel = pInverseCacher.GetPixel(x, y);
                        }
                        pImage.SetPixel(x, y, bkPixel);
                        continue;
                    }
                    if (black < white)
                    {
                        pImage.SetPixel(x, y, Color.FromArgb(255, Color.DarkBlue));
                    }
                    else
                    {
                        pImage.SetPixel(x, y, Color.FromArgb(255, Color.AliceBlue));
                    }
                }
            }
            pInverseCacher.UnlockBits();
            pCacher.UnlockBits();
            pImage.UnlockBits();
        }
        /// <summary>
        /// 绘制节点 - 旧的
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
            Rectangle shadowRect = new(rect.Left + NodeBorderWidth, rect.Top + NodeBorderWidth, rect.Width, rect.Height);
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
            g.DrawString(sName, font, new SolidBrush(NodeFG), rect, NodeFontFormat);
            g.Flush();
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
            Lattice.Drawing += LineDrawerCatalog[ID] = (image) => DrawLines(image, NodeRequire[penIndex], start.LatticedPoint, end.LatticedPoint);

        }
        public static void DrawLines(Bitmap image, Pen pen, Point startLoc, Point endLoc)
        {
            var widthDiff = endLoc.X - startLoc.X;
            var heightDiff = startLoc.Y - endLoc.Y;
            LatticeCell cell = new(startLoc.X, startLoc.Y);
            var paddingHeight = LatticeCell.NodePaddingHeight;
            var nodeWidth = LatticeCell.NodeWidth;
            if (Lattice.RectWithin(cell.RealRect, out var r)) { LastDrawnCells.Add(startLoc); }
            //
            // 竖线1
            //
            var y1 = cell.RealTop + paddingHeight;
            var halfHeight = heightDiff / 2;
            cell.LatticedTop -= halfHeight;
            var y2 = cell.RealTop + paddingHeight / 2;
            var x = cell.NodeRealLeft + nodeWidth / 2;
            DrawLine(image, x, (y1, y2), pen);
            for (int i = 0; i < halfHeight; i++)
            {
                LatticeCell drawnCell = new(startLoc.X, startLoc.Y--);
                if (Lattice.RectWithin(drawnCell.RealRect, out r)) { LastDrawnCells.Add(drawnCell.LatticedPoint); }
            }
            //
            // 横线
            //
            if (Math.Abs(widthDiff) > 0)
            {
                cell.LatticedLeft += widthDiff;
                var x2 = cell.NodeRealLeft + nodeWidth / 2;
                DrawLine(image, (x, x2), y2, pen);
                for (int i = 1; i <= widthDiff; i++)
                {
                    LatticeCell drawnCell = new(startLoc.X + widthDiff < 0 ? -i : i, startLoc.Y - halfHeight);
                    if (Lattice.RectWithin(drawnCell.RealRect, out r)) { LastDrawnCells.Add(drawnCell.LatticedPoint); }
                }
            }
            //
            // 竖线2
            //
            y1 = y2;
            var leaveHeight = heightDiff - halfHeight - 1;
            cell.LatticedTop -= leaveHeight;
            y2 = cell.RealTop;
            x = cell.NodeRealLeft + nodeWidth / 2;
            DrawLine(image, x, (y1, y2), pen);
            for (int i = 1; i <= leaveHeight; i++)
            {
                LatticeCell drawnCell = new(startLoc.X + widthDiff, startLoc.Y - halfHeight - i);
                if (Lattice.RectWithin(drawnCell.RealRect, out r)) { LastDrawnCells.Add(drawnCell.LatticedPoint); }
            }
        }
        private static void DrawLine(Bitmap image, int x, (int, int) y, Pen pen)
        {
            Rectangle lineRect = new(x - PenWidth / 2, Math.Min(y.Item1, y.Item2), PenWidth, Math.Abs(y.Item1 - y.Item2));
            if (!Lattice.RectWithin(lineRect, out var rect)) { return; }
            PointBitmap pImage = new(image);
            if (ShowBackGroung)
            {
                pImage.LockBits();
                PointBitmap pInverseCacher = new(BkInverseCacher);
                pInverseCacher.LockBits();
                //for (int i = 0; i < rect.Width; i++)
                //{
                //    var left = rect.Left + i;
                //    var top = rect.Top;
                //    var pixel = pInverseCacher.GetPixel(left, top);
                //    pImage.SetPixel(left, top, pixel);
                //}
                //for (int i = 0; i < rect.Width; i++)
                //{
                //    var left = rect.Left;
                //    var bottom = rect.Bottom;
                //    var pixel = pInverseCacher.GetPixel(left, bottom);
                //    pImage.SetPixel(left, bottom, pixel);
                //}
                for (int j = 0; j < rect.Height; j++)
                {
                    var left = rect.Left;
                    var top = rect.Top + j;
                    var pixel = pInverseCacher.GetPixel(left, top);
                    pImage.SetPixel(left, top, pixel);
                }
                for (int j = 0; j < rect.Height; j++)
                {
                    var right = rect.Right;
                    var top = rect.Top + j;
                    var pixel = pInverseCacher.GetPixel(right, top);
                    pImage.SetPixel(right, top, pixel);
                }
                //g.DrawImage(BkInverseCacher, rect, rect, GraphicsUnit.Pixel);
                //rect = new(rect.X + 1, rect.Y, rect.Width - 2, rect.Height);
                //g.DrawImage(BkCacher, rect, rect, GraphicsUnit.Pixel);
                pInverseCacher.UnlockBits();
                pImage.UnlockBits();
            }
            else
            {
                if (Lattice.LineWithin(x, y, pen.Width, out var line))
                {
                    //g.DrawLine(pen, line.Item1, line.Item2);
                }
            }
            //g.Flush();
        }
        private static void DrawLine(Bitmap image, (int, int) x, int y, Pen pen)
        {
            Rectangle lineRect = new(Math.Min(x.Item1, x.Item2), y - PenWidth / 2, Math.Abs(x.Item1 - x.Item2) + 2, PenWidth);
            if (!Lattice.RectWithin(lineRect, out var rect)) { return; }
            PointBitmap pImage = new(image);
            if (ShowBackGroung)
            {
                pImage.LockBits();
                PointBitmap pInverseCacher = new(BkInverseCacher);
                pInverseCacher.LockBits();
                for (int i = 0; i < rect.Width; i++)
                {
                    var left = rect.Left + i;
                    var top = rect.Top;
                    var pixel = pInverseCacher.GetPixel(left, top);
                    pImage.SetPixel(left, top, pixel);
                }
                for (int i = 0; i < rect.Width; i++)
                {
                    var left = rect.Left + i;
                    var bottom = rect.Bottom;
                    var pixel = pInverseCacher.GetPixel(left, bottom);
                    pImage.SetPixel(left, bottom, pixel);
                }
                //for (int j = 0; j < rect.Height; j++)
                //{
                //    var left = rect.Left;
                //    var top = rect.Top + j;
                //    var pixel = pInverseCacher.GetPixel(left, top);
                //    pImage.SetPixel(left, top, pixel);
                //}
                //for (int j = 0; j < rect.Height; j++)
                //{
                //    var right = rect.Right;
                //    var top = rect.Top + j;
                //    var pixel = pInverseCacher.GetPixel(right, top);
                //    pImage.SetPixel(right, top, pixel);
                //}
                //g.DrawImage(BkInverseCacher, rect, rect, GraphicsUnit.Pixel);
                //rect = new(rect.X + 1, rect.Y, rect.Width - 2, rect.Height);
                //g.DrawImage(BkCacher, rect, rect, GraphicsUnit.Pixel);
                pInverseCacher.UnlockBits();
                pImage.UnlockBits();
            }
            else
            {
                if (Lattice.LineWithin(x, y, pen.Width, out var line))
                {
                    //g.DrawLine(pen, line.Item1, line.Item2);
                }
            }
            //g.Flush();
        }
        public static void RedrawLastDrawnCells(Image image)
        {
            DateTime t1 = DateTime.Now;
            
            if (!ShowBackGroung)
            {
                return;
            }
            PointBitmap pImage = new((Bitmap)image);
            pImage.LockBits();
            PointBitmap pCacher = new(BkCacher);
            pCacher.LockBits();
            foreach (var point in LastDrawnCells)
            {
                LatticeCell cell = new(point.X, point.Y);
                var left = Lattice.LastCellWidth * point.X + Lattice.LastOriginLeft - Lattice.DrawRect.X + Lattice.DeviDiffInDrawRectWidth; ;
                var top = Lattice.LastCellHeight * point.Y + Lattice.LastOriginTop - Lattice.DrawRect.Y + Lattice.DeviDiffInDrawRectHeight;
                Rectangle rect = new(left, top, Lattice.LastCellWidth, Lattice.LastCellHeight);
                //var rect = cell.RealRect;
                //rect.X -= Lattice.OriginLeft - Lattice.LastOriginLeft;
                //rect.Y -= Lattice.OriginTop - Lattice.LastOriginTop;
                if (!Lattice.RectWithin(rect, out rect)) { return; }
                
                for (int i = 0; i < rect.Width; i++)
                {
                    for (int j = 0; j < rect.Height; j++)
                    {
                        var x = rect.Left + i;
                        var y = rect.Top + j;
                        var pixel = pCacher.GetPixel(x, y);
                        (pImage).SetPixel(x, y, pixel);
                    }
                }
                LastDrawnCells.Remove(point);
            }
            pCacher.UnlockBits();
            pImage.UnlockBits();
            DateTime t2 = DateTime.Now;
            TimeSpan ts = t2 - t1;
            test.Show();
            test.InfoText = $"{ts.TotalMilliseconds}";
        }
    }
}
