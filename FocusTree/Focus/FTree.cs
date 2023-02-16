using FocusTree.Focus;
using System.Xml.Serialization;

using NodeBranch = System.Collections.Generic.List<System.Collections.Generic.List<FocusTree.Focus.FNode>>;

namespace FocusTree.Tree
{
    /// <summary>
    /// 国策树类
    /// </summary>
    [XmlRoot("focus-tree")]
    public class FTree
    {
        #region ==== 属性 ====
        /// <summary>
        /// 树的名称（文件名）
        /// </summary>
        [XmlElement("tree-name")]
        public string Name = string.Empty;
        /// <summary>
        /// 节点链
        /// </summary>
        [XmlElement("node")]
        public List<FNode> NodeChain = new();
        
        #endregion
        #region ==== 构造 ====
        /// <summary>
        /// 从csv中读取节点树
        /// </summary>
        /// <param name="path">csv文件路径</param>
        public FTree(string path)
        {
            // 文件信息
            var fileinfo = new FileInfo(path);  
            if (!fileinfo.Exists) { throw new FileNotFoundException($"[2302152115] 无法从文件生成树 - 文件不存在: {path}"); }

            // 不含扩展名的文件名
            Name = Path.GetFileNameWithoutExtension(fileinfo.Name);

            try
            {
                var data = IO.FCsv.ReadCsv(path);
                // 将数据转换为节点
                var root = FNode.GenerateNodes(data);
                // 生成所有分支
                NodeBranch buffer = root.GetBranches();
                if (buffer.Count == 0)
                {
                    throw new Exception("未获得任何分支。");
                }
                // 按层级合并所有分支上的节点（得到 mLevels）
                CombineBranchNodes(buffer);
            }
            catch (Exception ex)
            {
                throw new Exception($"[2302152117] 生成树失败 - 文件: {path}\n{ex.Message}");
            }
        }
        /// <summary>
        /// 把所有分支转换为按层级划分的节点行，并合并每个层级上相同的节点。
        /// 将所有分支作为树的列，将节点的层级作为树的行。
        /// </summary>
        /// <param name="rawBranches"></param>
        private void CombineBranchNodes(NodeBranch rawBranches)
        {
            // 遍历到的层级
            int level = 0;
            while (true)
            {
                // debug用，在某层级放置断点
                if (level + 1 == 4)
                {
                    int a = 0;
                }
                // 枚举本层级的所有节点
                List<FNode> combineList = new List<FNode>();
                // 遍历所有分支的本层级节点
                for (int colum = 0; colum < rawBranches.Count; colum++)
                {
                    // 层级数没有超出分支的节点总数
                    if (level < rawBranches[colum].Count)
                    {
                        var node = rawBranches[colum][level];
                        // 查找节点在枚举里的索引
                        int nIndex = combineList.FindIndex(x => x.ID == node.ID);
                        // 节点不存在于枚举里
                        if (nIndex == -1)
                        {
                            node.EndColum = node.StartColum = colum;
                            // 新增节点枚举
                            combineList.Add(node);
                            NodeChain.Add(node);
                        }
                        else
                        {
                            combineList[nIndex].EndColum = colum;
                        }
                    }
                }
                // 本层级已无任何分支有节点
                // 本层级已经是末尾
                if (combineList.Count == 0)
                {
                    break;
                }
                level++;
            }
        }
        #endregion
        #region ==== 树的方法 ====
        /// <summary>
        /// （，，，）获取所有效果加成
        /// </summary>
        /// <returns></returns>
        //public List<string[]> GetAllEffects()
        //{
        //    List<string[]> effects = new List<string[]>();
        //    // 遍历树的所有层级
        //    for (int i = 0; i < mLevels.Count; i++)
        //    {
        //        var currentLevel = mLevels[i];
        //        // 遍历每个层级上的所有节点
        //        for (int nIndex = 0; nIndex < currentLevel.Count; nIndex++)
        //        {
        //            var node = currentLevel[nIndex];
        //            var str = node.FocusData.mEffects;
        //            if (str != null)
        //            {
        //                //var reg = "\\W(\\w+)\\W([+|-]\\d+)((?:%)?)\\W"; // 原： \W(\w+)\W([+|-]\d+)((?:%)?)\W
        //                var reg = "(\\w+)\\W?[+|-]\\d+%?"; // (\w+)\W?[+|-]\d+%?
        //                var reg2 = "((增加)?(添加)?\\d+个\\w+)"; // ((?:增加)?\d+个\w+)
        //                var reg3 = "(\\d+x\\d+%?\\w+：\\w+)"; // (\d+x\d+%?\w+：\w+)
        //                var reg4 = "(减少\\d.?\\d\\w+\\d+%?\\w+)"; // (减少\d.?\d\w+\d+%?\w+)
        //                var reg5 = "[\\u4e00-\\u9fa5]{1,})[（].+[）]"; // XX（）
        //                var reg6 = "获得(.+)，.+[（].+[）]"; // 获得...，其效果为（...）
        //                var reg7 = "以.+取代.+(?:，以|。以)"; // 以...取代...
        //                var matches = Regex.Matches(str, reg).Union(
        //                    Regex.Matches(str, reg2)).Union(
        //                    Regex.Matches(str, reg3)).Union(
        //                    Regex.Matches(str, reg4)).Union(
        //                    Regex.Matches(str, reg5)).Union(
        //                    Regex.Matches(str, reg6)).Union(
        //                    Regex.Matches(str, reg7)).ToArray();

        //                List<string> nodeEffects = new List<string>();
        //                if (matches.Length > 0)
        //                {
        //                    foreach (Match match in matches)
        //                    {
        //                        if (match.Success)
        //                        {
        //                            nodeEffects.Add(match.Groups[1].Value);
        //                        }
        //                    }
        //                }
        //                effects.Add((nodeEffects.ToArray()));
        //            }
        //        }
        //    }
        //    return effects;
        //}
        /// <summary>
        /// 更新树
        /// </summary>
        public void Update()
        {

            //var buffer = root.GetBranches();
            //if (buffer != null)
            //{
            //    // 分支总数
            //    mColumNum = buffer.Count;
            //    // 按层级合并所有分支上的节点（得到 mLevels）
            //    CombineBranchNodes(buffer);
            //}
            //else
            //    this.BadResult("未获得任何分支，无法生成树！");

        }
        #endregion
    }
}
