#define PointBmp
using FocusTree.Data.Focus;

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

        #region ==== 格元 ====

        /// <summary>
        /// 格元各个部分填充色
        /// </summary>
        public static Dictionary<LatticeCell.Parts, Color> CellPartsBG = new()
        {
            [LatticeCell.Parts.Node] = Color.FromArgb(255, Color.Orange),
            [LatticeCell.Parts.Left] = Color.FromArgb(255, Color.Gray),
            [LatticeCell.Parts.Top] = Color.FromArgb(255, Color.Gray),
            [LatticeCell.Parts.LeftTop] = Color.FromArgb(255, Color.Gray)
        };

        #endregion

        #region ==== 绘制背景和重绘 ====

        #endregion

        #region ==== 绘制国策节点 ====

        /// <summary>
        /// 绘制国策节点
        /// </summary>
        /// <param name="image"></param>
        /// <param name="focus"></param>
        /// <param name="fillColor">是否填充节点颜色</param>
        public static void DrawFocusNode(Image image, FocusData focus, bool fillColor)
        {
            if (fillColor)
            {
                DrawFocusNode((Bitmap)image, focus, CellPartsBG[LatticeCell.Parts.Node]);
            }
            else { DrawFocusNode((Bitmap)image, focus); }
        }
        /// <summary>
        /// 绘制无填充色的国策节点（上载绘制委托默认）
        /// </summary>
        /// <param name="image"></param>
        /// <param name="focus"></param>
        private static void DrawFocusNode(Bitmap image, FocusData focus)
        {
            LatticeCell cell = new(focus.LatticedPoint);
            var nodeRect = cell.CellPartsRealRect[LatticeCell.Parts.Node];
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
        private static void DrawFocusNode(Bitmap image, FocusData focus, Color fillColor)
        {
            LatticeCell cell = new(focus.LatticedPoint);
            var nodeRect = cell.CellPartsRealRect[LatticeCell.Parts.Node];
            if (!Lattice.RectWithin(nodeRect, out nodeRect)) { return; }
            var cellRect = cell.RealRect;
            if (cellRect.Width < LatticeCell.SizeMax.Width / 2 || cellRect.Height < LatticeCell.SizeMax.Height / 2)
            {
                DrawBlankNode(image, nodeRect, fillColor);
            }
            else { DrawStringNode(image, nodeRect, focus.Name, fillColor); }
        }

        #endregion

        #region ==== 绘制格元部分 ====

        /// <summary>
        /// 绘制指定的格元部分（ LatticeCell.Parts.Leave 不绘制）
        /// </summary>
        /// <param name="image"></param>
        /// <param name="point"></param>
        /// <param name="cellPart"></param>
        /// <param name="fillColor"></param>
        public static void DrawCellPart(Bitmap image, LatticedPoint point, LatticeCell.Parts cellPart)
        {
            if (cellPart == LatticeCell.Parts.Leave) { return; }
            LatticeCell cell = new(point);
            if (!Lattice.RectWithin(cell.CellPartsRealRect[cellPart], out var rect)) { return; }
            if (!CellPartsBG.TryGetValue(cellPart, out var fillColor)) { return; }
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pCache = new(Background.BackImage);
            pCache.LockBits();
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j < rect.Height; j++)
                {
                    var x = rect.Left + i;
                    var y = rect.Top + j;
                    var bkPixel = pCache.GetPixel(x, y);
                    pImage.SetPixel(x, y, GetMixedColor(bkPixel, fillColor));
                }
            }
            pCache.UnlockBits();
            pImage.UnlockBits();
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
            PointBitmap pCache = new(Background.BackImage);
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
            PointBitmap pCache = new(Background.BackImage);
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
                    else { pImage.SetPixel(x, y, GetMixedColor(bkPixel, fillColor)); }
                }
            }
            pCache.UnlockBits();
            pImage.UnlockBits();
        }
        /// <summary>
        /// 绘制有文字节点，无填充色
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rect"></param>
        /// <param name="white"></param>
        /// <param name="black"></param>
        private static void DrawStringNode(Bitmap image, Rectangle nodeRect, string name)
        {
            Color stringColor = GetNodeNamePattern(image, nodeRect, name) ? NodeFGDark : NodeFGBright;
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pCache = new(Background.BackImage);
            pCache.LockBits();
            for (int i = 0; i < nodeRect.Width; i++)
            {
                for (int j = 0; j < nodeRect.Height; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var bkPixel = pCache.GetPixel(x, y);
                    var pixel = pImage.GetPixel(x, y);
                    // string part
                    if (pixel.R != 255/* || pixel.G != 255 || pixel.B != 255*/)
                    {
                        pImage.SetPixel(x, y, stringColor);
                        continue;
                    }
                    // shading part
                    if (i <= NodeBorderWidth || i >= nodeRect.Width - NodeBorderWidth || j <= NodeBorderWidth || j >= nodeRect.Height - NodeBorderWidth)
                    {
                        pImage.SetPixel(x, y, GetInverseColor(bkPixel));
                    }
                    else { pImage.SetPixel(x, y, bkPixel); }
                }
            }
            pCache.UnlockBits();
            pImage.UnlockBits();
        }
        /// <summary>
        /// 绘制无文字节点，有填充色
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rect"></param>
        /// <param name="white"></param>
        /// <param name="black"></param>
        /// <param name="fillColor"></param>
        private static void DrawStringNode(Bitmap image, Rectangle nodeRect, string name, Color fillColor)
        {
            Color stringColor = GetNodeNamePattern(image, nodeRect, name) ? NodeFGDark : NodeFGBright;
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pCache = new(Background.BackImage);
            pCache.LockBits();
            for (int i = 0; i < nodeRect.Width; i++)
            {
                for (int j = 0; j < nodeRect.Height; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var bkPixel = pCache.GetPixel(x, y);
                    var pixel = pImage.GetPixel(x, y);
                    // string part
                    if (pixel.R != 255/* || pixel.G != 255 || pixel.B != 255*/)
                    {
                        pImage.SetPixel(x, y, stringColor);
                        continue;
                    }
                    // shading part
                    if (i <= NodeBorderWidth || i >= nodeRect.Width - NodeBorderWidth || j <= NodeBorderWidth || j >= nodeRect.Height - NodeBorderWidth)
                    {
                        pImage.SetPixel(x, y, GetInverseColor(bkPixel));
                    }
                    else { pImage.SetPixel(x, y, GetMixedColor(bkPixel, fillColor)); }
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
                    var remain = name.Length - start;
                    sName += $"{name.Substring(start, fontWidth > remain ? remain : fontWidth)}\n";
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
        /// <summary>
        /// 混合二色
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <returns></returns>
        private static Color GetMixedColor(Color color1, Color color2)
        {
            var A = (color1.A + color2.A) / 2;
            var R = (color1.R + color2.R) / 2;
            var G = (color1.G + color2.G) / 2;
            var B = (color1.B + color2.B) / 2;
            return Color.FromArgb(A, R, G, B);
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
            var rowDiff = endLoc.Row - startLoc.Row;
            LatticeCell cell = new(startLoc);
            var halfPaddingHeight = LatticeCell.NodePaddingHeight / 2;
            var halfPaddingWidth = LatticeCell.NodePaddingWidth / 2;
            var halfNodeWidth = LatticeCell.NodeWidth / 2;

            var x1 = cell.NodeRealLeft + halfNodeWidth;
            var y1 = cell.NodeRealTop;
            var y2 = cell.RealTop + halfPaddingHeight;
            DrawCrossLine(image, new(x1, y1), new(x1, y2));
            if (rowDiff < 0)
            {
                cell.LatticedLeft += colDiff;
                var x2 = cell.NodeRealLeft + halfNodeWidth;
                if (Math.Abs(colDiff) > 0)
                {
                    DrawCrossLine(image, new(x1, y2), new(x2, y2));
                }
                cell.LatticedTop += rowDiff + 1;
                DrawCrossLine(image, new(x2, y2), new(x2, cell.RealTop));
            }
            else
            {
                cell.LatticedLeft += colDiff < 0 ? colDiff + 1 : colDiff;
                var x2 = cell.RealLeft + halfPaddingWidth;
                DrawCrossLine(image, new(x1, y2), new(x2, y2));
                cell.LatticedTop += rowDiff + 1;
                var y3 = cell.RealTop + halfPaddingHeight;
                DrawCrossLine(image, new(x2, y2), new(x2, y3));
                var x3 = x2 + (colDiff < 0 ? -(halfPaddingWidth +  halfNodeWidth) : halfPaddingWidth + halfNodeWidth);
                DrawCrossLine(image, new(x2, y3), new(x3, y3));
                DrawCrossLine(image, new(x3, y3), new(x3, y3 - halfPaddingHeight));
            }
        }

        #endregion

        #region ==== 绘制线方法 ====

        /// <summary>
        /// 绘制横纵空心直线
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pen"></param>
        private static void DrawCrossLine(Bitmap image, Point p1, Point p2)
        {
            if (!Lattice.CrossLineWithin(p1, p2, NodeLineWidth, out var lineRect, out var isHorizon)) { return; }
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pCache = new(Background.BackImage);
            pCache.LockBits();
            if (isHorizon)
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
            else
            {
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
            }
            pCache.UnlockBits();
            pImage.UnlockBits();
        }

        #endregion
    }
}
