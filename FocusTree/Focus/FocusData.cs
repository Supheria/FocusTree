using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FocusTree.Tree
{
    /// <summary>
    /// 国策数据
    /// </summary>
    public struct SFocusData
    {
        /// <summary>
        /// 国策名称
        /// </summary>
        [XmlElement("name")]
        public string Name = "根节点";
        /// <summary>
        /// 字段是否以 * 开头
        /// </summary>
        [XmlElement("begin-with-star")]
        public bool BeginWithStar = false;
        /// <summary>
        /// 实施天数
        /// </summary>
        [XmlElement("duration")]
        public int Duration = -1;
        /// <summary>
        /// 国策效果
        /// </summary>
        [XmlElement("effects")]
        public string Effects = string.Empty;
        /// <summary>
        /// 国策描述
        /// </summary>
        [XmlElement("descript")]
        public string Descript = string.Empty;
        /// <summary>
        /// 备注
        /// </summary>
        [XmlElement("ps.")]
        public string Ps = string.Empty;

        /// <summary>
        /// 使用数据创建国策
        /// </summary>
        /// <param name="name">国策名称</param>
        /// <param name="isBeginWithstar">字段是否以 * 开头</param>
        /// <param name="duration">实施天数</param>
        /// <param name="effects">国策效果</param>
        /// <param name="descript">国策描述</param>
        /// <param name="ps">备注</param>
        public SFocusData(
            string name,
            bool isBeginWithstar,
            int duration,
            string effects,
            string descript,
            string ps
            )
        {
            Name = name;
            BeginWithStar = isBeginWithstar;
            Duration = duration;
            Effects = effects;
            Descript = descript;
            Ps = ps;
        }
    }
}
