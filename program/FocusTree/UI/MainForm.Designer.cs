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
            MainForm_StatusStrip = new StatusStrip();
            MainForm_StatusStrip_status = new ToolStripStatusLabel();
            MainForm_ProgressBar = new ToolStripProgressBar();
            MainForm_StatusStrip_filename = new ToolStripStatusLabel();
            MainForm_Openfile = new OpenFileDialog();
            MainForm_Openfile_batch = new OpenFileDialog();
            MainForm_Savefile = new SaveFileDialog();
            MainForm_Menu = new MenuStrip();
            MainForm_Menu_file = new ToolStripMenuItem();
            MainForm_Menu_file_new = new ToolStripMenuItem();
            MainForm_Menu_file_open = new ToolStripMenuItem();
            MainForm_Menu_file_save = new ToolStripMenuItem();
            MainForm_Menu_file_saveas = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            MainForm_Menu_file_backup = new ToolStripMenuItem();
            MainForm_Menu_file_backup_open = new ToolStripMenuItem();
            MainForm_Menu_file_backup_delete = new ToolStripMenuItem();
            MainForm_Menu_file_backup_seperator = new ToolStripSeparator();
            MainForm_Menu_file_backup_clear = new ToolStripMenuItem();
            MainForm_Menu_edit = new ToolStripMenuItem();
            MainForm_Menu_edit_undo = new ToolStripMenuItem();
            MainForm_Menu_edit_redo = new ToolStripMenuItem();
            MainForm_Menu_loc = new ToolStripMenuItem();
            MainForm_Menu_loc_camreset = new ToolStripMenuItem();
            MainForm_Menu_loc_camfocus = new ToolStripMenuItem();
            MainForm_Menu_node = new ToolStripMenuItem();
            MainForm_Menu_node_add = new ToolStripMenuItem();
            MainForm_Menu_window = new ToolStripMenuItem();
            MainForm_Menu_graph = new ToolStripMenuItem();
            MainForm_Menu_graph_reorderIds = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            MainForm_Menu_graph_saveas = new ToolStripMenuItem();
            MainForm_Menu_batch = new ToolStripMenuItem();
            MainForm_Menu_batch_reorderIds = new ToolStripMenuItem();
            MainForm_Menu_batch_saveasImage = new ToolStripMenuItem();
            MainForm_StatusStrip.SuspendLayout();
            MainForm_Menu.SuspendLayout();
            SuspendLayout();
            // 
            // MainForm_StatusStrip
            // 
            MainForm_StatusStrip.ImageScalingSize = new Size(24, 24);
            MainForm_StatusStrip.Items.AddRange(new ToolStripItem[] { MainForm_StatusStrip_status, MainForm_ProgressBar, MainForm_StatusStrip_filename });
            MainForm_StatusStrip.Location = new Point(0, 761);
            MainForm_StatusStrip.Name = "MainForm_StatusStrip";
            MainForm_StatusStrip.Padding = new Padding(2, 0, 22, 0);
            MainForm_StatusStrip.Size = new Size(1232, 31);
            MainForm_StatusStrip.TabIndex = 0;
            MainForm_StatusStrip.Text = "statusStrip1";
            // 
            // MainForm_StatusStrip_status
            // 
            MainForm_StatusStrip_status.Name = "MainForm_StatusStrip_status";
            MainForm_StatusStrip_status.Size = new Size(46, 24);
            MainForm_StatusStrip_status.Text = "状态";
            // 
            // MainForm_ProgressBar
            // 
            MainForm_ProgressBar.Name = "MainForm_ProgressBar";
            MainForm_ProgressBar.Size = new Size(157, 23);
            MainForm_ProgressBar.Step = 1;
            // 
            // MainForm_StatusStrip_filename
            // 
            MainForm_StatusStrip_filename.Name = "MainForm_StatusStrip_filename";
            MainForm_StatusStrip_filename.Size = new Size(46, 24);
            MainForm_StatusStrip_filename.Text = "文件";
            // 
            // MainForm_Openfile
            // 
            MainForm_Openfile.Title = "打开单个文件";
            // 
            // MainForm_Openfile_batch
            // 
            MainForm_Openfile_batch.Multiselect = true;
            MainForm_Openfile_batch.Title = "打开一个或多个文件";
            // 
            // MainForm_Savefile
            // 
            MainForm_Savefile.Filter = "xml文件 (.xml) |*.xml";
            MainForm_Savefile.Title = "另存为";
            // 
            // MainForm_Menu
            // 
            MainForm_Menu.ImageScalingSize = new Size(24, 24);
            MainForm_Menu.Items.AddRange(new ToolStripItem[] { MainForm_Menu_file, MainForm_Menu_edit, MainForm_Menu_node, MainForm_Menu_graph, MainForm_Menu_loc, MainForm_Menu_window, MainForm_Menu_batch });
            MainForm_Menu.Location = new Point(0, 0);
            MainForm_Menu.Name = "MainForm_Menu";
            MainForm_Menu.Padding = new Padding(9, 3, 0, 3);
            MainForm_Menu.Size = new Size(1232, 34);
            MainForm_Menu.TabIndex = 1;
            MainForm_Menu.Text = "menuStrip1";
            // 
            // MainForm_Menu_file
            // 
            MainForm_Menu_file.DropDownItems.AddRange(new ToolStripItem[] { MainForm_Menu_file_new, MainForm_Menu_file_open, MainForm_Menu_file_save, MainForm_Menu_file_saveas, toolStripSeparator1, MainForm_Menu_file_backup });
            MainForm_Menu_file.Name = "MainForm_Menu_file";
            MainForm_Menu_file.Size = new Size(62, 28);
            MainForm_Menu_file.Text = "文件";
            // 
            // MainForm_Menu_file_new
            // 
            MainForm_Menu_file_new.Name = "MainForm_Menu_file_new";
            MainForm_Menu_file_new.Size = new Size(270, 34);
            MainForm_Menu_file_new.Text = "新建";
            // 
            // MainForm_Menu_file_open
            // 
            MainForm_Menu_file_open.Name = "MainForm_Menu_file_open";
            MainForm_Menu_file_open.Size = new Size(270, 34);
            MainForm_Menu_file_open.Text = "打开";
            MainForm_Menu_file_open.Click += MainForm_Menu_file_open_Click;
            // 
            // MainForm_Menu_file_save
            // 
            MainForm_Menu_file_save.Name = "MainForm_Menu_file_save";
            MainForm_Menu_file_save.Size = new Size(270, 34);
            MainForm_Menu_file_save.Text = "保存";
            MainForm_Menu_file_save.Click += MainForm_Menu_file_save_Click;
            // 
            // MainForm_Menu_file_saveas
            // 
            MainForm_Menu_file_saveas.Name = "MainForm_Menu_file_saveas";
            MainForm_Menu_file_saveas.Size = new Size(270, 34);
            MainForm_Menu_file_saveas.Text = "另存为";
            MainForm_Menu_file_saveas.Click += MainForm_Menu_file_saveas_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(267, 6);
            // 
            // MainForm_Menu_file_backup
            // 
            MainForm_Menu_file_backup.DropDownItems.AddRange(new ToolStripItem[] { MainForm_Menu_file_backup_open, MainForm_Menu_file_backup_delete, MainForm_Menu_file_backup_seperator, MainForm_Menu_file_backup_clear });
            MainForm_Menu_file_backup.Name = "MainForm_Menu_file_backup";
            MainForm_Menu_file_backup.Size = new Size(270, 34);
            MainForm_Menu_file_backup.Text = "备份";
            MainForm_Menu_file_backup.DropDownOpening += MainForm_Menu_file_backup_DropDownOpening;
            MainForm_Menu_file_backup.DropDownOpened += MainForm_Menu_file_backup_DropDownOpened;
            // 
            // MainForm_Menu_file_backup_open
            // 
            MainForm_Menu_file_backup_open.Name = "MainForm_Menu_file_backup_open";
            MainForm_Menu_file_backup_open.Size = new Size(200, 34);
            MainForm_Menu_file_backup_open.Text = "打开";
            // 
            // MainForm_Menu_file_backup_delete
            // 
            MainForm_Menu_file_backup_delete.Name = "MainForm_Menu_file_backup_delete";
            MainForm_Menu_file_backup_delete.Size = new Size(200, 34);
            MainForm_Menu_file_backup_delete.Text = "删除";
            MainForm_Menu_file_backup_delete.Click += MainForm_Menu_file_backup_delete_Click;
            // 
            // MainForm_Menu_file_backup_seperator
            // 
            MainForm_Menu_file_backup_seperator.Name = "MainForm_Menu_file_backup_seperator";
            MainForm_Menu_file_backup_seperator.Size = new Size(197, 6);
            // 
            // MainForm_Menu_file_backup_clear
            // 
            MainForm_Menu_file_backup_clear.Name = "MainForm_Menu_file_backup_clear";
            MainForm_Menu_file_backup_clear.Size = new Size(200, 34);
            MainForm_Menu_file_backup_clear.Text = "打包并清空";
            MainForm_Menu_file_backup_clear.Click += MainForm_Menu_file_backup_clear_Click;
            // 
            // MainForm_Menu_edit
            // 
            MainForm_Menu_edit.DropDownItems.AddRange(new ToolStripItem[] { MainForm_Menu_edit_undo, MainForm_Menu_edit_redo });
            MainForm_Menu_edit.Name = "MainForm_Menu_edit";
            MainForm_Menu_edit.Size = new Size(62, 28);
            MainForm_Menu_edit.Text = "编辑";
            MainForm_Menu_edit.DropDownClosed += MainForm_Menu_edit_status_check;
            MainForm_Menu_edit.DropDownOpening += MainForm_Menu_edit_status_check;
            MainForm_Menu_edit.Click += MainForm_Menu_edit_Click;
            // 
            // MainForm_Menu_edit_undo
            // 
            MainForm_Menu_edit_undo.Enabled = false;
            MainForm_Menu_edit_undo.Name = "MainForm_Menu_edit_undo";
            MainForm_Menu_edit_undo.Size = new Size(146, 34);
            MainForm_Menu_edit_undo.Text = "撤回";
            MainForm_Menu_edit_undo.Click += MainForm_Menu_edit_undo_Click;
            // 
            // MainForm_Menu_edit_redo
            // 
            MainForm_Menu_edit_redo.Enabled = false;
            MainForm_Menu_edit_redo.Name = "MainForm_Menu_edit_redo";
            MainForm_Menu_edit_redo.Size = new Size(146, 34);
            MainForm_Menu_edit_redo.Text = "重做";
            MainForm_Menu_edit_redo.Click += MainForm_Menu_edit_redo_Click;
            // 
            // MainForm_Menu_loc
            // 
            MainForm_Menu_loc.DropDownItems.AddRange(new ToolStripItem[] { MainForm_Menu_loc_camreset, MainForm_Menu_loc_camfocus });
            MainForm_Menu_loc.Name = "MainForm_Menu_loc";
            MainForm_Menu_loc.Size = new Size(62, 28);
            MainForm_Menu_loc.Text = "位置";
            // 
            // MainForm_Menu_loc_camreset
            // 
            MainForm_Menu_loc_camreset.Name = "MainForm_Menu_loc_camreset";
            MainForm_Menu_loc_camreset.Size = new Size(270, 34);
            MainForm_Menu_loc_camreset.Text = "全景";
            MainForm_Menu_loc_camreset.Click += MainForm_Menu_camera_panorama_Click;
            // 
            // MainForm_Menu_loc_camfocus
            // 
            MainForm_Menu_loc_camfocus.Name = "MainForm_Menu_loc_camfocus";
            MainForm_Menu_loc_camfocus.Size = new Size(270, 34);
            MainForm_Menu_loc_camfocus.Text = "聚焦";
            MainForm_Menu_loc_camfocus.Click += MainForm_Menu_camera_focus_Click;
            // 
            // MainForm_Menu_node
            // 
            MainForm_Menu_node.DropDownItems.AddRange(new ToolStripItem[] { MainForm_Menu_node_add });
            MainForm_Menu_node.Name = "MainForm_Menu_node";
            MainForm_Menu_node.Size = new Size(62, 28);
            MainForm_Menu_node.Text = "节点";
            // 
            // MainForm_Menu_node_add
            // 
            MainForm_Menu_node_add.Name = "MainForm_Menu_node_add";
            MainForm_Menu_node_add.Size = new Size(200, 34);
            MainForm_Menu_node_add.Text = "添加新节点";
            // 
            // MainForm_Menu_window
            // 
            MainForm_Menu_window.Name = "MainForm_Menu_window";
            MainForm_Menu_window.Size = new Size(62, 28);
            MainForm_Menu_window.Text = "窗口";
            // 
            // MainForm_Menu_graph
            // 
            MainForm_Menu_graph.DropDownItems.AddRange(new ToolStripItem[] { MainForm_Menu_graph_reorderIds, toolStripSeparator2, MainForm_Menu_graph_saveas });
            MainForm_Menu_graph.Name = "MainForm_Menu_graph";
            MainForm_Menu_graph.Size = new Size(62, 28);
            MainForm_Menu_graph.Text = "图像";
            // 
            // MainForm_Menu_graph_reorderIds
            // 
            MainForm_Menu_graph_reorderIds.Name = "MainForm_Menu_graph_reorderIds";
            MainForm_Menu_graph_reorderIds.Size = new Size(270, 34);
            MainForm_Menu_graph_reorderIds.Text = "重排节点id";
            MainForm_Menu_graph_reorderIds.Click += MainForm_Menu_graph_reorderIds_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(267, 6);
            // 
            // MainForm_Menu_graph_saveas
            // 
            MainForm_Menu_graph_saveas.Name = "MainForm_Menu_graph_saveas";
            MainForm_Menu_graph_saveas.Size = new Size(270, 34);
            MainForm_Menu_graph_saveas.Text = "生成图片";
            MainForm_Menu_graph_saveas.Click += MainForm_Menu_graph_saveas_Click;
            // 
            // MainForm_Menu_batch
            // 
            MainForm_Menu_batch.DropDownItems.AddRange(new ToolStripItem[] { MainForm_Menu_batch_reorderIds, MainForm_Menu_batch_saveasImage });
            MainForm_Menu_batch.Name = "MainForm_Menu_batch";
            MainForm_Menu_batch.Size = new Size(62, 28);
            MainForm_Menu_batch.Text = "批量";
            // 
            // MainForm_Menu_batch_reorderIds
            // 
            MainForm_Menu_batch_reorderIds.Name = "MainForm_Menu_batch_reorderIds";
            MainForm_Menu_batch_reorderIds.Size = new Size(270, 34);
            MainForm_Menu_batch_reorderIds.Text = "重排节点ID";
            MainForm_Menu_batch_reorderIds.Click += MainForm_Menu_batch_reorderIds_Click;
            // 
            // MainForm_Menu_batch_saveasImage
            // 
            MainForm_Menu_batch_saveasImage.Name = "MainForm_Menu_batch_saveasImage";
            MainForm_Menu_batch_saveasImage.Size = new Size(270, 34);
            MainForm_Menu_batch_saveasImage.Text = "生成图片";
            MainForm_Menu_batch_saveasImage.Click += MainForm_Menu_batch_saveasImage_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1232, 792);
            Controls.Add(MainForm_StatusStrip);
            Controls.Add(MainForm_Menu);
            MainMenuStrip = MainForm_Menu;
            Margin = new Padding(5, 4, 5, 4);
            Name = "MainForm";
            Text = "MainForm";
            FormClosing += MainForm_FormClosing;
            MainForm_StatusStrip.ResumeLayout(false);
            MainForm_StatusStrip.PerformLayout();
            MainForm_Menu.ResumeLayout(false);
            MainForm_Menu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private ToolStripMenuItem MainForm_Menu_file_backup;
        private ToolStripMenuItem MainForm_Menu_file_backup_open;
        private ToolStripMenuItem MainForm_Menu_file_backup_clear;
        private ToolStripMenuItem MainForm_Menu_loc_camfocus;
        private ToolStripMenuItem MainForm_Menu_window;
        private ToolStripMenuItem MainForm_Menu_graph;
        private ToolStripMenuItem MainForm_Menu_graph_saveas;
        private ToolStripMenuItem MainForm_Menu_graph_reorderIds;
        private ToolStripMenuItem MainForm_Menu_batch;
        private ToolStripMenuItem MainForm_Menu_batch_reorderIds;
        private ToolStripMenuItem MainForm_Menu_batch_saveasImage;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem MainForm_Menu_file_backup_delete;
        private ToolStripSeparator MainForm_Menu_file_backup_seperator;
    }
}