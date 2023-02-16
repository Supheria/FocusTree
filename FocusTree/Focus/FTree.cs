using FocusTree.Focus;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FocusTree.Tree
{
    /// <summary>
    /// 国策树类
    /// </summary>
    [XmlRoot("focus-tree")]
    public class FTree:FMap
    {
        #region ==== 属性 ====
        /// <summary>
        /// 树的名称（文件名）
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 根节点
        /// </summary>
        public FNode RootNode { get; private set; }
        
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

            // 将数据转换为节点
            try
            {
                var data = IO.FCsv.ReadCsv(path);
                RootNode = GenerateTree(data);
            }
            catch (Exception ex)
            {
                throw new Exception($"[2302152117] 生成树失败 - 文件: {path}\n{ex.Message}");
            }
        }
        #endregion
        #region ==== 方法 ====
        public override HashSet<FMapNode> GetAllMapNodes() 
        {
            var fnodes = GetAllFNodes();
            var set = new HashSet<FMapNode>();
            foreach ( var node in fnodes )
            {
                set.Add(node);
            }
            return set;
        }
        public override FMapNode GetMapNodeById(int id)
        {
            return GetNodeById(RootNode,id);
        }
        /// <summary>
        /// 根据 ID 查找节点
        /// </summary>
        /// <param name="current">递归起始节点</param>
        /// <param name="id">要查找的 id</param>
        /// <returns>找到的节点</returns>
        private FNode GetNodeById(FNode current, int id)
        {
            if (current.ID == id) return current;
            else 
            {
                foreach(var child in current.Children)
                {
                    FNode result = GetNodeById(child, id);
                    if(result != null) { return result; }
                }
            }
            return null;
        }
        /// <summary>
        /// 根据二维原始字段数组生成所有节点
        /// </summary>
        /// <param name="data">二维原始字段数组</param>
        /// <param name="root">指定的root</param>
        /// <return>树的root</return>
        private static FNode GenerateTree(string[][] data, FNode root = null)
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
                int level = row.TakeWhile(col => string.IsNullOrWhiteSpace(col)).Count();
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

                //== 转换 ==//

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
                        if (lastNode.Parent == null)
                            throw new Exception($"位于 {rowCount} 行: 无法为节点找到对应的父节点。");
                        else
                            lastNode = lastNode.Parent; // lastNode指向自己的父节点
                    } // 当指向的父节点是新节点所在列的父节点时结束循环
                    while (level - 1 != lastNode.Level);
                    lastNode = new FNode(rowCount, level, lastNode, focusData); // lastNode指向新的节点
                }
                //==
            }
            return root;
        }
        /// <summary>
        /// 获取所有子节点 (不含根节点)
        /// </summary>
        /// <returns></returns>
        public List<FNode> GetAllFNodes()
        {
            var nodes = new List<FNode>();
            foreach(var child in RootNode.Children)
            {
                AddSubNodesToList(child, ref nodes);
            }
            return nodes;
        }
        /// <summary>
        /// 将一个节点的所有子节点添加到 nodes
        /// </summary>
        /// <param name="current">当前递归节点</param>
        /// <param name="nodes">添加节点到 nodes</param>
        private void AddSubNodesToList(FNode current, ref List<FNode> nodes)
        {
            nodes.Add(current);
            foreach (var child in current.Children)
            {
                AddSubNodesToList(child, ref nodes);
            }
        }
        #endregion
    }
}
