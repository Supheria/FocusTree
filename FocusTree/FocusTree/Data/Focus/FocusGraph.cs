#define DEBUG
using FocusTree.Graph;
using FocusTree.IO;
using FocusTree.IO.FileManage;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace FocusTree.Data.Focus
{
    /// <summary>
    /// 国策树图控制类
    /// </summary>
    public class FocusGraph : IHistoryable, IBackupable
    {
        #region ---- 基本变量 ----

        /// <summary>
        /// 以 ID 作为 Key 的所有节点
        /// </summary>
        Dictionary<int, FocusData> FocusCatalog;
        /// <summary>
        /// 国策列表
        /// </summary>
        public List<FocusData> FocusList { get { return FocusCatalog.Values.ToList(); } }
        /// <summary>
        /// 通过国策 ID 获得国策（不应该滥用，仅用在 require id 获取国策时），或修改国策
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FocusData this[int id] { get => FocusCatalog[id]; set => FocusCatalog[id] = value; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 所有节点的子链接（使用前调用 CreateNodeLinks ）
        /// </summary>
        Dictionary<int, List<int>> NodeLinks;

        #endregion

        #region ---- 节点操作 ----

        /// <summary>
        /// 添加节点 O(1)，绘图时记得重新调用 GetFocusMap
        /// </summary>
        /// <returns>是否添加成功</returns>
        public bool AddNode(FocusData focus)
        {
            if (FocusCatalog.TryAdd(focus.ID, focus) == false)
            {
                MessageBox.Show("[2303031210]提示：无法添加节点 - 无法加入字典。");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 删除节点 O(2n+)，绘图时记得重新调用 GetFocusMap
        /// </summary>
        /// <returns>是否成功删除</returns>
        public bool RemoveNode(int id)
        {
            if (FocusCatalog.ContainsKey(id) == false)
            {
                MessageBox.Show($"[2303031221]提示：无法移除节点 - FocusCatalog 未包含 ID = {id} 的节点。");
                return false;
            }
            // 在所有的节点依赖组合中删除此节点
            foreach (var focus in FocusCatalog.Values)
            {
                foreach (var require in focus.Requires)
                {
                    require.Remove(id);
                }
            }
            // 从节点表中删除此节点
            FocusCatalog.Remove(id);
            return true;
        }
        /// <summary>
        /// 编辑节点 O(1)，新数据的ID必须匹配
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="newData">要替换的数据</param>
        /// <returns>修改是否成功</returns>
        //public bool EditNode(FocusData focus)
        //{
        //    var id = focus.ID;
        //    if (FocusCatalog.ContainsKey(id) == false)
        //    {
        //        MessageBox.Show($"[2303031232]提示：无法编辑节点 - 新节点数据的 ID({id}) 不存在于 FocusCatalog。");
        //        return false;
        //    }
        //    FocusCatalog[id] = focus;
        //    return true;
        //}
        /// <summary>
        /// 获取所有无任何依赖的节点（根节点）  O(n)
        /// </summary>
        /// <returns>根节点</returns>
        //[Obsolete("经常出BUG，用的时候要小心")]
        public int[] GetRootNodes()
        {
            var result = new HashSet<int>();
            foreach (var focus in FocusCatalog)
            {
                if (focus.Value.Requires.Sum(x => x.Count) == 0)
                {
                    result.Add(focus.Key);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 使用 Requires 创建所有节点的 Links
        /// </summary>
        private void CreateNodeLinks()
        {
            // 这里一定要清空，因为是刷新
            NodeLinks = new();
            foreach (var focus in FocusCatalog.Values)
            {
                foreach (var requires in focus.Requires)
                {
                    foreach (var id in requires)
                    {
                        if (NodeLinks.TryAdd(id, new() { focus.ID }) == false)
                        {
                            NodeLinks[id].Add(focus.ID);
                        }
                    }
                }
            }
        }

        #endregion

        #region ---- 图像操作 ----

        /// <summary>
        /// 获取某个节点的所有分支
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="sort">是否按照节点ID排序</param>
        /// <param name="reverse">是否从根节点向末节点排序</param>
        /// <returns></returns>
        public List<int[]> GetBranches(int id, bool sort, bool reverse)
        {
            var branches = new List<int[]>();
            var steps = new Stack<int>();
            CreateNodeLinks();
            GetBranches(id, ref branches, ref steps, sort, reverse);
            return branches;
        }
        /// <summary>
        /// 获取若干个节点各自的所有分支
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="sort">是否按照节点ID排序</param>
        /// <param name="reverse">是否从根节点向末节点排序</param>
        /// <returns></returns>
        public List<int[]> GetBranches(int[] ids, bool sort, bool reverse)
        {
            var branches = new List<int[]>();
            var steps = new Stack<int>();
            CreateNodeLinks();
            foreach (var id in ids)
            {
                GetBranches(id, ref branches, ref steps, sort, reverse);
            }
            return branches;
        }
        private void GetBranches(int currentId, ref List<int[]> branches, ref Stack<int> steps, bool sort, bool reverse)
        {
            steps.Push(currentId);
            NodeLinks.TryGetValue(currentId, out var links);
            // 当前节点是末节点
            if (links == null)
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
                var linkList = links.ToList();
                if (sort == true)
                {
                    linkList.Sort();
                }
                foreach (var id in linkList)
                {
                    if (steps.Contains(id))
                    {
                        // 已经走过这个节点，跳过，避免死循环
                        continue;
                    }
                    else
                    {
                        GetBranches(id, ref branches, ref steps, sort, reverse);
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
        public void ResetAllNodesLatticedPoint()
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
                    var id = branches[x][y];
                    if (nodeCoordinates.ContainsKey(id))
                    {
                        nodeCoordinates[id][0] = nodeCoordinates[id][0] < x ? nodeCoordinates[id][0] : x;
                        nodeCoordinates[id][1] = nodeCoordinates[id][1] > x ? nodeCoordinates[id][1] : x;
                        nodeCoordinates[id][2] = nodeCoordinates[id][2] > y ? nodeCoordinates[id][2] : y;
                    }
                    else
                    {
                        nodeCoordinates.Add(id, new int[3] { x, x, y }); // [0]起始x, [1]终止x, [2]y
                    }
                }
            }
            Dictionary<int, Point> metaPoints = new();
            foreach (var coordinate in nodeCoordinates)
            {
                var x = coordinate.Value[0] + (coordinate.Value[1] - coordinate.Value[0]) / 2;
                Point point = new(x, coordinate.Value[2]);
                metaPoints[coordinate.Key] = point;
            }
            if (metaPoints.Count == 0) { return; }
            CleanBlanksForX(metaPoints);
        }
        /// <summary>
        /// 清除横坐标之间无节点的间隙
        /// </summary>
        /// <returns>id对应元坐标的字典</returns>
        private void CleanBlanksForX(Dictionary<int, Point> metaPoints)
        {
            // 集合相同x值的元坐标
            Dictionary<int, Dictionary<int, Point>> xMetaPoints = new();
            foreach (var pair in metaPoints)
            {
                var x = pair.Value.X;
                if (xMetaPoints.ContainsKey(x) == false)
                {
                    xMetaPoints.Add(x, new Dictionary<int, Point>());
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
                        LatticedPoint point = new(nodePoint.Value.X - blank, nodePoint.Value.Y);
                        var focus = FocusCatalog[nodePoint.Key];
                        focus.LatticedPoint = point;
                        FocusCatalog[nodePoint.Key] = focus;
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
            CreateNodeLinks();
            Dictionary<int, FocusData> TempFocusCatalog = new();
            var branches = GetBranches(GetRootNodes(), true, true);
            HashSet<int> visited = new();
            int newId = 1;
            foreach (var branch in branches)
            {
                foreach (var id in branch)
                {
                    if (!visited.Contains(id))
                    {
                        UpdateLinkNodesRequiresWithNewId(id, newId);
                        visited.Add(id);
                        var focus = FocusCatalog[id];
                        focus.ID = newId;
                        TempFocusCatalog.Add(newId, focus);
                        newId++;
                    }
                }
            }
            FocusCatalog = TempFocusCatalog;
        }
        /// <summary>
        /// 按新节点id更新原节点的所有子链接节点的依赖组
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newId"></param>
        /// <returns></returns>
        private void UpdateLinkNodesRequiresWithNewId(int id, int newId)
        {
            if (NodeLinks.TryGetValue(id, out var links) == false) { return; }
            foreach (var linkId in links)
            {
                List<HashSet<int>> newRequires = new();
                foreach (var require in FocusCatalog[linkId].Requires)
                {
                    HashSet<int> newRequire = new();
                    foreach (var requireId in require)
                    {
                        if (requireId == id)
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
                var focus = FocusCatalog[linkId];
                focus.Requires = newRequires;
                FocusCatalog[linkId] = focus;
            }
        }
        /// <summary>
        /// 获得整图元坐标矩形
        /// </summary>
        /// <returns></returns>
        public Rectangle GetMetaRect()
        {
            int left, top, right, bottom;
            left = top = right = bottom = 0;
            foreach (var focus in FocusCatalog.Values)
            {
                var point = focus.LatticedPoint;
                if (point.Col < left) { left = point.Col; }
                else if (point.Col > right) { right = point.Col; }
                if (point.Row < top) { top = point.Row; }
                else if (point.Row > bottom) { bottom = point.Row; }
            }
            return new(left, top, right - left + 1, bottom - top + 1);
        }
        /// <summary>
        /// 判断给定栅格化坐标是否存在于节点列表中
        /// </summary>
        /// <param name="latticedPoint"></param>
        /// <param name="id">默认为-1</param>
        /// <returns>如果有则返回true，id为节点id；否则返回false，id为-1</returns>
        public bool ContainLatticedPoint(LatticedPoint latticedPoint, out FocusData? focus)
        {
            focus = null;
            foreach (var f in FocusCatalog.Values)
            {
                if (latticedPoint == f.LatticedPoint)
                {
                    focus = f;
                    return true;
                }
            }
            return false;
        }
        public bool ContainLatticedPoint(LatticedPoint latticedPoint)
        {
            foreach (var f in FocusCatalog.Values)
            {
                if (latticedPoint == f.LatticedPoint)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region ---- xml序列化 ----

        /// <summary>
        /// 用于序列化
        /// </summary>
        private FocusGraph() { }
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
            FocusCatalog = new();
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
                            FocusCatalog.Add(node.FData.ID, node.FData);
                        }
                    } while (reader.Read());

                }
            } while (reader.Read());
        }
        public void WriteXml(XmlWriter writer)
        {
            // <State>
            writer.WriteStartElement("State");
            writer.WriteAttributeString("Name", Name);

            //==== 序列化节点数据 ====//

            // <Nodes>
            writer.WriteStartElement("Nodes");
            foreach (var focus in FocusCatalog.Values)
            {
                FocusNode node = new(focus);
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

        #region ---- 文件管理 ----

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
            var cachePath = FileCache.GetCachePath(this, "hashtest");
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

        #region ---- 历史和备份 ----

        public int HistoryIndex { get; set; } = 0;
        public int CurrentHistoryLength { get; set; } = 0;
        public FormattedData[] History { get; set; } = new FormattedData[20];
        public int LatestIndex { get; set; } = 0;
        public FormattedData Format()
        {
            var hashCode = GetHashString();
            if (!Directory.Exists(hashCode))
            {
                XmlIO.SaveToXml(this, FileCache.GetCachePath(this, hashCode));
            }
            return new(hashCode);
        }
        public void Deformat(FormattedData data)
        {
            FocusCatalog = new();
            FocusCatalog = XmlIO.LoadFromXml<FocusGraph>(FileCache.GetCachePath(this, data.Items[0])).FocusCatalog;
        }

        #endregion
    }
}
