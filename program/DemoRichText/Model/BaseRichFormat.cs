using System.Windows.Forms;

namespace DemoRichText.Model
{
    public abstract class BaseRichFormat : IRichFormat
    {
        public abstract void SetFormat(RichTextBox rtbInfo);
    }
}
