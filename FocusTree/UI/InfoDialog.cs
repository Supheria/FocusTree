using FocusTree.UI;
using FocusTree.UITool;

namespace FocusTree
{
    public partial class InfoDialog : Form
    {
        GraphBox Display;
        public bool ReadOnly;
        public Point Position;
        public bool DoShow = false;
        /// <summary>
        /// 节点信息对话框
        /// </summary>
        /// <param name="parent">父窗口</param>
        /// <param name="editDialog">是否修改信息并保存</param>
        public InfoDialog() { }
        internal InfoDialog(GraphBox display)
        {
            InitializeComponent();
            //TopLevel = false;
            TopMost = true;
            Display = display;

            Invalidated += OnInvalidated;
            FormClosing += InfoDialog_FormClosing;
            ResizeForm.SetTag(this);
            this.MinimumSize = Size;
        }
        #region ==== 窗体方法 ====
        /// <summary>
        /// 设置指向的节点
        /// </summary>
        /// <param name="node"></param>


        public void OnInvalidated(object sender, EventArgs args)
        {
            if (Display.SelectedNode == null)
            {
                return;
            }
            var focusData = Display.Graph.NodesCatalog[Display.SelectedNode.Value];
            Text = focusData.Name;
            txtDuration.Text = $"{focusData.Duration}日";
            txtDescript.Text = focusData.Descript;
            txtEffects.Text = focusData.Effects;
            //TxtRequire.Text = szRequire;
            // 设置窗口位置为节点控件右下角
            //（还要考虑越界情况）

            btnEvent.Text = Display.ReadOnly ? "开始" : "保存";

        }
        /// <summary>
        /// 将关闭窗体设置为隐藏窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            DoShow = false;
            this.Hide();
            e.Cancel = true;
        }
        #endregion
        #region ==== 控件事件 ====
        private void btnEvent_Click(object sender, EventArgs e)
        {
            if (Display.SelectedNode == null)
            {
                return;
            }
            if (ReadOnly)
            {
                EventShow();
            }
            else
            {
                EventEdit();
            }
        }
        /// <summary>
        /// 作为展示对话框
        /// </summary>
        private void EventShow()
        {

        }
        /// <summary>
        /// 作为修改对话框
        /// </summary>
        private void EventEdit()
        {

        }

        #endregion

        public void Show(Point pos)
        {
            if (DoShow)
            {
                Invalidate();
                base.Show();
                Location = pos;
            }
        }

        private void txtDuration_Enter(object sender, EventArgs e)
        {

        }

        private void txtDuration_DragEnter(object sender, DragEventArgs e)
        {
            //txtDuration.Focus();//获取焦点
            txtDuration.Select(this.txtDuration.SelectionStart, 0);//光标定位到文本最后
            txtDuration.ScrollToCaret();//滚动到光标处
        }

        private void txtDuration_MouseEnter(object sender, EventArgs e)
        {
            txtDuration.Focus();//获取焦点
            txtDuration.Select(this.txtDuration.SelectionStart, 1);//光标定位到文本最后
            txtDuration.ScrollToCaret();//滚动到光标处
        }

        private void InfoDialog_Resize(object sender, EventArgs e)
        {
            ResizeForm.ResizeControls(this);
        }

        private void InfoDialog_MouseDoubleClick(object sender, MouseEventArgs args)
        {
            if ((args.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                ResizeForm.DefultSize(this);
            }
        }
    }
}
