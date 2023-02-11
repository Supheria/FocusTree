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
            this.lblTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnTop
            // 
            this.btnTop.BackColor = System.Drawing.Color.Transparent;
            this.btnTop.BackColorEx = System.Drawing.Color.Transparent;
            this.btnTop.BackColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnTop.BackColorLeave = System.Drawing.Color.Transparent;
            this.btnTop.IsPlus = true;
            this.btnTop.Location = new System.Drawing.Point(32, 0);
            this.btnTop.Margin = new System.Windows.Forms.Padding(4);
            this.btnTop.Name = "btnTop";
            this.btnTop.Size = new System.Drawing.Size(15, 15);
            this.btnTop.TabIndex = 0;
            this.btnTop.TextColor = System.Drawing.Color.Black;
            // 
            // btnBottom
            // 
            this.btnBottom.BackColor = System.Drawing.Color.Transparent;
            this.btnBottom.BackColorEx = System.Drawing.Color.Transparent;
            this.btnBottom.BackColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnBottom.BackColorLeave = System.Drawing.Color.Transparent;
            this.btnBottom.IsPlus = true;
            this.btnBottom.Location = new System.Drawing.Point(32, 41);
            this.btnBottom.Margin = new System.Windows.Forms.Padding(4);
            this.btnBottom.Name = "btnBottom";
            this.btnBottom.Size = new System.Drawing.Size(15, 15);
            this.btnBottom.TabIndex = 2;
            this.btnBottom.TextColor = System.Drawing.Color.Black;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(18, 19);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(29, 17);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "title";
            // 
            // NodeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnBottom);
            this.Controls.Add(this.btnTop);
            this.Name = "NodeControl";
            this.Size = new System.Drawing.Size(78, 60);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FTControls.FTButton btnTop;
        private FTControls.FTButton btnBottom;
        private Label lblTitle;
    }
}
