using FocusTree.UI.Controls;

namespace FocusTree.UI.NodeToolDialogs
{
    public partial class NodeToolDialog : Form
    {
        internal GraphBox Display;

        public new void Show()
        {
            MessageBox.Show("没有选中的节点。");
        }
        public void Show(Point pos)
        {
            if (Display.SelectedNode == null)
            {
                return;
            }
            Invalidate();

            Location = pos;
            base.Show();
        }
    }
}
