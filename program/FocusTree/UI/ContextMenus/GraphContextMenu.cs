using FocusTree.IO.FileManege;
using System.Text.RegularExpressions;

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
            if (button == MouseButtons.Right)
            {
                InitializeInRightClick();
            }
            else if (button == MouseButtons.Middle)
            {
                InitializeInMiddleClick();
            }
            Show(showPoint);
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
            ToolStripMenuItem menuItem_file_saveas = new()
            {
                Text = "另存为"
            };
            menuItem_file_saveas.Click += MenuItem_file_saveas_Click;
            // 
            // 位置
            // 
            ToolStripMenuItem menuItem_loc_panorama = new()
            {
                Text = "全景"
            };
            menuItem_loc_panorama.Click += MenuItem_loc_panorama_Click;
            ToolStripMenuItem menuItem_loc_focus = new()
            {
                Text = "聚焦"
            };
            menuItem_loc_focus.Click += menuItem_loc_focus_Click;

            Items.AddRange(new ToolStripItem[] {
                menuItem_file_addNode,
                new ToolStripSeparator(),
                menuItem_edit_undo,
                menuItem_edit_redo,
                new ToolStripSeparator(),
                menuItem_file_save,
                menuItem_file_saveas,
                new ToolStripSeparator(),
                menuItem_loc_panorama,
                menuItem_loc_focus
                });
            Invalidated += menuItem_edit_status_check;
        }
        private void InitializeInMiddleClick()
        {
            var backupFiles = Display.Graph.GetBackupsList(Display.FilePath);
            if (backupFiles.Count == 0) { return; }

            ToolStripMenuItem item = new()
            {
                Tag = Display.FilePath,
                Text = Path.GetFileNameWithoutExtension(Display.FilePath)
            };
            item.Click += BackupItemClicked;
            Items.Add(item);

            foreach (var filePath in backupFiles)
            {
                item = new()
                {
                    Tag = filePath,
                    Text = GetBKDateTime(Path.GetFileName(filePath))
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
        private void MenuItem_file_saveas_Click(object sender, EventArgs args)
        {
            SaveFileDialog main_Savefile = new();
            main_Savefile.Filter = "xml文件 (.xml) |*.xml";
            main_Savefile.InitialDirectory = Path.GetDirectoryName(Display.FilePath);
            main_Savefile.FileName = Path.GetFileNameWithoutExtension(Display.FilePath) + "_new.xml";
            if (main_Savefile.ShowDialog() == DialogResult.OK)
            {
                Display.SaveAsNew(main_Savefile.FileName);
            }
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
        private static string GetBKDateTime(string path)
        {
            var match = Regex.Match(path, "^BK(\\d{4})(\\d{2})(\\d{2})(\\d{2})(\\d{2})(\\d{2})$");
            return $"{match.Groups[1].Value}/{match.Groups[2].Value}/{match.Groups[3].Value} {match.Groups[4].Value}:{match.Groups[5].Value}:{match.Groups[6].Value}";
        }
        private void DeleteBackupClicked(object sender, EventArgs e)
        {
            Display.Graph.DeleteBackup(Display.FilePath);
            Display.LoadGraph(Display.FilePath);
        }
        
    }
}
