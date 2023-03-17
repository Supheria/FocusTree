using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoRichText.Model
{
    public class DefaultRickFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {

        }
    }

    /// <summary>
    /// 加粗格式
    /// </summary>
    public class BoldRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            Font oldFont = rtbInfo.SelectionFont;
            Font newFont;
            if (oldFont.Bold)
            {
                newFont = new Font(oldFont, oldFont.Style & ~FontStyle.Bold);//支持位于运算
            }
            else
            {
                newFont = new Font(oldFont, oldFont.Style | FontStyle.Bold);
            }
            rtbInfo.SelectionFont = newFont;
        }
    }

    /// <summary>
    /// 斜体
    /// </summary>
    public class ItalicRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            Font oldFont = rtbInfo.SelectionFont;
            Font newFont;
            if (oldFont.Italic)
            {
                newFont = new Font(oldFont, oldFont.Style & ~FontStyle.Italic);
            }
            else
            {
                newFont = new Font(oldFont, oldFont.Style | FontStyle.Italic);
            }
            rtbInfo.SelectionFont = newFont;
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 下划线
    /// </summary>
    public class UnderLineRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            Font oldFont = rtbInfo.SelectionFont;
            Font newFont;
            if (oldFont.Underline)
            {
                newFont = new Font(oldFont, oldFont.Style & ~FontStyle.Underline);
            }
            else
            {
                newFont = new Font(oldFont, oldFont.Style | FontStyle.Underline);
            }
            rtbInfo.SelectionFont = newFont;
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 删除线
    /// </summary>
    public class StrikeLineRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            Font oldFont = rtbInfo.SelectionFont;
            Font newFont;
            if (oldFont.Underline)
            {
                newFont = new Font(oldFont, oldFont.Style & ~FontStyle.Strikeout);
            }
            else
            {
                newFont = new Font(oldFont, oldFont.Style | FontStyle.Strikeout);
            }
            rtbInfo.SelectionFont = newFont;
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 左对齐
    /// </summary>
    public class LeftRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            rtbInfo.SelectionAlignment = HorizontalAlignment.Left;
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 居中对齐
    /// </summary>
    public class CenterRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            if (rtbInfo.SelectionAlignment == HorizontalAlignment.Center)
            {
                rtbInfo.SelectionAlignment = HorizontalAlignment.Left;
            }
            else
            {
                rtbInfo.SelectionAlignment = HorizontalAlignment.Center;
            }

            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 右对齐
    /// </summary>
    public class RightRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            if (rtbInfo.SelectionAlignment == HorizontalAlignment.Right)
            {
                rtbInfo.SelectionAlignment = HorizontalAlignment.Left;
            }
            else
            {
                rtbInfo.SelectionAlignment = HorizontalAlignment.Right;
            }

            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 缩进对齐
    /// </summary>
    public class IndentRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            //每次以10个像素进行缩进
            rtbInfo.SelectionIndent = rtbInfo.SelectionIndent + 10;
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 缩进对齐
    /// </summary>
    public class OutIndentRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            //每次以10个像素进行缩进
            rtbInfo.SelectionIndent = rtbInfo.SelectionIndent - 10;
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 下标
    /// </summary>
    public class SubScriptRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            if (rtbInfo.SelectionCharOffset < 0)
            {
                rtbInfo.SelectionCharOffset = 0;
            }
            else {
                rtbInfo.SelectionCharOffset = -5;
            }
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 上标
    /// </summary>
    public class SuperScriptRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            if (rtbInfo.SelectionCharOffset > 0)
            {
                rtbInfo.SelectionCharOffset = 0;
            }
            else {
                rtbInfo.SelectionCharOffset = 5;
            }
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 字体
    /// </summary>
    public class FontRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            FontDialog f = new FontDialog();
            if (f.ShowDialog() == DialogResult.OK)
            {
                FontFamily family = f.Font.FontFamily;
                rtbInfo.SelectionFont = new Font(family, rtbInfo.SelectionFont.Size, rtbInfo.SelectionFont.Style);
            }
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 文本颜色
    /// </summary>
    public class ForeColorRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            ColorDialog f = new ColorDialog();
            if (f.ShowDialog() == DialogResult.OK)
            {

                rtbInfo.SelectionColor = f.Color;
            }
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 文本背景颜色
    /// </summary>
    public class BgColorRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            ColorDialog f = new ColorDialog();
            if (f.ShowDialog() == DialogResult.OK)
            {

                rtbInfo.SelectionBackColor = f.Color;
            }
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// UL列表,项目符号样式
    /// </summary>
    public class UlRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            if (rtbInfo.SelectionBullet)
            {
                rtbInfo.SelectionBullet = false;
            }
            else {
                rtbInfo.SelectionBullet = true;
                rtbInfo.BulletIndent = 10;
            }
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 图片插入
    /// </summary>
    public class PicRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            o.Title = "请选择图片";
            o.Filter = "jpeg|*.jpeg|jpg|*.jpg|png|*.png|gif|*.gif"; 
            if (o.ShowDialog() == DialogResult.OK) {
                string fileName = o.FileName;
                try
                {
                   Image bmp = Image.FromFile(fileName);
                   Clipboard.SetDataObject(bmp);

                    DataFormats.Format dataFormat = DataFormats.GetFormat(DataFormats.Bitmap);
                    if (rtbInfo.CanPaste(dataFormat))
                    {
                        rtbInfo.Paste(dataFormat);
                    }
                        
                }
                catch (Exception exc)
                {
                    MessageBox.Show("图片插入失败。" + exc.Message, "提示",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 删除
    /// </summary>
    public class DelRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            rtbInfo.SelectedText = "";
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 查找
    /// </summary>
    public class SearchRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            string find = rtbInfo.Tag.ToString();
            int index=  rtbInfo.Find(find, 0,RichTextBoxFinds.None);
            int startPos = index;
            int nextIndex = 0;
            while (nextIndex != startPos)//循环查找字符串，并用蓝色加粗12号Times New Roman标记之  
            {
                rtbInfo.SelectionStart = index;
                rtbInfo.SelectionLength = find.Length;
                rtbInfo.SelectionColor = Color.Blue;
                rtbInfo.SelectionFont = new Font("Times New Roman", (float)12, FontStyle.Bold);
                rtbInfo.Focus();
                nextIndex = rtbInfo.Find(find, index + find.Length, RichTextBoxFinds.None);
                if (nextIndex == -1)//若查到文件末尾，则充值nextIndex为初始位置的值，使其达到初始位置，顺利结束循环，否则会有异常。  
                {
                    nextIndex = startPos;
                }
                index = nextIndex;
            }
            rtbInfo.Focus();
        }
    }

    /// <summary>
    /// 打印
    /// </summary>
    public class PrintRichFormat : BaseRichFormat
    {
        private RichTextBox richTextbox;

        public override void SetFormat(RichTextBox rtbInfo)
        {
            this.richTextbox = rtbInfo;
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
            // 打印文档
            pd.Print();
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            //ev.Graphics.DrawString(richTextbox.Text);
            //ev.HasMorePages = true;
        }
    }

    /// <summary>
    /// 字体大小
    /// </summary>
    public class FontSizeRichFormat : BaseRichFormat
    {
        public override void SetFormat(RichTextBox rtbInfo)
        {
            string fontSize = rtbInfo.Tag.ToString();
            float fsize = 0.0f;
            if (float.TryParse(fontSize, out fsize)) {
                rtbInfo.SelectionFont = new Font(rtbInfo.Font.FontFamily, fsize, rtbInfo.SelectionFont.Style);
            }
            rtbInfo.Focus();
        }
    }
}
