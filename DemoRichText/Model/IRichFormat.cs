using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
