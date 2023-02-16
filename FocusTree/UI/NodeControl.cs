using System.ComponentModel;
using FocusTree.Focus;

namespace FocusTree
{
    /// <summary>
    /// 节点控件
    /// </summary>
    public partial class NodeControl : UserControl
    {
        public FNodeBase Node;
        /// <summary>
        /// 节点转换成控件
        /// </summary>
        /// <param name="node">要转换成控件的节点</param>
        public NodeControl(FNodeBase node)
        {
            InitializeComponent();
            Node = node;
            txtTitle.Text = Text = Name = node.FocusData.Name;
            Location = new Point(0, 0);
            int nColum = 
                (Node.EndColum - Node.StartColum) / 2 + Node.StartColum;
            Location = new Point(
                        nColum * Size.Height,
                        Node.Level * Size.Width
                        );
        }
        const string category = "FTControls";
        #region ==== 事件迁移 ====
        /// <summary>
        /// 鼠标单击事件
        /// </summary>
        [Description("单击控件时"), Category(category)]
        public MouseEventHandler TFMouseCilck;

        private void txtTitle_MouseClick(object sender, MouseEventArgs e)
        {
            if (TFMouseCilck == null)
                return;
            // 触发单击事件
            TFMouseCilck(this, e);
        }

        private void NodeControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (TFMouseCilck == null)
                return;
            // 触发单击事件
            TFMouseCilck(sender, e);
        }
        /// <summary>
        /// 单击加号
        /// </summary>
        [Description("单击上加号时"), Category(category)]
        public EventHandler ClickTopAddButton;
        [Description("单击下加号时"), Category(category)]
        public EventHandler ClickBottomAddButton;
        
        private void btnTop_TFClick(object sender, EventArgs e)
        {
            if (ClickTopAddButton == null)
                return;
            ClickTopAddButton(this, e);
        }

        private void btnBottom_TFClick(object sender, EventArgs e)
        {
            if (ClickBottomAddButton == null)
                return;
            ClickBottomAddButton(this, e);
        }
        #endregion
    }
}