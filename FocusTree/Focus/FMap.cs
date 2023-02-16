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
        /// <returns></returns>
        abstract public List<FMapNode> GetAllNodes();
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
