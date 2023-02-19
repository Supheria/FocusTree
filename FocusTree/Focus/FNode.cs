using FocusTree.Tree;
using System.Xml;
using System.Xml.Serialization;

namespace FocusTree.Focus
{
    /// <summary>
    /// 节点类
    /// </summary>
    public class FNode : FMapNode
    {
        #region ==== 重载 ====
        public override int ID { get; protected set; }
        public override int Level { get; protected set; }
        public override FData FocusData { get; protected set; }
        #endregion
        #region ==== 属性 ====
        //== 节点控制 ==//
        /// <summary>
        /// 父节点
        /// </summary>
        [Obsolete] //这个是只有树在用，后续可以优化掉
        public FNode Parent { get; protected set; }
        /// <summary>
        /// 子节点
        /// </summary>
        [Obsolete] //这个是只有树在用，后续可以优化掉
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
            // 设置国策数据
            FocusData = focusData;
        }
        /// <summary>
        /// FGraph 反序列化专用的
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="level">层级(所在的列数)</param>
        /// <param name="focusData">国策数据</param>
        /// <param name="reader">用来保证是反序列化专用(不要对它操作)</param>
        public FNode(
            int id,
            int level,
            ref FData focusData,
            XmlReader reader
            )
        {
            ID = id;
            Level = level;
            // 设置国策数据
            FocusData = focusData;
        }
        /// <summary>
        /// 给继承类专用的无参构造，如 FRootNode
        /// </summary>
        protected FNode() { }
        #endregion
    }
}
