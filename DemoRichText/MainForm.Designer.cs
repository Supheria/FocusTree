using System.Windows.Forms;

namespace DemoRichText
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.combFontSize = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnUl = new System.Windows.Forms.Button();
            this.btnForeColor = new System.Windows.Forms.Button();
            this.btnSuperScript = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnOutIdent = new System.Windows.Forms.Button();
            this.btnBackgroundColor = new System.Windows.Forms.Button();
            this.btnSubScript = new System.Windows.Forms.Button();
            this.btnPic = new System.Windows.Forms.Button();
            this.bntPrint = new System.Windows.Forms.Button();
            this.btnIdent = new System.Windows.Forms.Button();
            this.btnFont = new System.Windows.Forms.Button();
            this.btnStrikeLine = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnUnderLine = new System.Windows.Forms.Button();
            this.btnCenter = new System.Windows.Forms.Button();
            this.btnItalic = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnBold = new System.Windows.Forms.Button();
            this.rtbInfo = new System.Windows.Forms.RichTextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.combFontSize);
            this.splitContainer1.Panel1.Controls.Add(this.txtSearch);
            this.splitContainer1.Panel1.Controls.Add(this.btnDel);
            this.splitContainer1.Panel1.Controls.Add(this.btnUl);
            this.splitContainer1.Panel1.Controls.Add(this.btnForeColor);
            this.splitContainer1.Panel1.Controls.Add(this.btnSuperScript);
            this.splitContainer1.Panel1.Controls.Add(this.btnSearch);
            this.splitContainer1.Panel1.Controls.Add(this.btnOutIdent);
            this.splitContainer1.Panel1.Controls.Add(this.btnBackgroundColor);
            this.splitContainer1.Panel1.Controls.Add(this.btnSubScript);
            this.splitContainer1.Panel1.Controls.Add(this.btnPic);
            this.splitContainer1.Panel1.Controls.Add(this.bntPrint);
            this.splitContainer1.Panel1.Controls.Add(this.btnIdent);
            this.splitContainer1.Panel1.Controls.Add(this.btnFont);
            this.splitContainer1.Panel1.Controls.Add(this.btnStrikeLine);
            this.splitContainer1.Panel1.Controls.Add(this.btnRight);
            this.splitContainer1.Panel1.Controls.Add(this.btnUnderLine);
            this.splitContainer1.Panel1.Controls.Add(this.btnCenter);
            this.splitContainer1.Panel1.Controls.Add(this.btnItalic);
            this.splitContainer1.Panel1.Controls.Add(this.btnLeft);
            this.splitContainer1.Panel1.Controls.Add(this.btnBold);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBox1);
            this.splitContainer1.Panel2.Controls.Add(this.rtbInfo);
            this.splitContainer1.Size = new System.Drawing.Size(534, 362);
            this.splitContainer1.SplitterDistance = 94;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 0;
            // 
            // combFontSize
            // 
            this.combFontSize.FormattingEnabled = true;
            this.combFontSize.Items.AddRange(new object[] {
            "5",
            "5.5",
            "6.5",
            "7.5",
            "8",
            "9",
            "10",
            "10.5",
            "11",
            "12",
            "14",
            "16",
            "18",
            "20",
            "22",
            "24",
            "26",
            "28",
            "36",
            "48",
            "72"});
            this.combFontSize.Location = new System.Drawing.Point(152, 38);
            this.combFontSize.Name = "combFontSize";
            this.combFontSize.Size = new System.Drawing.Size(72, 20);
            this.combFontSize.TabIndex = 2;
            this.combFontSize.Text = "字体大小";
            this.combFontSize.SelectedIndexChanged += new System.EventHandler(this.combFontSize_SelectedIndexChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(13, 38);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(108, 21);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnDel
            // 
            this.btnDel.Image = global::DemoRichText.Properties.Resources.del;
            this.btnDel.Location = new System.Drawing.Point(488, 65);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(25, 25);
            this.btnDel.TabIndex = 0;
            this.btnDel.Tag = "Del";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnUl
            // 
            this.btnUl.Image = global::DemoRichText.Properties.Resources.ul;
            this.btnUl.Location = new System.Drawing.Point(320, 65);
            this.btnUl.Name = "btnUl";
            this.btnUl.Size = new System.Drawing.Size(25, 25);
            this.btnUl.TabIndex = 0;
            this.btnUl.Tag = "Ul";
            this.btnUl.UseVisualStyleBackColor = true;
            this.btnUl.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnForeColor
            // 
            this.btnForeColor.Image = global::DemoRichText.Properties.Resources.background;
            this.btnForeColor.Location = new System.Drawing.Point(152, 65);
            this.btnForeColor.Name = "btnForeColor";
            this.btnForeColor.Size = new System.Drawing.Size(25, 25);
            this.btnForeColor.TabIndex = 0;
            this.btnForeColor.Tag = "ForeColor";
            this.btnForeColor.UseVisualStyleBackColor = true;
            this.btnForeColor.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnSuperScript
            // 
            this.btnSuperScript.Image = global::DemoRichText.Properties.Resources.superscript;
            this.btnSuperScript.Location = new System.Drawing.Point(404, 65);
            this.btnSuperScript.Name = "btnSuperScript";
            this.btnSuperScript.Size = new System.Drawing.Size(25, 25);
            this.btnSuperScript.TabIndex = 0;
            this.btnSuperScript.Tag = "SuperScript";
            this.btnSuperScript.UseVisualStyleBackColor = true;
            this.btnSuperScript.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnSearch
            // 
            this.btnSearch.Image = global::DemoRichText.Properties.Resources.help_search;
            this.btnSearch.Location = new System.Drawing.Point(124, 35);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(25, 25);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Tag = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnOutIdent
            // 
            this.btnOutIdent.Image = global::DemoRichText.Properties.Resources.outdent;
            this.btnOutIdent.Location = new System.Drawing.Point(292, 65);
            this.btnOutIdent.Name = "btnOutIdent";
            this.btnOutIdent.Size = new System.Drawing.Size(25, 25);
            this.btnOutIdent.TabIndex = 0;
            this.btnOutIdent.Tag = "OutIndent";
            this.btnOutIdent.UseVisualStyleBackColor = true;
            this.btnOutIdent.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnBackgroundColor
            // 
            this.btnBackgroundColor.Image = global::DemoRichText.Properties.Resources.foreground;
            this.btnBackgroundColor.Location = new System.Drawing.Point(124, 65);
            this.btnBackgroundColor.Name = "btnBackgroundColor";
            this.btnBackgroundColor.Size = new System.Drawing.Size(25, 25);
            this.btnBackgroundColor.TabIndex = 0;
            this.btnBackgroundColor.Tag = "BGColor";
            this.btnBackgroundColor.UseVisualStyleBackColor = true;
            this.btnBackgroundColor.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnSubScript
            // 
            this.btnSubScript.Image = global::DemoRichText.Properties.Resources.subscript;
            this.btnSubScript.Location = new System.Drawing.Point(376, 65);
            this.btnSubScript.Name = "btnSubScript";
            this.btnSubScript.Size = new System.Drawing.Size(25, 25);
            this.btnSubScript.TabIndex = 0;
            this.btnSubScript.Tag = "SubScript";
            this.btnSubScript.UseVisualStyleBackColor = true;
            this.btnSubScript.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnPic
            // 
            this.btnPic.Image = global::DemoRichText.Properties.Resources.picture;
            this.btnPic.Location = new System.Drawing.Point(432, 65);
            this.btnPic.Name = "btnPic";
            this.btnPic.Size = new System.Drawing.Size(25, 25);
            this.btnPic.TabIndex = 0;
            this.btnPic.Tag = "Pic";
            this.btnPic.UseVisualStyleBackColor = true;
            this.btnPic.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // bntPrint
            // 
            this.bntPrint.Image = global::DemoRichText.Properties.Resources.help_print;
            this.bntPrint.Location = new System.Drawing.Point(460, 65);
            this.bntPrint.Name = "bntPrint";
            this.bntPrint.Size = new System.Drawing.Size(25, 25);
            this.bntPrint.TabIndex = 0;
            this.bntPrint.Tag = "Print";
            this.bntPrint.UseVisualStyleBackColor = true;
            this.bntPrint.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnIdent
            // 
            this.btnIdent.Image = global::DemoRichText.Properties.Resources.indent;
            this.btnIdent.Location = new System.Drawing.Point(264, 65);
            this.btnIdent.Name = "btnIdent";
            this.btnIdent.Size = new System.Drawing.Size(25, 25);
            this.btnIdent.TabIndex = 0;
            this.btnIdent.Tag = "Indent";
            this.btnIdent.UseVisualStyleBackColor = true;
            this.btnIdent.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnFont
            // 
            this.btnFont.Image = global::DemoRichText.Properties.Resources.font;
            this.btnFont.Location = new System.Drawing.Point(96, 65);
            this.btnFont.Name = "btnFont";
            this.btnFont.Size = new System.Drawing.Size(25, 25);
            this.btnFont.TabIndex = 0;
            this.btnFont.Tag = "Font";
            this.btnFont.UseVisualStyleBackColor = true;
            this.btnFont.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnStrikeLine
            // 
            this.btnStrikeLine.Image = global::DemoRichText.Properties.Resources.strikeout;
            this.btnStrikeLine.Location = new System.Drawing.Point(348, 65);
            this.btnStrikeLine.Name = "btnStrikeLine";
            this.btnStrikeLine.Size = new System.Drawing.Size(25, 25);
            this.btnStrikeLine.TabIndex = 0;
            this.btnStrikeLine.Tag = "StrikeLine";
            this.btnStrikeLine.UseVisualStyleBackColor = true;
            this.btnStrikeLine.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnRight
            // 
            this.btnRight.Image = global::DemoRichText.Properties.Resources.align_right;
            this.btnRight.Location = new System.Drawing.Point(236, 65);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(25, 25);
            this.btnRight.TabIndex = 0;
            this.btnRight.Tag = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnUnderLine
            // 
            this.btnUnderLine.Image = global::DemoRichText.Properties.Resources.underline;
            this.btnUnderLine.Location = new System.Drawing.Point(68, 65);
            this.btnUnderLine.Name = "btnUnderLine";
            this.btnUnderLine.Size = new System.Drawing.Size(25, 25);
            this.btnUnderLine.TabIndex = 0;
            this.btnUnderLine.Tag = "UnderLine";
            this.btnUnderLine.UseVisualStyleBackColor = true;
            this.btnUnderLine.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnCenter
            // 
            this.btnCenter.Image = global::DemoRichText.Properties.Resources.align_center;
            this.btnCenter.Location = new System.Drawing.Point(208, 65);
            this.btnCenter.Name = "btnCenter";
            this.btnCenter.Size = new System.Drawing.Size(25, 25);
            this.btnCenter.TabIndex = 0;
            this.btnCenter.Tag = "Center";
            this.btnCenter.UseVisualStyleBackColor = true;
            this.btnCenter.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnItalic
            // 
            this.btnItalic.Image = global::DemoRichText.Properties.Resources.italic;
            this.btnItalic.Location = new System.Drawing.Point(40, 65);
            this.btnItalic.Name = "btnItalic";
            this.btnItalic.Size = new System.Drawing.Size(25, 25);
            this.btnItalic.TabIndex = 0;
            this.btnItalic.Tag = "Italic";
            this.btnItalic.UseVisualStyleBackColor = true;
            this.btnItalic.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnLeft
            // 
            this.btnLeft.Image = global::DemoRichText.Properties.Resources.align_left;
            this.btnLeft.Location = new System.Drawing.Point(180, 65);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(25, 25);
            this.btnLeft.TabIndex = 0;
            this.btnLeft.Tag = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // btnBold
            // 
            this.btnBold.Image = global::DemoRichText.Properties.Resources.bold;
            this.btnBold.Location = new System.Drawing.Point(12, 65);
            this.btnBold.Name = "btnBold";
            this.btnBold.Size = new System.Drawing.Size(25, 25);
            this.btnBold.TabIndex = 0;
            this.btnBold.Tag = "Bold";
            this.btnBold.UseVisualStyleBackColor = true;
            this.btnBold.Click += new System.EventHandler(this.btnButtonClick);
            // 
            // rtbInfo
            // 
            this.rtbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbInfo.Location = new System.Drawing.Point(0, 0);
            this.rtbInfo.Name = "rtbInfo";
            this.rtbInfo.Size = new System.Drawing.Size(534, 266);
            this.rtbInfo.TabIndex = 0;
            this.rtbInfo.Text = "";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(236, 74);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 1;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(534, 362);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.Text = "Demo RichTextbox---MainForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private SplitContainer splitContainer1;
        private RichTextBox rtbInfo;

        #endregion

        private Button btnDel;
        private Button btnUl;
        private Button btnForeColor;
        private Button btnSearch;
        private Button btnOutIdent;
        private Button btnBackgroundColor;
        private Button bntPrint;
        private Button btnIdent;
        private Button btnFont;
        private Button btnStrikeLine;
        private Button btnRight;
        private Button btnUnderLine;
        private Button btnCenter;
        private Button btnItalic;
        private Button btnLeft;
        private Button btnBold;
        private Button btnSuperScript;
        private Button btnSubScript;
        private Button btnPic;
        private TextBox txtSearch;
        private ComboBox combFontSize;
        private TextBox textBox1;
    }
}

