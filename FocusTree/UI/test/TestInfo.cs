namespace FocusTree.UI.test
{
    public partial class TestInfo : Form
    {
        TestFormatter testFormatter = new();
        public string InfoText
        {
            get { return infoText; }
            set
            {
                infoText = value;
                Info.Text = $"错误 {erro}/{total}, 差异 {differ}/{total}, 正确 {good}/{total}\n{infoText}";
            }
        }
        string infoText = "";
        public int total = 0;
        public int erro = 0;
        public int differ = 0;
        public int good = 0;
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
            //TopMost = true;
            testFormatter.Show();
        }

        public void Initialize()
        {
            infoText = "";
            total = 0;
            erro = 0;
            differ = 0;
            good = 0;
            InfoText = string.Empty;
        }
    }
}
