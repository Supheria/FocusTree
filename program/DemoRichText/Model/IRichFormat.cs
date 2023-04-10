using System.Windows.Forms;

namespace DemoRichText.Model
{
    /// <summary>
    /// 富文本框格式
    /// </summary>
    public interface IRichFormat
    {
        void SetFormat(RichTextBox rtbInfo);
    }
}
