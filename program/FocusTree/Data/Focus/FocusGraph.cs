#define DEBUG
using FocusTree.IO;
using FocusTree.IO.FileManege;
using Newtonsoft.Json;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace FocusTree.Data.Focus
{
    public class FocusGraph : IHistoryable, IBackupable
    {
        #region ---- 基本变量 ----

        /// <summary>
        /// 节点id列表
        /// </summary>
        public List<int> IdList { get { return NodeCatalog.Keys.ToList(); } }
        /// <summary>
        /// 以 ID 作为 Key 的所有节点
        /// </summary>
        Dictionary<int, FocusNode> NodeCatalog;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }
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
            CreateNodesLinkes();
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
            CreateNodesLinkes();
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
            CreateNodesLinkes();
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

        /// <summary>
        /// 使用 Requires 创建所有节点的 Links
        /// </summary>
        private void CreateNodesLinkes()
        {
            foreach (var node in NodeCatalog.Values)
            {
                // 这里一定要清空，因为是刷新
                node.Links.Clear();
                foreach (var require in node.Requires)
                {
                    foreach (var requireId in require)
                    {
                        NodeCatalog[requireId].Links.Add(node.ID);
                    }
                }
            }
        }

        #endregion

        #region ---- 图像操作 ----

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
        /// 重置所有节点的元坐标
        /// 合并不同分支上的相同节点，并使节点在分支范围内尽量居中
        /// </summary>
        /// <param name="resetAll">是否重置所有：无论元坐标有无值都重置</param>
        /// <returns></returns>
        public void ResetNodeMetaPoints(bool resetAll)
        {
            var branches = GetBranches(GetRootNodes(), true, true);
            if (branches.Count == 0) { return; }
            var width = branches.Count;
            var height = branches.Max(x => x.Length);
            Dictionary<int, int[]> nodeCoordinates = new();
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
                        nodeCoordinates.Add(node, new int[3] { x, x, y }); // [0]起始x, [1]终止x, [2]y
                    }
                }
            }
            Dictionary<int, Vector2> metaPoints = new();
            foreach (var coordinate in nodeCoordinates)
            {
                if (!resetAll && NodeCatalog[coordinate.Key].MetaPoint != new Vector2(-1, -1)) { continue; }
                var x = coordinate.Value[0] + (coordinate.Value[1] - coordinate.Value[0]) / 2;
                var point = new Vector2(x, coordinate.Value[2]);
                metaPoints[coordinate.Key] = point;
            }
            if (metaPoints.Count == 0) { return; }
            CleanBlanksForX(metaPoints);
        }
        /// <summary>
        /// 清除横坐标之间无节点的间隙
        /// </summary>
        /// <returns>id对应元坐标的字典</returns>
        private void CleanBlanksForX(Dictionary<int, Vector2> metaPoints)
        {
            // 集合相同x值的元坐标
            Dictionary<float, Dictionary<int, Vector2>> xMetaPoints = new();
            foreach (var pair in metaPoints)
            {
                var x = pair.Value.X;
                if (xMetaPoints.ContainsKey(x) == false)
                {
                    xMetaPoints.Add(x, new Dictionary<int, Vector2>());
                }
                xMetaPoints[x].Add(pair.Key, pair.Value);
            }
            var blank = 0;
            var width = metaPoints.Max(x => x.Value.X);
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
                else { blank++; }
            }
        }
        /// <summary>
        /// 按分支顺序从左到右、从上到下重排节点ID
        /// </summary>
        public void ReorderNodeIds()
        {
            Dictionary<int, FocusNode> TempNodeCatalog = new();
            var branches = GetBranches(GetRootNodes(), true, true);
            HashSet<int> visited = new();
            int newId = 1;
            foreach (var branch in branches)
            {
                foreach (var nodeId in branch)
                {
                    if (!visited.Contains(nodeId))
                    {
                        UpdateLinkNodesRequiresWithNewId(nodeId, newId);
                        TempNodeCatalog.Add(newId, NodeCatalog[nodeId]);
                        visited.Add(nodeId);
                        NodeCatalog[nodeId].ID = newId;
                        newId++;
                    }
                }
            }
            NodeCatalog = TempNodeCatalog;
            CreateNodesLinkes();
            if (this.IsEdit()) { this.EnqueueHistory(); }
        }
        /// <summary>
        /// 按新节点id更新原节点的所有子链接节点的依赖组
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="newId"></param>
        /// <returns></returns>
        private void UpdateLinkNodesRequiresWithNewId(int nodeId, int newId)
        {
            var node = NodeCatalog[nodeId];
            foreach (var linkId in node.Links)
            {
                List<HashSet<int>> newRequires = new();
                foreach (var require in NodeCatalog[linkId].Requires)
                {
                    HashSet<int> newRequire = new();
                    foreach (var requireId in require)
                    {
                        if (requireId == node.ID)
                        {
                            newRequire.Add(newId);
                        }
                        else
                        {
                            newRequire.Add(requireId);
                        }
                    }
                    newRequires.Add(newRequire);
                }
                NodeCatalog[linkId].Requires = newRequires;
            }
        }
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

            do
            {
                if (reader.Name == "State" && reader.NodeType == XmlNodeType.Element)
                {
                    Name = reader.GetAttribute("Name");
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
            } while (reader.Read());
            CreateNodesLinkes();
            ResetNodeMetaPoints(false);
        }
        public void WriteXml(XmlWriter writer)
        {
            // <State>
            writer.WriteStartElement("State");
            writer.WriteAttributeString("Name", Name);

            //==== 序列化节点数据 ====//

            // <Nodes>
            writer.WriteStartElement("Nodes");
            foreach (var node in NodeCatalog.Values)
            {
                // <Node>
                node.WriteXml(writer);
                // </Node>
            }
            // </Nodes>
            writer.WriteEndElement();

            // </State>
            writer.WriteEndElement();
        }

        #endregion

        #region ---- 文件管理工具 ----

        public string FileManageDirName { get { return $"FG{GetHashString(Name)}"; } }
        private static string GetHashString(string str)
        {
            MD5 sha = MD5.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(str));
            string result = string.Empty;
            foreach (var b in hash)
            {
                result += b.ToString();
            }
            return result;
        }
        public string GetHashString()
        {
            var cachePath = this.GetCachePath("hashtest");
            XmlIO.SaveToXml(this, cachePath);
            FileStream data = new(cachePath, FileMode.Open);
            MD5 sha = MD5.Create();
            var hash = sha.ComputeHash(data);
            data.Close();
            string result = string.Empty;
            foreach (var b in hash)
            {
                result += b.ToString();
            }
            return result;
        }

        #endregion

        #region ---- 历史和备份工具 ----

        public int HistoryIndex { get; set; } = 0;
        public int CurrentHistoryLength { get; set; } = 0;
        public FormattedData[] History { get; set; } = new FormattedData[20];
        public FormattedData Latest { get; set; } = new();
        public FormattedData Format()
        {
            var hashCode = GetHashString();
            if (!Directory.Exists(hashCode))
            {
                XmlIO.SaveToXml(this, this.GetCachePath(hashCode));
            }
            return new(hashCode);
        }
        public void Deformat(FormattedData data)
        {
            NodeCatalog = new();
            NodeCatalog = XmlIO.LoadFromXml<FocusGraph>(this.GetCachePath(data.Items[0])).NodeCatalog;
            CreateNodesLinkes();
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
