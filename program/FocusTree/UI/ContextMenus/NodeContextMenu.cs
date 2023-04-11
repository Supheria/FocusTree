namespace FocusTree.UI.Controls
{
    class NodeContextMenu : ContextMenuStrip
    {
        private GraphBox Display;
        //private ToolStripMenuItem menuItem_add = new();
        //private ToolStripMenuItem menuItem_add_parent = new();
        //private ToolStripMenuItem menuItem_add_child = new();
        //private ToolStripMenuItem menuItem_edit = new();
        //private ToolStripMenuItem menuItem_edit_focus = new();
        //private ToolStripMenuItem menuItem_edit_requireGroup = new();
        //private ToolStripMenuItem menuItem_edit_link = new();
        //private ToolStripMenuItem menuItem_remove = new();
        //private ToolStripMenuItem menuItem_checkout = new();
        public NodeContextMenu(GraphBox display, Point showPoint)
        {
            Display = display;
            if (Display.ReadOnly)
            {
                InitializeInReadonly();
            }
            else
            {
                InitializeInEditable();
            }
            Show(showPoint);
        }
        private void InitializeInEditable()
        {
            //==== 添加 ====//
            //
            ToolStripMenuItem menuItem_add = new()
            {
                Text = "添加"
            };
            ToolStripMenuItem menuItem_add_require = new()
            {
                Text = "依赖"
            };
            ToolStripMenuItem menuItem_add_newNode = new()
            {
                Text = "新节点"
            };
            menuItem_add.DropDownItems.AddRange(new ToolStripItem[]
            {
                menuItem_add_require,
                menuItem_add_newNode
            });
            //
            //==== 编辑 ====//
            //
            ToolStripMenuItem menuItem_edit = new()
            {
                Text = "编辑"
            };
            ToolStripMenuItem menuItem_edit_focus = new()
            {
                Text = "国策"
            };
            menuItem_edit_focus.Click += MenuItem_edit_focus_Click;
            ToolStripMenuItem menuItem_edit_require = new()
            {
                Text = "依赖"
            };
            menuItem_edit.DropDownItems.AddRange(new ToolStripItem[]
            {
                menuItem_edit_focus,
                menuItem_edit_require
            });
            //
            //==== 删除节点 ====//
            //
            ToolStripMenuItem menuItem_removeNode = new()
            {
                Text = "删除节点"
            };
            menuItem_removeNode.Click += MenuItem_removeNode_Click;

            Items.AddRange(new ToolStripItem[] {
                menuItem_add,
                menuItem_edit,
                menuItem_removeNode
                });
        }

        private void InitializeInReadonly()
        {
            ToolStripMenuItem menuItem_checkout = new()
            {
                Text = "查看国策"
            };
            menuItem_checkout.Click += MenuItem_checkout_Click;

            Items.Add(menuItem_checkout);
        }
        public new void Show(Point location)
        {
            var data = Display.GetSelectedNodeData();
            var info = data == null ? "" : $"{data.Name}, {data.Duration}日\n{data.Descript}";
            Display.DrawInfo(info);
            base.Show(location);
        }

        private void MenuItem_edit_focus_Click(object sender, EventArgs e)
        {
            Display.ShowNodeInfo();
        }
        private void MenuItem_removeNode_Click(object sender, EventArgs e)
        {
            Display.RemoveNode();
        }
        private void MenuItem_checkout_Click(object sender, EventArgs e)
        {
            Display.ShowNodeInfo();
        }
    }
}
