namespace FocusTree.Graph
{
    public class ImageDrawer
    {
        Bitmap Target;
        Bitmap Source;
        Rectangle AllRect;
        public ImageDrawer(Bitmap target, Bitmap source, Rectangle rect)
        {
            Target = target;
            Source = source;
            AllRect = rect;
        }
        public void Bounds1()
        {
            Graphics g = Graphics.FromImage(Target);
            Rectangle rect = new(AllRect.Left, AllRect.Top, AllRect.Width / 2, AllRect.Height);
            g.DrawImage(Source, rect, rect, GraphicsUnit.Pixel);
            //PointBitmap.FillRectWithSource(Target, Source, rect);
        }
        public void Bounds2()
        {
            Graphics g = Graphics.FromImage(Target);
            var halfWidth = AllRect.Width / 2;
            Rectangle rect = new(AllRect.Left + halfWidth + 1, AllRect.Top, halfWidth, AllRect.Height);
            g.DrawImage(Source, rect, rect, GraphicsUnit.Pixel);
            //PointBitmap.FillRectWithSource(Target, Source, rect);
        }
    }
}
