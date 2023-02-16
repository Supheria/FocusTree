using FocusTree.Tree;

namespace FocusTree.Focus
{
    internal class FGraph
    {
        /// <summary>
        /// 以 ID 作为 Key 的所有节点
        /// </summary>
        private Dictionary<int, FNode> Nodes = new();

        /// <summary>
        /// 节点与其它节点的关系
        /// </summary>
        private Dictionary<int, List<NodeRelation>> Relations = new();

        /// <summary>
        /// 某个层级所包含的节点数量
        /// </summary>
        private Dictionary<int, int> LevelNodeCount = new();

        /// <summary>
        /// 将 FTree 转换为 FGraph
        /// </summary>
        /// <param name="tree">国策树</param>
        public FGraph(FTree tree) 
        { 
            //tree.
        }
    }
    class NodeRelation
    {
        NodeRelationType RelationType;
        public int[] IDs;
        public NodeRelation(NodeRelationType relationType, int[] iDs)
        {
            RelationType = relationType;
            IDs = iDs;
        }
    }
    public enum NodeRelationType
    {
        Require,    // 这个节点依赖于某些节点
        Linked      // 某些节点依赖于这个节点
    }
}
