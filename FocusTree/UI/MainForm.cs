using FocusTree.Focus;
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
        }

        private void main_Menu_file_open_csv_Click(object sender, EventArgs e)
        {
            main_Openfile.Filter = "csv files (.csv) |*.csv";
            var result = main_Openfile.ShowDialog();
            if(result == DialogResult.OK)
            {
                Display.Graph = new FGraph(new FTree(main_Openfile.FileName));
                Display.RelocateCenter();
                Display.Invalidate();
            }
        }

        private void main_Menu_loc_camreset_Click(object sender, EventArgs e)
        {
            Display.RelocateCenter();
            Display.Invalidate();
        }
    }
}
