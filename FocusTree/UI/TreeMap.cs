using FocusTree.Focus;
using System.Xml.Serialization;

namespace FocusTree
{
    public partial class TreeMap : UserControl
    {
        Tree.Tree mTree { get; init; }
        public Tree.Tree Tree
        {
            get { return mTree; }
        }
        /// <summary>
        /// 节点字典
        /// </summary>
        Dictionary<int, CNode> mNodeKeyID = new Dictionary<int, CNode> { { 0, new CNode() } };
        [XmlIgnore]
        public Dictionary<int, CNode> NodeKeyID
        {
            get { return mNodeKeyID; }
        }
        /// <summary>
        /// 节点信息窗口
        /// </summary>
        private InfoDialog mInfoDlg = new InfoDialog();

        public TreeMap(Tree.Tree tree)
        {
            InitializeComponent();
            mTree = tree;
            mInfoDlg = new InfoDialog(this);
            Name = Text = mTree.Name;
            SetMap();
        }
        public void SetMap()
        {
            foreach (var node in mTree.NodeChain)
            {
                NodeKeyID.Add(node.ID, node);
                NodeControl nodeCtrl = new NodeControl(node);
                nodeCtrl.TFMouseCilck += MouseClickNode;
                nodeCtrl.ClickTopAddButton += ClickNodeTopAdd;
                nodeCtrl.ClickBottomAddButton += ClickNodeBottomAdd;
                Controls.Add(nodeCtrl);
            }
        }
        #region ==== 节点控件事件 ====
        public void MouseClickNode(object sender, MouseEventArgs e)
        {
            // 隐藏信息窗口
            mInfoDlg.Hide();
            // 触发点击事件的节点
            mInfoDlg.SetNode((NodeControl)sender);
            // 非模态对话框
            mInfoDlg.Show();
        }
        public void ClickNodeTopAdd(object sender, EventArgs e)
        {
            string arg = "上加键，依赖于";
            var node = ((NodeControl)sender).Node;
            foreach (var id in node.ReliedIDs)
            {
                var name = NodeKeyID[id].FocusData.Name;
                arg += $"\"{name}\" ";
            }
            MessageBox.Show(arg);
        }
        public void ClickNodeBottomAdd(object sender, EventArgs e)
        {
            string arg = "下加键，被";
            var node = ((NodeControl)sender).Node;
            foreach (var id in node.ChildIDs)
            {
                var name = NodeKeyID[id].FocusData.Name;
                arg += $"\"{name}\" ";
            }
            arg += "依赖";
            MessageBox.Show(arg);
        }
        #endregion
        #region ==== 窗体方法 ====
        private static bool IsDragging = false; //用于指示当前是不是在拖拽状态
        private Point StartPoint = new Point(0, 0); //记录鼠标按下去的坐标, new是为了拿到空间, 两个0无所谓的
                                                    //记录动了多少距离,然后给窗体Location赋值,要设置Location,必须用一个Point结构体,不能直接给Location的X,Y赋值
        private Point OffsetPoint = new Point(0, 0);

        private void TreeMap_MouseDown(object sender, MouseEventArgs e)
        {
            //如果按下去的按钮不是左键就return,节省运算资源
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            //按下鼠标后,进入拖动状态:
            IsDragging = true;
            //保存刚按下时的鼠标坐标
            StartPoint.X = e.X;
            StartPoint.Y = e.Y;
        }

        private void TreeMap_MouseMove(object sender, MouseEventArgs e)
        {
            //鼠标移动时调用,检测到IsDragging为真时
            if (IsDragging == true)
            {
                //var p = Parent.Location;
                //用当前坐标减去起始坐标得到偏移量Offset
                OffsetPoint.X = e.X - StartPoint.X;
                OffsetPoint.Y = e.Y - StartPoint.Y;
                //将Offset转化为屏幕坐标赋值给Location,设置Form在屏幕中的位置,如果不作PointToScreen转换,你自己看看效果就好
                Location = PointToScreen(Parent.PointToClient(OffsetPoint));
            }
        }

        private void TreeMap_MouseUp(object sender, MouseEventArgs e)
        {
            //左键抬起时,及时把拖动判定设置为false,否则,你也可以试试效果
            IsDragging = false;
        }
        #endregion
    }
}
