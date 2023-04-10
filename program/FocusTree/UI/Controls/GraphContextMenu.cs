using FocusTree.IO.FileManege;
using System.Text.RegularExpressions;

namespace FocusTree.UI.Controls
{
    class GraphContextMenu : ContextMenuStrip
    {
        public MouseButtons ButtonTag;
        private GraphBox Display;
        //
        // MouseButtonRight
        //
        private ToolStripMenuItem GraphContextMenu_edit_save = new();
        private ToolStripMenuItem GraphContextMenu_edit_saveas = new();
        private ToolStripMenuItem GraphContextMenu_edit_undo = new();
        private ToolStripMenuItem GraphContextMenu_edit_redo = new();
        private ToolStripMenuItem GraphContextMenu_camera_panorama = new();
        private ToolStripMenuItem GraphContextMenu_camera_focus = new();
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
        private void GraphContextMenu_edit_save_Click(object sender, EventArgs args)
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
        private void GraphContextMenu_edit_saveas_Click(object sender, EventArgs args)
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
        private void GraphContextMenu_edit_undo_Click(object sender, EventArgs args)
        {
            Display.Undo();
            GraphContextMenu_edit_status_check();
            Invalidate();
        }
        private void GraphContextMenu_edit_redo_Click(object sender, EventArgs args)
        {
            Display.Redo();
            GraphContextMenu_edit_status_check();
            Invalidate();
        }
        private void GraphContextMenu_camera_panorama_Click(object sender, EventArgs args)
        {
            Display.CamLocatePanorama();
        }
        private void GraphContextMenu_camera_focus_Click(object sender, EventArgs args)
        {
            Display.CamLocateSelected();
        }
        public void GraphContextMenu_edit_status_check()
        {
            GraphContextMenu_edit_undo.Enabled = Display.HasPrevHistory();
            GraphContextMenu_edit_redo.Enabled = Display.HasNextHistory();
        }
        private void GraphContextMenu_edit_status_check(object sender, EventArgs e)
        {
            GraphContextMenu_edit_undo.Enabled = Display.HasPrevHistory();
            GraphContextMenu_edit_redo.Enabled = Display.HasNextHistory();
        }

        private void MouseButtonMiddle()
        {
            ToolStripMenuItem item = new()
            {
                Tag = Display.FilePath,
                Text = Path.GetFileNameWithoutExtension(Display.FilePath),
                Size = new Size(180, 22)
            };
            item.Click += FileItemClicked;
            Items.Add(item);

            foreach (var filePath in Display.Graph.GetBackupsList(Display.FilePath))
            {
                item = new()
                {
                    Tag = filePath,
                    Text = GetBKDateTime(Path.GetFileName(filePath)),
                    Size = new Size(180, 22)
                };
                item.Click += FileItemClicked;
                Items.Add(item);
            }
        }
        private string GetBKDateTime(string path)
        {
            var match = Regex.Match(path, "^BK(\\d{4})(\\d{2})(\\d{2})(\\d{2})(\\d{2})(\\d{2})$");
            return $"{match.Groups[1].Value}/{match.Groups[2].Value}/{match.Groups[3].Value} {match.Groups[4].Value}:{match.Groups[5].Value}:{match.Groups[6].Value}";
        }
        private void MouseButtonRight()
        {
            // 
            // GraphContextMenu_edit_save
            // 
            GraphContextMenu_edit_save.Name = "main_contextMenu_graph_save";
            GraphContextMenu_edit_save.Size = new Size(180, 22);
            GraphContextMenu_edit_save.Text = "保存";
            GraphContextMenu_edit_save.Click += GraphContextMenu_edit_save_Click;
            // 
            // GraphContextMenu_edit_saveas
            // 
            GraphContextMenu_edit_saveas.Name = "main_contextMenu_graph_save";
            GraphContextMenu_edit_saveas.Size = new Size(180, 22);
            GraphContextMenu_edit_saveas.Text = "另存为";
            GraphContextMenu_edit_saveas.Click += GraphContextMenu_edit_saveas_Click;
            // 
            // GraphContextMenu_edit_undo
            // 
            GraphContextMenu_edit_undo.Name = "main_contextMenu_graph_undo";
            GraphContextMenu_edit_undo.Size = new Size(180, 22);
            GraphContextMenu_edit_undo.Text = "撤销";
            GraphContextMenu_edit_undo.Click += GraphContextMenu_edit_undo_Click;
            // 
            // GraphContextMenu_edit_redo
            // 
            GraphContextMenu_edit_redo.Name = "main_contextMenu_graph_redo";
            GraphContextMenu_edit_redo.Size = new Size(180, 22);
            GraphContextMenu_edit_redo.Text = "重做";
            GraphContextMenu_edit_redo.Click += GraphContextMenu_edit_redo_Click;
            // 
            // GraphContextMenu_camera_panorama
            // 
            GraphContextMenu_camera_panorama.Name = "main_contextMenu_graph_camreset";
            GraphContextMenu_camera_panorama.Size = new Size(180, 22);
            GraphContextMenu_camera_panorama.Text = "全景";
            GraphContextMenu_camera_panorama.Click += GraphContextMenu_camera_panorama_Click;
            // 
            // GraphContextMenu_camera_focus
            // 
            GraphContextMenu_camera_focus.Name = "GraphContextMenu_camera_focus";
            GraphContextMenu_camera_focus.Size = new Size(180, 22);
            GraphContextMenu_camera_focus.Text = "聚焦";
            GraphContextMenu_camera_focus.Click += GraphContextMenu_camera_focus_Click;
            // 
            // spliter
            // 
            spliter1.Name = "spliter1";
            spliter2.Name = "spliter2";
            // 
            // main_contextMenu_graph
            // 
            Items.AddRange(new ToolStripItem[] {
                GraphContextMenu_edit_undo,
                GraphContextMenu_edit_redo,
                spliter1,
                GraphContextMenu_edit_save,
                GraphContextMenu_edit_saveas,
                spliter2,
                GraphContextMenu_camera_panorama,
                GraphContextMenu_camera_focus
                });
            Invalidated += GraphContextMenu_edit_status_check;
        }

        public new void Show(Point location)
        {
            base.Show(location);
            Invalidate();
        }
    }
}
