using FocusTree.IO;
using FocusTree.Data;

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
        }
        private void main_Menu_loc_camreset_Click(object sender, EventArgs e)
        {
            Display.RelocateCenter();
            Display.Invalidate();
        }
        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void main_Menu_file_open_Click(object sender, EventArgs e)
        {
            Display.SaveGraph();
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
            main_Savefile.InitialDirectory = Path.GetDirectoryName(Display.FilePath);
            main_Savefile.FileName = Path.GetFileNameWithoutExtension(Display.FilePath) + "_new.xml";
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
                catch(Exception ex)
                {
                    suc--;
                    MessageBox.Show($"{fileName}转存失败。\n{ex.Message}");
                }
            }
            MessageBox.Show($"成功转存{suc}个文件。");
        }

        private void main_Menu_edit_undo_Click(object sender, EventArgs e)
        {
            if (DataHistory.HasPrev())
            {
                DataHistory.Undo(Display.Graph);
            }
            main_Menu_edit_status_check();
            Display.Invalidate();
        }

        private void main_Menu_edit_redo_Click(object sender, EventArgs e)
        {
            if (DataHistory.HasNext())
            {
                DataHistory.Redo(Display.Graph);
            }
            main_Menu_edit_status_check();
            Display.Invalidate();
        }
        /// <summary>
        /// 更新撤回和重做按钮是否可用的状态
        /// </summary>
        public void main_Menu_edit_status_check()
        {
            main_Menu_edit_undo.Enabled = DataHistory.HasPrev();
            main_Menu_edit_redo.Enabled = DataHistory.HasNext();
        }
        private void main_Menu_edit_status_check(object sender, EventArgs e)
        {
            main_Menu_edit_undo.Enabled = DataHistory.HasPrev();
            main_Menu_edit_redo.Enabled = DataHistory.HasNext();
        }

        private void main_Menu_edit_Click(object sender, EventArgs e)
        {
            main_Menu_edit_status_check();
        }
        public void UpdateText()
        {
            Text = main_StatusStrip_filename.Text = Display.FileName;
        }
    }
}
