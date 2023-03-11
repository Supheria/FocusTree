using FocusTree.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //Invalidate();

            Location = pos;
            base.Show();
        }
    }
}
