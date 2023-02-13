namespace FocusTree
{
    partial class NodeControl
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnTop = new FTControls.FTButton();
            this.btnBottom = new FTControls.FTButton();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnTop
            // 
            this.btnTop.BackColor = System.Drawing.Color.Transparent;
            this.btnTop.BackColorEx = System.Drawing.Color.Transparent;
            this.btnTop.BackColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnTop.BackColorLeave = System.Drawing.Color.Transparent;
            this.btnTop.IsPlus = true;
            this.btnTop.Location = new System.Drawing.Point(31, 7);
            this.btnTop.Margin = new System.Windows.Forms.Padding(4);
            this.btnTop.Name = "btnTop";
            this.btnTop.Size = new System.Drawing.Size(15, 15);
            this.btnTop.TabIndex = 0;
            this.btnTop.TextColor = System.Drawing.Color.Black;
            this.btnTop.TFClick += new System.EventHandler(this.btnTop_TFClick);
            // 
            // btnBottom
            // 
            this.btnBottom.BackColor = System.Drawing.Color.Transparent;
            this.btnBottom.BackColorEx = System.Drawing.Color.Transparent;
            this.btnBottom.BackColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnBottom.BackColorLeave = System.Drawing.Color.Transparent;
            this.btnBottom.IsPlus = true;
            this.btnBottom.Location = new System.Drawing.Point(31, 52);
            this.btnBottom.Margin = new System.Windows.Forms.Padding(4);
            this.btnBottom.Name = "btnBottom";
            this.btnBottom.Size = new System.Drawing.Size(15, 15);
            this.btnBottom.TabIndex = 2;
            this.btnBottom.TextColor = System.Drawing.Color.Black;
            this.btnBottom.TFClick += new System.EventHandler(this.btnBottom_TFClick);
            // 
            // txtTitle
            // 
            this.txtTitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTitle.Location = new System.Drawing.Point(-1, 29);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(78, 16);
            this.txtTitle.TabIndex = 3;
            this.txtTitle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtTitle.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtTitle_MouseClick);
            // 
            // NodeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.btnBottom);
            this.Controls.Add(this.btnTop);
            this.Name = "NodeControl";
            this.Size = new System.Drawing.Size(78, 78);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NodeControl_MouseClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FTControls.FTButton btnTop;
        private FTControls.FTButton btnBottom;
        private TextBox txtTitle;
    }
}
