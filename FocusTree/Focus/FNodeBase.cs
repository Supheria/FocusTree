using FocusTree.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FocusTree.Focus
{
    public abstract class FNodeBase
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        [XmlElement("ID")] public abstract int ID { get; set; }
        /// <summary>
        /// 依赖的节点ID
        /// </summary>
        [XmlElement("relied-ID")] public abstract List<int> ReliedIDs { get; set; }
        /// <summary>
        /// 子节点ID
        /// </summary>
        [XmlElement("child-ID")] public abstract List<int> ChildIDs { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        [XmlElement("level")] public abstract int Level { get; set; }
        /// <summary>
        /// 节点在树中的起始列
        /// </summary>
        [XmlElement("start-colum")] public abstract int StartColum { get; set; }
        /// <summary>
        /// 节点在树中的终止列
        /// </summary>
        [XmlElement("end-colum")] public abstract int EndColum { get; set; }
        /// <summary>
        /// 国策数据
        /// </summary>
        [XmlElement("focus-data")] public abstract FData FocusData { get; set; }
    }
}
