using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

using NodeBranch = System.Collections.Generic.List<System.Collections.Generic.List<FocusTree.CNode>>;

namespace FocusTree.Tree
{
    /// <summary>
    /// 国策树类
    /// </summary>
    [XmlRoot("focus-tree")]
    public class CFocusTree
    {
        #region ==== 树的属性 ====
        /// <summary>
        /// 树的名称（文件名）
        /// </summary>
        string mName = string.Empty;
        [XmlElement("tree-name")]
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        /// <summary>
        /// 节点链
        /// </summary>
        List<CNode> mNodeChain = new List<CNode>();
        [XmlElement("node")]
        public List<CNode> NodeChain
        {
            get { return mNodeChain; }
            set { mNodeChain = value; }
        }
        #endregion
        #region ==== 树的初始化 ====
        /// <summary>
        /// 将原始数据转化为节点树
        /// </summary>
        /// <param name="szCsv">csv文件路径</param>
        public CFocusTree() { }
        public CFocusTree(string szCsv)
        {

            Match match = Regex.Match(szCsv, "([^\\\\]*)(\\.\\w+)$");
            mName = match.Groups[1].Value;
            try
            {
                var data = IO.FtCsv.ReadCsv(szCsv);
                var root = new CNode();
                // 将数据转换为节点
                GenerateNodes(data, root);
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
                throw new Exception($"{ex.Message}\n生成\"{szCsv}\"的树失败！");
            }
        }
        /// <summary>
        /// 根据二维原始字段数组生成所有节点
        /// </summary>
        /// <param name="data">二维原始字段数组</param>
        private void GenerateNodes(string[][] data, CNode root)
        {
            // 上一次循环处理的节点
            CNode lastNode = root;
            // 循环处理到的行数
            int nRow = 0;
            // 遍历所有行
            foreach (var row in data)
            {
                //行数从1开始
                nRow++;
                // 获取该行非空列的所在位置
                // 从头循环匹配所有为空并统计总数，数量就是第一个非空的index
                int level = row.TakeWhile(col =>
                    string.IsNullOrWhiteSpace(col)).Count();
                // 获取原始字段
                SFocusData focusData;
                try
                {
                    focusData = GetFocusData(row[level]);
                }
                catch (Exception ex)
                {
                    throw new Exception($"无法读取第{nRow}行原始字段，{ex.Message}");
                }
                #region ==== 转换方法 =====
                // 如果新节点与上一节点的右移距离大于1，则表示产生了断层
                if (level > lastNode.Level + 1)
                    throw new Exception($"位于 {nRow} 行: 本行节点与上方节点的层级有断层。");
                // 如果新节点与上一节点的右移距离等于1，则新节点是上一节点的子节点
                if (level == lastNode.Level + 1)
                {
                    lastNode = new CNode(nRow, level, lastNode, focusData); // lastNode指向新的节点
                }
                // 如果新节点与上一节点在同列或更靠左，向上寻找新节点所在列的父节点
                else
                {
                    do
                    {
                        if (lastNode.Parent == null)
                            throw new Exception($"位于 {nRow} 行: 无法为节点找到对应的父节点。");
                        else
                            lastNode = lastNode.Parent; // lastNode指向自己的父节点
                    } // 当指向的父节点是新节点所在列的父节点时结束循环
                    while (level - 1 != lastNode.Level);
                    lastNode = new CNode(nRow, level, lastNode, focusData); // lastNode指向新的节点
                }
                #endregion
            }
        }
        /// <summary>
        /// 根据文本设置节点的国策数据
        /// </summary>
        /// <param name="text">原始国策字段</param>
        private static SFocusData GetFocusData(string text)
        {
            // 在 C# 中的字符串，{ 需要转义，通过分割一对来避免歧义。 原 Regex: (.+?){(\d+)天}{(.+?)}(?:{(.+)})?(.+)?
            var pattern = "(.+?){" + "(\\d+)天}{" + "(.+?)}(?:{" + "(.+)})?(.+)?";
            try
            {
                var match = Regex.Match(text, pattern);
                // Groups[0] 是匹配成功部分的文本，应当等同于 text。
                // 从[1]开始才是括号匹配的数据
                // 是否以 * 开头
                var isBeginWithStar = match.Groups[1].Value.StartsWith("*");
                // 名称
                string name;
                // 如果以 * 开头，则去掉 *
                if (isBeginWithStar)
                    name = match.Groups[1].Value.Substring(1);
                else
                    name = match.Groups[1].Value;
                // 天数
                int duration = int.Parse(match.Groups[2].Value);
                // 效果
                string effects = match.Groups[3].Value;
                // 描述
                string descript = match.Groups[4].Value;
                // 备注
                string ps = match.Groups[5].Value;
                return new SFocusData(
                    name,
                    isBeginWithStar,
                    duration,
                    effects,
                    descript,
                    ps
                    );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"正则匹配时发生了异常。\n" +
                    $"试图解析的内容: {text}\n" +
                    $"异常信息: {ex.Message}");
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
                List<CNode> combineList = new List<CNode>();
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
                            mNodeChain.Add(node);
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
