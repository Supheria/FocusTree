namespace FocusTree.UI
{
    partial class GraphTest
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
            gBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)gBox).BeginInit();
            SuspendLayout();
            // 
            // gBox
            // 
            gBox.Dock = DockStyle.Fill;
            gBox.Location = new Point(0, 0);
            gBox.Name = "gBox";
            gBox.Size = new Size(800, 450);
            gBox.TabIndex = 0;
            gBox.TabStop = false;
            // 
            // GraphTest
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(gBox);
            Name = "GraphTest";
            Text = "GraphTest";
            ((System.ComponentModel.ISupportInitialize)gBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        public PictureBox gBox;
    }
}