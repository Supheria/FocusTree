using FocusTree.Tree;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using static FocusTree.Focus.FGraphStruct;
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
        public override HashSet<FMapNode> GetLeafNodes(int id)
        {
            var nodes = new HashSet<FMapNode>();
            var queue = new Queue<int>();
            GetLeafNodes(id, ref nodes, ref queue);
            return nodes;
        }
        private void GetLeafNodes(int current, ref HashSet<FMapNode> nodes, ref Queue<int> steps)
        {
            steps.Enqueue(current);

            var childs = Relations[current].Where(x => x.Type == FRelations.Linked);
            // 当前节点是叶节点，累加并退出
            if (childs.Sum(x => x.IDs.Length) == 0) { nodes.Add(Nodes[current]); }
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
                            GetLeafNodes(child, ref nodes, ref steps);
                        }
                    }
                }
            }

            steps.Dequeue();
        }
        public FGraphStruct GetStruct()
        {
            return new FGraphStruct(Nodes.Values.ToArray(), Relations, LevelNodeCount);
        }
    }
    public struct FGraphStruct : IXmlSerializable
    {
        // -- 以下参数按序列化顺序排序 --
        public FMapNode[] Nodes { get; init; }
        public Dictionary<int, List<NodeRelation>> Relations { get; init; }
        public Dictionary<int, int> LevelNodeCount { get; init; }
        // -- 
        public FGraphStruct(
            FMapNode[] nodes,
            Dictionary<int, List<NodeRelation>> relations,
            Dictionary<int, int> levelNodeCount
            )
        {
            Nodes = nodes;
            Relations = relations;
            LevelNodeCount = levelNodeCount;
        }

        // -- 序列化工具 --
        static XmlSerializer FData_serial = new (typeof(FData));
        static XmlSerializerNamespaces NullXmlNameSpace = new (new XmlQualifiedName[] { new XmlQualifiedName("","")});

        // -- 序列化方法 --
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            //==== 序列化节点数据 ====//

            // <Nodes> 序列化 Nodes (国策节点数据)
            writer.WriteStartElement("Nodes");
            foreach (var node in Nodes)
            {
                writer.WriteStartElement("Node");
                // <Node>
                writer.WriteAttributeString("ID", node.ID.ToString());
                writer.WriteAttributeString("Level", node.ID.ToString());
                // <Data> 序列化 FData (国策节点数据)
                writer.WriteStartElement("Data");
                FData_serial.Serialize(writer, node.FocusData, NullXmlNameSpace);
                writer.WriteEndElement();
                // </Data>
                writer.WriteEndElement();
                // </Node>
            }
            writer.WriteEndElement();
            // </Nodes>

            //==== 序列化节点关系 ====//

            // <NodesRelations> 序列化节点关系字典
            writer.WriteStartElement("NodesRelations");
            foreach (var r_pair in Relations)
            {
                // <RNode> 当前节点
                writer.WriteStartElement("RNode");
                writer.WriteAttributeString("ID", r_pair.Key.ToString());

                foreach (var relation in r_pair.Value.OrderBy(x => x.Type))
                {
                    // <[Relation类型]> 关系类型
                    writer.WriteElementString(relation.Type.ToString(), IdArrayToString(relation.IDs));
                    // </[Relation类型]>
                }

                writer.WriteEndElement();
                // </RNode>
            }
            writer.WriteEndElement();
            // </Relations>

            //==== 序列化层级节点数量 ====//

            // <LevelNodeCount> 序列化节点关系字典
            writer.WriteStartElement("LevelNodeCount");
            foreach (var r_pair in LevelNodeCount)
            {
                // <Level> 层级节点数
                writer.WriteStartElement("Level");
                writer.WriteAttributeString("Level", r_pair.Key.ToString());
                writer.WriteAttributeString("Nodes", r_pair.Value.ToString());
                writer.WriteEndElement();
                // </Level>
            }
            writer.WriteEndElement();
            // </LevelNodeCount>
        }
        public static string IdArrayToString(int[] ids)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < ids.Length; i++) { sb.Append(ids[i].ToString() + (i < ids.Length - 1 ? ", " : "")); }
            return sb.ToString();
        }
        public static int[] IdArrayFromString(string ids)
        {
            var split = ids.Split(',').Where(x => !string.IsNullOrWhiteSpace(x));
            return split.Select(x => int.Parse(x)).ToArray();
        }
    }
}
