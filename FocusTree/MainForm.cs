using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FocusTree
{
    public partial class MainForm : Form
    {
        private int childFormNumber = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "窗口 " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            #region ==== 打开文件 ====
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // 允许多选文件
            openFileDialog.Multiselect = true;
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "CSV文件(*.csv)|*.csv|XML文件(*.xml)|*.xml|所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.Cancel)
            {
                return;
            }
            #endregion
            for (int i = 0; i < openFileDialog.FileNames.Length; i++)
            {
                string FileName = openFileDialog.FileNames[i];
                try
                {
                    TreeForm treeForm = new TreeForm(FileName);
                    treeForm.MdiParent = this;
                    treeForm.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeForm form = (TreeForm)this.ActiveMdiChild;
            if (form == null)
            {
                MessageBox.Show("没有树可以保存。");
                return;
            }
            #region ==== 选择保存路径 ====
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            // 打开的文件选择对话框上的标题
            saveFileDialog.Title = "保存为：";
            // 设置文件类型
            saveFileDialog.Filter = "国策树文件(*.xml)|*.xml";
            // 设置默认文件名
            saveFileDialog.FileName = form.Name;
            // 记忆上次打开的目录
            saveFileDialog.RestoreDirectory = true;
            //按下确定选择的按钮
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            #endregion
            try
            {
                form.SerializeToXml(saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void SaveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TreeForm form in MdiChildren)
            {
                try
                {
                    form.SerializeToXml();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
