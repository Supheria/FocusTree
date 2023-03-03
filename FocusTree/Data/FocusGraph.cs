using FocusTree.IO;
using System.Numerics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using static System.Windows.Forms.LinkLabel;

namespace FocusTree.Data
{
    public class FocusGraph : IXmlSerializable
    {
        #region ---- 基本变量 ----

        /// <summary>
        /// 文件名
        /// </summary>
        public string FilePath { get; private set; }
        /// <summary>
        /// 以 ID 作为 Key 的所有节点
        /// </summary>
        private Dictionary<int, FocusData> NodesCatalog;
        /// <summary>
        /// 节点依赖的节点组合
        /// </summary>
        private Dictionary<int, List<HashSet<int>>> RequireGroups;
        /// <summary>
        /// 依赖于节点的节点 (自动生成) (父节点, 多个子节点)
        /// </summary>
        private Dictionary<int, HashSet<int>> LinkedNodes;
        /// <summary>
        /// 节点显示位置
        /// </summary>
        private Dictionary<int, Point> NodePoints;

        #endregion

        #region ---- 节点操作 ----

        /// <summary>
        /// 用ID读取节点 O(1)
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>节点数据</returns>
        public FocusData GetNode(int id)
        {
            if (NodesCatalog.TryGetValue(id, out FocusData focusData) == false)
            {
                throw new Exception($"异常：无法获取节点 - NodesCatalog 未包含 ID = {id} 的节点。");
            }
            return focusData;
        }
        /// <summary>
        /// 添加节点 O(1)，绘图时记得重新调用 GetNodeMap
        /// </summary>
        /// <returns>是否添加成功</returns>
        public bool AddNode(FocusData node)
        {
            if (NodesCatalog.TryAdd(node.ID, node) == false) 
            {
                MessageBox.Show("提示：无法添加节点 - 无法加入字典。");
                return false;
            }
            UpdateGraph();
            return true;
        }
        /// <summary>
        /// 删除节点 O(2n+)，绘图时记得重新调用 GetNodeMap
        /// </summary>
        /// <returns>是否成功删除</returns>
        public bool RemoveNode(int id)
        {
            if (NodesCatalog.ContainsKey(id) == false)
            {
                MessageBox.Show($"提示：无法移除节点 - NodesCatalog 未包含 ID = {id} 的节点。");
                return false;
            }
            // 删除此节点所依赖的节点组合
            RequireGroups.Remove(id);
            // 在所有的节点依赖组合中删除此节点
            foreach (var requireGruops in RequireGroups.Values)
            {
                foreach (var requireGroup in requireGruops)
                {
                    requireGroup.Remove(id);
                }
            }
            // 从节点表中删除此节点
            NodesCatalog.Remove(id);
            UpdateGraph();
            return true;
        }
        /// <summary>
        /// 编辑节点 O(1)，新数据的ID必须匹配
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="newData">要替换的数据</param>
        /// <returns>修改是否成功</returns>
        public bool EditNode(FocusData newData)
        {
            var id = newData.ID;
            if (NodesCatalog.ContainsKey(id) == false)
            {
                MessageBox.Show($"提示：无法编辑节点 - 新节点数据的 ID({id}) 不存在于 NodesCatalog。");
                return false;
            }
            NodesCatalog[id] = newData;
            UpdateGraph();
            return true;
        }
        /// <summary>
        /// 获取所有无任何依赖的节点（根节点）  O(n)
        /// </summary>
        /// <returns>根节点</returns>
        //[Obsolete("经常出BUG，用的时候要小心")]
        public HashSet<int> GetRootNodes()
        {
            var result = new HashSet<int>();
            foreach (var id in NodesCatalog.Keys)
            {
                if (RequireGroups.TryGetValue(id, out List<HashSet<int>> requireGroups))
                {
                    if (requireGroups.Sum(x => x.Count) == 0)
                    {
                        result.Add(id);
                    }
                }
                else
                {
                    result.Add(id);
                    MessageBox.Show($"提示：RequireGroups 未包含 ID = {id} 对应的条目，\n已自动添加空条目。");
                    RequireGroups.Add(id, new List<HashSet<int>>());
                }
            }
            return result;
        }
        /// <summary>
        /// 获取节点所依赖的节点组合  O(1)
        /// </summary>
        /// <param name="id">节点id</param>
        /// <returns>依赖关系列表</returns>
        /// <summary>
        public List<HashSet<int>> GetNodeRequireGroups(int id)
        {
            if (RequireGroups.TryGetValue(id, out List<HashSet<int>> requireGroups) == false)
            {
                throw new Exception($"异常：无法获取此节点所依赖的节点组合 - \nRequireGroups 未包含 ID = {id} 对应的条目");
            }
            return requireGroups;
        }
        /// <summary>
        /// 获取依赖于节点的节点（不含分组关系） O(1)
        /// </summary>
        /// <param name="id">节点id</param>
        /// <returns>连接的节点</returns>
        public HashSet<int> GetNodeLinkedNodes(int id)
        {
            if (LinkedNodes.TryGetValue(id, out HashSet<int> linkedNodes) == false)
            {
                throw new Exception($"异常：无法获依赖此节点的节点 - \nLinkedNodes 未包含 ID = {id} 对应的条目");
            }
            return linkedNodes;
        }

        #endregion

        #region ---- 图像操作 ----

        /// <summary>
        /// 根据节点和依赖更新 Graph，重建依赖关系和节点位置图
        /// </summary>
        public void UpdateGraph()
        {
            CreateLinked();
            NodePoints = GetNodePoints();
            DataHistory.Enqueue(this);
        }
        /// <summary>
        /// 获取节点 LinkedNodes 的迭代器（与子节点的连接）
        /// </summary>
        /// <returns>LinkedNodes 迭代器</returns>
        public IEnumerator<KeyValuePair<int, HashSet<int>>> GetLinkedNodesEnumerator() 
        { 
            return LinkedNodes.GetEnumerator(); 
        }
        /// <summary>
        /// 获取 NodesCatalog 的迭代器
        /// </summary>
        /// <returns>NodesCatalog 的迭代器</returns>
        public IEnumerator<KeyValuePair<int, FocusData>> GetNodesCatalogEnumerator() 
        { 
            return NodesCatalog.GetEnumerator(); 
        }
        /// <summary>
        /// 获取 NodePoints 的迭代器
        /// </summary>
        /// <returns>NodePoints 的迭代器</returns>
        public IEnumerator<KeyValuePair<int, Point>> GetNodePointsEnumerator() 
        { 
            return NodePoints.GetEnumerator(); 
        }
        /// <summary>
        /// 获取 NodePoints 中指定节点的Point
        /// </summary>
        /// <param name="index">节点ID</param>
        /// <returns>节点的Point</returns>
        public Point GetNodePointsElement(int id) 
        { 
            if (NodePoints.TryGetValue(id, out Point point) == false)
            {
                throw new Exception($"异常：无法获取节点的Point - NodePoints 未包含 ID = {id} 对应的条目。");
            }
            return point; 
        }
        /// <summary>
        /// 获取绘图用的已自动排序后的 NodeMap
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, Point> GetNodePoints()
        {
            NodePoints = new Dictionary<int, Point>();
            var rootNodes = GetRootNodes().ToArray();
            var branches = GetBranches(rootNodes, true, true);
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
                        NodePoints[id] = point;
                    }
                }
            }
            return NodePoints;
        }
        /// <summary>
        /// 获取 NodeMap 中心位置和尺寸
        /// </summary>
        /// <returns>Center + Size</returns>
        public Vector4 GetNodeMapBounds()
        {
            bool first = true;
            var bounds = new RectangleF();
            var enumer = NodePoints.GetEnumerator();
            while (enumer.MoveNext())
            {
                var p = enumer.Current.Value;
                if (first) 
                { 
                    bounds = new RectangleF(p.X, p.Y, p.X, p.Y); 
                    first = false; 
                    continue; 
                }
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
        /// <summary>
        /// 获取某个节点的所有分支
        /// </summary>
        /// <param name="node">节点ID</param>
        /// <param name="sort">是否按照节点ID排序</param>
        /// <param name="reverse">是否从根节点向末节点排序</param>
        /// <returns></returns>
        public List<int[]> GetBranches(int node, bool sort, bool reverse)
        {
            var branches = new List<int[]>();
            var steps = new Stack<int>();
            GetBranches(node, ref branches, ref steps, sort, reverse);
            return branches;
        }
        public List<int[]> GetBranches(int[] nodes, bool sort, bool reverse)
        {
            var branches = new List<int[]>();
            var steps = new Stack<int>();
            foreach (var id in nodes)
            {
                GetBranches(id, ref branches, ref steps, sort, reverse);
            }
            return branches;
        }
        private void GetBranches(int currentNode, ref List<int[]> branches, ref Stack<int> steps, bool sort, bool reverse)
        {
            steps.Push(currentNode);
            // 当前节点是末节点
            if (LinkedNodes.TryGetValue(currentNode, out HashSet<int> linkedNodes) == false || linkedNodes.Count == 0)
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
                var linkedNodesList = linkedNodes.ToList();
                if (sort == true)
                {
                    linkedNodesList.Sort();
                }
                foreach (var node in linkedNodesList)
                {
                    if (steps.Contains(node))
                    {
                        // 已经走过这个节点，跳过，避免死循环
                        continue;
                    }
                    else
                    {
                        GetBranches(node, ref branches, ref steps, sort, reverse);
                    }
                }
            }

            steps.Pop();
        }

        #endregion

        #region ---- 读写与重载 ---

        /// <summary>
        /// 从 csv 文件中读取 Graph
        /// </summary>
        /// <param name="path">国策树 .csv文件</param>
        public FocusGraph(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception("[2302191048] 文件不存在: " + path);
            }

            FilePath = path;
            NodesCatalog = new();
            RequireGroups = new();

            // 根据不同扩展名以对应方式加载
            switch (Path.GetExtension(path).ToLower())
            {
                // 从 Csv 读取
                case ".csv":
                    {
                        CsvReader.ReadGraphFromCsv(path, ref NodesCatalog, ref RequireGroups);
                        DataHistory.Clear();
                        // 推断 Link 关系
                        UpdateGraph();
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
            LinkedNodes = new();

            foreach (var requireGroups in RequireGroups)
            {
                foreach (var requireGroup in requireGroups.Value)
                {
                    foreach (var requireNode in requireGroup)
                    {
                        // 如果父节点没有创建 HashSet 就创建一个新的
                        LinkedNodes.TryAdd(requireNode, new HashSet<int>());
                        // 向 LinkedNodes 里父节点的对应条目里添加当前节点（HashSet自动忽略重复项）
                        LinkedNodes[requireNode].Add(requireGroups.Key);
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
        private FocusGraph() { }

        #endregion

        #region ---- 序列化方法 ----
        // -- 序列化工具 --
        static XmlSerializer FData_serial = new(typeof(FocusData));
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
            NodesCatalog = new();
            RequireGroups = new();

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
                            var node = (FocusData)FData_serial.Deserialize(reader);
                            NodesCatalog.Add(node.ID, node);
                        }
                        else { reader.Read(); }
                    }
                }
                if (reader.Name == "Relation")
                {
                    int id = int.Parse(reader["ID"]);
                    var relations = ReadRelation(reader);
                    if (relations != null)
                    {
                        RequireGroups[id] = relations;
                    }
                }
            }
            UpdateGraph(); DataHistory.Clear(); DataHistory.Enqueue(this);
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

                    if (!reader.HasValue) { return null; }

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
            foreach (var node in NodesCatalog)
            {
                FData_serial.Serialize(writer, node.Value, NullXmlNameSpace);
            }
            writer.WriteEndElement();
            // </Nodes>

            //==== 序列化节点关系 ====//

            // <NodesRelations> 序列化节点关系字典
            writer.WriteStartElement("NodesRelations");
            foreach (var r_pair in RequireGroups)
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
        internal Dictionary<int, FocusData> GraphDataNodes_Get()
        {
            return NodesCatalog;
        }
        /// <summary>
        /// 仅允许被 FHistory 访问的依赖指针
        /// </summary>
        /// <param name="accessClass">用于限制访问类的工具</param>
        /// <returns>指针</returns>
        internal Dictionary<int, List<HashSet<int>>> GraphDataRequires_Get()
        {
            return RequireGroups;
        }
        internal void GraphDataNodes_Set(Dictionary<int, FocusData> nodes)
        {
            NodesCatalog = nodes;
        }
        internal void GraphDataRequires_Set(Dictionary<int, List<HashSet<int>>> requires)
        {
            RequireGroups = requires;
        }

        #endregion
    }
}
