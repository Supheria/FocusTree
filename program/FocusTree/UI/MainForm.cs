#define DEBUG
using FocusTree.Data.Focus;
using FocusTree.IO;
using FocusTree.IO.FileManege;
using FocusTree.UI.Graph;
using System.IO.Compression;

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

            
            Shown += MainForm_Shown;

            foreach (var name in Display.ToolDialogs.Keys)
            {
                ToolStripMenuItem item = new()
                {
                    Text = name
                };
                item.Click += MainForm_Menu_window_display_toolDialog_Click;
                this.MainForm_Menu_window.DropDownItems.Add(item);
            }
#if DEBUG
            Display.LoadGraph("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\FILES\\隐居村落_测试连线用.xml");

            //WindowState = FormWindowState.Minimized;
            //Display.SaveAsNew("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\国策\\国策测试\\test.xml");
            //Display.LoadGraph("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\国策\\国策测试\\test.xml");77
            //var a = new InfoDialog(Display);
            //Display.SelectedNode = 1;
            //a.Show(new(Screen.PrimaryScreen.Bounds.Width / 3, Screen.PrimaryScreen.Bounds.Height / 3));
#endif


        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Lattice.SetBounds(Display.ClientRectangle);
            Lattice.Draw(Display.gCore);
            Display.Invalidate();
        }

        #region ==== File ====

        private void MainForm_Menu_file_open_Click(object sender, EventArgs e)
        {
            if (Display.GraphEdited == true)
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
            UpdateText();
        }
        private void MainForm_Menu_file_save_Click(object sender, EventArgs e)
        {
            Display.SaveGraph();
            UpdateText();
        }
        private void MainForm_Menu_file_saveas_Click(object sender, EventArgs e)
        {
            MainForm_Savefile.InitialDirectory = Path.GetDirectoryName(Display.FilePath);
            MainForm_Savefile.FileName = Path.GetFileNameWithoutExtension(Display.FilePath) + "_new.xml";
            if (MainForm_Savefile.ShowDialog() == DialogResult.OK)
            {
                Display.SaveAsNew(MainForm_Savefile.FileName);
            }
            UpdateText();
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
            var backupList = Display.Graph.GetBackupsList(Display.FilePath);
            if (backupList.Count == 1) { return; }

            MainForm_Menu_file_backup_open.Visible = true;
            ToolStripMenuItem item;
            foreach (var pair in backupList)
            {
                item = new()
                {
                    Tag = pair.Item1,
                    Text = pair.Item2
                };
                item.Click += BackupItemClicked;
                MainForm_Menu_file_backup_open.DropDownItems.Add(item);
            }
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
        }
        private void MainForm_Menu_file_backup_delete_Click(object sender, EventArgs e)
        {
            Display.Graph.DeleteBackup();
            Display.LoadGraph(Display.FilePath);
            MainForm_StatusStrip_status.Text = "已删除";
        }
        private void MainForm_Menu_file_backup_clear_Click(object sender, EventArgs e)
        {
            MainForm_StatusStrip_status.Text = "正在打包";
            FolderBrowserDialog folderBrowser = new()
            {
                Description = "选择要打包到文件夹"
            };
            if (folderBrowser.ShowDialog() == DialogResult.Cancel)
            {
                MainForm_StatusStrip_status.Text = "已取消";
                return;
            }
            var zipPath = Path.Combine(folderBrowser.SelectedPath, DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒") + ".ftbp");
            FileBackup.Clear(zipPath);
            MainForm_StatusStrip_status.Text = "备份已打包";
        }
        private void MainForm_Menu_file_backup_unpack_Click(object sender, EventArgs e)
        {
            MainForm_StatusStrip_status.Text = "正在解包";
            MainForm_Openfile.Filter = "备份打包文件|*.ftbp";
            if (MainForm_Openfile.ShowDialog() == DialogResult.Cancel)
            {
                MainForm_StatusStrip_status.Text = "已取消";
                return;
            }
            var zipPath = MainForm_Openfile.FileName;
            ZipFile.ExtractToDirectory(zipPath, FileBackup.RootDirectoryName, true);
            MainForm_StatusStrip_status.Text = "备份已解包";
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

        private void MainForm_Menu_loc_panorama_Click(object sender, EventArgs e)
        {
            Display.CamLocatePanorama();
        }
        private void MainForm_Menu_loc_focus_Click(object sender, EventArgs e)
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
        private void MainForm_Menu_graph_setNodePointAuto_Click(object sender, EventArgs e)
        {
            Display.ResetNodeLatticedPoints();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="motion">编辑动作</param>
        public void UpdateText(string motion)
        {
            MainForm_StatusStrip_status.Text = motion;
        }
        private void UpdateText()
        {
            MainForm_ProgressBar.Value = 0;
            if (Display.ReadOnly)
            {
                Text = Display.FileName;
                MainForm_StatusStrip_filename.Text = Display.FilePath;
                MainForm_StatusStrip_status.Text = "正在预览";
            }
            else if (Display.GraphEdited == true)
            {
                Text = Display.FileName;
                MainForm_StatusStrip_filename.Text = Display.FilePath + "*";
                MainForm_StatusStrip_status.Text = "正在编辑";
            }
            else if (Display.GraphEdited == false)
            {
                Text = Display.FileName;
                MainForm_StatusStrip_filename.Text = Display.FilePath;
                MainForm_StatusStrip_status.Text = "就绪";
            }
            else
            {
                Text = "FocusTree";
                MainForm_StatusStrip_status.Text = "等待打开文件";
                MainForm_StatusStrip_filename.Text = "";
            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Display.Graph == null || Display.GraphEdited == false)
            {
                FileCache.Clear();
                return;
            }
            var result = MessageBox.Show("是否保存当前编辑？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            if (result == DialogResult.Yes)
            {
                Display.SaveGraph();
            }
        }

        #endregion
    }
}
