using FocusTree.Tree;
using System.Numerics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using static System.Windows.Forms.LinkLabel;

namespace FocusTree.Focus
{
    public class FGraph : FMap, IXmlSerializable
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name;

        /// <summary>
        /// 以 ID 作为 Key 的所有节点
        /// </summary>
        private Dictionary<int, FMapNode> Nodes;

        /// <summary>
        /// 节点依赖的节点 (子节点, 多组父节点)
        /// </summary>
        private Dictionary<int, List<int[]>> Requires;
        /// <summary>
        /// 依赖于节点的节点 (自动生成) (父节点, 多个子节点)
        /// </summary>
        private Dictionary<int, HashSet<int>> Linked;

        /// <summary>
        /// 节点显示位置
        /// </summary>
        private Dictionary<int, Point> NodeMap;

        /// <summary>
        /// 将 FTree 转换为 FGraph
        /// </summary>
        /// <param name="tree">国策树</param>
        [Obsolete] // 这个函数后续需要被优化掉
        public FGraph(FTree tree)
        {
            Name = tree.Name;
            Nodes = new();
            Requires = new();

            var nodes = tree.GetAllFNodes();
            foreach (var node in nodes)
            {
                // 添加节点到字典
                Nodes.Add(node.ID, node);

                // 树中的数据关系是单向一一对应的，读取 Require 推断 Linked
                if (node.Parent != null && node.Parent.ID >= 0)
                {
                    Requires.Add(node.ID, new List<int[]> { new int[] { node.Parent.ID } });
                }

            }
            // 推断 Link 关系
            CreateLinked();

            NodeMap = GetNodeMap();
        }
        /// <summary>
        /// 使用 Requires 创建 Linked
        /// </summary>
        private void CreateLinked()
        {
            // 这里一定要重新初始化，因为是刷新
            Linked = new Dictionary<int, HashSet<int>>();

            foreach (var require in Requires)
            {
                foreach (var parents in require.Value)
                {
                    foreach (var parent in parents)
                    {
                        // 如果父节点没有创建 HashSet 就创建一个新的
                        Linked.TryAdd(parent, new HashSet<int>());
                        // 向 HashSet 里添加子节点（忽略重复项）
                        Linked[parent].Add(require.Key);
                    }
                }
            }
        }
        /// <summary>
        /// 将 XML 转换为 FGraph
        /// </summary>
        public FGraph(Dictionary<int, FMapNode> nodes, Dictionary<int, List<int[]>> requires)
        {

        }
        /// <summary>
        /// 用于序列化
        /// </summary>
        private FGraph() { }

        #region ---- FMap 抽象函数功能 ----
        [Obsolete]
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
        public override HashSet<FMapNode> GetLevelNodes(int level)
        {
            return Nodes.Values.Where(x => x.Level == level).ToHashSet();
        }
        public override HashSet<FMapNode> GetSiblingNodes(int id)
        {
            var requires = Requires[id];

            var set = new HashSet<FMapNode>();

            // 看起来很多，其实不会循环很多次，比遍历所有对应关系快多了
            foreach (var require in requires)
            {
                foreach (var required_id in require)
                {
                    var sib_ids = Linked[required_id];
                    foreach (var sib_id in sib_ids)
                    {
                        set.Add(Nodes[sib_id]);
                    }
                }
            }

            return set;
        }
        public override int GetBranchWidth(int id)
        {
            int count = 0;
            var stack = new Stack<int>();
            GetBranchWidth(id, ref count, ref stack);
            return count;
        }
        /// <summary>
        /// 获取当前节点下叶节点数量
        /// </summary>
        /// <param name="current">当前递归节点</param>
        /// <param name="count">总数</param>
        /// <param name="steps">已走路径，用于禁止走重复的节点，避免死循环</param>
        private void GetBranchWidth(int current, ref int count, ref Stack<int> steps)
        {
            steps.Push(current);

            var hasChild = Linked.TryGetValue(current, out HashSet<int> childs);
            // 当前节点是叶节点，累加并退出
            if (!hasChild) { count++; }
            else
            {
                foreach (var child in childs)
                {
                    // 已经走过这个节点，所以跳过，避免死循环
                    if (steps.Contains(child)) { continue; }
                    else
                    {
                        GetBranchWidth(child, ref count, ref steps);
                    }
                }
            }

            steps.Pop();
        }
        public override HashSet<FMapNode> GetLeafNodes(int id)
        {
            var nodes = new HashSet<FMapNode>();
            var stack = new Stack<int>();
            GetLeafNodes(id, ref nodes, ref stack);
            return nodes;
        }
        private void GetLeafNodes(int current, ref HashSet<FMapNode> nodes, ref Stack<int> steps)
        {
            steps.Push(current);

            var hasChild = Linked.TryGetValue(current, out HashSet<int> childs);
            // 当前节点是叶节点，累加并退出
            if (!hasChild) { nodes.Add(Nodes[current]); }
            else
            {
                foreach (var child in childs)
                {
                    // 已经走过这个节点，所以跳过，避免死循环
                    if (steps.Contains(child)) { continue; }
                    else
                    {
                        GetLeafNodes(child, ref nodes, ref steps);
                    }
                }
            }
            steps.Pop();
        }
        #endregion

        #region ---- 特有方法 ----
        /// <summary>
        /// 获取节点与关联的节点的依赖关系列表
        /// </summary>
        /// <param name="id">节点id</param>
        /// <returns>依赖关系列表</returns>
        /// <summary>
        public List<int[]> GetNodeRequires(int id)
        {
            return Requires[id];
        }
        public IEnumerator<KeyValuePair<int, HashSet<int>>> GetLinkedEnumerator() { return Linked.GetEnumerator(); }
        /// <summary>
        /// 获取 Nodes 的迭代器
        /// </summary>
        /// <returns>Nodes 的迭代器</returns>
        public IEnumerator<KeyValuePair<int, FMapNode>> GetNodesEnumerator() { return Nodes.GetEnumerator(); }
        /// <summary>
        /// 获取 NodeMap 的迭代器
        /// </summary>
        /// <returns>NodeMap 的迭代器</returns>
        public IEnumerator<KeyValuePair<int, Point>> GetNodeMapEnumerator() { return NodeMap.GetEnumerator(); }
        /// <summary>
        /// 获取 NodeMap 中指定的节点矩形信息
        /// </summary>
        /// <param name="index">节点ID</param>
        /// <returns>矩形信息</returns>
        public Point GetNodeMapElement(int index) { return NodeMap[index]; }
        /// <summary>
        /// 获取绘图用的已自动排序后的 NodeMap
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, Point> GetNodeMap()
        {
            NodeMap = new Dictionary<int, Point>();
            var branches = GetBranches(GetLevelNodes(0).Select(x => x.ID).ToArray(), true, true);
            var visited = new HashSet<int>();
            var width = branches.Count;
            var height = branches.Max(x => x.Length);

            for (int x = 0; x < branches.Count; x++)
            {
                var branch = branches[x];
                for (int y = 0; y < branch.Length; y++)
                {
                    var id = branch[y];
                    if (visited.Add(id))
                    {
                        var point = new Point(x, y);
                        NodeMap[id] = point;
                    }
                }
            }
            return NodeMap;
        }
        /// <summary>
        /// 获取 NodeMap 中心位置和尺寸
        /// </summary>
        /// <returns>Center + Size</returns>
        public Vector4 GetNodeMapBounds()
        {
            bool first = true;
            var bounds = new RectangleF();
            var enumer = NodeMap.GetEnumerator();
            while (enumer.MoveNext())
            {
                var p = enumer.Current.Value;
                if (first) { bounds = new RectangleF(p.X, p.Y, p.X, p.Y); first = false; continue; }
                if (p.X < bounds.X) { bounds.X = p.X; }
                if (p.X > bounds.Width) { bounds.Width = p.X; }
                if (p.Y < bounds.Y) { bounds.Y = p.Y; }
                if (p.Y > bounds.Height) { bounds.Height = p.Y; }
            }
            return new Vector4(
                (bounds.X + bounds.Width) / 2,
                (bounds.Y + bounds.Height) / 2,
                bounds.Width - bounds.X,
                bounds.Height - bounds.Y
                );
        }

        public List<int[]> GetBranches(int id, bool sort = false, bool reverse = false)
        {
            var branches = new List<int[]>();
            var stack = new Stack<int>();
            GetBranches(id, ref branches, ref stack, sort, reverse);
            return branches;
        }
        public List<int[]> GetBranches(int[] ids, bool sort = false, bool reverse = false)
        {
            var branches = new List<int[]>();
            var stack = new Stack<int>();
            foreach (var id in ids)
            {
                GetBranches(id, ref branches, ref stack, sort, reverse);
            }
            return branches;
        }
        private void GetBranches(int current, ref List<int[]> branches, ref Stack<int> steps, bool sort, bool reverse)
        {
            steps.Push(current);

            var hasChild = Linked.TryGetValue(current, out HashSet<int> childs);
            // 当前节点是叶节点，累加并退出
            if (!hasChild)
            {
                if (reverse)
                {
                    branches.Add(steps.Reverse().ToArray());
                }
                else
                {
                    branches.Add(steps.ToArray());
                }
            }
            else
            {
                // 是否按照 id 排序
                if (!sort)
                {
                    foreach (var child in childs)
                    {
                        // 已经走过这个节点，所以跳过，避免死循环
                        if (steps.Contains(child)) { continue; }
                        else
                        {
                            GetBranches(child, ref branches, ref steps, sort, reverse);
                        }
                    }
                }
                else
                {
                    var relation_ids = new List<int>();
                    foreach (var child in childs) { relation_ids.Add(child); }
                    relation_ids.Sort();
                    foreach (var id in relation_ids)
                    {
                        if (steps.Contains(id)) { continue; }
                        else
                        {
                            GetBranches(id, ref branches, ref steps, sort, reverse);
                        }
                    }
                }
            }

            steps.Pop();
        }
        #endregion

        #region ---- 序列化方法 ----
        // -- 序列化工具 --
        static XmlSerializer FData_serial = new(typeof(FData));
        static XmlSerializerNamespaces NullXmlNameSpace = new(new XmlQualifiedName[] { new XmlQualifiedName("", "") });

        // -- 序列化方法 --
        /// <summary>
        /// 序列化预留方法，默认返回 null
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        public async void ReadXml(XmlReader reader)
        {
            Nodes = new();
            Requires = new ();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "Node":
                                {
                                    var node = ReadNode(reader);
                                    Nodes.Add(node.ID, node);
                                    break;
                                }
                            case "Relation":
                                {
                                    int id = int.Parse(reader["ID"]);
                                    var relations = ReadRelation(reader);
                                    Requires[id] = relations;
                                    break;
                                }
                        }
                        break;
                }
            }
            CreateLinked();
            NodeMap = GetNodeMap();
        }
        /// <summary>
        /// 反序列化时用于读取节点的数据
        /// </summary>
        /// <param name="reader">读取到节点的流</param>
        /// <returns>节点</returns>
        /// <exception cref="Exception">节点中缺少FData的异常</exception>
        private FNode ReadNode(XmlReader reader)
        {
            int id = int.Parse(reader["ID"]);
            int level = int.Parse(reader["Level"]);
            while (reader.Read())
            {
                if (reader.Name == "FData")
                {
                    FData fdata = (FData)FData_serial.Deserialize(reader);
                    return new FNode(id, level, ref fdata, reader);
                }
                // 如果读取到了节点尾部
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    throw new Exception("[2302190831] 读取XML文件的时候节点没有读取到 FData");
                }
            }
            throw new Exception("[2302190819] 读取XML文件的时候节点没有读取到 FData");
        }
        /// <summary>
        /// 反序列化时用于读取节点的关系
        /// </summary>
        /// <param name="reader">读取到节点关系的流</param>
        /// <returns>当前节点关系</returns>
        private List<int[]> ReadRelation(XmlReader reader)
        {
            var relations = new List<int[]>();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    return relations;
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Require")
                {
                    reader.Read();
                    var requir_str = reader.ReadContentAsString();
                    relations.Add(IdArrayFromString(requir_str));
                    // 如果顺序反过来这里需要 continue
                }
            }
            throw new Exception("[2302191020] 读取 Relation列表 时未能找到结束标签");
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
                writer.WriteAttributeString("ID", node.Key.ToString());
                writer.WriteAttributeString("Level", node.Value.Level.ToString());
                FData_serial.Serialize(writer, node.Value.FocusData, NullXmlNameSpace);
                writer.WriteEndElement();
                // </Node>
            }
            writer.WriteEndElement();
            // </Nodes>

            //==== 序列化节点关系 ====//

            // <NodesRelations> 序列化节点关系字典
            writer.WriteStartElement("NodesRelations");
            foreach (var r_pair in Requires)
            {
                // <RNode> 当前节点
                writer.WriteStartElement("Relation");
                writer.WriteAttributeString("ID", r_pair.Key.ToString());

                foreach (var require in r_pair.Value)
                {
                    // <Require> 关系类型
                    writer.WriteElementString("Require", IdArrayToString(require));
                    // </Require>
                }

                writer.WriteEndElement();
                // </RNode>
            }
            writer.WriteEndElement();
            // </Relations>
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
        #endregion
    }
}
