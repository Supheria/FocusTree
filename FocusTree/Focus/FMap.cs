﻿using FocusTree.Tree;
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
