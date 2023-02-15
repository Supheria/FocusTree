#region ==== 类型别称 ====
// 分支包类型
// 外层List：记录所有分支
using BranchPackType = System.Collections.Generic.List<System.Collections.Generic.List<FocusTree.CNode>>;
// 分支类型
// 内层CNode：以节点作为分支上的叶
// List：记录从末节点到根节点的所有叶，作为一个分支（倒序分支）
using BranchType = System.Collections.Generic.List<FocusTree.CNode>;
#endregion
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.ComponentModel;
using FocusTree.Tree;

namespace FocusTree
{
    /// <summary>
    /// 节点控件
    /// </summary>
    public partial class NodeControl : UserControl
    {
        CNode mNode = new CNode();
        public CNode Node
        {
            get { return mNode; }
            set { mNode = value; }
        }
        /// <summary>
        /// 节点转换成控件
        /// </summary>
        /// <param name="node">要转换成控件的节点</param>
        public NodeControl(CNode node)
        {
            InitializeComponent();
            mNode = node;
            txtTitle.Text = Text = Name = node.FocusData.Name;
            Location = new Point(0, 0);
            int nColum = 
                (mNode.EndColum - mNode.StartColum) / 2 + mNode.StartColum;
            Location = new Point(
                        nColum * Size.Height,
                        mNode.Level * Size.Width
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
    /// <summary>
    /// 节点类
    /// </summary>
    public class CNode
    {
        #region ==== 节点属性（序列化） ====
        /// <summary>
        /// 节点ID
        /// </summary>
        int mID = 0;
        [XmlElement("ID")]
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }
        /// <summary>
        /// 依赖的节点ID
        /// </summary>
        List<int> mReliedIDs = new List<int>();
        [XmlElement("relied-ID")]
        public List<int> ReliedIDs
        {
            get { return mReliedIDs; }
            set { mReliedIDs = value; }
        }
        /// <summary>
        /// 子节点ID
        /// </summary>
        List<int> mChildrenIDs = new List<int>();
        [XmlElement("child-ID")]
        public List<int> ChildIDs
        {
            get { return mChildrenIDs; }
            set { mChildrenIDs = value; }
        }
        /// <summary>
        /// 层级
        /// </summary>
        int mLevel = -1;
        [XmlElement("level")]
        public int Level
        {
            get { return mLevel; }
            set { mLevel = value; }
        }
        /// <summary>
        /// 节点在树中的起始列
        /// </summary>
        int mStartColum = 0;
        [XmlElement("start-colum")]
        public int StartColum
        {
            get { return mStartColum; }
            set { mStartColum = value; }
        }
        /// <summary>
        /// 节点在树中的终止列
        /// </summary>
        int mEndColum = 0;
        [XmlElement("end-colum")]
        public int EndColum
        {
            get { return mEndColum; }
            set { mEndColum = value; }
        }
        /// <summary>
        /// 国策数据
        /// </summary>
        FocusData mFocusData = new FocusData();
        [XmlElement("focus-data")]
        public FocusData FocusData
        {
            get { return mFocusData; }
            set { mFocusData = value; }
        }
        #endregion
        #region ==== 节点控制器 ====
        /// <summary>
        /// 父节点
        /// </summary>
        CNode? mParent = null;
        [XmlIgnore]
        public CNode? Parent
        {
            get { return mParent; }
        }
        /// <summary>
        /// 子节点
        /// </summary>
        List<CNode> mChildren = new List<CNode>();
        [XmlIgnore]
        public List<CNode> Children
        {
            get { return mChildren; }
        }
        #endregion
        #region ==== 初始化节点 ====
        /// <summary>
        /// 创建节点，并作为子节点加入在父节点下
        /// </summary>
        /// <param name="ID">节点ID</param>
        /// <param name="level">层级(所在的列数)</param>
        /// <param name="parent">父节点</param>
        /// <param name="focusData">国策数据</param>
        public CNode() { }
        public CNode(
            int id,
            int level,
            CNode parent,
            FocusData focusData
            )
        {
            ID = id;
            Level = level;
            mParent = parent;
            // 把节点加入父节点的子集
            mParent.Children.Add(this);
            mParent.ChildIDs.Add(ID);
            ReliedIDs.Add(mParent.ID);
            // 设置国策数据
            FocusData = focusData;
        }
        #endregion
        #region ==== 节点方法 ====
        /// <summary>
        /// 获取nExcludeLevel节点之下的所有正序的分支
        /// </summary>
        /// <param name="nExcludeLevel">分支所不包含的层级，默认不包含根节点</param>
        /// <returns>所有分支打包成的分支包</returns>
        public BranchPackType GetBranches(int nExcludeLevel = -1)
        {

            // 创建本节点的分支包
            BranchPackType branchPack = new BranchPackType();
            // 这是分支上最后一个节点
            if (mChildren.Count == 0)
            {
                // 从末节点开始创建一条新分支
                BranchType branch = new BranchType { this };
                branchPack.Add(branch);
                return branchPack;
            }
            // 遍历子所有节点的分支包
            for (int i = 0; i < mChildren.Count; i++)
            {
                // 获取子节点的分支包
                BranchPackType buffer = mChildren[i].GetBranches();
                // 把分支包解包，并在每支节点上加入本节点的ID，最后打包到branchPack
                for (int j = 0; j < buffer.Count; j++)
                {
                    BranchType branch = buffer[j];
                    if (Level != nExcludeLevel)
                    {
                        // 在分支上加入本节点
                        branch.Add(this);
                    }
                    else
                    {
                        // 反序分支
                        branch.Reverse();
                    }
                    // 将分支加入新分支包
                    branchPack.Add(branch);
                }
            }
            return branchPack;
        }
        #endregion
    }
}