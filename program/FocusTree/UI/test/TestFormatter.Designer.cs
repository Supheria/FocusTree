namespace FocusTree.UI.test
{
    partial class TestFormatter
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
            Input = new RichTextBox();
            Output = new RichTextBox();
            richTextBox1 = new RichTextBox();
            SuspendLayout();
            // 
            // Input
            // 
            Input.Anchor = AnchorStyles.Bottom;
            Input.Location = new Point(0, 0);
            Input.Name = "Input";
            Input.Size = new Size(800, 181);
            Input.TabIndex = 0;
            Input.Text = "";
            // 
            // Output
            // 
            Output.Anchor = AnchorStyles.Top;
            Output.Location = new Point(0, 262);
            Output.Name = "Output";
            Output.Size = new Size(800, 188);
            Output.TabIndex = 1;
            Output.Text = "";
            // 
            // richTextBox1
            // 
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Location = new Point(0, 0);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(800, 450);
            richTextBox1.TabIndex = 2;
            richTextBox1.Text = "";
            richTextBox1.Visible = false;
            // 
            // TestFormatter
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(richTextBox1);
            Controls.Add(Output);
            Controls.Add(Input);
            Name = "TestFormatter";
            Text = "TestFormatter";
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox Input;
        private RichTextBox Output;
        private RichTextBox richTextBox1;
    }
}