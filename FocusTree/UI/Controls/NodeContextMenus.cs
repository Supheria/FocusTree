﻿using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System;

namespace FocusTree.UI.Controls
{
    class NodeContextMenu : ContextMenuStrip
    {
        private Point LocationOnScreen;
        private GraphBox Display;
        private ToolStripMenuItem menuItem_add = new();
        private ToolStripMenuItem menuItem_add_parent = new();
        private ToolStripMenuItem menuItem_add_child = new();
        private ToolStripMenuItem menuItem_edit = new();
        private ToolStripMenuItem menuItem_edit_focus = new();
        private ToolStripMenuItem menuItem_edit_requireGroup = new();
        private ToolStripMenuItem menuItem_edit_link = new();
        private ToolStripMenuItem menuItem_remove = new();
        private ToolStripMenuItem menuItem_checkout = new();
        private ToolStripMenuItem test = new();
        public NodeContextMenu(GraphBox display)
        {
            
            //MouseMove += NodeContextMenu_MouseMove;
            Display = display;

            //==== 添加 ====//

            // main_contextMenu_node_add
            // 
            menuItem_add.DropDownItems.AddRange(new ToolStripItem[]
            {
                menuItem_add_parent,
                menuItem_add_child
            });
            menuItem_add.Name = "menuItem_add";
            menuItem_add.Size = new Size(180, 22);
            menuItem_add.Text = "添加";
            menuItem_add.MergeIndex = 0;
            //menuItem_add.MouseMove += MenuItem_add_MouseMove;
            //menuItem_add.MouseLeave += MenuItem_add_MouseLeave;
            menuItem_add.Tag = new Point(0, 0);
            // 
            // menuItem_add_parent
            //
            menuItem_add_parent.Name = "menuItem_add_parent";
            menuItem_add_parent.Size = new Size(180, 22);
            menuItem_add_parent.Text = "父节点";
            menuItem_add_parent.DropDownItems.Add("test");
            //menuItem_add_parent.MouseMove += MenuItem_add_parent_MouseMove;
            //menuItem_add_parent.MouseLeave += MenuItem_add_parent_MouseLeave;
            menuItem_add.Tag = new Point(menuItem_add.Width, 0);
            // 
            // menuItem_add_child
            //
            menuItem_add_child.Name = "menuItem_add_child";
            menuItem_add_child.Size = new Size(180, 22);
            menuItem_add_child.Text = "子节点";

            //==== 编辑 ====//

            // main_contextMenu_node_edit
            // 
            menuItem_edit.DropDownItems.AddRange(new ToolStripItem[]
            {
                menuItem_edit_focus,
                menuItem_edit_requireGroup,
                menuItem_edit_link
            });
            menuItem_edit.Name = "menuItem_edit";
            menuItem_edit.Size = new Size(180, 22);
            menuItem_edit.Text = "编辑";
            //menuItem_edit.MouseMove += MenuItem_edit_MouseMove;
            //menuItem_edit.MouseLeave += MenuItem_edit_MouseLeave; ;
            menuItem_edit.Tag = new Point(0, 0);
            // 
            // menuItem_edit_focus
            //
            menuItem_edit_focus.Name = "menuItem_edit_focus";
            menuItem_edit_focus.Size = new Size(180, 22);
            menuItem_edit_focus.Text = "国策";
            menuItem_edit_focus.Click += EditFocus;
            menuItem_edit_focus.DropDownItems.Add("hello");
            menuItem_edit_focus.MouseMove += MenuItem_edit_focus_MouseMove;
            //menuItem_edit_focus.MergeIndex = 1;
            // 
            // menuItem_edit_requireGroup
            //
            menuItem_edit_requireGroup.Name = "menuItem_edit_requireGroup";
            menuItem_edit_requireGroup.Size = new Size(180, 22);
            menuItem_edit_requireGroup.Text = "依赖组";
            // 
            // menuItem_edit_link
            //
            menuItem_edit_link.Name = "menuItem_edit_link";
            menuItem_edit_link.Size = new Size(180, 22);
            menuItem_edit_link.Text = "子链接";

            //==== 删除 ====//

            // 
            // menuItem_remove
            // 
            menuItem_remove.Name = "menuItem_remove";
            menuItem_remove.Size = new Size(180, 22);
            menuItem_remove.Text = "删除节点";
            menuItem_remove.Click += NodeRemove;

            //==== 查看 ====//

            //
            // menuItem_checkout
            //
            menuItem_checkout.Name = "menuItem_checkout";
            menuItem_checkout.Size = new Size(180, 22);
            menuItem_checkout.Text = "查看国策";
            menuItem_checkout.Click += NodeCheckOut;
            // 
            // menuItem
            // 
            Name = "menuItem";
            Size = new Size(181, 92);
            if (display.ReadOnly)
            {
                Items.Add(menuItem_checkout);
            }
            else
            {
                Items.AddRange(new ToolStripItem[] {
                menuItem_add,
                menuItem_edit,
                menuItem_remove
                });
            }
            //ItemRange = new()
            //{
            //    ["添加"] = GetMenuItemRange(menuItem_add),
            //    ["编辑"] = GetMenuItemRange(menuItem_edit)
            //};
            this.Closed += NodeContextMenu_Closed; ;
        }

        private void MenuItem_edit_focus_MouseMove(object sender, MouseEventArgs e)
        {
            Display.DrawAddtionalInfo($"MenuItem_edit{e.Location}");
        }

        private void NodeContextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            Display.DrawingDefreeze();
        }

        private void EditFocus(object sender, EventArgs e)
        {
            Display.ShowNodeInfo();
        }
        private void NodeRemove(object sender, EventArgs e)
        {
            Display.RemoveNode();
        }
        private void NodeCheckOut(object sender, EventArgs e)
        {
            Display.ShowNodeInfo();
        }
        public new void Show(Point location)
        {
            LocationOnScreen = location;
            
            var a = location.ToString();
            Display.DrawingFreeze();
            var data = Display.GetSelectedNodeData();
            var info = $"{data.Name}, {data.ID}";
            Display.DrawAddtionalInfo(info);
            base.Show(location);
        }
    }
}
