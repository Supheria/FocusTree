namespace FocusTree.UI
{
    class NodeContextMenu : ContextMenuStrip
    {
        private GraphBox Display;
        private ToolStripMenuItem pic_contextMenu_node_add = new();
        private ToolStripMenuItem pic_contextMenu_node_edit = new();
        private ToolStripMenuItem pic_contextMenu_node_remove = new();
        private ToolStripMenuItem pic_contextMenu_node_checkout = new();
        public NodeContextMenu(GraphBox display)
        {
            Display = display;
            // 
            // main_contextMenu_node_add
            // 
            pic_contextMenu_node_add.Name = "main_contextMenu_node_add";
            pic_contextMenu_node_add.Size = new Size(180, 22);
            pic_contextMenu_node_add.Text = "添加国策";
            // 
            // main_contextMenu_node_edit
            // 
            pic_contextMenu_node_edit.Name = "main_contextMenu_node_edit";
            pic_contextMenu_node_edit.Size = new Size(180, 22);
            pic_contextMenu_node_edit.Text = "编辑国策";
            pic_contextMenu_node_edit.Click += NodeEdit;
            // 
            // main_contextMenu_node_remove
            // 
            pic_contextMenu_node_remove.Name = "main_contextMenu_node_remove";
            pic_contextMenu_node_remove.Size = new Size(180, 22);
            pic_contextMenu_node_remove.Text = "删除国策";
            pic_contextMenu_node_remove.Click += NodeRemove;
            //
            // main_contextMenu_node_checkout
            //
            pic_contextMenu_node_checkout.Name = "main_contextMenu_node_checkout";
            pic_contextMenu_node_checkout.Size = new Size(180, 22);
            pic_contextMenu_node_checkout.Text = "查看国策";
            pic_contextMenu_node_checkout.Click += NodeCheckOut;
            // 
            // main_contextMenu_node
            // 
            Name = "main_contextMenu_node";
            Size = new Size(181, 92);
            if (display.ReadOnly)
            {
                Items.Add(pic_contextMenu_node_checkout);
            }
            else
            {
                Items.AddRange(new ToolStripItem[] {
                pic_contextMenu_node_add,
                pic_contextMenu_node_edit,
                pic_contextMenu_node_remove
                });
            }
        }

        private void NodeEdit(object sender, EventArgs args)
        {
            Display.ShowNodeInfo();
        }
        private void NodeRemove(object sender, EventArgs args)
        {
            Display.RemoveNode();
        }
        private void NodeCheckOut(object sender, EventArgs args)
        {
            Display.ShowNodeInfo();
        }
    }
}
