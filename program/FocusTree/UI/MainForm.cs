#define DEBUG
using FocusTree.Data.Focus;
using FocusTree.IO;
using FocusTree.IO.FileManege;
using FocusTree.UI.Controls;
using System.Windows.Forms;

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
        private void MainForm_Menu_file_backup_open_Click(object sender, EventArgs e)
        {
            if (Display.Graph != null && Display.GraphEdited)
            {
                if (MessageBox.Show("要放弃当前的修改吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }
            }
            MainForm_Openfile.Filter = "全部文件|*.*";
            var oldInitDir = MainForm_Openfile.InitialDirectory;
            MainForm_Openfile.InitialDirectory = Backup.SubRootDirectoryName;
            if (MainForm_Openfile.ShowDialog() == DialogResult.OK)
            {
                Display.LoadGraph(MainForm_Openfile.FileName);
            }
            MainForm_Openfile.InitialDirectory = oldInitDir;

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

        private void MainForm_Menu_batch_saveas_Click(object sender, EventArgs e)
        {
            MainForm_Openfile_batch.Title = "批量转存";
            var fileNames = GetBatchPath();
            if (fileNames == null) { return; }
            FolderBrowserDialog folderBrowser = new();
            folderBrowser.InitialDirectory = Path.GetDirectoryName(fileNames[0]);
            if (folderBrowser.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            int suc = 0;
            foreach (var fileName in fileNames)
            {
                try
                {
                    var graph = XmlIO.LoadFromXml<FocusGraph>(fileName);
                    graph.BackupFile(fileName);
                    graph.Save(Path.Combine(folderBrowser.SelectedPath, Path.GetFileName(fileName)));
                    suc++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{fileName}转存失败。\n{ex.Message}");
                }
            }
            MessageBox.Show($"成功{suc}个，共{fileNames.Length}个。");
        }
        private void MainForm_Menu_batch_saveasImage_Click(object sender, EventArgs e)
        {
            MainForm_Openfile_batch.Title = "批量生成图片";
            var fileNames = GetBatchPath();
            if (fileNames.Length == 0) { return; }
            int suc = 0;
            foreach (var fileName in fileNames)
            {
                try
                {
                    var graph = XmlIO.LoadFromXml<FocusGraph>(fileName);
                    var savePath = Path.ChangeExtension(fileName, ".jpg");
                    NodeMapDrawer.SaveasImage(graph, savePath);
                    suc++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{fileName}无法生成图片。\n{ex.Message}");
                }
            }
            MessageBox.Show($"成功{suc}个，共{fileNames.Length}个。");
        }
        private void MainForm_Menu_batch_reorderIds_Click(object sender, EventArgs e)
        {
            MainForm_Openfile_batch.Title = "批量重排节点ID";
            var fileNames = GetBatchPath();
            if (fileNames.Length == 0) { return; }
            int suc = 0;
            foreach (var fileName in fileNames)
            {
                try
                {
                    var graph = XmlIO.LoadFromXml<FocusGraph>(fileName);
                    graph.ReorderNodeIds();
                    graph.BackupFile(fileName);
                    graph.Save(fileName);
                    suc++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{fileName}无法重排节点ID。\n{ex.Message}");
                }
            }
            MessageBox.Show($"成功{suc}个，共{fileNames.Length}个。");
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
            if (Display.Graph == null)
            {
                Text = "国策树";
                MainForm_StatusStrip_filename.Text = "等待打开文件";
                MainForm_StatusStrip_status.Text = "";
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
