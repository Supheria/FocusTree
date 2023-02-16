using FocusTree.Tree;
using System.Xml.Serialization;

#region ==== 类型别称 ====
// 分支包类型
// 外层List：记录所有分支
using BranchPackType = System.Collections.Generic.List<System.Collections.Generic.List<FocusTree.Focus.FNode>>;
// 分支类型
// 内层FNode：以节点作为分支上的叶
// List：记录从末节点到根节点的所有叶，作为一个分支（倒序分支）
using BranchType = System.Collections.Generic.List<FocusTree.Focus.FNode>;
#endregion

namespace FocusTree.Focus
{
    /// <summary>
    /// 节点类
    /// </summary>
    public class FNode
    {
        #region ==== 属性 ====
        /// <summary>
        /// 节点ID
        /// </summary>
        [XmlElement("ID")] public int ID { get; set; }
        /// <summary>
        /// 依赖的节点ID
        /// </summary>
        [XmlElement("relied-ID")] public List<int> ReliedIDs { get; set; } = new();
        /// <summary>
        /// 子节点ID
        /// </summary>
        [XmlElement("child-ID")] public List<int> ChildIDs { get; set; } = new();
        /// <summary>
        /// 层级
        /// </summary>
        [XmlElement("level")] public int Level { get; set; }
        /// <summary>
        /// 节点在树中的起始列
        /// </summary>
        [XmlElement("start-colum")] public int StartColum { get; set; } = 0;
        /// <summary>
        /// 节点在树中的终止列
        /// </summary>
        [XmlElement("end-colum")] public int EndColum { get; set; } = 0;
        /// <summary>
        /// 国策数据
        /// </summary>
        [XmlElement("focus-data")] public FData FocusData { get; set; }
        #endregion
        #region ==== 节点控制器 ====
        /// <summary>
        /// 父节点
        /// </summary>
        [XmlIgnore] public FNode Parent { get; protected set; }
        /// <summary>
        /// 子节点
        /// </summary>
        [XmlIgnore] public List<FNode> Children { get; protected set; } = new();
        #endregion
        #region ==== 初始化节点 ====
        /// <summary>
        /// 创建节点，并作为子节点加入在父节点下
        /// </summary>
        /// <param name="ID">节点ID</param>
        /// <param name="level">层级(所在的列数)</param>
        /// <param name="parent">父节点</param>
        /// <param name="focusData">国策数据</param>
        public FNode(
            int id,
            int level,
            FNode parent,
            FData focusData
            )
        {
            ID = id;
            Level = level;
            Parent = parent;
            // 把节点加入父节点的子集 (不含根节点)
            Parent.Children.Add(this);
            Parent.ChildIDs.Add(ID);
            ReliedIDs.Add(Parent.ID);
            // 设置国策数据
            FocusData = focusData;
        }
        /// <summary>
        /// 给继承类专用的无参构造，如 FRootNode
        /// </summary>
        protected FNode(){}
        #endregion
        #region ==== 节点方法 ====
        /// <summary>
        /// 根据二维原始字段数组生成所有节点
        /// </summary>
        /// <param name="data">二维原始字段数组</param>
        /// <param name="root">指定的root</param>
        /// <return>树的root</return>
        public static FNode GenerateNodes(string[][] data, FNode root = null)
        {
            // 如果没有传入指定的 root，则创建新的 root
            root ??= new FRootNode(); // 复合分配

            // 上一次循环处理的节点
            FNode lastNode = root;
            // 循环处理到的行数
            int rowCount = 0;
            // 遍历所有行
            foreach (var row in data)
            {
                //行数从1开始
                rowCount++;
                // 获取该行非空列的所在位置
                // 从头循环匹配所有为空并统计总数，数量就是第一个非空的index
                int level = row.TakeWhile(col =>
                    string.IsNullOrWhiteSpace(col)).Count();
                // 获取原始字段
                FData focusData;
                try
                {
                    focusData = new FData(row[level]);
                }
                catch (Exception ex)
                {
                    throw new Exception($"无法读取第{rowCount}行原始字段，{ex.Message}");
                }
                #region ==== 转换方法 =====
                // 如果新节点与上一节点的右移距离大于1，则表示产生了断层
                if (level > lastNode.Level + 1)
                    throw new Exception($"位于 {rowCount} 行: 本行节点与上方节点的层级有断层。");
                // 如果新节点与上一节点的右移距离等于1，则新节点是上一节点的子节点
                if (level == lastNode.Level + 1)
                {
                    lastNode = new FNode(rowCount, level, lastNode, focusData); // lastNode指向新的节点
                }
                // 如果新节点与上一节点在同列或更靠左，向上寻找新节点所在列的父节点
                else
                {
                    do
                    {
                        // 根节点不受此处验证影响
                        if (lastNode.Parent == null)
                            throw new Exception($"位于 {rowCount} 行: 无法为节点找到对应的父节点。");
                        else
                            lastNode = lastNode.Parent; // lastNode指向自己的父节点
                    } // 当指向的父节点是新节点所在列的父节点时结束循环
                    while (level - 1 != lastNode.Level);
                    lastNode = new FNode(rowCount, level, lastNode, focusData); // lastNode指向新的节点
                }
                #endregion
            }
            return root;
        }
        /// <summary>
        /// 获取nExcludeLevel节点之下的所有正序的分支
        /// </summary>
        /// <param name="nExcludeLevel">分支所不包含的层级，默认不包含根节点</param>
        /// <returns>所有分支打包成的分支包</returns>
        public BranchPackType GetBranches(int nExcludeLevel = -1)
        {

            // 创建本节点的分支包
            BranchPackType branchPack = new BranchPackType();
            // 这是分支上最后一个节点
            if (Children.Count == 0)
            {
                // 从末节点开始创建一条新分支
                BranchType branch = new BranchType { this };
                branchPack.Add(branch);
                return branchPack;
            }
            // 遍历子所有节点的分支包
            for (int i = 0; i < Children.Count; i++)
            {
                // 获取子节点的分支包
                BranchPackType buffer = Children[i].GetBranches();
                // 把分支包解包，并在每支节点上加入本节点的ID，最后打包到branchPack
                for (int j = 0; j < buffer.Count; j++)
                {
                    BranchType branch = buffer[j];
                    if (Level != nExcludeLevel)
                    {
                        // 在分支上加入本节点
                        branch.Add(this);
                    }
                    else
                    {
                        // 反序分支
                        branch.Reverse();
                    }
                    // 将分支加入新分支包
                    branchPack.Add(branch);
                }
            }
            return branchPack;
        }
        #endregion
    }
}
