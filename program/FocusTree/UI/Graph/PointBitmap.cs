using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace FocusTree.UI.Graph
{
    /// <summary>
    /// 指针法 Bitmap 颜色赋值，来源 https://www.cnblogs.com/ybqjymy/p/12897892.html
    /// </summary>
    class PointBitmap
    {
        /// <summary>
        /// 指向的源图片
        /// </summary>
        readonly Bitmap Source = null;
        /// <summary>
        /// 指针首地址
        /// </summary>
        IntPtr Iptr = IntPtr.Zero;
        /// <summary>
        /// 数据存储结构
        /// </summary>
        BitmapData BmpData = null;
        /// <summary>
        /// 图片位深
        /// </summary>
        public int Depth { get; private set; }
        public PointBitmap(Bitmap source)
        {
            Source = source;
        }
        /// <summary>
        /// 根据源图片大小和位深设置并调用 Bitmap.LockBits（只做了对 32、24、8 位的处理）
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void LockBits()
        {
            Depth = Image.GetPixelFormatSize(Source.PixelFormat);
            Rectangle rect = new(0, 0, Source.Width, Source.Height);
            if (Depth == 32 || Depth == 24 || Depth == 8)
            {
                BmpData = Source.LockBits(rect, ImageLockMode.ReadWrite, Source.PixelFormat);
                Iptr = BmpData.Scan0;
                return;
            }
            throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
        }
        /// <summary>
        /// 调用 Bitmap.LockBits
        /// </summary>
        public void UnlockBits()
        {
            Source.UnlockBits(BmpData);
        }
        /// <summary>
        /// 返回指针指向的地址的数据（代换 Bitmap.GetPixel)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPixel(int x, int y)
        {
            unsafe
            {
                var ptr = (byte*)Iptr;
                ptr += y * BmpData.Stride + x * (Depth >> 3);
                if (Depth == 32)
                {
                    return Color.FromArgb(ptr[3], ptr[2], ptr[1], ptr[0]);
                }
                if (Depth == 24)
                {
                    return Color.FromArgb(ptr[2], ptr[1], ptr[0]);
                }
                else
                {
                    int r = ptr[0];
                    return Color.FromArgb(r, r, r);
                }
            }
        }
        /// <summary>
        /// 设置指针指向的地址的数据（代换 Bitmap.SetPixel）
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="c"></param>
        public void SetPixel(int x, int y, Color c)
        {
            unsafe
            {
                var ptr = (byte*)Iptr;
                ptr += y * BmpData.Stride + x * (Depth >> 3);
                if (Depth == 32)
                {
                    ptr[3] = c.A;
                    ptr[2] = c.R;
                    ptr[1] = c.G;
                    ptr[0] = c.B;
                }
                else if (Depth == 24)
                {
                    ptr[2] = c.R;
                    ptr[1] = c.G;
                    ptr[0] = c.B;
                }
                else if (Depth == 8)
                {
                    //ptr[2] = c.R;
                    //ptr[1] = c.G;
                    ptr[0] = c.B;
                }
            }
        }
    }
}
