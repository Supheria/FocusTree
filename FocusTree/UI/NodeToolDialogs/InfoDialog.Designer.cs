namespace FocusTree.UI.NodeToolDialogs
{
    partial class InfoDialog : NodeToolDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pcbIcon = new System.Windows.Forms.PictureBox();
            this.btnEvent = new System.Windows.Forms.Button();
            this.txtRequire = new System.Windows.Forms.TextBox();
            this.txtDescript = new System.Windows.Forms.TextBox();
            this.lblEffects = new System.Windows.Forms.Label();
            this.txtEffects = new System.Windows.Forms.TextBox();
            this.txtDuration = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pcbIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pcbIcon
            // 
            this.pcbIcon.Location = new System.Drawing.Point(12, 12);
            this.pcbIcon.Name = "pcbIcon";
            this.pcbIcon.Size = new System.Drawing.Size(100, 100);
            this.pcbIcon.TabIndex = 0;
            this.pcbIcon.TabStop = false;
            // 
            // btnEvent
            // 
            this.btnEvent.Location = new System.Drawing.Point(230, 12);
            this.btnEvent.Name = "btnEvent";
            this.btnEvent.Size = new System.Drawing.Size(66, 23);
            this.btnEvent.TabIndex = 2;
            this.btnEvent.Text = "event";
            this.btnEvent.UseVisualStyleBackColor = true;
            this.btnEvent.Click += new System.EventHandler(this.btnEvent_Click);
            // 
            // txtRequire
            // 
            this.txtRequire.AcceptsReturn = true;
            this.txtRequire.Location = new System.Drawing.Point(118, 41);
            this.txtRequire.Multiline = true;
            this.txtRequire.Name = "txtRequire";
            this.txtRequire.Size = new System.Drawing.Size(178, 71);
            this.txtRequire.TabIndex = 3;
            // 
            // txtDescript
            // 
            this.txtDescript.Location = new System.Drawing.Point(12, 118);
            this.txtDescript.Multiline = true;
            this.txtDescript.Name = "txtDescript";
            this.txtDescript.Size = new System.Drawing.Size(284, 98);
            this.txtDescript.TabIndex = 4;
            // 
            // lblEffects
            // 
            this.lblEffects.AutoSize = true;
            this.lblEffects.Location = new System.Drawing.Point(98, 235);
            this.lblEffects.Name = "lblEffects";
            this.lblEffects.Size = new System.Drawing.Size(112, 17);
            this.lblEffects.TabIndex = 5;
            this.lblEffects.Text = "==== 效果 ====";
            // 
            // txtEffects
            // 
            this.txtEffects.Location = new System.Drawing.Point(12, 255);
            this.txtEffects.Multiline = true;
            this.txtEffects.Name = "txtEffects";
            this.txtEffects.Size = new System.Drawing.Size(284, 148);
            this.txtEffects.TabIndex = 6;
            // 
            // txtDuration
            // 
            this.txtDuration.Location = new System.Drawing.Point(124, 12);
            this.txtDuration.Name = "txtDuration";
            this.txtDuration.Size = new System.Drawing.Size(100, 23);
            this.txtDuration.TabIndex = 7;
            // 
            // InfoDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 415);
            this.Controls.Add(this.txtDuration);
            this.Controls.Add(this.txtEffects);
            this.Controls.Add(this.lblEffects);
            this.Controls.Add(this.txtDescript);
            this.Controls.Add(this.txtRequire);
            this.Controls.Add(this.btnEvent);
            this.Controls.Add(this.pcbIcon);
            this.MinimizeBox = false;
            this.Name = "InfoDialog";
            this.Text = "InfoDialog";
            ((System.ComponentModel.ISupportInitialize)(this.pcbIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox pcbIcon;
        private Button btnEvent;
        private TextBox txtRequire;
        private TextBox txtDescript;
        private Label lblEffects;
        private TextBox txtEffects;
        private TextBox txtDuration;
    }
}