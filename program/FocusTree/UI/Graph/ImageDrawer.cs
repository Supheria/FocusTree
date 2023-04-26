using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.UI.Graph
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
        public void DrawRect1()
        {
            Graphics g = Graphics.FromImage(Target);
            Rectangle rect = new(AllRect.Left, AllRect.Top, AllRect.Width / 2, AllRect.Height);
            g.DrawImage(Source, rect, rect, GraphicsUnit.Pixel);
            //PointBitmap.FillRectWithSource(Target, Source, rect);
        }
        public void DrawRect2()
        {
            Graphics g = Graphics.FromImage(Target);
            var halfWidth = AllRect.Width / 2;
            Rectangle rect = new(AllRect.Left + halfWidth + 1, AllRect.Top, halfWidth, AllRect.Height);
            g.DrawImage(Source, rect, rect, GraphicsUnit.Pixel);
            //PointBitmap.FillRectWithSource(Target, Source, rect);
        }
    }
}
