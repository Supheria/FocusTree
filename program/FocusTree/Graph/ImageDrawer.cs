namespace FocusTree.Graph
{
    public static class ImageDrawer
    {
        public static void DrawImageOn(Bitmap source, Bitmap target, Rectangle toRect, bool ignoreTransparent)
        {
            PointBitmap pSource = new(source);
            PointBitmap pTarget = new(target);
            pSource.LockBits();
            pTarget.LockBits();
            var right = toRect.Right;
            var bottom = toRect.Bottom;
            for (int x = toRect.X; x < right; x++)
            {
                for (int y = toRect.Y; y < bottom; y++)
                {
                    var pixel = pSource.GetPixel(x, y);
                    if (!ignoreTransparent || pixel.A != 0 || pixel.R != 0 || pixel.G != 0 || pixel.B != 0)
                    {
                        pTarget.SetPixel(x, y, pixel);
                    }
                }
            }
            pSource.UnlockBits();
            pTarget.UnlockBits();
        }
    }
}
