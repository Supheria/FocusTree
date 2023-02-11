namespace FocusTree
{
    public partial class InfoDialog : Form
    {
        CNode? mNode;
        bool nEditDialog;
        /// <summary>
        /// 节点信息对话框
        /// </summary>
        /// <param name="parent">父窗口</param>
        /// <param name="editDialog">是否修改信息并保存</param>
        public InfoDialog() { }
        public InfoDialog(Control parent, bool editDialog = true)
        {
            InitializeComponent();
            TopLevel = false;
            Parent = parent;
            nEditDialog = editDialog;
            btnEvent.Text = editDialog ? "保存" : "开始";
        }
        #region ==== 窗体方法 ====
        /// <summary>
        /// 设置指向的节点
        /// </summary>
        /// <param name="node"></param>
        public void SetNode(NodeControl nodeCtrl)
        {
            mNode = nodeCtrl.mNode;
            var focusData = mNode.FocusData;
            Text = focusData.Name;
            txtDuration.Text = $"{focusData.Duration}日";
            txtDescript.Text = focusData.Descript;
            txtEffects.Text = focusData.Effects;
            //TxtRequire.Text = szRequire;
            // 设置窗口位置为节点控件右下角
            //（还要考虑越界情况）
            SetLocation(nodeCtrl);
        }
        /// <summary>
        /// 设置窗口位置
        /// </summary>
        /// <param name="nodeCtrl"></param>
        private void SetLocation(NodeControl nodeCtrl)
        {
            Location = new Point(
                nodeCtrl.Location.X + nodeCtrl.Width,
                nodeCtrl.Location.Y + nodeCtrl.Height);
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
            if (nEditDialog)
            {
                EventEdit();
            }
            else
            {
                EventShow();
            }
        }
        /// <summary>
        /// 作为修改对话框
        /// </summary>
        private void EventEdit()
        {

        }
        /// <summary>
        /// 作为展示对话框
        /// </summary>
        private void EventShow()
        {

        }
        #endregion
    }
}
