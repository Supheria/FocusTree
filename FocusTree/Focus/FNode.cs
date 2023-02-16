﻿using FocusTree.Tree;
using System.Xml.Serialization;

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
        //== 节点控制 ==//
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
            // 把节点加入父节点的子集
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
    }
}