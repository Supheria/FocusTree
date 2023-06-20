#define PointBmp
using FocusTree.Data.Focus;

namespace FocusTree.Graph
{
    /// <summary>
    /// 国策树绘图工具
    /// </summary>
    public static class GraphDrawer
    {
        #region ==== 字体、边框和颜色 ====

        /// <summary>
        /// 节点字体
        /// </summary>
        public static string NodeFont { get; set; } = "黑体";
        /// <summary>
        /// 展示信息字体
        /// </summary>
        public static string InfoFont { get; set; } = "仿宋";
        /// <summary>
        /// 节点字体样式
        /// </summary>
        public static StringFormat NodeFontFormat { get; set; } = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        /// <summary>
        /// 国策节点边框宽度
        /// </summary>
        public static int FocusNodeBorderWidth { get; set; } = 2;
        /// <summary>
        /// 关系线宽度
        /// </summary>
        public static int RequireLineWidth { get; set; } = 2;
        /// <summary>
        /// 国策节点文字颜色
        /// </summary>
        public static Color FocusNodeFG { get; set; } = Color.Black;
        /// <summary>
        /// 国策节点底色-普遍
        /// </summary>
        public static Color FocusNodeBG_Normal { get; set; } = Color.White;
        /// <summary>
        /// 国策节点底色-选中
        /// </summary>
        public static Color FocusNodeBG_Selected { get; set; } = Color.Orange;
        /// <summary>
        /// 非国策格元各个部分选中时的底色
        /// </summary>
        public static Dictionary<LatticeCell.Parts, Color> CellSelectedPartsBG = new()
        {
            [LatticeCell.Parts.Node] = Color.Orange,
            [LatticeCell.Parts.Left] = Color.Gray,
            [LatticeCell.Parts.Top] = Color.Gray,
            [LatticeCell.Parts.LeftTop] = Color.Gray
        };

        #endregion

        /// <summary>
        /// 显示节点名称的最小格元边长
        /// </summary>
        public static int ShowNodeNameCellLength { get; set; } = 65;

        #region ==== 绘制国策节 ====

        /// <summary>
        /// 绘制普通的国策节点
        /// </summary>
        /// <param name="image"></param>
        /// <param name="focus"></param>
        public static void DrawFocusNodeNormal(Bitmap image, FocusData focus)
        {
            LatticeCell cell = new(focus.LatticedPoint);
            var nodeRect = cell.NodeRealRect;
            if (!Lattice.RectWithin(nodeRect, out nodeRect)) { return; }
            if (LatticeCell.Length < ShowNodeNameCellLength)
            {
                DrawFocusNode(image, nodeRect, null);
            }
            else { DrawFocusNode(image, nodeRect, FocusNodeBG_Normal, focus.Name); }
        }
        /// <summary>
        /// 绘制选中的国策节点
        /// </summary>
        /// <param name="image"></param>
        /// <param name="focus"></param>
        public static void DrawFocusNodeSelected(Bitmap image, FocusData focus)
        {
            LatticeCell cell = new(focus.LatticedPoint);
            var nodeRect = cell.NodeRealRect;
            if (!Lattice.RectWithin(nodeRect, out nodeRect)) { return; }
            if (LatticeCell.Length < ShowNodeNameCellLength)
            {
                DrawFocusNode(image, nodeRect, FocusNodeBG_Selected);
            }
            else { DrawFocusNode(image, nodeRect, FocusNodeBG_Selected, focus.Name); }
        }
        /// <summary>
        /// 绘制无文字国策节点
        /// </summary>
        /// <param name="image"></param>
        /// <param name="nodeRect"></param>
        private static void DrawFocusNode(Bitmap image, Rectangle nodeRect, Color? shading)
        {
            if (shading.HasValue)
            {
                DrawFocusNodeShadingAndBorder(image, nodeRect, shading.Value);
            }
            else { DrawFocusNodeBorderOnly(image, nodeRect); }
        }
        /// <summary>
        /// 绘制有文字国策节点
        /// </summary>
        /// <param name="image"></param>
        /// <param name="nodeRect"></param>
        /// <param name="shading"></param>
        /// <param name="focusName"></param>
        private static void DrawFocusNode(Bitmap image, Rectangle nodeRect, Color shading, string focusName)
        {
            DrawFocusNodeShadingAndBorder(image, nodeRect, shading);

            var fontHeight = focusName.Length / 3;
            if (focusName.Length % 3 != 0) { fontHeight++; }
            if (fontHeight == 0) { fontHeight++; }
            var fontWidth = focusName.Length / fontHeight;
            if (focusName.Length % 3 != 0) { fontWidth++; }
            var fontSizeH = 0.7f * nodeRect.Height / fontHeight;
            var fontSizeW = 0.7f * nodeRect.Width / fontWidth;
            var fontSize = Math.Min(fontSizeH, fontSizeW);
            string sName = focusName;
            if (fontHeight > 1)
            {
                sName = string.Empty;
                for (int i = 0; i < fontHeight; i++)
                {
                    var start = i * fontWidth;
                    var remain = focusName.Length - start;
                    sName += $"{focusName.Substring(start, fontWidth > remain ? remain : fontWidth)}\n";
                }
            }
            var font = new Font(NodeFont, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var g = Graphics.FromImage(image);
            g.DrawString(sName, font, new SolidBrush(FocusNodeFG), nodeRect, NodeFontFormat);
            g.Flush(); g.Dispose();
        }
        /// <summary>
        /// 仅绘制国策节点的边框
        /// </summary>
        /// <param name="image"></param>
        /// <param name="nodeRect"></param>
        /// <param name="shading"></param>
        public static void DrawFocusNodeBorderOnly(Bitmap image, Rectangle nodeRect)
        {
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pBack = new(Background.BackImage);
            pBack.LockBits();
            // left & right
            var widthDistance = nodeRect.Width - FocusNodeBorderWidth;
            for (int i = 0; i < FocusNodeBorderWidth; i++)
            {
                for (int j = 0; j < nodeRect.Height; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    pImage.SetPixel(x, y, GetInverseColor(pBack.GetPixel(x, y)));
                    x += widthDistance;
                    pImage.SetPixel(x, y, GetInverseColor(pBack.GetPixel(x, y)));
                }
            }
            // top & bottom
            var heightDistance = nodeRect.Height - FocusNodeBorderWidth;
            for (int i = FocusNodeBorderWidth; i < nodeRect.Width - FocusNodeBorderWidth; i++)
            {
                for (int j = 0; j < FocusNodeBorderWidth; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    pImage.SetPixel(x, y, GetInverseColor(pBack.GetPixel(x, y)));
                    y += heightDistance;
                    pImage.SetPixel(x, y, GetInverseColor(pBack.GetPixel(x, y)));
                }
            }
            pBack.UnlockBits();
            pImage.UnlockBits();
        }
        /// <summary>
        /// 绘制国策节点的底纹和边框
        /// </summary>
        /// <param name="image"></param>
        /// <param name="nodeRect"></param>
        /// <param name="shading"></param>
        public static void DrawFocusNodeShadingAndBorder(Bitmap image, Rectangle nodeRect, Color shading)
        {
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pBack = new(Background.BackImage);
            pBack.LockBits();
            for (int i = 0; i < nodeRect.Width; i++)
            {
                for (int j = 0; j < nodeRect.Height; j++)
                {
                    var x = nodeRect.Left + i;
                    var y = nodeRect.Top + j;
                    var bkPixel = pBack.GetPixel(x, y);
                    if (i <= FocusNodeBorderWidth || i >= nodeRect.Width - FocusNodeBorderWidth || j <= FocusNodeBorderWidth || j >= nodeRect.Height - FocusNodeBorderWidth)
                    {
                        pImage.SetPixel(x, y, GetInverseColor(bkPixel));
                    }
                    else { pImage.SetPixel(x, y, GetMixedColor(bkPixel, shading)); }
                }
            }
            pBack.UnlockBits();
            pImage.UnlockBits();
        }

        #endregion

        #region ==== 绘制关系线 ====

        /// <summary>
        /// 绘制关系线
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
            DrawCrossHollowLine(image, new(x1, y1), new(x1, y2));
            if (rowDiff < 0)
            {
                cell.LatticedLeft += colDiff;
                var x2 = cell.NodeRealLeft + halfNodeWidth;
                if (Math.Abs(colDiff) > 0)
                {
                    DrawCrossHollowLine(image, new(x1, y2), new(x2, y2));
                }
                cell.LatticedTop += rowDiff + 1;
                DrawCrossHollowLine(image, new(x2, y2), new(x2, cell.RealTop));
            }
            else
            {
                cell.LatticedLeft += colDiff < 0 ? colDiff + 1 : colDiff;
                var x2 = cell.RealLeft + halfPaddingWidth;
                DrawCrossHollowLine(image, new(x1, y2), new(x2, y2));
                cell.LatticedTop += rowDiff + 1;
                var y3 = cell.RealTop + halfPaddingHeight;
                DrawCrossHollowLine(image, new(x2, y2), new(x2, y3));
                var x3 = x2 + (colDiff < 0 ? -(halfPaddingWidth + halfNodeWidth) : halfPaddingWidth + halfNodeWidth);
                DrawCrossHollowLine(image, new(x2, y3), new(x3, y3));
                DrawCrossHollowLine(image, new(x3, y3), new(x3, y3 - halfPaddingHeight));
            }
        }
        /// <summary>
        /// 绘制横纵空心直线
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pen"></param>
        private static void DrawCrossHollowLine(Bitmap image, Point p1, Point p2)
        {
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pBack = new(Background.BackImage);
            pBack.LockBits();
            var halfLineWidth = RequireLineWidth / 2;
            if (p1.Y == p2.Y)
            {
                Rectangle lineRect = new(Math.Min(p1.X, p2.X), p1.Y - halfLineWidth, Math.Abs(p1.X - p2.X) + halfLineWidth, RequireLineWidth);
                if (Lattice.RectWithin(lineRect, out lineRect))
                {
                    // top & bottom
                    var bottom = lineRect.Bottom;
                    for (int i = 0; i < lineRect.Width; i++)
                    {
                        var x = lineRect.Left + i;
                        pImage.SetPixel(x, lineRect.Top, GetInverseColor(pBack.GetPixel(x, lineRect.Top)));
                        pImage.SetPixel(x, bottom, GetInverseColor(pBack.GetPixel(x, bottom)));
                    }
                }
            }
            else
            {
                Rectangle lineRect = new(p1.X - halfLineWidth, Math.Min(p1.Y, p2.Y), RequireLineWidth, Math.Abs(p1.Y - p2.Y) + halfLineWidth);
                if (Lattice.RectWithin(lineRect, out lineRect))
                {
                    // left & right
                    var right = lineRect.Right;
                    for (int j = 0; j < lineRect.Height; j++)
                    {
                        var y = lineRect.Top + j;
                        pImage.SetPixel(lineRect.Left, y, GetInverseColor(pBack.GetPixel(lineRect.Left, y)));
                        pImage.SetPixel(right, y, GetInverseColor(pBack.GetPixel(right, y)));
                    }
                }
            }
            pBack.UnlockBits();
            pImage.UnlockBits();
        }

        #endregion

        #region ==== 绘制非国策格元 ====

        /// <summary>
        /// 绘制指定的格元部分
        /// </summary>
        /// <param name="image"></param>
        /// <param name="point"></param>
        /// <param name="cellPart"></param>
        public static void DrawSelectedCellPart(Bitmap image, LatticedPoint point, LatticeCell.Parts cellPart)
        {
            LatticeCell cell = new(point);
            if (!CellSelectedPartsBG.TryGetValue(cellPart, out var shading)) { return; }
            if (!Lattice.RectWithin(cell.CellPartsRealRect[cellPart], out var rect)) { return; }
            PointBitmap pImage = new(image);
            pImage.LockBits();
            PointBitmap pBack = new(Background.BackImage);
            pBack.LockBits();
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j < rect.Height; j++)
                {
                    var x = rect.Left + i;
                    var y = rect.Top + j;
                    pImage.SetPixel(x, y, GetMixedColor(pBack.GetPixel(x, y), shading));
                }
            }
            pBack.UnlockBits();
            pImage.UnlockBits();
        }

        #endregion

        #region ==== 颜色处理 ====

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
    }
}
