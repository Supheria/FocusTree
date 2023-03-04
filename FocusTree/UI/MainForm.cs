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
            Controls.Add(Display);
            main_StatusStrip_filename.Text = "等待打开文件";
            main_StatusStrip_status.Text = "";
            main_Openfile.FileName = "";
        }

        private void main_Menu_file_open_csv_Click(object sender, EventArgs e)
        {
            main_Openfile.Filter = "csv files (.csv) |*.csv";
            var result = main_Openfile.ShowDialog();
            if (result == DialogResult.OK)
            {
                Display.Graph = new FocusGraph(main_Openfile.FileName);
                Display.RelocateCenter();
                Display.Invalidate();
                main_StatusStrip_filename.Text = Path.GetFileNameWithoutExtension(main_Openfile.FileName);
            }
        }

        private void main_Menu_loc_camreset_Click(object sender, EventArgs e)
        {
            Display.RelocateCenter();
            Display.Invalidate();
        }

        private void main_Menu_file_open_xml_Click(object sender, EventArgs e)
        {
            main_Openfile.Filter = "xml files (.xml) |*.xml";
            var result = main_Openfile.ShowDialog();
            if (result == DialogResult.OK)
            {
                Display.Graph = XmlIO.LoadGraph(main_Openfile.FileName);
                Display.RelocateCenter();
                Display.Invalidate();
                main_StatusStrip_filename.Text = Path.GetFileNameWithoutExtension(main_Openfile.FileName);
            }
        }

        private void main_Menu_file_save_Click(object sender, EventArgs e)
        {
            if (Display.Graph == null || Display.Graph.FilePath == null)
            {
                MessageBox.Show("[2302191440]错误，当前没有可保存的有效文件"); return;
            }
            var dir = Directory.CreateDirectory("backups");
            var path = Display.Graph.FilePath;
            var savepath = Path.Combine(dir.FullName, DateTime.Now.ToString("yyyyMMddHHmmss ") + Path.GetFileName(path));
            // 备份文件
            var path_xml = Path.ChangeExtension(path, ".xml");
            if (File.Exists(path_xml))
            {
                File.Copy(path_xml, Path.ChangeExtension(savepath, ".xml"), true);
            }
            else
            {
                File.Copy(path, savepath, true);
            }
            // 保存
            XmlIO.SaveGraph(path_xml, Display.Graph);
            Display.Graph.SetFileName(path_xml);
        }
        [Obsolete("这个功能还没写完")]
        private void main_Menu_file_saveas_Click(object sender, EventArgs e)
        {
            MessageBox.Show("这个功能还没写完");
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
    }
}
