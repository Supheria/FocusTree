using FocusTree.IO;

namespace FocusTree.UI
{
    partial class GraphForm
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
            GraphFrom_StatusStrip = new StatusStrip();
            GraphFrom_StatusStrip_status = new ToolStripStatusLabel();
            GraphFrom_ProgressBar = new ToolStripProgressBar();
            GraphFrom_StatusStrip_filename = new ToolStripStatusLabel();
            GraphFrom_Openfile = new OpenFileDialog();
            GraphFrom_Openfile_batch = new OpenFileDialog();
            GraphFrom_Savefile = new SaveFileDialog();
            GraphFrom_Menu = new MenuStrip();
            GraphFrom_Menu_file = new ToolStripMenuItem();
            GraphFrom_Menu_file_new = new ToolStripMenuItem();
            GraphFrom_Menu_file_open = new ToolStripMenuItem();
            GraphFrom_Menu_file_save = new ToolStripMenuItem();
            GraphFrom_Menu_file_saveas = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            GraphFrom_Menu_file_backup = new ToolStripMenuItem();
            GraphFrom_Menu_file_backup_open = new ToolStripMenuItem();
            GraphFrom_Menu_file_backup_delete = new ToolStripMenuItem();
            GraphFrom_Menu_file_backup_seperator = new ToolStripSeparator();
            GraphFrom_Menu_file_backup_clear = new ToolStripMenuItem();
            GraphFrom_Menu_file_backup_unpack = new ToolStripMenuItem();
            GraphFrom_Menu_edit = new ToolStripMenuItem();
            GraphFrom_Menu_edit_undo = new ToolStripMenuItem();
            GraphFrom_Menu_edit_redo = new ToolStripMenuItem();
            GraphFrom_Menu_node = new ToolStripMenuItem();
            GraphFrom_Menu_node_add = new ToolStripMenuItem();
            GraphFrom_Menu_graph = new ToolStripMenuItem();
            GraphFrom_Menu_graph_reorderIds = new ToolStripMenuItem();
            GraphFrom_Menu_graph_setNodePointAuto = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            GraphFrom_Menu_graph_saveas = new ToolStripMenuItem();
            GraphFrom_Menu_loc = new ToolStripMenuItem();
            GraphFrom_Menu_loc_camreset = new ToolStripMenuItem();
            GraphFrom_Menu_loc_camfocus = new ToolStripMenuItem();
            GraphFrom_Menu_window = new ToolStripMenuItem();
            GraphFrom_Menu_batch = new ToolStripMenuItem();
            GraphFrom_Menu_batch_reorderIds = new ToolStripMenuItem();
            GraphFrom_Menu_batch_saveasImage = new ToolStripMenuItem();
            GraphFrom_Menu_setting = new ToolStripMenuItem();
            GraphFrom_Menu_setting_backImage = new ToolStripMenuItem();
            GraphFrom_Menu_setting_backImage_show = new ToolStripMenuItem();
            GraphFrom_StatusStrip.SuspendLayout();
            GraphFrom_Menu.SuspendLayout();
            SuspendLayout();
            // 
            // GraphFrom_StatusStrip
            // 
            GraphFrom_StatusStrip.ImageScalingSize = new Size(24, 24);
            GraphFrom_StatusStrip.Items.AddRange(new ToolStripItem[] { GraphFrom_StatusStrip_status, GraphFrom_ProgressBar, GraphFrom_StatusStrip_filename });
            GraphFrom_StatusStrip.Location = new Point(0, 537);
            GraphFrom_StatusStrip.Name = "GraphFrom_StatusStrip";
            GraphFrom_StatusStrip.Size = new Size(784, 24);
            GraphFrom_StatusStrip.TabIndex = 0;
            GraphFrom_StatusStrip.Text = "statusStrip1";
            // 
            // GraphFrom_StatusStrip_status
            // 
            GraphFrom_StatusStrip_status.Name = "GraphFrom_StatusStrip_status";
            GraphFrom_StatusStrip_status.Size = new Size(32, 19);
            GraphFrom_StatusStrip_status.Text = "状态";
            // 
            // GraphFrom_ProgressBar
            // 
            GraphFrom_ProgressBar.Name = "GraphFrom_ProgressBar";
            GraphFrom_ProgressBar.Size = new Size(100, 18);
            GraphFrom_ProgressBar.Step = 1;
            // 
            // GraphFrom_StatusStrip_filename
            // 
            GraphFrom_StatusStrip_filename.Name = "GraphFrom_StatusStrip_filename";
            GraphFrom_StatusStrip_filename.Size = new Size(32, 19);
            GraphFrom_StatusStrip_filename.Text = "文件";
            // 
            // GraphFrom_Openfile
            // 
            GraphFrom_Openfile.Title = "打开单个文件";
            // 
            // GraphFrom_Openfile_batch
            // 
            GraphFrom_Openfile_batch.Multiselect = true;
            GraphFrom_Openfile_batch.Title = "打开一个或多个文件";
            // 
            // GraphFrom_Savefile
            // 
            GraphFrom_Savefile.Filter = "xml文件 (.xml) |*.xml";
            GraphFrom_Savefile.Title = "另存为";
            // 
            // GraphFrom_Menu
            // 
            GraphFrom_Menu.ImageScalingSize = new Size(24, 24);
            GraphFrom_Menu.Items.AddRange(new ToolStripItem[] { GraphFrom_Menu_file, GraphFrom_Menu_edit, GraphFrom_Menu_node, GraphFrom_Menu_graph, GraphFrom_Menu_loc, GraphFrom_Menu_window, GraphFrom_Menu_batch, GraphFrom_Menu_setting });
            GraphFrom_Menu.Location = new Point(0, 0);
            GraphFrom_Menu.Name = "GraphFrom_Menu";
            GraphFrom_Menu.Size = new Size(784, 25);
            GraphFrom_Menu.TabIndex = 1;
            GraphFrom_Menu.Text = "menuStrip1";
            // 
            // GraphFrom_Menu_file
            // 
            GraphFrom_Menu_file.DropDownItems.AddRange(new ToolStripItem[] { GraphFrom_Menu_file_new, GraphFrom_Menu_file_open, GraphFrom_Menu_file_save, GraphFrom_Menu_file_saveas, toolStripSeparator1, GraphFrom_Menu_file_backup });
            GraphFrom_Menu_file.Name = "GraphFrom_Menu_file";
            GraphFrom_Menu_file.Size = new Size(44, 21);
            GraphFrom_Menu_file.Text = "文件";
            // 
            // GraphFrom_Menu_file_new
            // 
            GraphFrom_Menu_file_new.Name = "GraphFrom_Menu_file_new";
            GraphFrom_Menu_file_new.Size = new Size(112, 22);
            GraphFrom_Menu_file_new.Text = "新建";
            // 
            // GraphFrom_Menu_file_open
            // 
            GraphFrom_Menu_file_open.Name = "GraphFrom_Menu_file_open";
            GraphFrom_Menu_file_open.Size = new Size(112, 22);
            GraphFrom_Menu_file_open.Text = "打开";
            GraphFrom_Menu_file_open.Click += GraphFrom_Menu_file_open_Click;
            // 
            // GraphFrom_Menu_file_save
            // 
            GraphFrom_Menu_file_save.Name = "GraphFrom_Menu_file_save";
            GraphFrom_Menu_file_save.Size = new Size(112, 22);
            GraphFrom_Menu_file_save.Text = "保存";
            GraphFrom_Menu_file_save.Click += GraphFrom_Menu_file_save_Click;
            // 
            // GraphFrom_Menu_file_saveas
            // 
            GraphFrom_Menu_file_saveas.Name = "GraphFrom_Menu_file_saveas";
            GraphFrom_Menu_file_saveas.Size = new Size(112, 22);
            GraphFrom_Menu_file_saveas.Text = "另存为";
            GraphFrom_Menu_file_saveas.Click += GraphFrom_Menu_file_saveas_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(109, 6);
            // 
            // GraphFrom_Menu_file_backup
            // 
            GraphFrom_Menu_file_backup.DropDownItems.AddRange(new ToolStripItem[] { GraphFrom_Menu_file_backup_open, GraphFrom_Menu_file_backup_delete, GraphFrom_Menu_file_backup_seperator, GraphFrom_Menu_file_backup_clear, GraphFrom_Menu_file_backup_unpack });
            GraphFrom_Menu_file_backup.Name = "GraphFrom_Menu_file_backup";
            GraphFrom_Menu_file_backup.Size = new Size(112, 22);
            GraphFrom_Menu_file_backup.Text = "备份";
            GraphFrom_Menu_file_backup.DropDownOpening += GraphFrom_Menu_file_backup_DropDownOpening;
            GraphFrom_Menu_file_backup.DropDownOpened += GraphFrom_Menu_file_backup_DropDownOpened;
            // 
            // GraphFrom_Menu_file_backup_open
            // 
            GraphFrom_Menu_file_backup_open.Name = "GraphFrom_Menu_file_backup_open";
            GraphFrom_Menu_file_backup_open.Size = new Size(136, 22);
            GraphFrom_Menu_file_backup_open.Text = "打开";
            // 
            // GraphFrom_Menu_file_backup_delete
            // 
            GraphFrom_Menu_file_backup_delete.Name = "GraphFrom_Menu_file_backup_delete";
            GraphFrom_Menu_file_backup_delete.Size = new Size(136, 22);
            GraphFrom_Menu_file_backup_delete.Text = "删除";
            GraphFrom_Menu_file_backup_delete.Click += GraphFrom_Menu_file_backup_delete_Click;
            // 
            // GraphFrom_Menu_file_backup_seperator
            // 
            GraphFrom_Menu_file_backup_seperator.Name = "GraphFrom_Menu_file_backup_seperator";
            GraphFrom_Menu_file_backup_seperator.Size = new Size(133, 6);
            // 
            // GraphFrom_Menu_file_backup_clear
            // 
            GraphFrom_Menu_file_backup_clear.Name = "GraphFrom_Menu_file_backup_clear";
            GraphFrom_Menu_file_backup_clear.Size = new Size(136, 22);
            GraphFrom_Menu_file_backup_clear.Text = "打包并清空";
            GraphFrom_Menu_file_backup_clear.Click += GraphFrom_Menu_file_backup_clear_Click;
            // 
            // GraphFrom_Menu_file_backup_unpack
            // 
            GraphFrom_Menu_file_backup_unpack.Name = "GraphFrom_Menu_file_backup_unpack";
            GraphFrom_Menu_file_backup_unpack.Size = new Size(136, 22);
            GraphFrom_Menu_file_backup_unpack.Text = "解包";
            GraphFrom_Menu_file_backup_unpack.Click += GraphFrom_Menu_file_backup_unpack_Click;
            // 
            // GraphFrom_Menu_edit
            // 
            GraphFrom_Menu_edit.DropDownItems.AddRange(new ToolStripItem[] { GraphFrom_Menu_edit_undo, GraphFrom_Menu_edit_redo });
            GraphFrom_Menu_edit.Name = "GraphFrom_Menu_edit";
            GraphFrom_Menu_edit.Size = new Size(44, 21);
            GraphFrom_Menu_edit.Text = "编辑";
            GraphFrom_Menu_edit.DropDownClosed += GraphFrom_Menu_edit_status_check;
            GraphFrom_Menu_edit.DropDownOpening += GraphFrom_Menu_edit_status_check;
            GraphFrom_Menu_edit.Click += GraphFrom_Menu_edit_Click;
            // 
            // GraphFrom_Menu_edit_undo
            // 
            GraphFrom_Menu_edit_undo.Enabled = false;
            GraphFrom_Menu_edit_undo.Name = "GraphFrom_Menu_edit_undo";
            GraphFrom_Menu_edit_undo.Size = new Size(100, 22);
            GraphFrom_Menu_edit_undo.Text = "撤回";
            GraphFrom_Menu_edit_undo.Click += GraphFrom_Menu_edit_undo_Click;
            // 
            // GraphFrom_Menu_edit_redo
            // 
            GraphFrom_Menu_edit_redo.Enabled = false;
            GraphFrom_Menu_edit_redo.Name = "GraphFrom_Menu_edit_redo";
            GraphFrom_Menu_edit_redo.Size = new Size(100, 22);
            GraphFrom_Menu_edit_redo.Text = "重做";
            GraphFrom_Menu_edit_redo.Click += GraphFrom_Menu_edit_redo_Click;
            // 
            // GraphFrom_Menu_node
            // 
            GraphFrom_Menu_node.DropDownItems.AddRange(new ToolStripItem[] { GraphFrom_Menu_node_add });
            GraphFrom_Menu_node.Name = "GraphFrom_Menu_node";
            GraphFrom_Menu_node.Size = new Size(44, 21);
            GraphFrom_Menu_node.Text = "节点";
            // 
            // GraphFrom_Menu_node_add
            // 
            GraphFrom_Menu_node_add.Name = "GraphFrom_Menu_node_add";
            GraphFrom_Menu_node_add.Size = new Size(136, 22);
            GraphFrom_Menu_node_add.Text = "添加新节点";
            // 
            // GraphFrom_Menu_graph
            // 
            GraphFrom_Menu_graph.DropDownItems.AddRange(new ToolStripItem[] { GraphFrom_Menu_graph_reorderIds, GraphFrom_Menu_graph_setNodePointAuto, toolStripSeparator2, GraphFrom_Menu_graph_saveas });
            GraphFrom_Menu_graph.Name = "GraphFrom_Menu_graph";
            GraphFrom_Menu_graph.Size = new Size(44, 21);
            GraphFrom_Menu_graph.Text = "图像";
            // 
            // GraphFrom_Menu_graph_reorderIds
            // 
            GraphFrom_Menu_graph_reorderIds.Name = "GraphFrom_Menu_graph_reorderIds";
            GraphFrom_Menu_graph_reorderIds.Size = new Size(135, 22);
            GraphFrom_Menu_graph_reorderIds.Text = "重排节点id";
            GraphFrom_Menu_graph_reorderIds.Click += GraphFrom_Menu_graph_reorderIds_Click;
            // 
            // GraphFrom_Menu_graph_setNodePointAuto
            // 
            GraphFrom_Menu_graph_setNodePointAuto.Name = "GraphFrom_Menu_graph_setNodePointAuto";
            GraphFrom_Menu_graph_setNodePointAuto.Size = new Size(135, 22);
            GraphFrom_Menu_graph_setNodePointAuto.Text = "自动排版";
            GraphFrom_Menu_graph_setNodePointAuto.Click += GraphFrom_Menu_graph_autoLayout_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(132, 6);
            // 
            // GraphFrom_Menu_graph_saveas
            // 
            GraphFrom_Menu_graph_saveas.Name = "GraphFrom_Menu_graph_saveas";
            GraphFrom_Menu_graph_saveas.Size = new Size(135, 22);
            GraphFrom_Menu_graph_saveas.Text = "生成图片";
            GraphFrom_Menu_graph_saveas.Click += GraphFrom_Menu_graph_saveas_Click;
            // 
            // GraphFrom_Menu_loc
            // 
            GraphFrom_Menu_loc.DropDownItems.AddRange(new ToolStripItem[] { GraphFrom_Menu_loc_camreset, GraphFrom_Menu_loc_camfocus });
            GraphFrom_Menu_loc.Name = "GraphFrom_Menu_loc";
            GraphFrom_Menu_loc.Size = new Size(44, 21);
            GraphFrom_Menu_loc.Text = "位置";
            // 
            // GraphFrom_Menu_loc_camreset
            // 
            GraphFrom_Menu_loc_camreset.Name = "GraphFrom_Menu_loc_camreset";
            GraphFrom_Menu_loc_camreset.Size = new Size(100, 22);
            GraphFrom_Menu_loc_camreset.Text = "全景";
            GraphFrom_Menu_loc_camreset.Click += GraphFrom_Menu_camLoc_panorama_Click;
            // 
            // GraphFrom_Menu_loc_camfocus
            // 
            GraphFrom_Menu_loc_camfocus.Name = "GraphFrom_Menu_loc_camfocus";
            GraphFrom_Menu_loc_camfocus.Size = new Size(100, 22);
            GraphFrom_Menu_loc_camfocus.Text = "聚焦";
            GraphFrom_Menu_loc_camfocus.Click += GraphFrom_Menu_camLoc_focus_Click;
            // 
            // GraphFrom_Menu_window
            // 
            GraphFrom_Menu_window.Name = "GraphFrom_Menu_window";
            GraphFrom_Menu_window.Size = new Size(44, 21);
            GraphFrom_Menu_window.Text = "窗口";
            // 
            // GraphFrom_Menu_batch
            // 
            GraphFrom_Menu_batch.DropDownItems.AddRange(new ToolStripItem[] { GraphFrom_Menu_batch_reorderIds, GraphFrom_Menu_batch_saveasImage });
            GraphFrom_Menu_batch.Name = "GraphFrom_Menu_batch";
            GraphFrom_Menu_batch.Size = new Size(44, 21);
            GraphFrom_Menu_batch.Text = "批量";
            // 
            // GraphFrom_Menu_batch_reorderIds
            // 
            GraphFrom_Menu_batch_reorderIds.Name = "GraphFrom_Menu_batch_reorderIds";
            GraphFrom_Menu_batch_reorderIds.Size = new Size(180, 22);
            GraphFrom_Menu_batch_reorderIds.Text = "重排节点ID";
            GraphFrom_Menu_batch_reorderIds.Click += GraphFrom_Menu_batch_reorderIds_Click;
            // 
            // GraphFrom_Menu_batch_saveasImage
            // 
            GraphFrom_Menu_batch_saveasImage.Name = "GraphFrom_Menu_batch_saveasImage";
            GraphFrom_Menu_batch_saveasImage.Size = new Size(180, 22);
            GraphFrom_Menu_batch_saveasImage.Text = "生成图片";
            GraphFrom_Menu_batch_saveasImage.Click += GraphFrom_Menu_batch_saveasImage_Click;
            // 
            // GraphFrom_Menu_setting
            // 
            GraphFrom_Menu_setting.DropDownItems.AddRange(new ToolStripItem[] { GraphFrom_Menu_setting_backImage });
            GraphFrom_Menu_setting.Name = "GraphFrom_Menu_setting";
            GraphFrom_Menu_setting.Size = new Size(44, 21);
            GraphFrom_Menu_setting.Text = "设置";
            // 
            // GraphFrom_Menu_setting_backImage
            // 
            GraphFrom_Menu_setting_backImage.DropDownItems.AddRange(new ToolStripItem[] { GraphFrom_Menu_setting_backImage_show });
            GraphFrom_Menu_setting_backImage.Name = "GraphFrom_Menu_setting_backImage";
            GraphFrom_Menu_setting_backImage.Size = new Size(180, 22);
            GraphFrom_Menu_setting_backImage.Text = "背景图片";
            // 
            // GraphFrom_Menu_setting_backImage_show
            // 
            GraphFrom_Menu_setting_backImage_show.Checked = true;
            GraphFrom_Menu_setting_backImage_show.CheckOnClick = true;
            GraphFrom_Menu_setting_backImage_show.CheckState = CheckState.Checked;
            GraphFrom_Menu_setting_backImage_show.Name = "GraphFrom_Menu_setting_backImage_show";
            GraphFrom_Menu_setting_backImage_show.Size = new Size(180, 22);
            GraphFrom_Menu_setting_backImage_show.Text = "显示";
            GraphFrom_Menu_setting_backImage_show.Click += GraphFrom_Menu_setting_backImage_show_Click;
            // 
            // GraphFrom
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 561);
            Controls.Add(GraphFrom_StatusStrip);
            Controls.Add(GraphFrom_Menu);
            MainMenuStrip = GraphFrom_Menu;
            Name = "GraphFrom";
            Text = "GraphFrom";
            FormClosing += GraphFrom_FormClosing;
            GraphFrom_StatusStrip.ResumeLayout(false);
            GraphFrom_StatusStrip.PerformLayout();
            GraphFrom_Menu.ResumeLayout(false);
            GraphFrom_Menu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip GraphFrom_StatusStrip;
        private ToolStripStatusLabel GraphFrom_StatusStrip_filename;
        private ToolStripProgressBar GraphFrom_ProgressBar;
        private OpenFileDialog GraphFrom_Openfile;
        private OpenFileDialog GraphFrom_Openfile_batch;
        private SaveFileDialog GraphFrom_Savefile;
        private MenuStrip GraphFrom_Menu;
        private ToolStripMenuItem GraphFrom_Menu_file;
        private ToolStripMenuItem GraphFrom_Menu_file_new;
        private ToolStripMenuItem GraphFrom_Menu_file_open;
        private ToolStripMenuItem GraphFrom_Menu_file_save;
        private ToolStripMenuItem GraphFrom_Menu_file_saveas;
        private ToolStripMenuItem GraphFrom_Menu_edit;
        private ToolStripMenuItem GraphFrom_Menu_edit_undo;
        private ToolStripMenuItem GraphFrom_Menu_edit_redo;
        private ToolStripStatusLabel GraphFrom_StatusStrip_status;
        private ToolStripMenuItem GraphFrom_Menu_loc;
        private ToolStripMenuItem GraphFrom_Menu_loc_camreset;
        private ToolStripMenuItem GraphFrom_Menu_node;
        private ToolStripMenuItem GraphFrom_Menu_node_add;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem GraphFrom_Menu_file_backup;
        private ToolStripMenuItem GraphFrom_Menu_file_backup_open;
        private ToolStripMenuItem GraphFrom_Menu_file_backup_clear;
        private ToolStripMenuItem GraphFrom_Menu_loc_camfocus;
        private ToolStripMenuItem GraphFrom_Menu_window;
        private ToolStripMenuItem GraphFrom_Menu_graph;
        private ToolStripMenuItem GraphFrom_Menu_graph_saveas;
        private ToolStripMenuItem GraphFrom_Menu_graph_reorderIds;
        private ToolStripMenuItem GraphFrom_Menu_batch;
        private ToolStripMenuItem GraphFrom_Menu_batch_reorderIds;
        private ToolStripMenuItem GraphFrom_Menu_batch_saveasImage;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem GraphFrom_Menu_file_backup_delete;
        private ToolStripSeparator GraphFrom_Menu_file_backup_seperator;
        private ToolStripMenuItem GraphFrom_Menu_file_backup_unpack;
        private ToolStripMenuItem GraphFrom_Menu_graph_setNodePointAuto;
        private ToolStripMenuItem GraphFrom_Menu_setting;
        private ToolStripMenuItem GraphFrom_Menu_setting_backImage;
        private ToolStripMenuItem GraphFrom_Menu_setting_backImage_show;
    }
}