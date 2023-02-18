namespace FocusTree.UI
{
    partial class MainForm
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
            this.main_StatusStrip = new System.Windows.Forms.StatusStrip();
            this.main_StatusStrip_filename = new System.Windows.Forms.ToolStripStatusLabel();
            this.main_ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.main_StatusStrip_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.main_Openfile = new System.Windows.Forms.OpenFileDialog();
            this.main_Menu = new System.Windows.Forms.MenuStrip();
            this.main_Menu_file = new System.Windows.Forms.ToolStripMenuItem();
            this.main_Menu_file_new = new System.Windows.Forms.ToolStripMenuItem();
            this.main_Menu_file_open = new System.Windows.Forms.ToolStripMenuItem();
            this.main_Menu_file_open_csv = new System.Windows.Forms.ToolStripMenuItem();
            this.main_Menu_file_open_xml = new System.Windows.Forms.ToolStripMenuItem();
            this.main_Menu_file_save = new System.Windows.Forms.ToolStripMenuItem();
            this.main_Menu_file_saveas = new System.Windows.Forms.ToolStripMenuItem();
            this.main_Menu_edit = new System.Windows.Forms.ToolStripMenuItem();
            this.main_Menu_edit_undo = new System.Windows.Forms.ToolStripMenuItem();
            this.main_Menu_edit_redo = new System.Windows.Forms.ToolStripMenuItem();
            this.main_Menu_loc = new System.Windows.Forms.ToolStripMenuItem();
            this.main_Menu_loc_camreset = new System.Windows.Forms.ToolStripMenuItem();
            this.main_Menu_node = new System.Windows.Forms.ToolStripMenuItem();
            this.main_Menu_node_add = new System.Windows.Forms.ToolStripMenuItem();
            this.main_StatusStrip.SuspendLayout();
            this.main_Menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // main_StatusStrip
            // 
            this.main_StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.main_StatusStrip_filename,
            this.main_ProgressBar,
            this.main_StatusStrip_status});
            this.main_StatusStrip.Location = new System.Drawing.Point(0, 539);
            this.main_StatusStrip.Name = "main_StatusStrip";
            this.main_StatusStrip.Size = new System.Drawing.Size(784, 22);
            this.main_StatusStrip.TabIndex = 0;
            this.main_StatusStrip.Text = "statusStrip1";
            // 
            // main_StatusStrip_filename
            // 
            this.main_StatusStrip_filename.Name = "main_StatusStrip_filename";
            this.main_StatusStrip_filename.Size = new System.Drawing.Size(53, 17);
            this.main_StatusStrip_filename.Text = "加载中...";
            // 
            // main_ProgressBar
            // 
            this.main_ProgressBar.Name = "main_ProgressBar";
            this.main_ProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // main_StatusStrip_status
            // 
            this.main_StatusStrip_status.Name = "main_StatusStrip_status";
            this.main_StatusStrip_status.Size = new System.Drawing.Size(53, 17);
            this.main_StatusStrip_status.Text = "加载中...";
            // 
            // main_Openfile
            // 
            this.main_Openfile.FileName = "openFileDialog1";
            // 
            // main_Menu
            // 
            this.main_Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.main_Menu_file,
            this.main_Menu_edit,
            this.main_Menu_loc,
            this.main_Menu_node});
            this.main_Menu.Location = new System.Drawing.Point(0, 0);
            this.main_Menu.Name = "main_Menu";
            this.main_Menu.Size = new System.Drawing.Size(784, 25);
            this.main_Menu.TabIndex = 1;
            this.main_Menu.Text = "menuStrip1";
            // 
            // main_Menu_file
            // 
            this.main_Menu_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.main_Menu_file_new,
            this.main_Menu_file_open,
            this.main_Menu_file_save,
            this.main_Menu_file_saveas});
            this.main_Menu_file.Name = "main_Menu_file";
            this.main_Menu_file.Size = new System.Drawing.Size(44, 21);
            this.main_Menu_file.Text = "文件";
            // 
            // main_Menu_file_new
            // 
            this.main_Menu_file_new.Name = "main_Menu_file_new";
            this.main_Menu_file_new.Size = new System.Drawing.Size(112, 22);
            this.main_Menu_file_new.Text = "新建";
            // 
            // main_Menu_file_open
            // 
            this.main_Menu_file_open.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.main_Menu_file_open_csv,
            this.main_Menu_file_open_xml});
            this.main_Menu_file_open.Name = "main_Menu_file_open";
            this.main_Menu_file_open.Size = new System.Drawing.Size(112, 22);
            this.main_Menu_file_open.Text = "打开";
            // 
            // main_Menu_file_open_csv
            // 
            this.main_Menu_file_open_csv.Name = "main_Menu_file_open_csv";
            this.main_Menu_file_open_csv.Size = new System.Drawing.Size(127, 22);
            this.main_Menu_file_open_csv.Text = ".csv 文件";
            this.main_Menu_file_open_csv.Click += new System.EventHandler(this.main_Menu_file_open_csv_Click);
            // 
            // main_Menu_file_open_xml
            // 
            this.main_Menu_file_open_xml.Name = "main_Menu_file_open_xml";
            this.main_Menu_file_open_xml.Size = new System.Drawing.Size(127, 22);
            this.main_Menu_file_open_xml.Text = ".xml 文件";
            // 
            // main_Menu_file_save
            // 
            this.main_Menu_file_save.Name = "main_Menu_file_save";
            this.main_Menu_file_save.Size = new System.Drawing.Size(112, 22);
            this.main_Menu_file_save.Text = "保存";
            // 
            // main_Menu_file_saveas
            // 
            this.main_Menu_file_saveas.Name = "main_Menu_file_saveas";
            this.main_Menu_file_saveas.Size = new System.Drawing.Size(112, 22);
            this.main_Menu_file_saveas.Text = "另存为";
            // 
            // main_Menu_edit
            // 
            this.main_Menu_edit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.main_Menu_edit_undo,
            this.main_Menu_edit_redo});
            this.main_Menu_edit.Name = "main_Menu_edit";
            this.main_Menu_edit.Size = new System.Drawing.Size(44, 21);
            this.main_Menu_edit.Text = "编辑";
            // 
            // main_Menu_edit_undo
            // 
            this.main_Menu_edit_undo.Name = "main_Menu_edit_undo";
            this.main_Menu_edit_undo.Size = new System.Drawing.Size(100, 22);
            this.main_Menu_edit_undo.Text = "撤回";
            // 
            // main_Menu_edit_redo
            // 
            this.main_Menu_edit_redo.Name = "main_Menu_edit_redo";
            this.main_Menu_edit_redo.Size = new System.Drawing.Size(100, 22);
            this.main_Menu_edit_redo.Text = "重做";
            // 
            // main_Menu_loc
            // 
            this.main_Menu_loc.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.main_Menu_loc_camreset});
            this.main_Menu_loc.Name = "main_Menu_loc";
            this.main_Menu_loc.Size = new System.Drawing.Size(44, 21);
            this.main_Menu_loc.Text = "位置";
            // 
            // main_Menu_loc_camreset
            // 
            this.main_Menu_loc_camreset.Name = "main_Menu_loc_camreset";
            this.main_Menu_loc_camreset.Size = new System.Drawing.Size(148, 22);
            this.main_Menu_loc_camreset.Text = "重置相机位置";
            this.main_Menu_loc_camreset.Click += new System.EventHandler(this.main_Menu_loc_camreset_Click);
            // 
            // main_Menu_node
            // 
            this.main_Menu_node.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.main_Menu_node_add});
            this.main_Menu_node.Name = "main_Menu_node";
            this.main_Menu_node.Size = new System.Drawing.Size(44, 21);
            this.main_Menu_node.Text = "节点";
            // 
            // main_Menu_node_add
            // 
            this.main_Menu_node_add.Name = "main_Menu_node_add";
            this.main_Menu_node_add.Size = new System.Drawing.Size(180, 22);
            this.main_Menu_node_add.Text = "添加新节点";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.main_StatusStrip);
            this.Controls.Add(this.main_Menu);
            this.MainMenuStrip = this.main_Menu;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.main_StatusStrip.ResumeLayout(false);
            this.main_StatusStrip.PerformLayout();
            this.main_Menu.ResumeLayout(false);
            this.main_Menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private StatusStrip main_StatusStrip;
        private ToolStripStatusLabel main_StatusStrip_filename;
        private ToolStripProgressBar main_ProgressBar;
        private OpenFileDialog main_Openfile;
        private MenuStrip main_Menu;
        private ToolStripMenuItem main_Menu_file;
        private ToolStripMenuItem main_Menu_file_new;
        private ToolStripMenuItem main_Menu_file_open;
        private ToolStripMenuItem main_Menu_file_open_csv;
        private ToolStripMenuItem main_Menu_file_open_xml;
        private ToolStripMenuItem main_Menu_file_save;
        private ToolStripMenuItem main_Menu_file_saveas;
        private ToolStripMenuItem main_Menu_edit;
        private ToolStripMenuItem main_Menu_edit_undo;
        private ToolStripMenuItem main_Menu_edit_redo;
        private ToolStripStatusLabel main_StatusStrip_status;
        private ToolStripMenuItem main_Menu_loc;
        private ToolStripMenuItem main_Menu_loc_camreset;
        private ToolStripMenuItem main_Menu_node;
        private ToolStripMenuItem main_Menu_node_add;
    }
}