using FocusTree.Data;
using FocusTree.IO;

namespace FocusTree.UI.Controls
{
    class GraphContextMenu : ContextMenuStrip
    {
        public MouseButtons ButtonTag;
        private GraphBox Display;
        //
        // MouseButtonRight
        //
        private ToolStripMenuItem pic_contextMenu_graph_save = new();
        private ToolStripMenuItem pic_contextMenu_graph_saveas = new();
        private ToolStripMenuItem pic_contextMenu_graph_undo = new();
        private ToolStripMenuItem pic_contextMenu_graph_redo = new();
        private ToolStripMenuItem pic_contextMenu_graph_camreset = new();
        private ToolStripMenuItem pic_contextMenu_graph_camfocus = new();
        private ToolStripSeparator spliter1 = new();
        private ToolStripSeparator spliter2 = new();

        public GraphContextMenu(GraphBox display, MouseButtons button)
        {
            ButtonTag = button;
            Display = display;

            Name = "main_contextMenu_node";
            Size = new Size(181, 92);
            if (button == MouseButtons.Right)
            {
                MouseButtonRight();
            }
            else if (button == MouseButtons.Middle)
            {
                MouseButtonMiddle();
            }
        }
        private void FileItemClicked(object sender, EventArgs args)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            if (item.Tag.ToString() == Display.FilePath)
            {
                return;
            }
            if (Display.GraphEdited)
            {
                if (MessageBox.Show("要放弃当前的更改切换到备份吗？", "提示 ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }
            }
            Display.LoadGraph(item.Tag.ToString());
            ButtonTag = MouseButtons.None;
        }
        private void GraphSave(object sender, EventArgs args)
        {
            if (Display.Graph == null)
            {
                MessageBox.Show("[2302191440]没有可以保存的图像");
            }
            else
            {
                Display.SaveGraph();
            }
        }
        private void GraphSaveas(object sender, EventArgs args)
        {
            if (Display.Graph == null)
            {
                MessageBox.Show("[2303051524]没有可以保存的图像");
                return;
            }
            SaveFileDialog main_Savefile = new();
            main_Savefile.Filter = "xml文件 (.xml) |*.xml";
            main_Savefile.InitialDirectory = Path.GetDirectoryName(Display.FilePath);
            main_Savefile.FileName = Path.GetFileNameWithoutExtension(Display.FilePath) + "_new.xml";
            if (main_Savefile.ShowDialog() == DialogResult.OK)
            {
                Display.SaveAsNew(main_Savefile.FileName);
            }
        }
        private void GraphUndo(object sender, EventArgs args)
        {
            Display.Undo();
            main_Menu_edit_status_check();
            Display.Invalidate();
        }
        private void GraphRedo(object sender, EventArgs args)
        {
            Display.Redo();
            main_Menu_edit_status_check();
            Display.Invalidate();
        }
        private void GraphCamReset(object sender, EventArgs args)
        {
            Display.CamLocatePanorama();
        }
        private void GraphCamFocus(object sender, EventArgs args)
        {
            Display.CamLocateSelected();
            Display.Invalidate();
        }
        public void main_Menu_edit_status_check()
        {
            pic_contextMenu_graph_undo.Enabled = GraphHistory.HasPrev();
            pic_contextMenu_graph_redo.Enabled = GraphHistory.HasNext();
        }
        private void main_Menu_edit_status_check(object sender, EventArgs e)
        {
            pic_contextMenu_graph_undo.Enabled = GraphHistory.HasPrev();
            pic_contextMenu_graph_redo.Enabled = GraphHistory.HasNext();
        }

        private void MouseButtonMiddle()
        {
            ToolStripMenuItem item = new();
            item.Tag = Display.Graph.FilePath;
            item.Text = Path.GetFileNameWithoutExtension(Display.Graph.FilePath);
            item.Size = new Size(180, 22);
            item.Click += FileItemClicked;
            Items.Add(item);

            foreach (var filePath in Backup.GetBackupsList(Display.Graph.FilePath))
            {
                item = new();
                item.Tag = filePath.FullName;
                item.Text = Path.GetFileNameWithoutExtension(filePath.FullName);
                item.Size = new Size(180, 22);
                item.Click += FileItemClicked;
                Items.Add(item);
            }
        }
        private void MouseButtonRight()
        {
            // 
            // pic_contextMenu_graph_save
            // 
            pic_contextMenu_graph_save.Name = "main_contextMenu_graph_save";
            pic_contextMenu_graph_save.Size = new Size(180, 22);
            pic_contextMenu_graph_save.Text = "保存";
            pic_contextMenu_graph_save.Click += GraphSave;
            // 
            // pic_contextMenu_graph_saveas
            // 
            pic_contextMenu_graph_saveas.Name = "main_contextMenu_graph_save";
            pic_contextMenu_graph_saveas.Size = new Size(180, 22);
            pic_contextMenu_graph_saveas.Text = "另存为";
            pic_contextMenu_graph_saveas.Click += GraphSaveas;
            // 
            // pic_contextMenu_graph_undo
            // 
            pic_contextMenu_graph_undo.Name = "main_contextMenu_graph_undo";
            pic_contextMenu_graph_undo.Size = new Size(180, 22);
            pic_contextMenu_graph_undo.Text = "撤销";
            pic_contextMenu_graph_undo.Click += GraphUndo;
            // 
            // pic_contextMenu_graph_redo
            // 
            pic_contextMenu_graph_redo.Name = "main_contextMenu_graph_redo";
            pic_contextMenu_graph_redo.Size = new Size(180, 22);
            pic_contextMenu_graph_redo.Text = "重做";
            pic_contextMenu_graph_redo.Click += GraphRedo;
            // 
            // pic_contextMenu_graph_camreset
            // 
            pic_contextMenu_graph_camreset.Name = "main_contextMenu_graph_camreset";
            pic_contextMenu_graph_camreset.Size = new Size(180, 22);
            pic_contextMenu_graph_camreset.Text = "全景";
            pic_contextMenu_graph_camreset.Click += GraphCamReset;
            // 
            // pic_contextMenu_graph_camfocus
            // 
            pic_contextMenu_graph_camfocus.Name = "pic_contextMenu_graph_camfocus";
            pic_contextMenu_graph_camfocus.Size = new Size(180, 22);
            pic_contextMenu_graph_camfocus.Text = "聚焦";
            pic_contextMenu_graph_camfocus.Click += GraphCamFocus;
            // 
            // spliter
            // 
            spliter1.Name = "spliter1";
            spliter2.Name = "spliter2";
            // 
            // main_contextMenu_graph
            // 
            Items.AddRange(new ToolStripItem[] {
                pic_contextMenu_graph_undo,
                pic_contextMenu_graph_redo,
                spliter1,
                pic_contextMenu_graph_save,
                pic_contextMenu_graph_saveas,
                spliter2,
                pic_contextMenu_graph_camreset,
                pic_contextMenu_graph_camfocus
                });
            Invalidated += main_Menu_edit_status_check;
            this.Closed += GraphContextMenu_Closed;
        }

        private void GraphContextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            //Display.DrawingDefreeze();
        }

        public new void Show(Point location)
        {
            //Display.DrawingFreeze();
            base.Show(location);
        }
    }
}
