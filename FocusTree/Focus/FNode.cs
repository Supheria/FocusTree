using FocusTree.Tree;
using System.Xml.Serialization;

namespace FocusTree.Focus
{
    /// <summary>
    /// 节点类
    /// </summary>
    public class FNode: FMapNode
    {
        #region ==== 重载 ====
        public override int ID { get; protected set; }
        public override int Level { get; protected set; }
        public override FData FocusData { get; protected set; }
        #endregion
        #region ==== 属性 ====
        /// <summary>
        /// 依赖的节点ID
        /// </summary>
        public List<int> ReliedIDs { get; set; } = new();
        /// <summary>
        /// 子节点ID
        /// </summary>
        public List<int> ChildIDs { get; set; } = new();
        /// <summary>
        /// 节点在树中的起始列
        /// </summary>
        public int StartColum { get; set; } = 0;
        /// <summary>
        /// 节点在树中的终止列
        /// </summary>
        public int EndColum { get; set; } = 0;
        //== 节点控制 ==//
        /// <summary>
        /// 父节点
        /// </summary>
        public FNode Parent { get; protected set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public List<FNode> Children { get; protected set; } = new();
        #endregion
        #region ==== 初始化节点 ====
        /// <summary>
        /// 创建节点，并作为子节点加入在父节点下
        /// </summary>
        /// <param name="ID">节点ID</param>
        /// <param name="level">层级(所在的列数)</param>
        /// <param name="parent">父节点</param>
        /// <param name="focusData">国策数据</param>
        public FNode(
            int id,
            int level,
            FNode parent,
            FData focusData
            )
        {
            ID = id;
            Level = level;
            Parent = parent;
            // 把节点加入父节点的子集
            Parent.Children.Add(this);
            Parent.ChildIDs.Add(ID);
            ReliedIDs.Add(Parent.ID);
            // 设置国策数据
            FocusData = focusData;
        }
        /// <summary>
        /// 给继承类专用的无参构造，如 FRootNode
        /// </summary>
        protected FNode(){}
        #endregion
    }
}
