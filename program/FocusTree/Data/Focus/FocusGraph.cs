#define DEBUG
using FocusTree.IO;
using FocusTree.IO.FileManege;
using Newtonsoft.Json;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FocusTree.Data.Focus
{
    public class FocusGraph : IXmlSerializable, IHistoryable, IFileManageable
    {
        #region ---- 存档文件路径 ----

        /// <summary>
        /// 存档文件路径
        /// </summary>
        internal string FilePath;

        #endregion

        #region ---- 基本变量 ----

        /// <summary>
        /// 节点id列表
        /// </summary>
        public List<int> IdList { get { return NodeCatalog.Keys.ToList(); } }
        /// <summary>
        /// 以 ID 作为 Key 的所有节点
        /// </summary>
        Dictionary<int, FocusNode> NodeCatalog;

        #endregion

        #region ---- 图像信息 ----

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return Path.GetFileNameWithoutExtension(FilePath); }
        }
        /// <summary>
        /// 节点数量
        /// </summary>
        public int NodesCount
        {
            get { return NodeCatalog.Count; }
        }
        /// <summary>
        /// 分支数量
        /// </summary>
        public int BranchesCount { get; private set; }
        /// <summary>
        /// 全图的元中心坐标和元尺寸
        /// </summary>
        /// <returns></returns>
        public (Vector2, SizeF) GetGraphMetaData()
        {
            bool first = true;
            var bounds = new RectangleF();
            foreach (var node in NodeCatalog.Values)
            {
                var point = node.MetaPoint;
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

        #region ---- 节点操作 ----

        /// <summary>
        /// 添加节点 O(1)，绘图时记得重新调用 GetNodeMap
        /// </summary>
        /// <returns>是否添加成功</returns>
        public bool AddNode(FocusNode node)
        {
            if (NodeCatalog.TryAdd(node.ID, node) == false)
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
            if (NodeCatalog.ContainsKey(id) == false)
            {
                MessageBox.Show($"[2303031221]提示：无法移除节点 - NodeCatalog 未包含 ID = {id} 的节点。");
                return false;
            }
            // 在所有的节点依赖组合中删除此节点
            foreach (var node in NodeCatalog.Values)
            {
                foreach (var require in node.Requires)
                {
                    require.Remove(id);
                }
            }
            // 从节点表中删除此节点
            NodeCatalog.Remove(id);
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
        public bool EditNode(FocusNode newData)
        {
            var id = newData.ID;
            if (NodeCatalog.ContainsKey(id) == false)
            {
                MessageBox.Show($"[2303031232]提示：无法编辑节点 - 新节点数据的 ID({id}) 不存在于 NodeCatalog。");
                return false;
            }
            NodeCatalog[id] = newData;
            CreateLinkes();
            SetMetaPoints();
            return true;
        }
        /// <summary>
        /// 获取所有无任何依赖的节点（根节点）  O(n)
        /// </summary>
        /// <returns>根节点</returns>
        //[Obsolete("经常出BUG，用的时候要小心")]
        public int[] GetRootNodes()
        {
            var result = new HashSet<int>();
            foreach (var node in NodeCatalog)
            {
                if (node.Value.Requires.Sum(x => x.Count) == 0)
                {
                    result.Add(node.Key);
                }
            }
            return result.ToArray();
        }

        #endregion

        #region ---- 图像操作 ----

        /// <summary>
        /// 使用 Requires 创建 Linked
        /// </summary>
        private void CreateLinkes()
        {
            foreach (var node in NodeCatalog.Values)
            {
                // 这里一定要清空，因为是刷新
                node.Links.Clear();
            }
            foreach (var node in NodeCatalog)
            {
                foreach (var require in node.Value.Requires)
                {
                    foreach (var requireId in require)
                    {
                        NodeCatalog[requireId].Links.Add(node.Key);
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
            var branches = GetBranches(GetRootNodes(), true, true);
            BranchesCount = branches.Count;
            CombineBranchNodes(branches);
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
            if (NodeCatalog[currentNode].Links.Count == 0)
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
                var linkList = NodeCatalog[currentNode].Links.ToList();
                if (sort == true)
                {
                    linkList.Sort();
                }
                foreach (var node in linkList)
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
            if (branches.Count == 0)
            {
                return;
            }
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
            foreach (var coordinate in nodeCoordinates)
            {
                var x = coordinate.Value[0] + (coordinate.Value[1] - coordinate.Value[0]) / 2;
                var point = new Vector2(x, coordinate.Value[2]);
                NodeCatalog[coordinate.Key].MetaPoint = point;
            }
            CleanBlanksForX();
        }
        /// <summary>
        /// 清除横坐标之间无节点的间隙
        /// </summary>
        /// <param name="metaPoints"></param>
        /// <returns></returns>
        private void CleanBlanksForX()
        {
            // 集合相同x值的元坐标
            Dictionary<float, Dictionary<int, Vector2>> xMetaPoints = new();
            foreach (var node in NodeCatalog)
            {
                var x = node.Value.MetaPoint.X;
                if (xMetaPoints.ContainsKey(x) == false)
                {
                    xMetaPoints.Add(x, new Dictionary<int, Vector2>());
                }
                xMetaPoints[x].Add(node.Key, node.Value.MetaPoint);
            }
            var blank = 0;
            var width = NodeCatalog.Max(x => x.Value.MetaPoint.X);
            for (int x = 0; x <= width; x++)
            {
                if (xMetaPoints.ContainsKey(x))
                {
                    foreach (var nodePoint in xMetaPoints[x])
                    {
                        var point = new Vector2(nodePoint.Value.X - blank, nodePoint.Value.Y);
                        NodeCatalog[nodePoint.Key].MetaPoint = point;
                    }
                }
                else
                {
                    blank++;
                }
            }
        }

        /// <summary>
        /// 按 NodeCatalog 的顺序重排节点ID
        /// </summary>
        public void ReorderNodeIds()
        {
            Dictionary<int, FocusNode> TempNodeCatalog = new();
            var branches = GetBranches(GetRootNodes(), true, true);
            foreach (var branch in branches)
            {
                foreach (var node in branch)
                {
                    TempNodeCatalog.TryAdd(node, NodeCatalog[node]);
                }
            }
            NodeCatalog = TempNodeCatalog;
            Dictionary<int, FocusNode> newNodeCatalog = new();
            Dictionary<int, int> ExchangePairs = new();
            Dictionary<int, List<HashSet<int>>> tempRequires = new();
            Dictionary<int, List<HashSet<int>>> newRequires = new();
            var enumer = NodeCatalog.GetEnumerator();
            for (int newId = 1; enumer.MoveNext(); newId++)
            {
                var data = enumer.Current.Value;
                data.ID = newId;
                newNodeCatalog.Add(newId, data);
                ExchangePairs.Add(enumer.Current.Key, newId);
                foreach (var child in enumer.Current.Value.Links)
                {
                    List<HashSet<int>> newGroups = new();
                    foreach (var requireGroup in NodeCatalog[child].Requires)
                    {
                        HashSet<int> newRequireGroup = new();
                        foreach (var parent in requireGroup)
                        {
                            if (parent == enumer.Current.Key)
                            {
                                newRequireGroup.Add(newId);
                            }
                            else
                            {
                                newRequireGroup.Add(parent);
                            }
                        }
                        newGroups.Add(newRequireGroup);
                    }
                    if (tempRequires.TryAdd(child, newGroups) == false)
                    {
                        tempRequires[child] = newGroups;
                    }
                }
            }
            enumer = NodeCatalog.GetEnumerator();
            while (enumer.MoveNext())
            {
                List<HashSet<int>> groups;
                if (tempRequires.TryGetValue(enumer.Current.Key, out groups))
                {
                    newRequires.Add(ExchangePairs[enumer.Current.Key], groups);
                }
            }
            NodeCatalog = newNodeCatalog;
            foreach (var requires in newRequires)
            {
                NodeCatalog[requires.Key].Requires = requires.Value;
            }
            CreateLinkes();
            SetMetaPoints();
            this.EnqueueHistory();
        }
        /// <summary>
        /// 保存为xml文件，并更新存档文件路径
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            FilePath = path;
            this.SaveToXml(path);
            Latest = Format();
        }

        #endregion

        #region ---- xml序列化 ----

        /// <summary>
        /// 用于序列化
        /// </summary>
        private FocusGraph()
        {
        }

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
            NodeCatalog = new();

            while (reader.Read())
            {
                //if (reader.NodeType != XmlNodeType.Element) { continue; }
                if (reader.Name == "FilePath")
                {
                    FilePath = reader.ReadElementContentAsString();
                }
                if (reader.Name == "Nodes")
                {
                    if (reader.ReadToDescendant("Node") == false) { continue; }
                    do
                    {
                        if (reader.Name == "Nodes" && reader.NodeType == XmlNodeType.EndElement) { break; }
                        if (reader.Name == "Node")
                        {
                            FocusNode node = new();
                            try { node.ReadXml(reader); }
                            catch { continue; }
                            NodeCatalog.Add(node.ID, node);
                        }
                    } while (reader.Read());

                }
            }
            CreateLinkes();
            SetMetaPoints();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("FilePath", FilePath);

            //==== 序列化节点数据 ====//

            // <Nodes>
            writer.WriteStartElement("Nodes");
            foreach (var node in NodeCatalog.Values)
            {
                // <Node>
                node.WriteXml(writer);
                // </Node>
            }

            writer.WriteEndElement();
            // </Nodes>
        }

        #endregion

        #region ---- 文件管理工具 ----

        public string FileManageDirectory { get; private set; } = "FoucsGraph";
        public new int GetHashCode()
        {
            var cachePath = this.GetCachePath("hashTemp");
            XmlIO.SaveToXml(this, cachePath); 
            HashAlgorithm sha = SHA256.Create();
            FileStream data = new(cachePath, FileMode.Open);
            byte[] hash = sha.ComputeHash(data);
            data.Close();
            return BitConverter.ToInt32(hash, 0);
        }

        #endregion

        #region ---- 历史和备份工具 ----

        public int HistoryIndex { get; set; } = 0;
        public int CurrentHistoryLength { get; set; } = 0;
        public FormattedData[] History { get; set; } = new FormattedData[20];
        public FormattedData Latest { get; set; }
        public FormattedData Format()
        {
            var hashCode = GetHashCode().ToString();
            if (!Directory.Exists(hashCode))
            {
                XmlIO.SaveToXml(this, this.GetCachePath(hashCode));
            }
            return new(hashCode);
        }
        public void Deformat(FormattedData data)
        {
            NodeCatalog = new();
            var g = XmlIO.LoadFromXml<FocusGraph>(this.GetCachePath(data.Items[0]));
            NodeCatalog = g.NodeCatalog;
            CreateLinkes();
            SetMetaPoints();
        }
        /// <summary>
        /// 开始新的历史记录
        /// </summary>
        public void NewHistory()
        {
            ObjectHistory.NewHistory(this);
            this.ClearCache();
        }

        #endregion

        #region ---- 变量获取器 ----

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public FocusNode GetNode(int id)
        {
            return NodeCatalog.TryGetValue(id, out var node) ? node : throw new ArgumentException("不存在的节点id");
        }
        /// <summary>
        /// 获取当前的所有 Node
        /// </summary>
        /// <returns></returns>
        public FocusNode[] GetNodes()
        {
            return NodeCatalog.Values.ToArray();
        }

        #endregion
    }
}
