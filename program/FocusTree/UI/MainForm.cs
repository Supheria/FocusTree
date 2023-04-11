#define DEBUG
using FocusTree.Data.Focus;
using FocusTree.IO;
using FocusTree.IO.FileManege;
using FocusTree.UI.Controls;
using System.Text.RegularExpressions;

namespace FocusTree.UI
{
    public partial class MainForm : Form
    {
        readonly GraphBox Display;
        public MainForm()
        {
            Display = new GraphBox(this);
            InitializeComponent();
            UpdateText();

            foreach (var name in Display.ToolDialogs.Keys)
            {
                ToolStripMenuItem item = new();
                item.Text = name;
                item.Click += MainForm_Menu_window_display_toolDialog_Click;
                this.MainForm_Menu_window.DropDownItems.Add(item);
            }
#if DEBUG
            //Display.LoadGraph("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\国策\\隐居村落.xml");
            //WindowState = FormWindowState.Minimized;
            //Display.SaveAsNew("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\国策\\国策测试\\test.xml");
            //Display.LoadGraph("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\国策\\国策测试\\test.xml");
            //var a = new InfoDialog(Display);
            //Display.SelectedNode = 1;
            //a.Show(new(Screen.PrimaryScreen.Bounds.Width / 3, Screen.PrimaryScreen.Bounds.Height / 3));
#endif


        }

        #region ==== File ====

        private void MainForm_Menu_file_open_Click(object sender, EventArgs e)
        {
            if (Display.Graph != null && Display.GraphEdited)
            {
                if (MessageBox.Show("要放弃当前的更改吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }
            }
            MainForm_Openfile.Filter = "xml文件|*.xml";
            if (MainForm_Openfile.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            MainForm_Openfile.InitialDirectory = Path.GetDirectoryName(MainForm_Openfile.FileName);
            Display.LoadGraph(MainForm_Openfile.FileName);
        }
        private void MainForm_Menu_file_save_Click(object sender, EventArgs e)
        {
            Display.SaveGraph();
        }
        private void MainForm_Menu_file_saveas_Click(object sender, EventArgs e)
        {
            MainForm_Savefile.InitialDirectory = Path.GetDirectoryName(Display.FilePath);
            MainForm_Savefile.FileName = Path.GetFileNameWithoutExtension(Display.FilePath) + "_new.xml";
            if (MainForm_Savefile.ShowDialog() == DialogResult.OK)
            {
                Display.SaveAsNew(MainForm_Savefile.FileName);
            }
        }
        private void MainForm_Menu_file_backup_clear_Click(object sender, EventArgs e)
        {
            Backup.Clear();
        }

        #endregion

        #region ==== File_Backup ====

        private void MainForm_Menu_file_backup_DropDownOpening(object sender, EventArgs e)
        {
            MainForm_Menu_file_backup_open_ReadBackupList(sender, e);
            MainForm_Menu_file_backup_delete.Visible = Display.ReadOnly;
        }

        private void MainForm_Menu_file_backup_DropDownOpened(object sender, EventArgs e)
        {
            MainForm_Menu_file_backup_seperator.Visible = MainForm_Menu_file_backup_open.Visible;
        }
        private void MainForm_Menu_file_backup_open_ReadBackupList(object sender, EventArgs e)
        {
            MainForm_Menu_file_backup_open.Visible = false;
            if (Display.Graph == null) { return; }

            MainForm_Menu_file_backup_open.DropDownItems.Clear();
            var backupFiles = Display.Graph.GetBackupsList(Display.FilePath);
            if (backupFiles.Count == 0) { return; }

            MainForm_Menu_file_backup_open.Visible = true;
            ToolStripMenuItem item = new()
            {
                Tag = Display.FilePath,
                Text = Path.GetFileNameWithoutExtension(Display.FilePath),
                Size = new Size(180, 22)
            };
            item.Click += BackupItemClicked;
            MainForm_Menu_file_backup_open.DropDownItems.Add(item);

            foreach (var filePath in backupFiles)
            {
                item = new()
                {
                    Tag = filePath,
                    Text = GetBKDateTime(Path.GetFileName(filePath)),
                    Size = new Size(180, 22)
                };
                item.Click += BackupItemClicked;
                MainForm_Menu_file_backup_open.DropDownItems.Add(item);
            }
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
        }
        private static string GetBKDateTime(string path)
        {
            var match = Regex.Match(path, "^BK(\\d{4})(\\d{2})(\\d{2})(\\d{2})(\\d{2})(\\d{2})$");
            return $"{match.Groups[1].Value}/{match.Groups[2].Value}/{match.Groups[3].Value} {match.Groups[4].Value}:{match.Groups[5].Value}:{match.Groups[6].Value}";
        }

        private void MainForm_Menu_file_backup_delete_Click(object sender, EventArgs e)
        {
            Display.Graph.DeleteBackup(Display.FilePath);
            Display.LoadGraph(Display.FilePath);
        }

        #endregion

        #region ==== Edit ====

        private void MainForm_Menu_edit_undo_Click(object sender, EventArgs e)
        {
            Display.Undo();
            MainForm_Menu_edit_status_check();
        }

        private void MainForm_Menu_edit_redo_Click(object sender, EventArgs e)
        {
            Display.Redo();
            MainForm_Menu_edit_status_check();
        }
        /// <summary>
        /// 更新撤回和重做按钮是否可用的状态
        /// </summary>
        public void MainForm_Menu_edit_status_check()
        {
            MainForm_Menu_edit_undo.Enabled = Display.HasPrevHistory();
            MainForm_Menu_edit_redo.Enabled = Display.HasNextHistory();
        }
        private void MainForm_Menu_edit_status_check(object sender, EventArgs e)
        {
            MainForm_Menu_edit_undo.Enabled = Display.HasPrevHistory();
            MainForm_Menu_edit_redo.Enabled = Display.HasNextHistory();
        }

        private void MainForm_Menu_edit_Click(object sender, EventArgs e)
        {
            MainForm_Menu_edit_status_check();
        }

        #endregion

        #region ==== Camera ====

        private void MainForm_Menu_camera_panorama_Click(object sender, EventArgs e)
        {
            Display.CamLocatePanorama();
        }
        private void MainForm_Menu_camera_focus_Click(object sender, EventArgs e)
        {
            Display.CamLocateSelected();
        }

        #endregion

        #region ==== Windows ====
        private void MainForm_Menu_window_display_toolDialog_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            Display.ToolDialogs[item.Text].Show();
        }

        #endregion

        #region ==== Graph ====

        private void MainForm_Menu_graph_saveas_Click(object sender, EventArgs e)
        {
            var savePath = Path.ChangeExtension(Display.FilePath, ".jpg");
            NodeMapDrawer.SaveasImage(Display.Graph, savePath);
        }
        private void MainForm_Menu_graph_reorderIds_Click(object sender, EventArgs e)
        {
            Display.ReorderNodeIds();
        }

        #endregion

        #region ==== Batch ====

        private void MainForm_Menu_batch_reorderIds_Click(object sender, EventArgs e)
        {
            MainForm_StatusStrip_status.Text = "正在转存";
            MainForm_Openfile_batch.Title = "批量重排节点ID";
            var fileNames = GetBatchPath();
            if (fileNames.Length == 0) { return; }
            FolderBrowserDialog folderBrowser = new()
            {
                InitialDirectory = Path.Combine(Path.GetDirectoryName(fileNames[0]), "batch")
            };
            if (folderBrowser.ShowDialog() == DialogResult.Cancel) { return; }
            MainForm_ProgressBar.Maximum = fileNames.Length;
            MainForm_ProgressBar.Value = 0;
            int suc = 0;
            foreach (var fileName in fileNames)
            {
                try
                {
                    var graph = XmlIO.LoadFromXml<FocusGraph>(fileName);
                    graph.ReorderNodeIds();
                    graph.SaveToXml(Path.Combine(folderBrowser.SelectedPath, Path.GetFileName(fileName)));
                    suc++;
                    MainForm_ProgressBar.PerformStep();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{fileName}转存失败。\n{ex.Message}");
                }
            }
            MainForm_ProgressBar.Value = 0;
            MainForm_StatusStrip_status.Text = $"成功{suc}个，共{fileNames.Length}个";
        }
        private void MainForm_Menu_batch_saveasImage_Click(object sender, EventArgs e)
        {
            MainForm_StatusStrip_status.Text = "正在生成图片";
            MainForm_Openfile_batch.Title = "批量生成图片";
            var fileNames = GetBatchPath();
            if (fileNames.Length == 0) { return; }
            FolderBrowserDialog folderBrowser = new()
            {
                InitialDirectory = Path.Combine(Path.GetDirectoryName(fileNames[0]))
            };
            if (folderBrowser.ShowDialog() == DialogResult.Cancel) { return; }
            MainForm_ProgressBar.Maximum = fileNames.Length;
            MainForm_ProgressBar.Value = 0;
            int suc = 0;
            foreach (var fileName in fileNames)
            {
                try
                {
                    var graph = XmlIO.LoadFromXml<FocusGraph>(fileName);
                    var savePath = Path.Combine(folderBrowser.SelectedPath, Path.GetFileName(fileName));
                    savePath = Path.ChangeExtension(savePath, ".jpg");
                    NodeMapDrawer.SaveasImage(graph, savePath);
                    suc++;
                    MainForm_ProgressBar.PerformStep();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{fileName}无法生成图片。\n{ex.Message}");
                }
            }
            MainForm_ProgressBar.Value = 0;
            MainForm_StatusStrip_status.Text = $"成功{suc}个，共{fileNames.Length}个";
        }
        private string[] GetBatchPath()
        {
            MainForm_Openfile_batch.Filter = "xml文件 (.xml) |*.xml";
            if (MainForm_Openfile_batch.ShowDialog() == DialogResult.Cancel)
            {
                return Array.Empty<string>();
            }
            MainForm_Openfile_batch.InitialDirectory = Path.GetDirectoryName(MainForm_Openfile_batch.FileNames[0]);
            return MainForm_Openfile_batch.FileNames;
        }

        #endregion

        #region ==== MainForm ====

        public void UpdateText()
        {
            MainForm_ProgressBar.Value = 0;
            if (Display.Graph == null)
            {
                Text = "FocusTree";
                MainForm_StatusStrip_status.Text = "等待打开文件";
                MainForm_StatusStrip_filename.Text = "";
                return;
            }
            Text = Display.FileName;
            if (Display.ReadOnly)
            {
                MainForm_StatusStrip_filename.Text = Display.FilePath;
                MainForm_StatusStrip_status.Text = "正在预览";
            }
            else if (Display.GraphEdited)
            {
                MainForm_StatusStrip_filename.Text = Display.FilePath;
                MainForm_StatusStrip_status.Text = "正在编辑";
            }
            else
            {
                MainForm_StatusStrip_filename.Text = Display.FilePath;
                MainForm_StatusStrip_status.Text = "就绪";
            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cache.Clear();
        }

        #endregion
    }
}
