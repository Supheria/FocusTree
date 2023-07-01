using System.Text;
using FocusTree.Data.Focus;
using FocusTree.IO;

namespace FocusTree.UI.test
{
    public partial class TestInfo : Form
    {
        TestFormatter testFormatter = new();

        HashSet<string> _infoText = new();
        public int Total = 0;
        public int Error = 0;
        public int Differ = 0;
        public int Good = 0;
        public TestInfo()
        {
            InitializeComponent();
            Info.Left = ClientRectangle.Left;
            Info.Top = ClientRectangle.Top;
            Info.Width = ClientRectangle.Width;
            Info.Height = ClientRectangle.Height;
            Info.Dock = DockStyle.Fill;
            Info.WordWrap = false;
            Info.ZoomFactor = 2f;
            Closing += (sender, args) =>
            {
                Hide();
                renew();
                args.Cancel = true;
            };
            //TopMost = true;
            //testFormatter.Show();
        }

        public void Append(string text)
        {
            _infoText.Add(text);
            var sb = new StringBuilder().AppendLine($"错误 {Error}/{Total}, 差异 {Differ}/{Total}, 正确 {Good}/{Total}")
                .AppendLine();
            foreach (var info in _infoText)
                sb.AppendLine(info);
            Info.Text = sb.ToString();
        }

        public void Initialize()
        {
            _infoText = new();
            Total = 0;
            Error = 0;
            Differ = 0;
            Good = 0;
        }

        public void renew()
        {
            _infoText = new();
            Total = 0;
            Error = 0;
            Differ = 0;
            Good = 0;
        }

        private void ToolStripMenuItemOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new();
            openFile.Filter = "所有文件|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                Initialize();
                Text = openFile.FileName;
                var g = XmlIO.LoadFromXml<FocusGraph>(openFile.FileName);
                Info.Text += "\n\n=====Successfull=====\n";
                foreach (var focus in g.FocusList)
                {
                    Info.Text += focus.Id + ". ";
                    foreach (var effect in g[focus.Id].RawEffects)
                    {
                        Info.Text += effect.ToString() + "\n";
                    }
                }
            }
        }
    }
}
