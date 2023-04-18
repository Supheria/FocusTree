using FocusTree.IO.FileManege;
using FocusTree.UI.Graph;

namespace FocusTree.UI.Controls
{
    class GraphContextMenu : ContextMenuStrip
    {
        public MouseButtons ButtonTag;
        private GraphBox Display;

        private ToolStripMenuItem menuItem_edit_undo = new();
        private ToolStripMenuItem menuItem_edit_redo = new();

        public GraphContextMenu(GraphBox display, Point showPoint, MouseButtons button)
        {
            ButtonTag = button;
            Display = display;
            if (button == MouseButtons.Middle)
            {
                InitializeInMiddleClick();
                Show(showPoint);
            }
            else if (button == MouseButtons.Right && !Display.ReadOnly)
            {
                InitializeInRightClick();
                Show(showPoint);
            }
        }
        private void InitializeInRightClick()
        {
            // 添加节点
            //
            ToolStripMenuItem menuItem_file_addNode = new()
            {
                Text = "添加节点"
            };
            //
            // 编辑-撤销
            //
            menuItem_edit_undo.Text = "撤销";
            menuItem_edit_undo.Click += MenuItem_edit_undo_Click;
            // 
            // 编辑 - 重做
            //
            menuItem_edit_redo.Text = "重做";
            menuItem_edit_redo.Click += MenuItem_edit_redo_Click;
            //
            // 文件
            // 
            ToolStripMenuItem menuItem_file_save = new()
            {
                Text = "保存"
            };
            menuItem_file_save.Click += MenuItem_file_save_Click;

            Items.AddRange(new ToolStripItem[] {
                menuItem_file_addNode,
                new ToolStripSeparator(),
                menuItem_edit_undo,
                menuItem_edit_redo,
                new ToolStripSeparator(),
                menuItem_file_save
                });
            Invalidated += menuItem_edit_status_check;
        }
        private void InitializeInMiddleClick()
        {
            var backupList = Display.Graph.GetBackupsList(Display.FilePath);
            if (backupList.Count == 1) { return; }

            ToolStripMenuItem item;
            foreach (var pair in backupList)
            {
                item = new()
                {
                    Tag = pair.Item1,
                    Text = pair.Item2
                };
                item.Click += BackupItemClicked;
                Items.Add(item);
            }
            if (!Display.ReadOnly) { return; }
            item = new()
            {
                Text = "删除"
            };
            ToolStripSeparator spliter = new();
            item.Click += DeleteBackupClicked;
            Items.Add(spliter);
            Items.Add(item);
        }
        private void MenuItem_file_save_Click(object sender, EventArgs args)
        {
            Display.SaveGraph();
        }
        private void MenuItem_edit_undo_Click(object sender, EventArgs args)
        {
            Display.Undo();
            menuItem_edit_status_check();
            Invalidate();
        }
        private void MenuItem_edit_redo_Click(object sender, EventArgs args)
        {
            Display.Redo();
            menuItem_edit_status_check();
            Invalidate();
        }
        private void MenuItem_loc_panorama_Click(object sender, EventArgs args)
        {
            Display.CamLocatePanorama();
        }
        private void menuItem_loc_focus_Click(object sender, EventArgs args)
        {
            Display.CamLocateSelected();
        }
        public void menuItem_edit_status_check()
        {
            menuItem_edit_undo.Enabled = Display.HasPrevHistory();
            menuItem_edit_redo.Enabled = Display.HasNextHistory();
        }
        private void menuItem_edit_status_check(object sender, EventArgs e)
        {
            menuItem_edit_undo.Enabled = Display.HasPrevHistory();
            menuItem_edit_redo.Enabled = Display.HasNextHistory();
        }

        private void BackupItemClicked(object sender, EventArgs args)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            if (Display.GraphEdited == true)
            {
                if (MessageBox.Show("要放弃当前的更改切换到备份吗？", "提示 ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }
            }
            Display.LoadGraph(item.Tag.ToString());
            ButtonTag = MouseButtons.None;
        }
        private void DeleteBackupClicked(object sender, EventArgs e)
        {
            Display.Graph.DeleteBackup();
            Display.LoadGraph(Display.FilePath);
        }

    }
}
