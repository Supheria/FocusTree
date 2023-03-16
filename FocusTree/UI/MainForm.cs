using FocusTree.IO;
using FocusTree.Tool.IO;
using FocusTree.Tool.UI;
using FocusTree.UI.Controls;
using FocusTree.UI.NodeToolDialogs;

namespace FocusTree.UI
{
    public partial class MainForm : Form
    {
        GraphBox Display;
        public MainForm()
        {
            Display = new GraphBox(this);
            InitializeComponent();
            MainForm_StatusStrip_filename.Text = "等待打开文件";
            MainForm_StatusStrip_status.Text = "";
            MainForm_Openfile.FileName = "";

            foreach (var name in Display.ToolDialogs.Keys)
            {
                ToolStripMenuItem item = new();
                item.Text = name;
                item.Click += MainForm_Menu_window_display_toolDialog_Click;
                this.MainForm_Menu_window.DropDownItems.Add(item);
            }
            //Display.LoadGraph("D:\\Non_E\\documents\\GitHub\\FocusTree\\FocusTree\\国策\\隐居村落.xml");

            //var a = new InfoDialog(Display);
            //Display.SelectedNode = 1;
            //a.Show(new(Screen.PrimaryScreen.Bounds.Width / 3, Screen.PrimaryScreen.Bounds.Height / 3));

            
        }
        private void MainForm_Menu_camera_panorama_Click(object sender, EventArgs e)
        {
            Display.CamLocatePanorama();
        }
        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 打开备份
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            MainForm_Openfile.InitialDirectory = Backup.DirectoryName;
            if (MainForm_Openfile.ShowDialog() == DialogResult.OK)
            {
                Display.LoadGraph(MainForm_Openfile.FileName);
            }
            MainForm_Openfile.InitialDirectory = oldInitDir;

        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Menu_file_save_Click(object sender, EventArgs e)
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
        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //[Obsolete("这个功能还没完善")]
        private void MainForm_Menu_file_saveas_Click(object sender, EventArgs e)
        {
            if (Display.Graph == null)
            {
                MessageBox.Show("[2303051524]没有可以保存的图像");
                return;
            }
            MainForm_Savefile.InitialDirectory = Path.GetDirectoryName(Display.Graph.FilePath);
            MainForm_Savefile.FileName = Path.GetFileNameWithoutExtension(Display.Graph.FilePath) + "_new.xml";
            if (MainForm_Savefile.ShowDialog() == DialogResult.OK)
            {
                Display.SaveAsNew(MainForm_Savefile.FileName);
            }
        }
        /// <summary>
        /// 清空备份
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Menu_file_backup_clear_Click(object sender, EventArgs e)
        {
            Backup.Clear();
        }
        /// <summary>
        /// 批量转存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Menu_file_batch_saveas_Click(object sender, EventArgs e)
        {
            MainForm_Openfile_batch.Filter = "csv文件 (.csv) |*.csv";
            if (MainForm_Openfile_batch.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            var fileNames = MainForm_Openfile_batch.FileNames;
            MainForm_Openfile_batch.InitialDirectory = Path.GetDirectoryName(fileNames[0]);
            var suc = MainForm_Openfile_batch.FileNames.Length;
            foreach (var fileName in fileNames)
            {
                try
                {
                    var graph = CsvReader.LoadGraph(fileName);
                    Backup.BackupFile(graph);
                    XmlIO.SaveGraph(graph.FilePath, graph);
                }
                catch (Exception ex)
                {
                    suc--;
                    MessageBox.Show($"{fileName}转存失败。\n{ex.Message}");
                }
            }
            MessageBox.Show($"成功转存{suc}个文件。");
        }

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
        public void UpdateText()
        {
            if (Display.Graph == null)
            {
                Text = "国策树";
                MainForm_StatusStrip_filename.Text = "未加载";
                MainForm_StatusStrip_status.Text = "";
            }
            Text = Display.FileName;
            if (Display.ReadOnly)
            {
                MainForm_StatusStrip_filename.Text = "正在预览";
                MainForm_StatusStrip_status.Text = "";
            }
            else if (Display.GraphEdited)
            {
                MainForm_StatusStrip_filename.Text = "正在编辑";
                MainForm_StatusStrip_status.Text = "";
            }
            else
            {
                MainForm_StatusStrip_filename.Text = "就绪";
                MainForm_StatusStrip_status.Text = "";
            }
        }

        private void MainForm_Menu_camera_focus_Click(object sender, EventArgs e)
        {
            Display.CamLocateSelected();
        }
        private void MainForm_Menu_window_display_toolDialog_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            Display.ToolDialogs[item.Text].Show();
        }
    }
}
