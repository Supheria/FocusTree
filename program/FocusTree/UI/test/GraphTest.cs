using FocusTree.UI.Graph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FocusTree.UI
{
    public partial class GraphTest : Form
    {
        Image ImageCacher;
        Rectangle GraphRect;
        bool MouseFlag;
        Point MousePointFlag;
        Graphics gCore;

        public GraphTest()
        {
            InitializeComponent();
            gBox.SizeChanged += GBox_SizeChanged;
            gBox.MouseDown += GBox_MouseClick;
            gBox.MouseUp += GBox_MouseUp;
            gBox.MouseMove += GBox_MouseMove;
        }

        private void GBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MouseFlag) { return; }
            var newPoint = e.Location;

            //LatticeCell.Width = 50;
            //LatticeCell.Height = 50;
            var diffWidth = MousePointFlag.X - newPoint.X;
            var diffHeight = MousePointFlag.Y - newPoint.Y;
            SetImage(diffWidth, diffHeight);
            Text = $"{gBox.Image.Width}, {gBox.Image.Height}";
        }

        private void GBox_MouseUp(object sender, MouseEventArgs e)
        {
            MouseFlag = false;
        }

        private void GBox_MouseClick(object sender, MouseEventArgs e)
        {
            MouseFlag = true;
        }

        private void GBox_SizeChanged(object sender, EventArgs e)
        {
            Rectangle cutRect = new(50, 50, gBox.Width, gBox.Height);
            SetImage(0, 0);
        }
        private void SetImage(int left, int top)
        {
            gBox.Image = new Bitmap(gBox.Width, gBox.Height);
            gCore = Graphics.FromImage(gBox.Image);
            //gCore.Clear(Color.White);
            var width = LatticeCell.Width * GraphRect.Width;
            var height = LatticeCell.Height * GraphRect.Height;
            gCore.DrawImage(ImageCacher, gBox.ClientRectangle, left, top, gBox.Width, gBox.Height, GraphicsUnit.Pixel);
            //var width = LatticeCell.Width * GraphRect.Width;
            //var height = LatticeCell.Height * GraphRect.Height;
            //gBox.Image = ImageCacher.GetThumbnailImage(width, height, null, IntPtr.Zero);
            gCore.Flush();
        }

        public void LoadImageCacher(Image cacher, Rectangle gRect)
        {
            ImageCacher = cacher;
            GraphRect = gRect;
            if (gBox.Width <= 0 || gBox.Height <= 0) return;
            var g = Graphics.FromImage(ImageCacher);
            Lattice.Draw(g);
        }
        
    }
}
