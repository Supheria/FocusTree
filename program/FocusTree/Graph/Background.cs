﻿using FocusTree.Properties;
using System.Drawing.Drawing2D;

namespace FocusTree.Graph
{
    public static class Background
    {
        public static string BackImagePath { get; set; } = "Background.jpg";
        /// <summary>
        /// 当前的背景图片
        /// </summary>
        static Bitmap Image;
        /// <summary>
        /// 应该仅当使用 PointBitmap 时才调用此成员，不应对此有任何直接赋值的操作
        /// </summary>
        public static Bitmap BackImage { get => Image ?? InitializeImage(); }
        /// <summary>
        /// 背景图片的大小
        /// </summary>
        public static Size Size { get => BackImage.Size; }
        /// <summary>
        /// 无图片背景
        /// </summary>
        public static Color BlankBackground { get; set; } = Color.WhiteSmoke;
        /// <summary>
        /// 是否显示背景图片
        /// </summary>
        public static bool Show { get; set; } = true;
        /// <summary>
        /// 新键背景缓存，并重绘背景
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rect"></param>
        public static void DrawNew(Image image)
        {
            SetImage(image.Size);
            Redraw(image);
        }
        /// <summary>
        /// 重绘背景（首次重绘应该使用 DrawNew）
        /// </summary>
        /// <param name="image"></param>
        public static void Redraw(Image image)
        {
            Graphics g = Graphics.FromImage(image);
            g.CompositingMode = CompositingMode.SourceCopy;
            g.CompositingQuality = CompositingQuality.Invalid;
            g.DrawImage(Image, 0, 0);
            g.Flush(); g.Dispose();
        }
        /// <summary>
        /// 初始化图片
        /// </summary>
        /// <returns></returns>
        private static Bitmap InitializeImage()
        {
            if (File.Exists(BackImagePath))
            {
                Image = (Bitmap)System.Drawing.Image.FromFile(BackImagePath);
            }
            else { Image = Resources.BackImage; }
            return Image;
        }
        /// <summary>
        /// 根据图源重设背景图片为指定大小
        /// </summary>
        /// <param name="size"></param>
        private static void SetImage(Size size)
        {
            if (Show)
            {
                InitializeFromSourceImage(size);
                return;
            }
            var Width = size.Width;
            var Height = size.Height;
            Image?.Dispose();
            Image = new Bitmap(Width, Height);
            PointBitmap pCache = new(Image);
            pCache.LockBits();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    pCache.SetPixel(i, j, BlankBackground);
                }
            }
            pCache.UnlockBits();
        }
        /// <summary>
        /// 从图源设置背景
        /// </summary>
        /// <param name="size"></param>
        private static void InitializeFromSourceImage(Size size)
        {
            Bitmap sourceImage;
            if (File.Exists(BackImagePath))
            {
                sourceImage = (Bitmap)System.Drawing.Image.FromFile(BackImagePath);
            }
            else { sourceImage = Resources.BackImage; }
            var Width = size.Width;
            var Height = size.Height;
            var bkWidth = Width;
            var bkHeight = Height;
            double sourceRatio = (double)sourceImage.Width / (double)sourceImage.Height;
            double clientRatio = (double)Width / (double)Height;
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
            if (Image.Width != bkWidth || Image.Height != bkHeight)
            {
                Image?.Dispose();
                Image = new(bkWidth, bkHeight);
                var g = Graphics.FromImage(Image);
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.Invalid;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(sourceImage, 0, 0, bkWidth, bkHeight);
                g.Flush(); g.Dispose();
                sourceImage.Dispose();
            }
        }
    }
}