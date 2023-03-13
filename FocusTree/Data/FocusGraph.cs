using System.Numerics;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FocusTree.Tool;

namespace FocusTree.Data
{
    public class FocusGraph : IXmlSerializable, IHistoryable<(string, string)>
    {
        #region ---- 存档文件名 ----

        /// <summary>
        /// 存档文件名
        /// </summary>
        internal string FilePath;

        #endregion

        #region ---- 基本变量 ----

        public List<int> IdList
        {
            get { return NodesCatalog.Keys.ToList(); }
        }
        /// <summary>
        /// 以 ID 作为 Key 的所有节点
        /// </summary>
        Dictionary<int, FocusData> NodesCatalog;
        /// <summary>
        /// 节点依赖的节点组合
        /// </summary>
        Dictionary<int, List<HashSet<int>>> RequireGroups;
        /// <summary>
        /// 节点的子链接 (自动生成) (父节点, 多个子节点)
        /// </summary>
        Dictionary<int, HashSet<int>> ChildLinkes;
        /// <summary>
        /// 节点显示位置
        /// </summary>
        Dictionary<int, Vector2> MetaPoints;

        #endregion

        #region ---- 基本信息 ----

        public int NodesCount
        {
            get { return NodesCatalog.Count; }
        }
        public int BranchesCount { get; private set; }

        #endregion

        #region ---- 节点操作 ----

        /// <summary>
        /// 添加节点 O(1)，绘图时记得重新调用 GetNodeMap
        /// </summary>
        /// <returns>是否添加成功</returns>
        public bool AddNode(FocusData node)
        {
            if (NodesCatalog.TryAdd(node.ID, node) == false)
            {
                MessageBox.Show("[2303031210]提示：无法添加节点 - 无法加入字典。");
                return false;
            }
            CreateLinkes();
            SetMetaPoints();
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
                MessageBox.Show($"[2303031221]提示：无法移除节点 - NodesCatalog 未包含 ID = {id} 的节点。");
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
            CreateLinkes();
            SetMetaPoints();
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
                MessageBox.Show($"[2303031232]提示：无法编辑节点 - 新节点数据的 ID({id}) 不存在于 NodesCatalog。");
                return false;
            }
            NodesCatalog[id] = newData;
            CreateLinkes();
            SetMetaPoints();
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
                if (RequireGroups.TryGetValue(id, out List<HashSet<int>> requireGroups) == false ||
                    requireGroups.Sum(x => x.Count) == 0)
                {
                    result.Add(id);
                }
            }
            return result;
        }

        #endregion

        #region ---- 图像操作 ----

        /// <summary>
        /// 使用 Requires 创建 Linked
        /// </summary>
        private void CreateLinkes()
        {
            // 这里一定要重新初始化，因为是刷新
            ChildLinkes = new();

            foreach (var requireGroups in RequireGroups)
            {
                foreach (var requireGroup in requireGroups.Value)
                {
                    foreach (var requireNode in requireGroup)
                    {
                        // 如果父节点没有创建 HashSet 就创建一个新的
                        ChildLinkes.TryAdd(requireNode, new HashSet<int>());
                        // 向 ChildLinkes 里父节点的对应条目里添加当前节点（HashSet自动忽略重复项）
                        ChildLinkes[requireNode].Add(requireGroups.Key);
                    }
                }
            }
        }
        /// <summary>
        /// 获取绘图用的已自动排序后的 NodeMap
        /// </summary>
        /// <returns></returns>
        private void SetMetaPoints()
        {
            var rootNodes = GetRootNodes().ToArray();
            var branches = GetBranches(rootNodes, true, true);
            BranchesCount = branches.Count;
            CombineBranchNodes(branches);
            CleanBlanksForX();
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
            if (ChildLinkes.TryGetValue(currentNode, out HashSet<int> childLinkes) == false || childLinkes.Count == 0)
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
                var childLinkesList = childLinkes.ToList();
                if (sort == true)
                {
                    childLinkesList.Sort();
                }
                foreach (var node in childLinkesList)
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
        /// <summary>
        /// 合并不同分支上的相同节点，使节点在分支范围内尽量居中
        /// </summary>
        /// <param name="branches"></param>
        /// <returns></returns>
        private void CombineBranchNodes(List<int[]> branches)
        {
            Dictionary<int, List<int>> nodeCoordinates = new();
            var width = branches.Count;
            var height = branches.Max(x => x.Length);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y >= branches[x].Length)
                    {
                        continue;
                    }
                    var node = branches[x][y];
                    if (nodeCoordinates.ContainsKey(node))
                    {
                        nodeCoordinates[node][0] = nodeCoordinates[node][0] < x ? nodeCoordinates[node][0] : x;
                        nodeCoordinates[node][1] = nodeCoordinates[node][1] > x ? nodeCoordinates[node][1] : x;
                        nodeCoordinates[node][2] = nodeCoordinates[node][2] > y ? nodeCoordinates[node][2] : y;
                    }
                    else
                    {
                        nodeCoordinates.Add(node, new List<int>());
                        // 起始x, [0]
                        nodeCoordinates[node].Add(x);
                        // 终止x, [1]
                        nodeCoordinates[node].Add(x);
                        // y, [2]
                        nodeCoordinates[node].Add(y);
                    }
                }
            }
            MetaPoints = new();
            foreach (var coordinate in nodeCoordinates)
            {
                var x = coordinate.Value[0] + (coordinate.Value[1] - coordinate.Value[0]) / 2;
                var point = new Vector2(x, coordinate.Value[2]);
                MetaPoints[coordinate.Key] = point;
            }
        }
        /// <summary>
        /// 清除横坐标之间无节点的间隙
        /// </summary>
        /// <param name="metaPoints"></param>
        /// <returns></returns>
        private void CleanBlanksForX()
        {
            Dictionary<float, Dictionary<int, Vector2>> xMetaPoints = new();
            foreach (var nodePoint in MetaPoints)
            {
                var x = nodePoint.Value.X;
                if (xMetaPoints.ContainsKey(x) == false)
                {
                    xMetaPoints.Add(x, new Dictionary<int, Vector2>());
                }
                xMetaPoints[x].Add(nodePoint.Key, nodePoint.Value);
            }
            var blank = 0;
            var width = MetaPoints.Max(x => x.Value.X);
            for (int x = 0; x <= width; x++)
            {
                if (xMetaPoints.ContainsKey(x))
                {
                    foreach (var nodePoint in xMetaPoints[x])
                    {
                        var point = new Vector2(nodePoint.Value.X - blank, nodePoint.Value.Y);
                        MetaPoints[nodePoint.Key] = point;
                    }
                }
                else
                {
                    blank++;
                }
            }
        }
        /// <summary>
        /// 全图的元中心坐标和元尺寸
        /// </summary>
        /// <returns></returns>
        public (Vector2, SizeF) CenterMetaData()
        {
            bool first = true;
            var bounds = new RectangleF();
            foreach (var point in MetaPoints.Values)
            {
                if (first)
                {
                    bounds = new RectangleF(point.X, point.Y, point.X, point.Y);
                    first = false;
                }
                else
                {
                    bounds.X = point.X < bounds.X ? point.X : bounds.X;
                    bounds.Y = point.Y < bounds.Y ? point.Y : bounds.Y;
                    bounds.Width = point.X > bounds.Width ? point.X : bounds.Width;
                    bounds.Height = point.Y > bounds.Height ? point.Y : bounds.Height;
                }
            }
            return (
                new Vector2((bounds.X + bounds.Width) / 2, (bounds.Y + bounds.Height) / 2),
                new SizeF(bounds.Width - bounds.X + 1, bounds.Height - bounds.Y + 1)
                );
        }

        #endregion

        #region ---- 构造函数 ----
        /// <summary>
        /// 从CSV生成专用，不更新历史记录
        /// </summary>
        /// <param name="nodesCatalog">节点列表</param>
        /// <param name="requireGroups">依赖组合列表</param>
        internal FocusGraph(string path, Dictionary<int, FocusData> nodesCatalog, Dictionary<int, List<HashSet<int>>> requireGroups)
        {
            FilePath = path;
            NodesCatalog = nodesCatalog;
            RequireGroups = requireGroups;
            CreateLinkes();
            SetMetaPoints();
            ObjectHistory<(string, string)>.Initialize(this);
        }
        /// <summary>
        /// 用于序列化
        /// </summary>
        private FocusGraph()
        {
        }
        /// <summary>
        /// 更新文件路径和历史记录
        /// </summary>
        /// <param name="path">新的文件路径</param>
        /// <param name="graph">原来的graph</param>
        public FocusGraph(string path, FocusGraph graph)
        {
            FilePath = path;
            NodesCatalog = graph.NodesCatalog;
            RequireGroups = graph.RequireGroups;
            CreateLinkes();
            SetMetaPoints();
            ObjectHistory<(string, string)>.Initialize(this);
        }

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
                if (reader.Name == "FilePath")
                {
                    reader.Read();
                    FilePath = reader.ReadContentAsString();
                }
                if (reader.Name == "Nodes")
                {
                    reader.Read();
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Node")
                        {
                            var node = (FocusData)FData_serial.Deserialize(reader);
                            NodesCatalog[node.ID] = node;
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

            CreateLinkes();
            SetMetaPoints();
            ObjectHistory<(string, string)>.Initialize(this);
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
            writer.WriteElementString("FilePath", FilePath);

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

        #endregion

        #region ---- 历史和备份工具 ----

        public (string, string)[] History { get { return history; } }
        (string, string)[] history
            = new (string, string)[20];
        public (string, string) Serialize()
        {
            return JsObject.Serialize(NodesCatalog, RequireGroups);
        }
        public void Deserialize(int index)
        {
            var metas = JsObject.DeSerialize<
                Dictionary<int, FocusData>,
                Dictionary<int, List<HashSet<int>>>>(History[index]);
            NodesCatalog = metas.Item1;
            RequireGroups = metas.Item2;
            CreateLinkes();
            SetMetaPoints();
        }
        public bool Equals(FocusGraph other)
        {
            if (this == null && other == null)
            {
                return true;
            }
            else if (this == null || other == null)
            {
                return false;
            }
            else
            {
                return Serialize() == other.Serialize();
            }
        }
        public bool HasPrevHistory()
        {
            return ObjectHistory<(string, string)>.HasPrev();
        }
        public bool HasNextHistory()
        {
            return ObjectHistory<(string, string)>.HasNext();
        }
        public bool HasHistory()
        {
            return ObjectHistory<(string, string)>.HasHistory();
        }
        public void Undo()
        {
            if (ObjectHistory<(string, string)>.HasPrev())
            {
                ObjectHistory<(string, string)>.Undo(this);
            }
        }
        public void Redo()
        {
            if (ObjectHistory<(string, string)>.HasNext())
            {
                ObjectHistory<(string, string)>.Redo(this);
            }
        }
        public void HistoryEnqueue()
        {
            ObjectHistory<(string, string)>.Enqueue(this);
        }

        #endregion

        #region ---- 变量获取器 ----

        public FocusData GetNodeData(int id)
        {
            NodesCatalog.TryGetValue(id, out var focusData);
            return focusData;
        }
        public List<HashSet<int>> GetRequireGroups(int id)
        {
            RequireGroups.TryGetValue(id, out var groupList);
            return groupList;
        }
        public HashSet<int> GetChildLinks(int id)
        {
            ChildLinkes.TryGetValue(id, out var childLinks);
            return childLinks;
        }
        public Vector2 GetMetaPoint(int id)
        {
            MetaPoints.TryGetValue(id, out var metaPoint);
            {
                return metaPoint;
            }
        }
        public IEnumerator<KeyValuePair<int, FocusData>> GetNodesDataEnumerator()
        {
            return NodesCatalog.GetEnumerator();
        }
        public IEnumerator<KeyValuePair<int, List<HashSet<int>>>> GetRequireGroupsEnumerator()
        {
            return RequireGroups.GetEnumerator();
        }
        public IEnumerator<KeyValuePair<int, HashSet<int>>> GetChildLinksEnumerator()
        {
            return ChildLinkes.GetEnumerator();
        }
        public IEnumerator<KeyValuePair<int, Vector2>> GetMetaPointsEnumerator()
        {
            return MetaPoints.GetEnumerator();
        }

        #endregion
    }
}
