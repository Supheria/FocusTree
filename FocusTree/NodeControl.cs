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

namespace FocusTree
{
    /// <summary>
    /// 节点控件
    /// </summary>
    public partial class NodeControl : UserControl
    {
        public CNode mNode { get; private set; }
        /// <summary>
        /// 节点转换成控件
        /// </summary>
        /// <param name="node">要转换成控件的节点</param>
        public NodeControl(CNode node)
        {
            InitializeComponent();
            mNode = node;
            lblTitle.Text = Text = Name = node.FocusData.Name;
            Location = new Point(0, 0);
            int nColum = 
                (mNode.EndColum - mNode.StartColum) / 2 + mNode.StartColum;
            Location = new Point(
                        nColum * Size.Height,
                        mNode.Level * Size.Width
                        );
        }
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
        public List<int> ChildrenIDs
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
        SFocusData mFocusData = new SFocusData();
        [XmlElement("focus-data")]
        public SFocusData FocusData
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
            SFocusData focusData
            )
        {
            ID = id;
            Level = level;
            mParent = parent;
            // 把节点加入父节点的子集
            mParent.Children.Add(this);
            mParent.ChildrenIDs.Add(ID);
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
    /// <summary>
    /// 国策数据
    /// </summary>
    public struct SFocusData
    {
        /// <summary>
        /// 国策名称
        /// </summary>
        [XmlElement("name")]
        public string Name = string.Empty;
        /// <summary>
        /// 字段是否以 * 开头
        /// </summary>
        [XmlElement("begin-with-star")]
        public bool BeginWithStar = false;
        /// <summary>
        /// 实施天数
        /// </summary>
        [XmlElement("duration")]
        public int Duration = -1;
        /// <summary>
        /// 国策效果
        /// </summary>
        [XmlElement("effects")]
        public string Effects = string.Empty;
        /// <summary>
        /// 国策描述
        /// </summary>
        [XmlElement("descript")]
        public string Descript = string.Empty;
        /// <summary>
        /// 备注
        /// </summary>
        [XmlElement("ps.")]
        public string Ps = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">国策名称</param>
        /// <param name="isBeginWithstar">字段是否以 * 开头</param>
        /// <param name="duration">实施天数</param>
        /// <param name="effects">国策效果</param>
        /// <param name="descript">国策描述</param>
        /// <param name="ps">备注</param>
        public SFocusData() { }
        public SFocusData(
            string name,
            bool isBeginWithstar,
            int duration,
            string effects,
            string descript,
            string ps
            )
        {
            Name = name;
            BeginWithStar = isBeginWithstar;
            Duration = duration;
            Effects = effects;
            Descript = descript;
            Ps = ps;
        }
    }
}