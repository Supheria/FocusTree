//#define DEBUG
using FocusTree.Data.Focus;
using FocusTree.Graph;
using FocusTree.IO;
using FocusTree.IO.FileManage;
using System.Diagnostics;
using System.IO.Compression;

namespace FocusTree.UI
{
    public partial class GraphForm : Form
    {
        readonly GraphDisplayer Display;
        FormWindowState LastState;
        Stopwatch ResizeTimer = new();
        public GraphForm()
        {
            Display = new GraphDisplayer(this);
            InitializeComponent();
            UpdateText();

            Shown += GraphFrom_Shown;

            foreach (var name in Display.ToolDialogs.Keys)
            {
                ToolStripMenuItem item = new()
                {
                    Text = name
                };
                item.Click += GraphFrom_Menu_window_display_toolDialog_Click;
                this.GraphFrom_Menu_window.DropDownItems.Add(item);
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

        private void GraphFrom_Menu_file_open_Click(object sender, EventArgs e)
        {
            if (GraphBox.Edited == true)
            {
                if (MessageBox.Show("要放弃当前的更改吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }
            }
            GraphFrom_Openfile.Filter = "xml文件|*.xml";
            if (GraphFrom_Openfile.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            GraphFrom_Openfile.InitialDirectory = Path.GetDirectoryName(GraphFrom_Openfile.FileName);
            GraphBox.Load(GraphFrom_Openfile.FileName);
            Display.ResetDisplay();
            UpdateText();
        }

        private void GraphFrom_Menu_file_openCsv_Click(object sender, EventArgs e)
        {
            if (GraphBox.Edited == true)
            {
                if (MessageBox.Show("要放弃当前的更改吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }
            }
            GraphFrom_Openfile.Filter = "csv文件|*.csv";
            if (GraphFrom_Openfile.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            GraphFrom_Openfile.InitialDirectory = Path.GetDirectoryName(GraphFrom_Openfile.FileName);
            GraphBox.LoadCsv(GraphFrom_Openfile.FileName);
            Display.ResetDisplay();
            UpdateText();
        }

        private void GraphFrom_Menu_file_save_Click(object sender, EventArgs e)
        {
            GraphBox.Save();
            UpdateText();
        }

        private void GraphFrom_Menu_file_saveas_Click(object sender, EventArgs e)
        {
            GraphFrom_Savefile.InitialDirectory = Path.GetDirectoryName(GraphBox.FilePath);
            GraphFrom_Savefile.FileName = Path.GetFileNameWithoutExtension(GraphBox.FilePath) + "_new.xml";
            if (GraphFrom_Savefile.ShowDialog() == DialogResult.OK)
            {
                GraphBox.SaveToNew(GraphFrom_Savefile.FileName);
            }
            UpdateText();
        }

        #endregion

        #region ==== File_Backup ====

        private void GraphFrom_Menu_file_backup_DropDownOpening(object sender, EventArgs e)
        {
            GraphFrom_Menu_file_backup_open_ReadBackupList(sender, e);
            GraphFrom_Menu_file_backup_delete.Visible = GraphBox.ReadOnly;
        }

        private void GraphFrom_Menu_file_backup_DropDownOpened(object sender, EventArgs e)
        {
            GraphFrom_Menu_file_backup_seperator.Visible = GraphFrom_Menu_file_backup_open.Visible;
        }
        private void GraphFrom_Menu_file_backup_open_ReadBackupList(object sender, EventArgs e)
        {
            GraphFrom_Menu_file_backup_open.Visible = false;
            if (GraphBox.IsNull) { return; }

            GraphFrom_Menu_file_backup_open.DropDownItems.Clear();
            var backupList = GraphBox.BackupList;
            if (backupList.Count == 1) { return; }

            GraphFrom_Menu_file_backup_open.Visible = true;
            ToolStripMenuItem item;
            foreach (var pair in backupList)
            {
                item = new()
                {
                    Tag = pair.Item1,
                    Text = pair.Item2
                };
                item.Click += BackupItemClicked;
                GraphFrom_Menu_file_backup_open.DropDownItems.Add(item);
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
            Display.RefreshGraphBox();
        }
        private void GraphFrom_Menu_file_backup_delete_Click(object sender, EventArgs e)
        {
            GraphBox.DeleteBackup();
            GraphBox.Reload();
            Display.ResetDisplay();
            GraphFrom_StatusStrip_status.Text = "已删除";
        }
        private void GraphFrom_Menu_file_backup_clear_Click(object sender, EventArgs e)
        {
            GraphFrom_StatusStrip_status.Text = "正在打包";
            FolderBrowserDialog folderBrowser = new()
            {
                Description = "选择要打包到文件夹"
            };
            if (folderBrowser.ShowDialog() == DialogResult.Cancel)
            {
                GraphFrom_StatusStrip_status.Text = "已取消";
                return;
            }
            var zipPath = Path.Combine(folderBrowser.SelectedPath, DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒") + ".ftbp");
            FileBackup.Clear(zipPath);
            GraphFrom_StatusStrip_status.Text = "备份已打包";
        }
        private void GraphFrom_Menu_file_backup_unpack_Click(object sender, EventArgs e)
        {
            GraphFrom_StatusStrip_status.Text = "正在解包";
            GraphFrom_Openfile.Filter = "备份打包文件|*.ftbp";
            if (GraphFrom_Openfile.ShowDialog() == DialogResult.Cancel)
            {
                GraphFrom_StatusStrip_status.Text = "已取消";
                return;
            }
            var zipPath = GraphFrom_Openfile.FileName;
            ZipFile.ExtractToDirectory(zipPath, FileBackup.RootDirectoryName, true);
            GraphFrom_StatusStrip_status.Text = "备份已解包";
        }

        #endregion

        #region ==== Edit ====

        private void GraphFrom_Menu_edit_undo_Click(object sender, EventArgs e)
        {
            GraphBox.Undo();
            Display.RefreshGraphBox();
            GraphFrom_Menu_edit_status_check();
            UpdateText();
        }

        private void GraphFrom_Menu_edit_redo_Click(object sender, EventArgs e)
        {
            GraphBox.Redo();
            Display.RefreshGraphBox();
            GraphFrom_Menu_edit_status_check();
            UpdateText();
        }
        /// <summary>
        /// 更新撤回和重做按钮是否可用的状态
        /// </summary>
        public void GraphFrom_Menu_edit_status_check()
        {
            GraphFrom_Menu_edit_undo.Enabled = GraphBox.HasPrevHistory;
            GraphFrom_Menu_edit_redo.Enabled = GraphBox.HasNextHistory;
        }
        private void GraphFrom_Menu_edit_status_check(object sender, EventArgs e)
        {
            GraphFrom_Menu_edit_undo.Enabled = GraphBox.HasPrevHistory;
            GraphFrom_Menu_edit_redo.Enabled = GraphBox.HasNextHistory;
        }

        private void GraphFrom_Menu_edit_Click(object sender, EventArgs e)
        {
            GraphFrom_Menu_edit_status_check();
        }

        #endregion

        #region ==== Camera ====

        private void GraphFrom_Menu_camLoc_panorama_Click(object sender, EventArgs e)
        {
            Display.CameraLocatePanorama();
        }
        private void GraphFrom_Menu_camLoc_focus_Click(object sender, EventArgs e)
        {
            Display.CameraLocateSelectedNode(true);
        }

        #endregion

        #region ==== Windows ====
        private void GraphFrom_Menu_window_display_toolDialog_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            Display.ToolDialogs[item.Text].Show();
        }

        #endregion

        #region ==== Graph ====

        private void GraphFrom_Menu_graph_saveas_Click(object sender, EventArgs e)
        {
            NodeMapDrawer.SaveasImage(GraphBox.Graph, GraphBox.FilePath);
        }
        private void GraphFrom_Menu_graph_reorderIds_Click(object sender, EventArgs e)
        {
            GraphBox.ReorderFocusNodesID();
        }
        private void GraphFrom_Menu_graph_autoLayout_Click(object sender, EventArgs e)
        {
            GraphBox.AutoLayoutAllFocusNodes();
            Display.RefreshGraphBox();
        }

        #endregion

        #region ==== Batch ====

        private void GraphFrom_Menu_batch_reorderIds_Click(object sender, EventArgs e)
        {
            GraphFrom_StatusStrip_status.Text = "正在转存";
            GraphFrom_Openfile_batch.Title = "批量重排节点ID";
            var fileNames = GetBatchPath();
            if (fileNames.Length == 0) { return; }
            FolderBrowserDialog folderBrowser = new()
            {
                InitialDirectory = Path.Combine(Path.GetDirectoryName(fileNames[0]), "batch")
            };
            if (folderBrowser.ShowDialog() == DialogResult.Cancel) { return; }
            GraphFrom_ProgressBar.Maximum = fileNames.Length;
            GraphFrom_ProgressBar.Value = 0;
            int suc = 0;
            foreach (var fileName in fileNames)
            {
                try
                {
                    var graph = XmlIO.LoadFromXml<FocusGraph>(fileName);
                    graph.ReorderNodeIds();
                    XmlIO.SaveToXml(graph, Path.Combine(folderBrowser.SelectedPath, Path.GetFileName(fileName)));
                    suc++;
                    GraphFrom_ProgressBar.PerformStep();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{fileName}转存失败。\n{ex.Message}");
                }
            }
            GraphFrom_ProgressBar.Value = 0;
            GraphFrom_StatusStrip_status.Text = $"成功{suc}个，共{fileNames.Length}个";
        }
        private void GraphFrom_Menu_batch_saveasImage_Click(object sender, EventArgs e)
        {
            GraphFrom_StatusStrip_status.Text = "正在生成图片";
            GraphFrom_Openfile_batch.Title = "批量生成图片";
            var fileNames = GetBatchPath();
            if (fileNames.Length == 0) { return; }
            FolderBrowserDialog folderBrowser = new()
            {
                InitialDirectory = Path.Combine(Path.GetDirectoryName(fileNames[0]))
            };
            if (folderBrowser.ShowDialog() == DialogResult.Cancel) { return; }
            GraphFrom_ProgressBar.Maximum = fileNames.Length;
            GraphFrom_ProgressBar.Value = 0;
            int suc = 0;
            foreach (var fileName in fileNames)
            {
                try
                {
                    var graph = XmlIO.LoadFromXml<FocusGraph>(fileName);
                    var savePath = Path.Combine(folderBrowser.SelectedPath, Path.GetFileName(fileName));
                    NodeMapDrawer.SaveasImage(graph, savePath);
                    suc++;
                    GraphFrom_ProgressBar.PerformStep();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{fileName}无法生成图片。\n{ex.Message}");
                }
            }
            GraphFrom_ProgressBar.Value = 0;
            GraphFrom_StatusStrip_status.Text = $"成功{suc}个，共{fileNames.Length}个";
        }
        private string[] GetBatchPath()
        {
            GraphFrom_Openfile_batch.Filter = "xml文件 (.xml) |*.xml";
            if (GraphFrom_Openfile_batch.ShowDialog() == DialogResult.Cancel)
            {
                return Array.Empty<string>();
            }
            GraphFrom_Openfile_batch.InitialDirectory = Path.GetDirectoryName(GraphFrom_Openfile_batch.FileNames[0]);
            return GraphFrom_Openfile_batch.FileNames;
        }

        #endregion

        #region ==== Setting ====

        private void GraphFrom_Menu_setting_backImage_show_Click(object sender, EventArgs e)
        {
            if (Background.Show == true)
            {
                GraphFrom_Menu_setting_backImage_show.CheckState = CheckState.Unchecked;
                Background.Show = false;
            }
            else
            {
                GraphFrom_Menu_setting_backImage_show.CheckState = CheckState.Checked;
                Background.Show = true;
            }
            Background.DrawNew(Display.Image);
            Display.Refresh();
        }

        #endregion

        #region ==== GraphFrom ====

        /// <summary>
        /// 
        /// </summary>
        /// <param name="motion">编辑动作</param>
        public void UpdateText(string motion)
        {
            GraphFrom_StatusStrip_status.Text = motion;
        }
        private void UpdateText()
        {
            GraphFrom_ProgressBar.Value = 0;
            if (GraphBox.IsNull)
            {
                Text = "FocusTree";
                GraphFrom_StatusStrip_status.Text = "等待打开文件";
                GraphFrom_StatusStrip_filename.Text = "";
            }
            else if (GraphBox.ReadOnly)
            {
                Text = GraphBox.Name;
                GraphFrom_StatusStrip_filename.Text = GraphBox.FilePath;
                GraphFrom_StatusStrip_status.Text = "正在预览";
            }
            else if (GraphBox.Edited)
            {
                Text = GraphBox.Name;
                GraphFrom_StatusStrip_filename.Text = GraphBox.FilePath + "*";
                GraphFrom_StatusStrip_status.Text = "正在编辑";
            }
            else
            {
                Text = GraphBox.Name;
                GraphFrom_StatusStrip_filename.Text = GraphBox.FilePath;
                GraphFrom_StatusStrip_status.Text = "就绪";
            }
        }
        private void GraphFrom_Shown(object sender, EventArgs e)
        {
            Size size = Background.Size;
            Size = new(
                size.Width + Width - ClientRectangle.Width,
                size.Height + Height - ClientRectangle.Height
                );
            ResizeGraphDisplayer();
            SizeChanged += GraphFrom_SizeChanged;
            ResizeEnd += GraphForm_ResizeEnd;
            ResizeTimer.Start();

#if DEBUG
            //GraphBox.Load("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\FILES\\神佑村落.xml");
            //Display.ResetDisplay();
#endif
        }
        private void GraphForm_ResizeEnd(object sender, EventArgs e)
        {
            Background.DrawNew(Display.Image);
            Display.Refresh();
        }
        private void GraphFrom_SizeChanged(object sender, EventArgs e)
        {
            ResizeGraphDisplayer();
            if (ResizeTimer.ElapsedMilliseconds > 300)
            {
                Background.DrawNew(Display.Image);
                Display.Refresh();
                Lattice.DrawRect = Display.LatticeBound;
            }
            ResizeTimer.Restart();
        }
        public void ResizeGraphDisplayer()
        {
            if (Math.Min(ClientRectangle.Width, ClientRectangle.Height) <= 0)
            {
                return;
            }
            Display.Bounds = new(
                ClientRectangle.Left,
                ClientRectangle.Top + GraphFrom_Menu.Height,
                ClientRectangle.Width,
                ClientRectangle.Height - GraphFrom_Menu.Height - GraphFrom_StatusStrip.Height
                );
        }
        private void GraphFrom_FormClosing(object sender, FormClosingEventArgs e)
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
