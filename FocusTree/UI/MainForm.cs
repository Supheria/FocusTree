﻿using FocusTree.Focus;
using FocusTree.IO;
using FocusTree.Tree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FocusTree.UI
{
    public partial class MainForm : Form
    {
        DisplayBox Display;
        public MainForm()
        {
            Display = new DisplayBox(this);
            InitializeComponent();
            Controls.Add(Display);
            main_StatusStrip_filename.Text = "等待打开文件";
            main_StatusStrip_status.Text = "";
            main_Openfile.FileName = "";
        }

        private void main_Menu_file_open_csv_Click(object sender, EventArgs e)
        {
            main_Openfile.Filter = "csv files (.csv) |*.csv";
            var result = main_Openfile.ShowDialog();
            if(result == DialogResult.OK)
            {
                Display.Graph = new FGraph(main_Openfile.FileName);
                Display.RelocateCenter();
                Display.Invalidate();
                main_StatusStrip_filename.Text = Path.GetFileNameWithoutExtension(main_Openfile.FileName);
            }
        }

        private void main_Menu_loc_camreset_Click(object sender, EventArgs e)
        {
            Display.RelocateCenter();
            Display.Invalidate();
        }

        private void main_Menu_file_open_xml_Click(object sender, EventArgs e)
        {
            main_Openfile.Filter = "xml files (.xml) |*.xml";
            var result = main_Openfile.ShowDialog();
            if (result == DialogResult.OK)
            {
                Display.Graph = FXml.LoadGraph(main_Openfile.FileName);
                Display.RelocateCenter();
                Display.Invalidate();
                main_StatusStrip_filename.Text = Path.GetFileNameWithoutExtension(main_Openfile.FileName);
            }
        }
    }
}
