namespace FocusTree.UI.Controls
{
    class NodeContextMenu : ContextMenuStrip
    {
        private GraphBox Display;
        private ToolStripMenuItem pic_contextMenu_node_add = new();
        private ToolStripMenuItem pic_contextMenu_node_add_parent = new();
        private ToolStripMenuItem pic_contextMenu_node_add_child = new();
        private ToolStripMenuItem pic_contextMenu_node_edit = new();
        private ToolStripMenuItem pic_contextMenu_node_edit_focus = new();
        private ToolStripMenuItem pic_contextMenu_node_edit_requireGroup = new();
        private ToolStripMenuItem pic_contextMenu_node_edit_link = new();
        private ToolStripMenuItem pic_contextMenu_node_remove = new();
        private ToolStripMenuItem pic_contextMenu_node_checkout = new();
        public NodeContextMenu(GraphBox display)
        {
            Display = display;

            //==== 添加 ====//

            // main_contextMenu_node_add
            // 
            pic_contextMenu_node_add.DropDownItems.AddRange(new ToolStripItem[]
            {
                pic_contextMenu_node_add_parent,
                pic_contextMenu_node_add_child
            });
            pic_contextMenu_node_add.Name = "pic_contextMenu_node_add";
            pic_contextMenu_node_add.Size = new Size(180, 22);
            pic_contextMenu_node_add.Text = "添加";
            // 
            // pic_contextMenu_node_add_parent
            //
            pic_contextMenu_node_add_parent.Name = "pic_contextMenu_node_add_parent";
            pic_contextMenu_node_add_parent.Size = new Size(180, 22);
            pic_contextMenu_node_add_parent.Text = "父节点";
            // 
            // pic_contextMenu_node_add_child
            //
            pic_contextMenu_node_add_child.Name = "pic_contextMenu_node_add_child";
            pic_contextMenu_node_add_child.Size = new Size(180, 22);
            pic_contextMenu_node_add_child.Text = "子节点";

            //==== 编辑 ====//

            // main_contextMenu_node_edit
            // 
            pic_contextMenu_node_edit.DropDownItems.AddRange(new ToolStripItem[]
            {
                pic_contextMenu_node_edit_focus,
                pic_contextMenu_node_edit_requireGroup,
                pic_contextMenu_node_edit_link
            });
            pic_contextMenu_node_edit.Name = "pic_contextMenu_node_edit";
            pic_contextMenu_node_edit.Size = new Size(180, 22);
            pic_contextMenu_node_edit.Text = "编辑";
            // 
            // pic_contextMenu_node_edit_focus
            //
            pic_contextMenu_node_edit_focus.Name = "pic_contextMenu_node_edit_focus";
            pic_contextMenu_node_edit_focus.Size = new Size(180, 22);
            pic_contextMenu_node_edit_focus.Text = "国策";
            pic_contextMenu_node_edit_focus.Click += EditFocus;
            // 
            // pic_contextMenu_node_edit_requireGroup
            //
            pic_contextMenu_node_edit_requireGroup.Name = "pic_contextMenu_node_edit_requireGroup";
            pic_contextMenu_node_edit_requireGroup.Size = new Size(180, 22);
            pic_contextMenu_node_edit_requireGroup.Text = "依赖组";
            // 
            // pic_contextMenu_node_edit_link
            //
            pic_contextMenu_node_edit_link.Name = "pic_contextMenu_node_edit_link";
            pic_contextMenu_node_edit_link.Size = new Size(180, 22);
            pic_contextMenu_node_edit_link.Text = "子链接";

            //==== 删除 ====//

            // 
            // pic_contextMenu_node_remove
            // 
            pic_contextMenu_node_remove.Name = "pic_contextMenu_node_remove";
            pic_contextMenu_node_remove.Size = new Size(180, 22);
            pic_contextMenu_node_remove.Text = "删除节点";
            pic_contextMenu_node_remove.Click += NodeRemove;

            //==== 查看 ====//

            //
            // pic_contextMenu_node_checkout
            //
            pic_contextMenu_node_checkout.Name = "pic_contextMenu_node_checkout";
            pic_contextMenu_node_checkout.Size = new Size(180, 22);
            pic_contextMenu_node_checkout.Text = "查看国策";
            pic_contextMenu_node_checkout.Click += NodeCheckOut;
            // 
            // pic_contextMenu_node
            // 
            Name = "pic_contextMenu_node";
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

        private void EditFocus(object sender, EventArgs args)
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
