using FocusTree.Tree;

namespace FocusTree.Focus
{
    public class FGraph
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name;

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
            Name = tree.Name;

            var nodes = tree.GetAllNodes();
            foreach (var node in nodes)
            {
                // 添加节点到字典
                Nodes.Add(node.ID, node);
                // 层级节点计数
                if (!LevelNodeCount.TryAdd(node.Level, 1)) { LevelNodeCount[node.Level]++; }
                // 树中的数据关系是单向一一对应的，所以指定 Linked 关系并后续推定 Require
                Relations.Add(node.ID, new List<NodeRelation>());
                Relations[node.ID].Add(new NodeRelation(NodeRelationType.Linked, node.Children.Select(x => x.ID).ToArray())); // 添加子节点 ID 为 linked
            }
            // 已知 Linked 关系，指定 Require 关系
            foreach (var relation in Relations)
            {
                int parent_id = relation.Key;
                foreach (var child in relation.Value.First().IDs)
                {
                    Relations[child].Add(new NodeRelation(NodeRelationType.Require, new int[] { parent_id }));
                }
            }
        }
        private class NodeRelation
        {
            public NodeRelationType RelationType;
            public int[] IDs;
            public NodeRelation(NodeRelationType relationType, int[] iDs)
            {
                RelationType = relationType;
                IDs = iDs;
            }
        }
        private enum NodeRelationType
        {
            Require,    // 这个节点依赖于某些节点
            Linked      // 某些节点依赖于这个节点
        }
    }
}
