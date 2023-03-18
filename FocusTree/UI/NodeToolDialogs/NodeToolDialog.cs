﻿using FocusTree.Tool.UI;
using FocusTree.UI.Controls;

namespace FocusTree.UI.NodeToolDialogs
{
    public partial class NodeToolDialog : Form
    {
        internal GraphBox Display;
        /// <summary>
        /// 初始宽高比
        /// </summary>
        internal float SizeRatio = 0.618f;
        internal float HeightMaxlRatio = 3f;
        internal NodeToolDialog()
        {
            ResizeEnd += NodeToolDialog_ResizeEnd;
            SizeChanged += NodeToolDialog_SizeChanged;
            FormClosing += NodeToolDialog_FormClosing;
        }
        public virtual new void Close() { }

        private void NodeToolDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Close();
            e.Cancel = true;
            Hide();
        }

        private void NodeToolDialog_ResizeEnd(object sender, EventArgs e)
        {
            if (Width < 0.4f * Height)
            {
                Width = (int)(Height * 0.4f);
            }
            if (Bottom > Screen.PrimaryScreen.Bounds.Bottom)
            {
                Top -= Bottom - Screen.PrimaryScreen.Bounds.Bottom;
            }
            if (Left < Screen.PrimaryScreen.Bounds.Left)
            {
                Left = Screen.PrimaryScreen.Bounds.Left;
            }
            if (Right > Screen.PrimaryScreen.Bounds.Right)
            {
                Left -= Right - Screen.PrimaryScreen.Bounds.Right;
            }
        }

        private void NodeToolDialog_SizeChanged(object sender, EventArgs e)
        {
            ControlResize.SetTag(this);
            DrawClient();
        }

        protected virtual void DrawClient() { }

        public new void Show()
        {
            if (Display.SelectedNode == null)
            {
                MessageBox.Show("没有选中的节点。");
                return;
            }
            var point = Display.GetSelectedNodeCenterOnScreen();
            Show(point);
        }
        public void Show(Point pos)
        {
            if (Display.SelectedNode == null)
            {
                return;
            }

            Location = pos;
            base.Show();
        }
    }
}
