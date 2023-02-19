using FocusTree.Focus;
using FocusTree.IO;

namespace FocusTree.UI
{
    public partial class MainForm : Form
    {
        DisplayBox Display;
        public MainForm()
        {
            Display = new DisplayBox(this);
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
                Display.Graph = new FGraph(main_Openfile.FileName);
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
                Display.Graph = FXml.LoadGraph(main_Openfile.FileName);
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
            File.Copy(path, savepath, true);
            var path_xml = Path.ChangeExtension(path, ".xml");
            // 保存
            FXml.SaveGraph(path_xml, Display.Graph);
            Display.Graph.SetFileName(path_xml);
        }
        [Obsolete("这个功能还没写完")]
        private void main_Menu_file_saveas_Click(object sender, EventArgs e)
        {
            MessageBox.Show("这个功能还没写完");
        }
    }
}
