using FocusTree.Tree;
using static FocusTree.Focus.NodeRelation;

namespace FocusTree.Focus
{
    public class FGraph : FMap
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name;

        /// <summary>
        /// 以 ID 作为 Key 的所有节点
        /// </summary>
        private Dictionary<int, FMapNode> Nodes = new();

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

            var nodes = tree.GetAllFNodes();
            foreach (var node in nodes)
            {
                // 添加节点到字典
                Nodes.Add(node.ID, node);
                // 层级节点计数
                if (!LevelNodeCount.TryAdd(node.Level, 1)) { LevelNodeCount[node.Level]++; }
                // 树中的数据关系是单向一一对应的，所以指定 Linked 关系并后续推定 Require
                Relations.Add(node.ID, new List<NodeRelation>());
                Relations[node.ID].Add(new NodeRelation(FRelations.Linked, node.Children.Select(x => x.ID).ToArray())); // 添加子节点 ID 为 linked
            }
            // 已知 Linked 关系，指定 Require 关系
            foreach (var relation in Relations)
            {
                int parent_id = relation.Key;
                foreach (var child in relation.Value.First().IDs)
                {
                    Relations[child].Add(new NodeRelation(FRelations.Require, new int[] { parent_id }));
                }
            }
        }

        public override HashSet<FMapNode> GetAllMapNodes()
        {
            var set = new HashSet<FMapNode>();
            foreach (var node in Nodes.Values) { set.Add(node); }
            return set;
        }

        public override FMapNode GetMapNodeById(int id)
        {
            return Nodes[id];
        }
        public override int GetLevelNodeCount(int level)
        {
            return LevelNodeCount[level];
        }
        public override HashSet<FMapNode> GetLevelNodes(int level)
        {
            return Nodes.Values.Where(x => x.Level == level).ToHashSet();
        }
        public override HashSet<FMapNode> GetSiblingNodes(int id)
        {
            var requires = Relations[id]
                .Where(x => x.Type == FRelations.Require)
                .Select(x => x.IDs);

            var set = new HashSet<FMapNode>();

            // 看起来很多，其实不会循环很多次，比遍历所有对应关系快多了
            foreach (var require in requires)
            {
                foreach (var required_id in require)
                {
                    var sib_idss = Relations[required_id]
                        .Where(x => x.Type == FRelations.Linked)
                        .Select(x => x.IDs);
                    foreach (var sib_ids in sib_idss)
                    {
                        foreach (var sib_id in sib_ids)
                        {
                            set.Add(Nodes[sib_id]);
                        }
                    }
                }
            }

            return set;
        }
        public override int GetBranchWidth(int id)
        {
            int count = 0;
            var queue = new Queue<int>();
            GetBranchWidth(id, ref count, ref queue);
            return count;
        }
        /// <summary>
        /// 获取当前节点下叶节点数量
        /// </summary>
        /// <param name="current">当前递归节点</param>
        /// <param name="count">总数</param>
        /// <param name="steps">已走路径，用于禁止走重复的节点，避免死循环</param>
        private void GetBranchWidth(int current, ref int count, ref Queue<int> steps)
        {
            steps.Enqueue(current);

            var childs = Relations[current].Where(x => x.Type == FRelations.Linked);
            // 当前节点是叶节点，累加并退出
            if (childs.Sum(x => x.IDs.Length) == 0) { count++; }
            else
            {
                foreach (var child_relations in childs)
                {
                    foreach (var child in child_relations.IDs)
                    {
                        // 已经走过这个节点，所以跳过，避免死循环
                        if (steps.Contains(child)) { continue; }
                        else
                        {
                            GetBranchWidth(child, ref count, ref steps);
                        }
                    }
                }
            }

            steps.Dequeue();
        }
        public override List<NodeRelation> GetNodeRelations(int id)
        {
            return Relations[id];
        }
    }
}
