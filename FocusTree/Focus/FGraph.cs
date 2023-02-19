using FocusTree.IO;
using FocusTree.Tree;
using System.Numerics;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using static FocusTree.Focus.FHistory;

namespace FocusTree.Focus
{
    public class FGraph : IXmlSerializable
    {
        #region ---- 基本变量 ----

        /// <summary>
        /// 文件名
        /// </summary>
        public string FilePath { get; private set; }
        /// <summary>
        /// 以 ID 作为 Key 的所有节点
        /// </summary>
        private Dictionary<int, FData> Nodes;
        /// <summary>
        /// 节点依赖的节点 (子节点, 多组父节点)
        /// </summary>
        private Dictionary<int, List<HashSet<int>>> Requires;
        /// <summary>
        /// 依赖于节点的节点 (自动生成) (父节点, 多个子节点)
        /// </summary>
        private Dictionary<int, HashSet<int>> Linked;
        /// <summary>
        /// 节点显示位置
        /// </summary>
        private Dictionary<int, Point> NodeMap;

        #endregion

        #region ---- 节点操作 ----

        /// <summary>
        /// 用ID读取节点 O(1)
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>节点数据</returns>
        public FData GetNode(int id)
        {
            return Nodes[id];
        }
        /// <summary>
        /// 添加节点 O(1)，绘图时记得重新调用 GetNodeMap
        /// </summary>
        /// <returns>是否添加成功</returns>
        public bool AddNode(FData node)
        {
            var suc = Nodes.TryAdd(node.ID, node);
            UpdateNodes(); FHistory.Enqueue(this);
            return suc;
        }
        /// <summary>
        /// 删除节点 O(2n+)，绘图时记得重新调用 GetNodeMap
        /// </summary>
        /// <returns>是否成功删除</returns>
        public void RemoveNode(int id)
        {
            // 移除节点依赖项 (因为都是传递引用，所以可以直接操作)
            Requires.Remove(id);
            foreach (var requires in Requires.Values)
            {
                foreach (var requireGroup in requires)
                {
                    requireGroup.Remove(id);
                }
            }
            // 移除节点
            Nodes.Remove(id);
            // 重新创建节点连接
            UpdateNodes(); FHistory.Enqueue(this);
        }
        /// <summary>
        /// 编辑节点 O(1)，新数据的ID必须匹配
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="newData">要替换的数据</param>
        /// <returns>修改是否成功</returns>
        public bool EditNode(int id, FData newData)
        {
            // 编辑的节点ID不匹配
            if (id != newData.ID) { return false; }
            Nodes[id] = newData;

            UpdateNodes(); FHistory.Enqueue(this);

            return true;
        }
        /// <summary>
        /// 获取所有根节点 (不依赖任何节点的节点)  O(n)
        /// </summary>
        /// <returns>根节点</returns>
        public HashSet<int> GetRootNodes()
        {
            var result = new HashSet<int>();
            foreach (var id in Nodes.Keys)
            {
                if (!Requires.ContainsKey(id) || Requires[id].Sum(x => x.Count) == 0) { result.Add(id); }
            }
            return result;
        }
        /// <summary>
        /// 获取节点所依赖的节点关系  O(1)
        /// </summary>
        /// <param name="id">节点id</param>
        /// <returns>依赖关系列表</returns>
        /// <summary>
        public List<HashSet<int>> GetNodeRequires(int id)
        {
            Requires.TryGetValue(id, out List<HashSet<int>> requires);
            return requires;
        }
        /// <summary>
        /// 获取依赖于节点的节点（不含分组关系） O(1)
        /// </summary>
        /// <param name="id">节点id</param>
        /// <returns>连接的节点</returns>
        public HashSet<int> GetNodeLinks(int id)
        {
            Linked.TryGetValue(id, out HashSet<int> links);
            return links;
        }
        /// <summary>
        /// 根据节点和依赖更新 Graph
        /// </summary>
        public void UpdateNodes()
        {
            CreateLinked();
            NodeMap = GetNodeMap();
        }

        #endregion

        #region ---- 读写与重载 ---

        /// <summary>
        /// 从 csv 文件中读取 Graph
        /// </summary>
        /// <param name="path">国策树 .csv文件</param>
        public FGraph(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception("[2302191048] 文件不存在: " + path);
            }

            FilePath = path;
            Nodes = new();
            Requires = new();

            // 根据不同扩展名以对应方式加载
            switch (Path.GetExtension(path).ToLower())
            {
                // 从 Csv 读取
                case ".csv":
                    {
                        FCsv.ReadGraphFromCsv(path, ref Nodes, ref Requires);
                        // 推断 Link 关系
                        UpdateNodes();
                        FHistory.Clear(); FHistory.Enqueue(this);
                        break;
                    }
                // 不是 csv 文件时
                default: throw new Exception("[2302191420] 不是有效的 csv 文件");
            }
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
        /// 序列化时传递文件名
        /// </summary>
        public void SetFileName(string filename)
        {
            FilePath = filename;
        }
        /// <summary>
        /// 用于序列化
        /// </summary>
        private FGraph() { }

        #endregion

        #region ---- 特有方法 ----
        /// <summary>
        /// 获取节点 Linked 的迭代器（与子节点的连接）
        /// </summary>
        /// <returns>Linked 迭代器</returns>
        public IEnumerator<KeyValuePair<int, HashSet<int>>> GetLinkedEnumerator() { return Linked.GetEnumerator(); }
        /// <summary>
        /// 获取 Nodes 的迭代器
        /// </summary>
        /// <returns>Nodes 的迭代器</returns>
        public IEnumerator<KeyValuePair<int, FData>> GetNodesEnumerator() { return Nodes.GetEnumerator(); }
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
            var branches = GetBranches(GetRootNodes().ToArray(), true, true);
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

        public void ReadXml(XmlReader reader)
        {
            Nodes = new();
            Requires = new();

            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element) { continue; }
                if (reader.Name == "Nodes")
                {
                    reader.Read();
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Node")
                        {
                            var node = (FData)FData_serial.Deserialize(reader);
                            Nodes.Add(node.ID, node);
                        }
                        else { reader.Read(); }
                    }
                }
                if (reader.Name == "Relation")
                {
                    int id = int.Parse(reader["ID"]);
                    var relations = ReadRelation(reader);
                    Requires[id] = relations;
                }
            }
            UpdateNodes(); FHistory.Clear(); FHistory.Enqueue(this);
        }
        /// <summary>
        /// 反序列化时用于读取节点的关系
        /// </summary>
        /// <param name="reader">读取到节点关系的流</param>
        /// <returns>当前节点关系</returns>
        private List<HashSet<int>> ReadRelation(XmlReader reader)
        {
            var relations = new List<HashSet<int>>();
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
                FData_serial.Serialize(writer, node.Value, NullXmlNameSpace);
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

        #endregion

        #region ---- 其它工具 ----

        public static string IdArrayToString(HashSet<int> ids)
        {
            var splitmark = ", ";
            var sb = new StringBuilder();
            foreach (var id in ids) { sb.Append(id.ToString() + splitmark); }

            var str = sb.ToString().Trim();
            if (str.EndsWith(",")) { str = str.Substring(0, str.Length - 1); }

            return str;
        }
        public static HashSet<int> IdArrayFromString(string ids)
        {
            var split = ids.Split(',').Where(x => !string.IsNullOrWhiteSpace(x));
            return split.Select(x => int.Parse(x)).ToHashSet();
        }

        /// <summary>
        /// 仅允许被 FHistory 访问的节点指针
        /// </summary>
        /// <param name="accessClass">用于限制访问类的工具</param>
        /// <returns>指针</returns>
        internal Dictionary<int, FData> GraphDataNodes_Get()
        {
            return Nodes;
        }
        /// <summary>
        /// 仅允许被 FHistory 访问的依赖指针
        /// </summary>
        /// <param name="accessClass">用于限制访问类的工具</param>
        /// <returns>指针</returns>
        internal Dictionary<int, List<HashSet<int>>> GraphDataRequires_Get()
        {
            return Requires;
        }
        internal void GraphDataNodes_Set(Dictionary<int, FData> nodes)
        {
            Nodes = nodes;
        }
        internal void GraphDataRequires_Set(Dictionary<int, List<HashSet<int>>> requires)
        {
            Requires = requires;
        }

        #endregion
    }
}
