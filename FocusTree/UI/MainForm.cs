using FocusTree.Data;
using FocusTree.IO;
using FocusTree.UI.Controls;

namespace FocusTree.UI
{
    public partial class MainForm : Form
    {
        GraphBox Display;
        public MainForm()
        {
            Display = new GraphBox(this);
            InitializeComponent();
            main_StatusStrip_filename.Text = "等待打开文件";
            main_StatusStrip_status.Text = "";
            main_Openfile.FileName = "";

            foreach (var name in Display.ToolDialogs.Keys)
            {
                ToolStripMenuItem item = new();
                item.Text = name;
                item.Click += main_Menu_window_display_toolDialog_Click;
                this.main_Menu_window.DropDownItems.Add(item);
            }
        }
        private void main_Menu_loc_camreset_Click(object sender, EventArgs e)
        {
            Display.CamLocatePanorama();
        }
        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void main_Menu_file_open_Click(object sender, EventArgs e)
        {
            if (Display.Graph != null && Display.GraphEdited)
            {
                if (MessageBox.Show("要放弃当前的更改吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }
            }
            main_Openfile.Filter = "xml文件|*.xml";
            if (main_Openfile.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            main_Openfile.InitialDirectory = Path.GetDirectoryName(main_Openfile.FileName);
            Display.LoadGraph(main_Openfile.FileName);
        }
        /// <summary>
        /// 打开备份
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void main_Menu_file_backup_open_Click(object sender, EventArgs e)
        {
            if (Display.Graph != null && Display.GraphEdited)
            {
                if (MessageBox.Show("要放弃当前的修改吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }
            }
            main_Openfile.Filter = "全部文件|*.*";
            var oldInitDir = main_Openfile.InitialDirectory;
            main_Openfile.InitialDirectory = Backup.DirectoryName;
            if (main_Openfile.ShowDialog() == DialogResult.OK)
            {
                Display.LoadGraph(main_Openfile.FileName);
            }
            main_Openfile.InitialDirectory = oldInitDir;

        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void main_Menu_file_save_Click(object sender, EventArgs e)
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
        private void main_Menu_file_saveas_Click(object sender, EventArgs e)
        {
            if (Display.Graph == null)
            {
                MessageBox.Show("[2303051524]没有可以保存的图像");
                return;
            }
            main_Savefile.InitialDirectory = Path.GetDirectoryName(Display.Graph.FilePath);
            main_Savefile.FileName = Path.GetFileNameWithoutExtension(Display.Graph.FilePath) + "_new.xml";
            if (main_Savefile.ShowDialog() == DialogResult.OK)
            {
                Display.SaveAsNew(main_Savefile.FileName);
            }
        }
        /// <summary>
        /// 清空备份
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void main_Menu_file_backup_clear_Click(object sender, EventArgs e)
        {
            Backup.Clear();
        }
        /// <summary>
        /// 批量转存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void main_Menu_file_batch_saveas_Click(object sender, EventArgs e)
        {
            main_Openfile_batch.Filter = "csv文件 (.csv) |*.csv";
            if (main_Openfile_batch.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            var fileNames = main_Openfile_batch.FileNames;
            main_Openfile_batch.InitialDirectory = Path.GetDirectoryName(fileNames[0]);
            var suc = main_Openfile_batch.FileNames.Length;
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

        private void main_Menu_edit_undo_Click(object sender, EventArgs e)
        {
            Display.Undo();
            main_Menu_edit_status_check();
        }

        private void main_Menu_edit_redo_Click(object sender, EventArgs e)
        {
            Display.Redo();
            main_Menu_edit_status_check();
        }
        /// <summary>
        /// 更新撤回和重做按钮是否可用的状态
        /// </summary>
        public void main_Menu_edit_status_check()
        {
            main_Menu_edit_undo.Enabled = GraphHistory.HasPrev();
            main_Menu_edit_redo.Enabled = GraphHistory.HasNext();
        }
        private void main_Menu_edit_status_check(object sender, EventArgs e)
        {
            main_Menu_edit_undo.Enabled = GraphHistory.HasPrev();
            main_Menu_edit_redo.Enabled = GraphHistory.HasNext();
        }

        private void main_Menu_edit_Click(object sender, EventArgs e)
        {
            main_Menu_edit_status_check();
        }
        public void UpdateText()
        {
            if (Display.Graph == null)
            {
                Text = "国策树";
                main_StatusStrip_filename.Text = "未加载";
            }
            Text = Display.FileName;
            if(Display.ReadOnly)
            {
                main_StatusStrip_filename.Text = "正在预览";
            }
            else if (Display.GraphEdited)
            {
                main_StatusStrip_filename.Text = "正在编辑";
            }
            else
            {
                main_StatusStrip_filename.Text = "就绪";
            }
        }

        private void main_Menu_loc_camfocus_Click(object sender, EventArgs e)
        {
            Display.CamLocateSelected();
        }
        private void main_Menu_window_display_toolDialog_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            Display.ToolDialogs[item.Text].Show();
        }
    }
}
