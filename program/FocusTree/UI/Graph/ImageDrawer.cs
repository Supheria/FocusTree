using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FocusTree.UI.Graph
{
    public class ImageDrawer
    {
        /// <summary>
        /// 主 Bitmap
        /// </summary>
        Bitmap Canvas;
        /// <summary>
        /// 等待队列
        /// </summary>
        List<(Color, Point)> Waiting;
        /// <summary>
        /// 画图线程
        /// </summary>
        Task ThreadDraw;
        /// <summary>
        /// 主控线程
        /// </summary>
        Task ThreadMain;
        /// <summary>
        /// 已销毁
        /// </summary>
        bool Disposed = false;
        /// <summary>
        /// 当前正在画的颜色
        /// </summary>
        Color Pixel;
        /// <summary>
        /// 当前正在画的位置
        /// </summary>
        Point SetPos;
        public ImageDrawer()
        {
            Canvas = null;
        }
        public ImageDrawer(int width, int height)
        {
            if (width == 0 || height == 0) 
            { 
                Canvas = null; 
            }
            else
            {
                Canvas = new(width, height);
                Waiting = new();
                ThreadMain = new(ControlQueue);
                ThreadMain.Start();
            }
        }
        public void SetPixel(int x, int y, Color pixel)
        {
            if (Canvas == null) { return; }
            if (ThreadDraw != null && ThreadDraw.Status != TaskStatus.RanToCompletion)
            {
                Waiting.Add((pixel, new(x, y)));
                return;
            }
            Pixel = pixel;
            SetPos = new(x, y);
            ThreadDraw = new(Draw);
            ThreadDraw.Start();
        }
        public void ControlQueue()
        {
            while (!Disposed)
            {
                if (Waiting.Count == 0 || ThreadDraw.IsCompleted)
                {
                    ThreadMain.Wait(2);
                    continue;
                } 
                while (ThreadDraw.Status == TaskStatus.RanToCompletion) { ThreadMain.Wait(2); }
                //ThreadDraw?.Dispose();
                var pair = Waiting[0];
                Pixel = pair.Item1;
                SetPos = pair.Item2;
                ThreadDraw = new(Draw);
                ThreadDraw.Start();
                Waiting.Remove(pair);

            }
        }
        private void Draw()
        {
            //PointBitmap pCanvas = new(Canvas);
            //pCanvas.LockBits();
            Canvas.SetPixel(SetPos.X, SetPos.Y, Pixel);
            //pCanvas.UnlockBits();
        }
        public void Dispose()
        {
            if (Canvas == null) { return; }
            Disposed = true;
            //Canvas.Dispose();
            //ThreadMain.Dispose();
            //ThreadDraw.Dispose();
        }
        public Bitmap GetBitmap()
        {
            if (Canvas == null || Disposed) { throw new Exception(); }
            return Canvas;
        }
        public bool Finished()
        {
            if (ThreadMain == null) { return true; }
            return ThreadMain.Status == TaskStatus.RanToCompletion;
        }
    }
}
