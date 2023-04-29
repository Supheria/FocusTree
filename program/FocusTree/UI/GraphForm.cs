#define DEBUG
using FocusTree.Data.Focus;
using FocusTree.IO;
using FocusTree.IO.FileManege;
using FocusTree.Properties;
using FocusTree.Graph;
using System.IO.Compression;

namespace FocusTree.UI
{
    public partial class GraphForm : Form
    {
        readonly GraphDisplayer Display;
        
        public GraphForm()
        {
            Display = new GraphDisplayer(this);
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
            //Display.LoadGraph("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\FILES\\隐居村落_测试连线用.xml");
            //Display.LoadGraph("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\FILES\\神佑村落.xml");

            //WindowState = FormWindowState.Minimized;
            //Display.SaveAsNew("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\国策\\国策测试\\test.xml");
            //Display.LoadGraph("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\国策\\国策测试\\test.xml");77
            //var a = new InfoDialog(Display);
            //Display.SelectedNode = 1;
            //a.Show(new(Screen.PrimaryScreen.Bounds.Width / 3, Screen.PrimaryScreen.Bounds.Height / 3));
#endif
        }

        #region ==== File ====

        private void MainForm_Menu_file_open_Click(object sender, EventArgs e)
        {
            if (GraphBox.Edited == true)
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
            GraphBox.Load(MainForm_Openfile.FileName);
            Display.ResetDisplay();
            UpdateText();
        }
        private void MainForm_Menu_file_save_Click(object sender, EventArgs e)
        {
            GraphBox.Save();
            UpdateText();
        }
        private void MainForm_Menu_file_saveas_Click(object sender, EventArgs e)
        {
            MainForm_Savefile.InitialDirectory = Path.GetDirectoryName(GraphBox.FilePath);
            MainForm_Savefile.FileName = Path.GetFileNameWithoutExtension(GraphBox.FilePath) + "_new.xml";
            if (MainForm_Savefile.ShowDialog() == DialogResult.OK)
            {
                GraphBox.SaveToNew(MainForm_Savefile.FileName);
            }
            UpdateText();
        }

        #endregion

        #region ==== File_Backup ====

        private void MainForm_Menu_file_backup_DropDownOpening(object sender, EventArgs e)
        {
            MainForm_Menu_file_backup_open_ReadBackupList(sender, e);
            MainForm_Menu_file_backup_delete.Visible = GraphBox.ReadOnly;
        }

        private void MainForm_Menu_file_backup_DropDownOpened(object sender, EventArgs e)
        {
            MainForm_Menu_file_backup_seperator.Visible = MainForm_Menu_file_backup_open.Visible;
        }
        private void MainForm_Menu_file_backup_open_ReadBackupList(object sender, EventArgs e)
        {
            MainForm_Menu_file_backup_open.Visible = false;
            if (GraphBox.IsNull) { return; }

            MainForm_Menu_file_backup_open.DropDownItems.Clear();
            var backupList = GraphBox.BackupList;
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
            if (GraphBox.Edited == true)
            {
                if (MessageBox.Show("要放弃当前的更改切换到备份吗？", "提示 ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }
            }
            GraphBox.Load(item.Tag.ToString());
            Display.Refresh();
        }
        private void MainForm_Menu_file_backup_delete_Click(object sender, EventArgs e)
        {
            GraphBox.DeleteBackup();
            GraphBox.Reload();
            Display.ResetDisplay();
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
            GraphBox.Undo();
            Display.Refresh();
            MainForm_Menu_edit_status_check();
            UpdateText();
        }

        private void MainForm_Menu_edit_redo_Click(object sender, EventArgs e)
        {
            GraphBox.Redo();
            Display.Refresh();
            MainForm_Menu_edit_status_check();
            UpdateText();
        }
        /// <summary>
        /// 更新撤回和重做按钮是否可用的状态
        /// </summary>
        public void MainForm_Menu_edit_status_check()
        {
            MainForm_Menu_edit_undo.Enabled = GraphBox.HasPrevHistory;
            MainForm_Menu_edit_redo.Enabled = GraphBox.HasNextHistory;
        }
        private void MainForm_Menu_edit_status_check(object sender, EventArgs e)
        {
            MainForm_Menu_edit_undo.Enabled = GraphBox.HasPrevHistory;
            MainForm_Menu_edit_redo.Enabled = GraphBox.HasNextHistory;
        }

        private void MainForm_Menu_edit_Click(object sender, EventArgs e)
        {
            MainForm_Menu_edit_status_check();
        }

        #endregion

        #region ==== Camera ====

        private void MainForm_Menu_camLoc_panorama_Click(object sender, EventArgs e)
        {
            Display.CameraLocatePanorama();
        }
        private void MainForm_Menu_camLoc_focus_Click(object sender, EventArgs e)
        {
            Display.CameraLocateSelectedNode(true);
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
            NodeMapDrawer.SaveasImage(GraphBox.Graph, GraphBox.FilePath);
        }
        private void MainForm_Menu_graph_reorderIds_Click(object sender, EventArgs e)
        {
            GraphBox.ReorderFocusNodesID();
        }
        private void MainForm_Menu_graph_autoLayout_Click(object sender, EventArgs e)
        {
            GraphBox.AutoLayoutAllFocusNodes();
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

        #region ==== Setting ====

        private void MainForm_Menu_setting_backImage_show_Click(object sender, EventArgs e)
        {
            if (Background.Show == true)
            {
                MainForm_Menu_setting_backImage_show.CheckState = CheckState.Unchecked;
                Background.Show = false;
            }
            else
            {
                MainForm_Menu_setting_backImage_show.CheckState = CheckState.Checked;
                Background.Show = true;
            }
            Background.DrawNew(Display.Image);
            Lattice.Draw(Display.Image);
            Display.Invalidate();
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
            if (GraphBox.IsNull)
            {
                Text = "FocusTree";
                MainForm_StatusStrip_status.Text = "等待打开文件";
                MainForm_StatusStrip_filename.Text = "";
            }
            else if (GraphBox.ReadOnly)
            {
                Text = GraphBox.Name;
                MainForm_StatusStrip_filename.Text = GraphBox.FilePath;
                MainForm_StatusStrip_status.Text = "正在预览";
            }
            else if (GraphBox.Edited)
            {
                Text = GraphBox.Name;
                MainForm_StatusStrip_filename.Text = GraphBox.FilePath + "*";
                MainForm_StatusStrip_status.Text = "正在编辑";
            }
            else
            {
                Text = GraphBox.Name;
                MainForm_StatusStrip_filename.Text = GraphBox.FilePath;
                MainForm_StatusStrip_status.Text = "就绪";
            }
        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
            SizeChanged += MainForm_SizeChanged;
            Size size = Background.Size;
            Size = new(
                size.Width + Width - ClientRectangle.Width,
                size.Height + Height - ClientRectangle.Height
                );
#if DEBUG
            //GraphBox.Load("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\FILES\\神佑村落.xml");
            //Display.ResetDisplay();
#endif
        }
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (Math.Min(ClientRectangle.Width, ClientRectangle.Height) <= 0 || WindowState == FormWindowState.Minimized)
            {
                return;
            }
            Display.Bounds = new(
                ClientRectangle.Left,
                ClientRectangle.Top + MainForm_Menu.Height,
                ClientRectangle.Width,
                ClientRectangle.Height - MainForm_Menu.Height - MainForm_StatusStrip.Height
                );
            Display.Image?.Dispose();
            Display.Image = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            Background.DrawNew(Display.Image);
            Lattice.SetBounds(Display.LatticeBound);
            Lattice.Draw(Display.Image);
            Invalidate();
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (GraphBox.IsNull || GraphBox.Edited == false)
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
                GraphBox.Save();
            }
        }

        #endregion
    }
}
