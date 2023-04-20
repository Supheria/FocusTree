namespace FocusTree.UI.Controls
{
    class NodeContextMenu : ContextMenuStrip
    {
        private GraphBox Display;
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
            //==== 插入 ====//
            //
            ToolStripMenuItem menuItem_insert = new()
            {
                Text = "插入节点"
            };
            ToolStripMenuItem menuItem_insert_up = new()
            {
                Text = "向上"
            };
            ToolStripMenuItem menuItem_insert_down = new()
            {
                Text = "向下"
            };
            menuItem_insert.DropDownItems.AddRange(new ToolStripItem[]
            {
                menuItem_insert_up,
                menuItem_insert_down
            });
            //
            //==== 删除 ====//
            //
            ToolStripMenuItem menuItem_remove = new()
            {
                Text = "删除"
            };
            menuItem_remove.Click += MenuItem_removeNode_Click;

            Items.AddRange(new ToolStripItem[] {
                menuItem_edit,
                menuItem_insert,
                menuItem_remove
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
