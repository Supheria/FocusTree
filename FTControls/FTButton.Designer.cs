namespace FTControls
{
    partial class FTButton
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
            components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.BackColor = System.Drawing.Color.Transparent;
            this.label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label.Location = new System.Drawing.Point(0, 0);
            this.label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(mDefaultSideLength, mDefaultSideLength);
            this.label.TabIndex = 0;
            this.label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FTMouseDown);
            this.label.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FTMouseUp);
            this.label.MouseHover += new System.EventHandler(this.FTMouseHover);
            this.label.MouseLeave += new System.EventHandler(this.FTMouseLeave);
            // 
            // FTButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.label);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FTButton";
            this.Size = new System.Drawing.Size(mDefaultSideLength, mDefaultSideLength);
            this.ResumeLayout(false);
            this.DrawSign();
            this.Resize += new System.EventHandler(this.FTResize);

        }

        #endregion

        private Label label;
    }
}
