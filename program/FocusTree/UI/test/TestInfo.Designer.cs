namespace FocusTree.UI.test
{
    partial class TestInfo
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
            Info = new RichTextBox();
            menuStrip1 = new MenuStrip();
            ToolStripMenuItemOpen = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // Info
            // 
            Info.Location = new Point(14, 55);
            Info.Margin = new Padding(5, 4, 5, 4);
            Info.Name = "Info";
            Info.Size = new Size(155, 134);
            Info.TabIndex = 0;
            Info.Text = "";
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { ToolStripMenuItemOpen });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1257, 32);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // ToolStripMenuItemOpen
            // 
            ToolStripMenuItemOpen.Name = "ToolStripMenuItemOpen";
            ToolStripMenuItemOpen.Size = new Size(62, 28);
            ToolStripMenuItemOpen.Text = "打开";
            ToolStripMenuItemOpen.Click += ToolStripMenuItemOpen_Click;
            // 
            // TestInfo
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1257, 635);
            Controls.Add(Info);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(5, 4, 5, 4);
            Name = "TestInfo";
            Text = "TestInfo";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox Info;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem ToolStripMenuItemOpen;
    }
}