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
            Display.Graph = new FGraph(new FTree("人类财阀联合.csv"));
            Display.RelocateCenter(); 
            Display.Invalidate();
        }

        private void main_Menu_loc_camreset_Click(object sender, EventArgs e)
        {
            Display.RelocateCenter();
            Display.Invalidate();
        }
    }
}
