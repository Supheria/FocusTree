using FocusTree.Focus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.UI
{
    class NodeContextMenu : ContextMenuStrip
    {
        public int? NodeId;
        private DisplayBox Display;
        private ToolStripMenuItem pic_contextMenu_node_add = new ();
        private ToolStripMenuItem pic_contextMenu_node_edit = new ();
        private ToolStripMenuItem pic_contextMenu_node_remove = new ();
        public NodeContextMenu(DisplayBox display)
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
            // main_contextMenu_node
            // 
            Items.AddRange(new ToolStripItem[] {
            pic_contextMenu_node_add,
            pic_contextMenu_node_edit,
            pic_contextMenu_node_remove
            });

            Name = "main_contextMenu_node";
            Size = new Size(181, 92);
            
        }
        private void NodeEdit(object sender, EventArgs args)
        {
            if(NodeId != null)
            {
                var fnode = Display.GetFGraph().GetNode(NodeId.Value);
                MessageBox.Show($"{fnode.Name}\n\n" +
                    $"{fnode.Effects}\n\n" +
                    $"实施 {fnode.Duration} 天\n\n" +
                    $"{fnode.Descript}\n\n" +
                    $"{fnode.Ps}");
            }

            NodeId = null;
        }
        private void NodeRemove(object sender, EventArgs args)
        {
            if (NodeId != null)
            {
                Display.GetFGraph().RemoveNode(NodeId.Value);
                Display.Invalidate();
            }
            NodeId = null;
        }
    }
}
