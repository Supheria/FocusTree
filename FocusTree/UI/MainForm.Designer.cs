using FocusTree.IO;

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
            this.MainForm_StatusStrip = new System.Windows.Forms.StatusStrip();
            this.MainForm_StatusStrip_filename = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainForm_ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.MainForm_StatusStrip_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainForm_Openfile = new System.Windows.Forms.OpenFileDialog();
            this.MainForm_Openfile_batch = new System.Windows.Forms.OpenFileDialog();
            this.MainForm_Savefile = new System.Windows.Forms.SaveFileDialog();
            this.MainForm_Menu = new System.Windows.Forms.MenuStrip();
            this.MainForm_Menu_file = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_file_new = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_file_open = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_file_save = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_file_saveas = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MainForm_Menu_file_backup = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_file_backup_open = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_file_backup_clear = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_file_batch_saveas = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_edit = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_edit_undo = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_edit_redo = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_loc = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_loc_camreset = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_loc_camfocus = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_node = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_node_add = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_Menu_window = new System.Windows.Forms.ToolStripMenuItem();
            this.MainForm_StatusStrip.SuspendLayout();
            this.MainForm_Menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainForm_StatusStrip
            // 
            this.MainForm_StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainForm_StatusStrip_filename,
            this.MainForm_ProgressBar,
            this.MainForm_StatusStrip_status});
            this.MainForm_StatusStrip.Location = new System.Drawing.Point(0, 539);
            this.MainForm_StatusStrip.Name = "MainForm_StatusStrip";
            this.MainForm_StatusStrip.Size = new System.Drawing.Size(784, 22);
            this.MainForm_StatusStrip.TabIndex = 0;
            this.MainForm_StatusStrip.Text = "statusStrip1";
            // 
            // MainForm_StatusStrip_filename
            // 
            this.MainForm_StatusStrip_filename.Name = "MainForm_StatusStrip_filename";
            this.MainForm_StatusStrip_filename.Size = new System.Drawing.Size(53, 17);
            this.MainForm_StatusStrip_filename.Text = "加载中...";
            // 
            // MainForm_ProgressBar
            // 
            this.MainForm_ProgressBar.Name = "MainForm_ProgressBar";
            this.MainForm_ProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // MainForm_StatusStrip_status
            // 
            this.MainForm_StatusStrip_status.Name = "MainForm_StatusStrip_status";
            this.MainForm_StatusStrip_status.Size = new System.Drawing.Size(53, 17);
            this.MainForm_StatusStrip_status.Text = "加载中...";
            // 
            // MainForm_Openfile
            // 
            this.MainForm_Openfile.Title = "打开单个文件";
            // 
            // MainForm_Openfile_batch
            // 
            this.MainForm_Openfile_batch.Multiselect = true;
            this.MainForm_Openfile_batch.Title = "打开一个或多个文件";
            // 
            // MainForm_Savefile
            // 
            this.MainForm_Savefile.Filter = "xml文件 (.xml) |*.xml";
            this.MainForm_Savefile.Title = "另存为";
            // 
            // MainForm_Menu
            // 
            this.MainForm_Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainForm_Menu_file,
            this.MainForm_Menu_edit,
            this.MainForm_Menu_loc,
            this.MainForm_Menu_node,
            this.MainForm_Menu_window});
            this.MainForm_Menu.Location = new System.Drawing.Point(0, 0);
            this.MainForm_Menu.Name = "MainForm_Menu";
            this.MainForm_Menu.Size = new System.Drawing.Size(784, 25);
            this.MainForm_Menu.TabIndex = 1;
            this.MainForm_Menu.Text = "menuStrip1";
            // 
            // MainForm_Menu_file
            // 
            this.MainForm_Menu_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainForm_Menu_file_new,
            this.MainForm_Menu_file_open,
            this.MainForm_Menu_file_save,
            this.MainForm_Menu_file_saveas,
            this.MainForm_Menu_file_batch_saveas,
            this.toolStripSeparator1,
            this.MainForm_Menu_file_backup});
            this.MainForm_Menu_file.Name = "MainForm_Menu_file";
            this.MainForm_Menu_file.Size = new System.Drawing.Size(44, 21);
            this.MainForm_Menu_file.Text = "文件";
            // 
            // MainForm_Menu_file_new
            // 
            this.MainForm_Menu_file_new.Name = "MainForm_Menu_file_new";
            this.MainForm_Menu_file_new.Size = new System.Drawing.Size(124, 22);
            this.MainForm_Menu_file_new.Text = "新建";
            // 
            // MainForm_Menu_file_open
            // 
            this.MainForm_Menu_file_open.Name = "MainForm_Menu_file_open";
            this.MainForm_Menu_file_open.Size = new System.Drawing.Size(124, 22);
            this.MainForm_Menu_file_open.Text = "打开";
            this.MainForm_Menu_file_open.Click += new System.EventHandler(this.MainForm_Menu_file_open_Click);
            // 
            // MainForm_Menu_file_save
            // 
            this.MainForm_Menu_file_save.Name = "MainForm_Menu_file_save";
            this.MainForm_Menu_file_save.Size = new System.Drawing.Size(124, 22);
            this.MainForm_Menu_file_save.Text = "保存";
            this.MainForm_Menu_file_save.Click += new System.EventHandler(this.MainForm_Menu_file_save_Click);
            // 
            // MainForm_Menu_file_saveas
            // 
            this.MainForm_Menu_file_saveas.Name = "MainForm_Menu_file_saveas";
            this.MainForm_Menu_file_saveas.Size = new System.Drawing.Size(124, 22);
            this.MainForm_Menu_file_saveas.Text = "另存为";
            this.MainForm_Menu_file_saveas.Click += new System.EventHandler(this.MainForm_Menu_file_saveas_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(121, 6);
            // 
            // MainForm_Menu_file_backup
            // 
            this.MainForm_Menu_file_backup.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainForm_Menu_file_backup_open,
            this.MainForm_Menu_file_backup_clear});
            this.MainForm_Menu_file_backup.Name = "MainForm_Menu_file_backup";
            this.MainForm_Menu_file_backup.Size = new System.Drawing.Size(124, 22);
            this.MainForm_Menu_file_backup.Text = "备份";
            // 
            // MainForm_Menu_file_backup_open
            // 
            this.MainForm_Menu_file_backup_open.Name = "MainForm_Menu_file_backup_open";
            this.MainForm_Menu_file_backup_open.Size = new System.Drawing.Size(100, 22);
            this.MainForm_Menu_file_backup_open.Text = "打开";
            this.MainForm_Menu_file_backup_open.Click += new System.EventHandler(this.MainForm_Menu_file_backup_open_Click);
            // 
            // MainForm_Menu_file_backup_clear
            // 
            this.MainForm_Menu_file_backup_clear.Name = "MainForm_Menu_file_backup_clear";
            this.MainForm_Menu_file_backup_clear.Size = new System.Drawing.Size(100, 22);
            this.MainForm_Menu_file_backup_clear.Text = "清空";
            this.MainForm_Menu_file_backup_clear.Click += new System.EventHandler(this.MainForm_Menu_file_backup_clear_Click);
            // 
            // MainForm_Menu_file_batch_saveas
            // 
            this.MainForm_Menu_file_batch_saveas.Name = "MainForm_Menu_file_batch_saveas";
            this.MainForm_Menu_file_batch_saveas.Size = new System.Drawing.Size(124, 22);
            this.MainForm_Menu_file_batch_saveas.Text = "批量转存";
            this.MainForm_Menu_file_batch_saveas.Click += new System.EventHandler(this.MainForm_Menu_file_batch_saveas_Click);
            // 
            // MainForm_Menu_edit
            // 
            this.MainForm_Menu_edit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainForm_Menu_edit_undo,
            this.MainForm_Menu_edit_redo});
            this.MainForm_Menu_edit.Name = "MainForm_Menu_edit";
            this.MainForm_Menu_edit.Size = new System.Drawing.Size(44, 21);
            this.MainForm_Menu_edit.Text = "编辑";
            this.MainForm_Menu_edit.DropDownClosed += new System.EventHandler(this.MainForm_Menu_edit_status_check);
            this.MainForm_Menu_edit.DropDownOpening += new System.EventHandler(this.MainForm_Menu_edit_status_check);
            this.MainForm_Menu_edit.Click += new System.EventHandler(this.MainForm_Menu_edit_Click);
            // 
            // MainForm_Menu_edit_undo
            // 
            this.MainForm_Menu_edit_undo.Enabled = false;
            this.MainForm_Menu_edit_undo.Name = "MainForm_Menu_edit_undo";
            this.MainForm_Menu_edit_undo.Size = new System.Drawing.Size(100, 22);
            this.MainForm_Menu_edit_undo.Text = "撤回";
            this.MainForm_Menu_edit_undo.Click += new System.EventHandler(this.MainForm_Menu_edit_undo_Click);
            // 
            // MainForm_Menu_edit_redo
            // 
            this.MainForm_Menu_edit_redo.Enabled = false;
            this.MainForm_Menu_edit_redo.Name = "MainForm_Menu_edit_redo";
            this.MainForm_Menu_edit_redo.Size = new System.Drawing.Size(100, 22);
            this.MainForm_Menu_edit_redo.Text = "重做";
            this.MainForm_Menu_edit_redo.Click += new System.EventHandler(this.MainForm_Menu_edit_redo_Click);
            // 
            // MainForm_Menu_loc
            // 
            this.MainForm_Menu_loc.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainForm_Menu_loc_camreset,
            this.MainForm_Menu_loc_camfocus});
            this.MainForm_Menu_loc.Name = "MainForm_Menu_loc";
            this.MainForm_Menu_loc.Size = new System.Drawing.Size(44, 21);
            this.MainForm_Menu_loc.Text = "位置";
            // 
            // MainForm_Menu_loc_camreset
            // 
            this.MainForm_Menu_loc_camreset.Name = "MainForm_Menu_loc_camreset";
            this.MainForm_Menu_loc_camreset.Size = new System.Drawing.Size(100, 22);
            this.MainForm_Menu_loc_camreset.Text = "全景";
            this.MainForm_Menu_loc_camreset.Click += new System.EventHandler(this.MainForm_Menu_camera_panorama_Click);
            // 
            // MainForm_Menu_loc_camfocus
            // 
            this.MainForm_Menu_loc_camfocus.Name = "MainForm_Menu_loc_camfocus";
            this.MainForm_Menu_loc_camfocus.Size = new System.Drawing.Size(100, 22);
            this.MainForm_Menu_loc_camfocus.Text = "聚焦";
            this.MainForm_Menu_loc_camfocus.Click += new System.EventHandler(this.MainForm_Menu_camera_focus_Click);
            // 
            // MainForm_Menu_node
            // 
            this.MainForm_Menu_node.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainForm_Menu_node_add});
            this.MainForm_Menu_node.Name = "MainForm_Menu_node";
            this.MainForm_Menu_node.Size = new System.Drawing.Size(44, 21);
            this.MainForm_Menu_node.Text = "节点";
            // 
            // MainForm_Menu_node_add
            // 
            this.MainForm_Menu_node_add.Name = "MainForm_Menu_node_add";
            this.MainForm_Menu_node_add.Size = new System.Drawing.Size(136, 22);
            this.MainForm_Menu_node_add.Text = "添加新节点";
            // 
            // MainForm_Menu_window
            // 
            this.MainForm_Menu_window.Name = "MainForm_Menu_window";
            this.MainForm_Menu_window.Size = new System.Drawing.Size(44, 21);
            this.MainForm_Menu_window.Text = "窗口";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.MainForm_StatusStrip);
            this.Controls.Add(this.MainForm_Menu);
            this.MainMenuStrip = this.MainForm_Menu;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.MainForm_StatusStrip.ResumeLayout(false);
            this.MainForm_StatusStrip.PerformLayout();
            this.MainForm_Menu.ResumeLayout(false);
            this.MainForm_Menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private StatusStrip MainForm_StatusStrip;
        private ToolStripStatusLabel MainForm_StatusStrip_filename;
        private ToolStripProgressBar MainForm_ProgressBar;
        private OpenFileDialog MainForm_Openfile;
        private OpenFileDialog MainForm_Openfile_batch;
        private SaveFileDialog MainForm_Savefile;
        private MenuStrip MainForm_Menu;
        private ToolStripMenuItem MainForm_Menu_file;
        private ToolStripMenuItem MainForm_Menu_file_new;
        private ToolStripMenuItem MainForm_Menu_file_open;
        private ToolStripMenuItem MainForm_Menu_file_save;
        private ToolStripMenuItem MainForm_Menu_file_saveas;
        private ToolStripMenuItem MainForm_Menu_edit;
        private ToolStripMenuItem MainForm_Menu_edit_undo;
        private ToolStripMenuItem MainForm_Menu_edit_redo;
        private ToolStripStatusLabel MainForm_StatusStrip_status;
        private ToolStripMenuItem MainForm_Menu_loc;
        private ToolStripMenuItem MainForm_Menu_loc_camreset;
        private ToolStripMenuItem MainForm_Menu_node;
        private ToolStripMenuItem MainForm_Menu_node_add;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem MainForm_Menu_file_batch_saveas;
        private ToolStripMenuItem MainForm_Menu_file_backup;
        private ToolStripMenuItem MainForm_Menu_file_backup_open;
        private ToolStripMenuItem MainForm_Menu_file_backup_clear;
        private ToolStripMenuItem MainForm_Menu_loc_camfocus;
        private ToolStripMenuItem MainForm_Menu_window;
    }
}