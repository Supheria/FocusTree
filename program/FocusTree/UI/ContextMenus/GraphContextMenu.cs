namespace FocusTree.UI.Controls
{
    class GraphContextMenu : ContextMenuStrip
    {
        public MouseButtons ButtonTag;
        private GraphDisplayer Display;

        private ToolStripMenuItem menuItem_edit_undo = new();
        private ToolStripMenuItem menuItem_edit_redo = new();

        public GraphContextMenu(GraphDisplayer display, Point showPoint, MouseButtons button)
        {
            ButtonTag = button;
            Display = display;
            if (button == MouseButtons.Middle)
            {
                InitializeInMiddleClick();
                Show(showPoint);
            }
            else if (button == MouseButtons.Right && !GraphBox.ReadOnly)
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
            var backupList = GraphBox.BackupList;
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
            if (!GraphBox.ReadOnly) { return; }
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
            GraphBox.Save();
        }
        private void MenuItem_edit_undo_Click(object sender, EventArgs args)
        {
            GraphBox.Undo();
            Display.Refresh();
            menuItem_edit_status_check();
            Invalidate();
        }
        private void MenuItem_edit_redo_Click(object sender, EventArgs args)
        {
            GraphBox.Redo();
            Display.Refresh();
            menuItem_edit_status_check();
            Invalidate();
        }
        private void MenuItem_loc_panorama_Click(object sender, EventArgs args)
        {
            Display.CameraLocatePanorama();
        }
        private void menuItem_loc_focus_Click(object sender, EventArgs args)
        {
            Display.CameraLocateSelectedNode(true);
        }
        public void menuItem_edit_status_check()
        {
            menuItem_edit_undo.Enabled = GraphBox.HasPrevHistory;
            menuItem_edit_redo.Enabled = GraphBox.HasNextHistory;
        }
        private void menuItem_edit_status_check(object sender, EventArgs e)
        {
            menuItem_edit_undo.Enabled = GraphBox.HasPrevHistory;
            menuItem_edit_redo.Enabled = GraphBox.HasNextHistory;
        }

        private void BackupItemClicked(object sender, EventArgs args)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            if (GraphBox.Edited == true)
            {
                if (MessageBox.Show("要放弃当前的更改切换到备份吗？", "提示 ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }
            }
            GraphBox.Load(item.Tag.ToString());
            Display.Refresh();
            ButtonTag = MouseButtons.None;
        }
        private void DeleteBackupClicked(object sender, EventArgs e)
        {
            GraphBox.DeleteBackup();
            GraphBox.Reload();
            Display.ResetDisplay();
        }
    }
}
