using FocusTree.IO;
using FocusTree.Data;

namespace FocusTree.UI
{
    public partial class MainForm : Form
    {
        GraphBox Display;
        private void InitDisplay(string path)
        {
            Display.Graph = XmlIO.LoadGraph(path);
            Display.RelocateCenter();
            Display.Invalidate();
        }
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
        private void main_Menu_file_open_Click(object sender, EventArgs e)
        {
            if (main_Openfile.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            main_Openfile.InitialDirectory = Path.GetDirectoryName(main_Openfile.FileName);
            InitDisplay(main_Openfile.FileName);
            main_StatusStrip_filename.Text = Path.GetFileNameWithoutExtension(main_Openfile.FileName);
        }
        private void main_Menu_file_save_Click(object sender, EventArgs e)
        {
            if (Display.Graph == null)
            {
                MessageBox.Show("[2302191440]没有可以保存的图像"); 
                return;
            }
            if (Path.GetDirectoryName(Display.Graph.FilePath) != Backup.DirectoryName)
            {
                Backup.BackupFile(Display.Graph.FilePath);
                XmlIO.SaveGraph(Display.Graph.FilePath, Display.Graph);
                return;
            }
            main_Savefile.FileName = Path.GetFileNameWithoutExtension(Display.Graph.FilePath) + "_new.xml";
            if (main_Savefile.ShowDialog() == DialogResult.OK)
            {
                XmlIO.SaveGraph(main_Savefile.FileName, Display.Graph);
                main_StatusStrip_filename.Text = Path.GetFileNameWithoutExtension(main_Savefile.FileName);
            }
        }
        [Obsolete("这个功能还没完善")]
        private void main_Menu_file_saveas_Click(object sender, EventArgs e)
        {
            if (Display.Graph == null)
            {
                MessageBox.Show("[2303051524]没有可以保存的图像");
                return;
            }
            main_Savefile.InitialDirectory = Path.GetDirectoryName(Display.Graph.FilePath);
            main_Savefile.FileName = Path.GetFileNameWithoutExtension(Display.Graph.FilePath) + "_new.xml";
            if (main_Savefile.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            XmlIO.SaveGraph(main_Savefile.FileName, Display.Graph);
            main_StatusStrip_filename.Text = Path.GetFileNameWithoutExtension(main_Savefile.FileName);
        }
        private void main_Menu_file_backup_open_Click(object sender, EventArgs e)
        {
            var oldInitDir = main_Openfile.InitialDirectory;
            main_Openfile.InitialDirectory = Backup.DirectoryName;
            if (main_Openfile.ShowDialog() == DialogResult.Cancel)
            {
                main_Openfile.InitialDirectory = oldInitDir;
                return;
            }
            main_Openfile.InitialDirectory = oldInitDir;
            InitDisplay(main_Openfile.FileName);
            main_StatusStrip_filename.Text = Path.GetFileNameWithoutExtension(main_Openfile.FileName);
        }
        private void main_Menu_file_batch_saveas_Click(object sender, EventArgs e)
        {
            
            if (main_Openfile_batch.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            var fileNames = main_Openfile_batch.FileNames;
            main_Openfile_batch.InitialDirectory = Path.GetDirectoryName(fileNames[0]);
            foreach (var fileName in fileNames)
            {
                var graph = new FocusGraph(fileName);
                var xmlPath = Path.ChangeExtension(fileName, ".xml");
                Backup.BackupFile(xmlPath);
                XmlIO.SaveGraph(xmlPath, graph);
            }
            MessageBox.Show($"成功转存{main_Openfile_batch.FileNames.Length}个文件。");
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

        private void main_Menu_file_backup_clear_Click(object sender, EventArgs e)
        {
            Backup.Clear();
        }
    }
}
