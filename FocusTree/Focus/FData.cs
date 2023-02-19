using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FocusTree.Tree
{
    /// <summary>
    /// 国策数据
    /// </summary>
    [XmlRoot("Node")]
    public struct FData
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        [XmlAttribute("ID")]
        public int ID;
        /// <summary>
        /// 国策名称
        /// </summary>
        [XmlElement("Name")]
        public string Name = "未定义";
        /// <summary>
        /// 字段是否以 * 开头
        /// </summary>
        [XmlElement("Star")]
        public bool BeginWithStar = false;
        /// <summary>
        /// 实施天数
        /// </summary>
        [XmlElement("Duration")]
        public int Duration = -1;
        /// <summary>
        /// 国策效果
        /// </summary>
        [XmlElement("Effects")]
        public string Effects = string.Empty;
        /// <summary>
        /// 国策描述
        /// </summary>
        [XmlElement("Descript")]
        public string Descript = string.Empty;
        /// <summary>
        /// 备注
        /// </summary>
        [XmlElement("Ps.")]
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
        public FData(
            int id,
            string name,
            bool isBeginWithstar,
            int duration,
            string effects,
            string descript,
            string ps
            )
        {
            ID = id;
            Name = name;
            BeginWithStar = isBeginWithstar;
            Duration = duration;
            Effects = effects;
            Descript = descript;
            Ps = ps;
        }
        /// <summary>
        /// 根节点专用的 FData
        /// </summary>
        /// <param name="isRoot"></param>
        /// <exception cref="Exception"></exception>
        public FData(bool isRoot)
        {
            if (isRoot)
            {
                this = new FData();
                Name = "根节点";
            }
            else
            {
                throw new Exception("[2302160957] 未定义的 FData数据类型");
            }
        }
        /// <summary>
        /// 从文本中解析 FocusData(国测数据)
        /// </summary>
        /// <param name="text">文本</param>
        /// <exception cref="Exception">正则匹配异常</exception>
        public FData(int id, string text)
        {
            // 在 C# 中的字符串，{ 需要转义，通过分割一对来避免歧义。 原 Regex: (.+?){(\d+)天}{(.+?)}(?:{(.+)})?(.+)?
            var pattern = "(.+?){" + "(\\d+)天}{" + "(.+?)}(?:{" + "(.+)})?(.+)?";
            try
            {
                var match = Regex.Match(text, pattern);
                // Groups[0] 是匹配成功部分的文本，应当等同于 text。
                // 从[1]开始才是括号匹配的数据
                // 是否以 * 开头
                var isBeginWithStar = match.Groups[1].Value.StartsWith("*");
                // 名称
                string name;
                // 如果以 * 开头，则去掉 *
                if (isBeginWithStar)
                    name = match.Groups[1].Value[1..];
                else
                    name = match.Groups[1].Value;
                // 天数
                int duration = int.Parse(match.Groups[2].Value);
                // 效果
                string effects = match.Groups[3].Value;
                // 描述
                string descript = match.Groups[4].Value;
                // 备注
                string ps = match.Groups[5].Value;

                // 使用数据创建实例
                this = new FData(
                    id,
                    name,
                    isBeginWithStar,
                    duration,
                    effects,
                    descript,
                    ps
                    );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"正则匹配时发生了异常。\n" +
                    $"试图解析的内容: {text}\n" +
                    $"异常信息: {ex.Message}");
            }
        }
    }
}
