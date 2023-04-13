﻿using FocusTree.Data.Hoi4Object;

namespace FocusTree.Data.Focus
{
    /// <summary>
    /// 国策节点数据
    /// </summary>
    public struct FocusData
    {
        /// <summary>
        /// 国策效果
        /// </summary>
        public List<Sentence> Effects = new();
        /// <summary>
        /// 元坐标
        /// </summary>
        public string MetaPoint;
        /// <summary>
        /// 节点ID
        /// </summary>
        public string ID;
        /// <summary>
        /// 国策名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 字段是否以 * 开头
        /// </summary>
        public string BeginWithStar;
        /// <summary>
        /// 实施国策所需的天数
        /// </summary>
        public string Duration;
        /// <summary>
        /// 国策描述
        /// </summary>
        public string Descript;
        /// <summary>
        /// 备注
        /// </summary>
        public string Ps;
        public FocusData(string id, string name, string beginWithStar, string duration, string descript, string ps, string metaPoint)
        {
            ID = id;
            Name = name;
            BeginWithStar = beginWithStar;
            Duration = duration;
            Descript = descript;
            Ps = ps;
            MetaPoint = metaPoint;
        }
    }
}
