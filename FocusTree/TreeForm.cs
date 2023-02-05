#region ==== 类型别称 ====
// 测试点阵图类型
using MapType = System.Collections.Generic.List<System.Collections.Generic.List<string>>;
// 层级类型
using LevelType = System.Collections.Generic.List<FocusTree.CNode>;
// 层级合集类型
using LevelPackType = System.Collections.Generic.List<System.Collections.Generic.List<FocusTree.CNode>>;
// 分支包类型
using BranchPackType = System.Collections.Generic.List<System.Collections.Generic.List<FocusTree.CNode>>;
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Formats.Asn1;
using System.Text.RegularExpressions;
using CSVFile;
using System.Runtime.Serialization.Formatters.Binary;

namespace FocusTree
{
    public partial class TreeForm : Form
    {
        private CFocusTree mTree;
        /// <summary>
        /// 节点信息窗口
        /// </summary>
        private InfoDialog mInfoDlg;
        /// <summary>
        /// 画图开始的位置
        /// </summary>
        private Point ImageStartLocation;
        #region  ==== 初始化窗体 ====
        /// <summary>
        /// 窗体初始化
        /// </summary>
        public TreeForm(string szPath)
        {
            Match match = Regex.Match(szPath, "([^\\\\]*)(\\.\\w+)$");
            if (match.Groups[2].Value == ".csv" || match.Groups[2].Value == ".CSV")
            {
                mTree = new CFocusTree(szPath);
            }
            else
            {
                LoadFromFile(szPath);
            }
            InitForm();
            SetImage();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitForm()
        {
            InitializeComponent();
            mInfoDlg = new InfoDialog(this);
            ImageStartLocation = new Point(0, 0);
            Name = Text = mTree.mName;
        }
        #endregion
        #region ==== 节点控件事件 ====
        public void ClickNode(object sender, MouseEventArgs e)
        {
            // 隐藏信息窗口
            mInfoDlg.Hide();
            // 触发点击事件的节点
            mInfoDlg.SetNode((NodeControl)sender);
            // 非模态对话框
            mInfoDlg.Show();
        }
        #endregion
        #region ==== 窗体方法 ====
        public void SetImage()
        {
            // 遍历至最大层级数
            for (int nLevel = 0; nLevel < mTree.mLevels.Count; nLevel++)
            {
                var currentLevel = mTree.mLevels[nLevel];
                // 记录已经使用过的叶节点索引
                int nIndex = 0;
                // 新建一行，并在合适的位置插入ID值，空ID值用"|"代替
                List<string> nList = new List<string>();
                //
                int columWidth = 50;
                // 每个层级都有固定的列数，而列数就是分支数
                for (int nColum = 0; ; nColum++)
                {
                    if (nColum >= mTree.mColumNum || nIndex >= currentLevel.Count)
                    {
                        break;
                    }
                    NodeControl nodeCtrl = new NodeControl(currentLevel[nIndex]);

                    // 到达目标列
                    if (nColum == nodeCtrl.ToSetColum)
                    {

                        nodeCtrl.Location = new Point(
                            ImageStartLocation.X + nColum * nodeCtrl.Size.Height,
                            ImageStartLocation.Y + nLevel * nodeCtrl.Size.Width);
                        nodeCtrl.MouseClick += new MouseEventHandler(ClickNode);
                        Controls.Add(nodeCtrl);
                        nIndex++;
                    }
                }
            }
        }
        #endregion
        #region ==== 文件方法 ====
        /// <summary>
        /// 序列化并保存数据
        /// </summary>
        /// <param name="szSave"></param>
        /// <exception cref="Exception"></exception>
        public void SaveToFile(string szSave)
        {
            BinaryWriter bw;
            try
            {
                bw = new BinaryWriter(new FileStream(szSave,
                                FileMode.Create));
            }
            catch (IOException ex)
            {
                throw new Exception($"{ex.Message}\n无法创建\"{szSave}。\"");
            }
            // 写入文件
            try
            {
                // 序列化 mTree
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, mTree);
                var buffer = stream.GetBuffer();
                // 写入文件
                bw.Write(buffer);
                bw.Close();
            }
            catch (IOException ex)
            {
                bw.Close();
                throw new Exception($"{ex.Message}\n无法写入\"{szSave}。\"");
            }
        }
        /// <summary>
        /// 读取数据并反序列化
        /// </summary>
        /// <param name="szBinary"></param>
        /// <exception cref="Exception"></exception>
        private void LoadFromFile(string szBinary)
        {
            BinaryReader br;
            FileStream fileStream = new FileStream(szBinary, FileMode.Open);
            try
            {
                br = new BinaryReader(fileStream);
            }
            catch (IOException ex)
            {
                throw new Exception($"{ex.Message}\n无法打开\"{szBinary}。\"");
            }
            try
            {
                //获取文件长度
                byte[] buffer = new byte[fileStream.Length];
                //读取文件中的内容并保存到字节数组中
                br.Read(buffer, 0, buffer.Length);
                br.Close();
                // 反序列化
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream(buffer);
                mTree = (CFocusTree)formatter.Deserialize(stream);

            }
            catch (IOException ex)
            {
                br.Close();
                throw new Exception($"{ex.Message}\n无法读取\"{szBinary}。\"");
            }

        }
        #endregion
    }

    [Serializable]
    /// <summary>
    /// 国策树类
    /// </summary>
    public class CFocusTree
    {
        #region ==== 树的属性 ====
        /// <summary>
        /// 树的名称（CSV文件名）
        /// </summary>
        public string mName { get; init; }
        /// <summary>
        /// 按层级合并所有分支上的节点，
        /// mLevels[层级数][节点序号]
        /// </summary>
        public LevelPackType mLevels { get; private set; }
        /// <summary>
        /// 树的列数（即分支数量）
        /// </summary>
        public int mColumNum { get; private set; }
        #endregion
        #region ==== 树的初始化 ====
        /// <summary>
        /// 将原始数据转化为节点树
        /// </summary>
        /// <param name="szCsv">csv文件路径</param>
        public CFocusTree(string szCsv)
        {
            Match match = Regex.Match(szCsv, "([^\\\\]*)(\\.\\w+)$");
            mName = match.Groups[1].Value;
            try
            {
                var data = ReadCsv(szCsv);
                var root = new CNode();
                // 将数据转换为节点
                GenerateNodes(data, root);
                // 生成所有分支
                BranchPackType buffer = root.GetBranches();
                if (buffer.Count == 0)
                {
                    throw new Exception("未获得任何分支。");
                }
                // 分支总数
                mColumNum = buffer.Count;
                // 按层级合并所有分支上的节点（得到 mLevels）
                CombineBranchNodes(buffer);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}\n生成\"{szCsv}\"的树失败！");
            }
        }
        /// <summary>
        /// 读取CSV文件，获得二维原始字段数组
        /// </summary>
        /// <param name="szPath"></param>
        /// <returns></returns>
        private string[][] ReadCsv(string szPath)
        {
            string[][]? data = null;
            // 读取设置
            var settings = new CSVSettings
            {
                FieldDelimiter = ',', // 字段分隔符
                TextQualifier = '\"', // 文本限定符
                HeaderRowIncluded = false // 第一行不做标头
            };
            try
            {
                //从文件读取数据 (string[])
                CSVReader csvData = CSVReader.FromFile(szPath, settings);
                data = csvData.ToArray(); //作为二维数组返回
                if (data == null)
                    throw new Exception("未获得CSVReader指针。");
            }
            catch (Exception ex)
            {
                throw new Exception($"读取CSV文件失败：{ex.Message}");
            }
            return data;
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
                string text = row[level];
                #region 转换方法
                // 如果新节点与上一节点的右移距离大于1，则表示产生了断层
                if (level > lastNode.mLevel + 1)
                    throw new Exception($"位于 {nRow} 行: 本行节点与上方节点的层级有断层。");
                // 如果新节点与上一节点的右移距离等于1，则新节点是上一节点的子节点
                if (level == lastNode.mLevel + 1)
                {
                    lastNode = new CNode(nRow, level, lastNode, text); // lastNode指向新的节点
                }
                // 如果新节点与上一节点在同列或更靠左，向上寻找新节点所在列的父节点
                else
                {
                    do
                    {
                        if (lastNode.mParent == null)
                            throw new Exception($"位于 {nRow} 行: 无法为节点找到对应的父节点。");
                        else
                            lastNode = lastNode.mParent; // lastNode指向自己的父节点
                    } // 当指向的父节点是新节点所在列的父节点时结束循环
                    while (level - 1 != lastNode.mLevel);
                    lastNode = new CNode(nRow, level, lastNode, text); // lastNode指向新的节点
                }
                #endregion
            }
        }
        /// <summary>
        /// 把所有分支转换为按层级划分的节点行，并合并每个层级上相同的节点。
        /// 将所有分支作为树的列，将节点的层级作为树的行。
        /// </summary>
        /// <param name="rawBranches"></param>
        private void CombineBranchNodes(BranchPackType rawBranches)
        {
            mLevels = new LevelPackType();
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
                LevelType combineList = new LevelType();
                // 遍历所有分支的本层级节点
                for (int colum = 0; colum < rawBranches.Count; colum++)
                {
                    // 层级数没有超出分支的节点总数
                    if (level < rawBranches[colum].Count)
                    {
                        var node = rawBranches[colum][level];
                        // 查找节点在枚举里的索引
                        int nIndex = combineList.FindIndex(x => x.mID == node.mID);
                        // 节点不存在于枚举里
                        if (nIndex == -1)
                        {
                            // 节点的终止列和起始列设置为当前列
                            node.mEndColum = node.mStartColum = colum;
                            // 新增节点枚举
                            combineList.Add(node);
                        }
                        else
                        {
                            // 节点的终止列设置为当前列
                            combineList[nIndex].mEndColum = colum;
                        }
                    }
                }
                // 本层级已无任何分支有节点
                // 本层级已经是末尾
                if (combineList.Count == 0)
                {
                    break;
                }
                // 将本层级的合并节点加入集合
                mLevels.Add(combineList);
                level++;
            }
        }
        #endregion
        #region ==== 树的方法 ====
        /// <summary>
        /// （，，，）获取所有效果加成
        /// </summary>
        /// <returns></returns>
        public List<string[]> GetAllEffects()
        {
            List<string[]> effects = new List<string[]>();
            // 遍历树的所有层级
            for (int i = 0; i < mLevels.Count; i++)
            {
                var currentLevel = mLevels[i];
                // 遍历每个层级上的所有节点
                for (int nIndex = 0; nIndex < currentLevel.Count; nIndex++)
                {
                    var node = currentLevel[nIndex];
                    var str = node.mFocusData.mEffects;
                    if (str != null)
                    {
                        ////var reg = "\\W(\\w+)\\W([+|-]\\d+)((?:%)?)\\W"; // 原： \W(\w+)\W([+|-]\d+)((?:%)?)\W
                        //var reg = "(\\w+)\\W?[+|-]\\d+%?"; // (\w+)\W?[+|-]\d+%?
                        //var reg2 = "((增加)?(添加)?\\d+个\\w+)"; // ((?:增加)?\d+个\w+)
                        //var reg3 = "(\\d+x\\d+%?\\w+：\\w+)"; // (\d+x\d+%?\w+：\w+)
                        //var reg4 = "(减少\\d.?\\d\\w+\\d+%?\\w+)"; // (减少\d.?\d\w+\d+%?\w+)
                        //var reg5 = "[\\u4e00-\\u9fa5]{1,})[（].+[）]"; // XX（）
                        //var reg6 = "获得(.+)，.+[（].+[）]"; // 获得...，其效果为（...）
                        //var reg7 = "以.+取代.+(?:，以|。以)"; // 以...取代...
                        //var matches = Regex.Matches(str, reg).Union(
                        //    Regex.Matches(str, reg2)).Union(
                        //    Regex.Matches(str, reg3)).Union(
                        //    Regex.Matches(str, reg4)).Union(
                        //    Regex.Matches(str, reg5)).Union(
                        //    Regex.Matches(str, reg6)).Union(
                        //    Regex.Matches(str, reg7)).ToArray();

                        //List<string> nodeEffects = new List<string>();
                        //if (matches.Length > 0)
                        //{
                        //    foreach (Match match in matches)
                        //    {
                        //        if (match.Success)
                        //        {
                        //            nodeEffects.Add(match.Groups[1].Value);
                        //        }
                        //    }
                        //}
                        //effects.Add((nodeEffects.ToArray()));
                    }
                }
            }
            return effects;
        }
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
