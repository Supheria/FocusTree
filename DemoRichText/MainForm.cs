using DemoRichText.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoRichText
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        

        public void btnButtonClick(object sender, EventArgs e) {
            Button btn = (Button)sender;
            BTNType btnType;
            if (Enum.TryParse<BTNType>(btn.Tag.ToString(), out btnType)) {
                if (btnType == BTNType.Search) {
                    if (!string.IsNullOrEmpty(this.txtSearch.Text.Trim()))
                    {
                        this.rtbInfo.Tag = this.txtSearch.Text.Trim();
                    }
                    else {
                        return;
                    }
                    
                }
                IRichFormat richFomat = RichFormatFactory.CreateRichFormat(btnType);
                richFomat.SetFormat(this.rtbInfo);
            }
        }

        private void combFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            float fsize = 12.0f;
            if (combFontSize.SelectedIndex > -1) {
                if (float.TryParse(combFontSize.SelectedItem.ToString(), out fsize)) {
                    rtbInfo.Tag = fsize.ToString();
                    IRichFormat richFomat = RichFormatFactory.CreateRichFormat(BTNType.FontSize);
                    richFomat.SetFormat(this.rtbInfo);
                }
                return;
            }
        }
    }
}
