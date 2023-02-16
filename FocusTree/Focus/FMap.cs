using FocusTree.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FocusTree.Focus
{
    public abstract class FMap
    {
        /// <summary>
        /// 获取结构中所有节点的方法
        /// </summary>
        /// <returns>结构中的所有有效节点</returns>
        abstract public HashSet<FMapNode> GetAllMapNodes();
        /// <summary>
        /// 使用 ID 获取特定节点
        /// </summary>
        /// <returns>获取的Node</returns>
        abstract public FMapNode GetMapNodeById(int id);
        /// <summary>
        /// 获取特定层有多少个节点
        /// </summary>
        /// <param name="level">层级</param>
        /// <returns>层级节点数量</returns>
        abstract public int GetLevelNodeCount(int level);
        /// <summary>
        /// 获取某一层的所有节点
        /// </summary>
        /// <param name="level">层级</param>
        /// <returns>层级中所有节点</returns>
        abstract public HashSet<FMapNode> GetLevelNodes(int level);
        /// <summary>
        /// 返回兄弟节点 (具有 相似依赖 和 Level)<br/>
        /// 相似依赖指如果本节点依赖多个节点，那么返回所有至少含有一个对节点依赖的节点
        /// </summary>
        /// <param name="id">根据 ID 查找</param>
        /// <returns>兄弟节点</returns>
        abstract public HashSet<FMapNode> GetSiblingNodes(int id);
    }
    public abstract class FMapNode
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public abstract int ID { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public abstract int Level { get; set; }
        /// <summary>
        /// 国策数据
        /// </summary>
        public abstract FData FocusData { get; set; }
    }
}
