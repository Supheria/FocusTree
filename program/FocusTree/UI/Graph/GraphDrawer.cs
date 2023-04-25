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

        #region ==== 背景图片 ====

        public static string BackImagePath = "Background.jpg";
        /// <summary>
        /// 背景图片
        /// </summary>
        public static Bitmap BackImage { get; private set; }
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
        public static bool ShowBackground { get; private set; } = false;

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
        public static HashSet<Point> LastDrawnCells = new();

        #endregion

        #region ==== 加载背景图片 ====

        static GraphDrawer()
        {
            LoadBackImage();
        }
        /// <summary>
        /// 加载背景图片并反色
        /// </summary>
        public static void LoadBackImage()
        {
            if (!File.Exists(BackImagePath)) { return; }
            BackImage = (Bitmap)Image.FromFile(BackImagePath);
            ShowBackground = true;

            var inversePath = Path.Combine(Path.GetDirectoryName(BackImagePath), Path.GetFileNameWithoutExtension(BackImagePath) + "_Inverse.jpg");
            if (File.Exists(inversePath))
            {
                BackImageInverse = (Bitmap)Image.FromFile(inversePath);
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
            BackImageInverse.Save(inversePath);
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
            if (BkCacher == null || new Size(bkWidth, bkHeight) != BkCacher.Size)
            {
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
            if (!ShowBackground)
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
                        else { pImage.SetPixel(x, y, NodeBG_Normal); }
                    }
                }
                pImage.UnlockBits();
                return;
            }

            pImage.LockBits();
            PointBitmap pInverseCacher = new(BkInverseCacher);
            pInverseCacher.LockBits();
            // top
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
            // bottom
            for (int i = nodeRect.Width - NodeBorderWidth; i < nodeRect.Width; i++)
            {
                if (i < 0) { break; }
                for (int j = 0; j < nodeRect.Height; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var pixel = pInverseCacher.GetPixel(x, y);
                    pImage.SetPixel(x, y, pixel);
                }
            }
            // left
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
            // right
            for (int i = NodeBorderWidth; i < nodeRect.Width - NodeBorderWidth; i++)
            {
                for (int j = nodeRect.Height - NodeBorderWidth; j < nodeRect.Height; j++)
                {
                    if (j <= 0) { break; }
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var pixel = pInverseCacher.GetPixel(x, y);
                    pImage.SetPixel(x, y, pixel);
                }
            }
            pInverseCacher.UnlockBits();
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
            if (ShowBackground)
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
            if (!ShowBackground)
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
                        else { pImage.SetPixel(x, y, NodeBG_Normal); }
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
                        pImage.SetPixel(x, y, NodeFG_BkDark);
                    }
                    else
                    {
                        pImage.SetPixel(x, y, NodeFG_BkLight);
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
        public static void DrawRequireLine(Bitmap image, Pen pen, Point startLoc, Point endLoc)
        {
            var widthDiff = endLoc.X - startLoc.X;
            var heightDiff = startLoc.Y - endLoc.Y;
            LatticeCell cell = new(startLoc.X, startLoc.Y);
            LatticeCell endCell = new(endLoc.X, endLoc.Y);
            if (!Lattice.RectWithin(cell.NodeRealRect) && !Lattice.RectWithin(endCell.NodeRealRect)) { return; }
            var paddingHeight = LatticeCell.NodePaddingHeight;
            var nodeWidth = LatticeCell.NodeWidth;
            //if (Lattice.RectWithin(cell.RealRect, out var r)) { LastDrawnCells.Add(startLoc); }
            //
            // 竖线1
            //
            var x = cell.NodeRealLeft + nodeWidth / 2;
            var y1 = cell.RealTop + paddingHeight;
            var halfHeightDiff = heightDiff / 2;
            cell.LatticedTop -= halfHeightDiff;
            var y2 = cell.RealTop + paddingHeight / 2;
            DrawLine(image, new(x, y1), new(x, y2), pen, false);
            ///
            /// no test yet
            /// 
            for (int i = 1; i < halfHeightDiff; i++)
            {
                LatticeCell drawnCell = new(startLoc.X, startLoc.Y - i);
                if (Lattice.RectWithin(drawnCell.RealRect)) { LastDrawnCells.Add(drawnCell.LatticedPoint); }
            }
            //
            // 横线
            //
            var cellY = startLoc.Y - halfHeightDiff;
            if (Math.Abs(widthDiff) > 0)
            {
                cell.LatticedLeft += widthDiff;
                var x2 = cell.NodeRealLeft + nodeWidth / 2;
                DrawLine(image, new(x, y2), new(x2, y2), pen, true);
                ///
                for (int i = 1; i <= Math.Abs(widthDiff); i++)
                {
                    LatticeCell drawnCell = new(startLoc.X + (widthDiff < 0 ? -i : i), cellY);
                    if (Lattice.RectWithin(drawnCell.RealRect)) { LastDrawnCells.Add(drawnCell.LatticedPoint); }
                }
            }
            //
            // 竖线2
            //
            y1 = y2;
            var leaveHeight = heightDiff - halfHeightDiff - 1;
            cell.LatticedTop -= leaveHeight;
            y2 = cell.RealTop;
            x = cell.NodeRealLeft + nodeWidth / 2;
            DrawLine(image, new(x, y1), new(x, y2), pen, false);
            ///
            /// no test yet
            ///
            var cellX = startLoc.X + widthDiff;
            for (int i = 0; i < leaveHeight; i++)
            {
                LatticeCell drawnCell = new(cellX, cellY - i);
                if (Lattice.RectWithin(drawnCell.RealRect)) { LastDrawnCells.Add(drawnCell.LatticedPoint); }
            }
        }
        /// <summary>
        /// 绘制竖线
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pen"></param>
        private static void DrawLine(Bitmap image, Point p1, Point p2, Pen pen, bool horizon)
        {
            if (!ShowBackground)
            {
                (Point, Point) line = new();
                if (p1.X == p2.X) 
                { 
                    if (!Lattice.LineWithin(p1.X, (p1.Y, p2.Y), pen.Width, out line)) { return; }
                }
                else 
                {
                    if (Lattice.LineWithin((p1.X, p2.X), p1.Y, pen.Width, out line)) { return; } 
                }
                var g = Graphics.FromImage(image);
                g.DrawLine(pen, line.Item1, line.Item2);
                g.Flush(); g.Dispose();
                return;
            }
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
            if (!Lattice.RectWithin(lineRect, out var rect)) { return; }
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pInverseCacher = new(BkInverseCacher);
            pInverseCacher.LockBits();
            if (horizon)
            {
                // top
                for (int i = 0; i < rect.Width; i++)
                {
                    var left = rect.Left + i;
                    var top = rect.Top;
                    var pixel = pInverseCacher.GetPixel(left, top);
                    pImage.SetPixel(left, top, pixel);
                }
                // bottom
                var bottom = rect.Bottom;
                if (rect.Top < bottom)
                {
                    for (int i = 0; i < rect.Width; i++)
                    {
                        var left = rect.Left + i;
                        var pixel = pInverseCacher.GetPixel(left, bottom);
                        pImage.SetPixel(left, bottom, pixel);
                    }
                }
            }
            // left
            for (int j = 0; j < rect.Height; j++)
            {
                var left = rect.Left;
                var top = rect.Top + j;
                var pixel = pInverseCacher.GetPixel(left, top);
                pImage.SetPixel(left, top, pixel);
            }
            // right
            var right = rect.Right;
            if (rect.Left < right)
            {
                for (int j = 0; j < rect.Height; j++)
                {
                    var top = rect.Top + j;
                    var pixel = pInverseCacher.GetPixel(right, top);
                    pImage.SetPixel(right, top, pixel);
                }
            }
            pInverseCacher.UnlockBits();
            pImage.UnlockBits();
        }
        ///// <summary>
        ///// 绘制横线
        ///// </summary>
        ///// <param name="image"></param>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <param name="pen"></param>
        //private static void DrawLine(Bitmap image, (int, int) x, int y, Pen pen)
        //{
        //    if (!ShowBackground)
        //    {
        //        if (Lattice.LineWithin(x, y, pen.Width, out var line))
        //        {
        //            var g = Graphics.FromImage(image);
        //            g.DrawLine(pen, line.Item1, line.Item2);
        //            g.Flush(); g.Dispose();
        //        }
        //        return;
        //    }
        //    Rectangle lineRect = new(Math.Min(x.Item1, x.Item2), y - NodeBorderWidth / 2, Math.Abs(x.Item1 - x.Item2) + 2, NodeBorderWidth);
        //    if (!Lattice.RectWithin(lineRect, out var rect)) { return; }
        //    PointBitmap pImage = new(image);
        //    pImage.LockBits();
        //    PointBitmap pInverseCacher = new(BkInverseCacher);
        //    pInverseCacher.LockBits();
        //    for (int i = 0; i < rect.Width; i++)
        //    {
        //        var left = rect.Left + i;
        //        var top = rect.Top;
        //        var pixel = pInverseCacher.GetPixel(left, top);
        //        pImage.SetPixel(left, top, pixel);
        //    }
        //    for (int i = 0; i < rect.Width; i++)
        //    {
        //        var left = rect.Left + i;
        //        var bottom = rect.Bottom;
        //        var pixel = pInverseCacher.GetPixel(left, bottom);
        //        pImage.SetPixel(left, bottom, pixel);
        //    }
        //    pInverseCacher.UnlockBits();
        //    pImage.UnlockBits();
        //}
        public static void RedrawLastDrawnCells(Image image)
        {
            Graphics g = Graphics.FromImage(image);
            if (!ShowBackground)
            {
                foreach (var point in LastDrawnCells)
                {
                    var left = Lattice.LastCellWidth * point.X + Lattice.LastOriginLeft;
                    var top = Lattice.LastCellHeight * point.Y + Lattice.LastOriginTop ;
                    Rectangle rect = new(left, top, Lattice.LastCellWidth, Lattice.LastCellHeight);
                    if (!Lattice.RectWithin(rect, out rect)) { return; }

                    g.FillRectangle(new SolidBrush(Color.White), rect);
                    LastDrawnCells.Remove(point);
                }
            }
            else
            {
                foreach (var point in LastDrawnCells)
                {
                    var left = Lattice.LastCellWidth * point.X + Lattice.LastOriginLeft;
                    var top = Lattice.LastCellHeight * point.Y + Lattice.LastOriginTop;
                    Rectangle rect = new(left, top, Lattice.LastCellWidth, Lattice.LastCellHeight);
                    //var rect = cell.RealRect;
                    //rect.X -= Lattice.OriginLeft - Lattice.LastOriginLeft;
                    //rect.Y -= Lattice.OriginTop - Lattice.LastOriginTop;
                    if (RectWithin(rect, out rect))
                    {
                        g.DrawImage(BkCacher, rect, rect, GraphicsUnit.Pixel);
                    }
                    LastDrawnCells.Remove(point);
                }
            }
            g.Flush(); g.Dispose();
        }
        public static bool RectWithin(Rectangle rect, out Rectangle saveRect)
        {
            saveRect = Rectangle.Empty;
            var left = rect.Left;
            var right = rect.Right;
            var top = rect.Top;
            var bottom = rect.Bottom;
            var width = rect.Width;
            var height = rect.Height;
            var dr = Lattice.LastDrawRect;
            if (left < dr.Left)
            {
                if (right <= dr.Left) { return false; }
                left = dr.Left;
            }
            if (right > dr.Right)
            {
                if (left >= dr.Right) { return false; }
                right = dr.Right;
            }
            if (top < dr.Top)
            {
                if (bottom <= dr.Top) { return false; }
                top = dr.Top;
            }
            if (bottom > dr.Bottom)
            {
                if (top >= dr.Bottom) { return false; }
                bottom = dr.Bottom;
            }
            saveRect = new(left, top, right - left, bottom - top);
            //if (saveRect.Height <= 0 || saveRect.Width <= 0) 
            //{ return false; }
            return true;
        }
    }
}
