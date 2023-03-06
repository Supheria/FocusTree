using FocusTree.UI.Controls;
using FocusTree.UITool;

namespace FocusTree.UI.Forms
{
    public partial class InfoDialog : Form
    {
        GraphBox Display;
        public bool ReadOnly;
        public Point Position;
        /// <summary>
        /// 节点信息对话框
        /// </summary>
        /// <param name="parent">父窗口</param>
        /// <param name="editDialog">是否修改信息并保存</param>
        public InfoDialog() { }
        internal InfoDialog(GraphBox display)
        {
            InitializeComponent();
            TopMost = true;
            Display = display;
            MinimumSize = Size;
            ResizeForm.SetTag(this);

            Invalidated += OnInvalidated;
            FormClosing += InfoDialog_FormClosing;
            SizeChanged += InfoDialog_SizeChanged;
            DoubleClick += InfoDialog_DoubleClick;
            ResizeForm.SetTag(this);
        }

        private void InfoDialog_DoubleClick(object sender, EventArgs e)
        {
            ResizeForm.DefultSize(this);
        }

        private void InfoDialog_SizeChanged(object sender, EventArgs e)
        {
            ResizeForm.ResizeControls(this);
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
            Text = focusData.Name + $" (ID: {focusData.ID})";
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
            Invalidate();
            base.Show();
            Location = pos;
        }
    }
}
