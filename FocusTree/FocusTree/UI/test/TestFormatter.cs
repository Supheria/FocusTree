using System.Text;
using FocusTree.Data.Hoi4Object;

namespace FocusTree.UI.test
{
    public partial class TestFormatter : Form
    {
        public TestFormatter()
        {
            InitializeComponent();
            Input.TextChanged += Input_TextChanged;
            SizeChanged += TestFormatter_SizeChanged;
            DrawClient();
        }

        private void DrawClient()
        {
            int padding = 20;

            Input.Left = ClientRectangle.Left;
            Input.Top = ClientRectangle.Top;
            Input.Width = ClientRectangle.Width;
            Input.Height = (int)(ClientRectangle.Height * 0.5) - padding;

            Output.Left = ClientRectangle.Left;
            Output.Top = Input.Bottom + padding * 2;
            Output.Width = Input.Width;
            Output.Height = Input.Height;
            Output.ReadOnly = true;
            Output.WordWrap = false;
        }

        private void TestFormatter_SizeChanged(object sender, EventArgs e)
        {
            DrawClient();
        }

        private void Input_TextChanged(object sender, EventArgs e)
        {
            Output.Text = "";
            if (!FormatRawEffectSentence.Formatter(Input.Text, out var formatted))
                return;
            var sb = new StringBuilder();
            foreach (var sentence in formatted)
                sb.AppendLine(sentence.ToString());
            Output.Text = sb.ToString();
        }

    }
}
