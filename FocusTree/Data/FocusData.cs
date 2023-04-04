using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace FocusTree.Data
{
    /// <summary>
    /// 国策数据
    /// </summary>
    [XmlRoot("Node")]
    public struct FocusData
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
        [XmlElement("Effect")]
        public string[] Effects = null;
        string[] effects
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
        /// 从文本中解析 FocusData(国测数据)
        /// </summary>
        /// <param name="text">文本</param>
        /// <exception cref="Exception">正则匹配异常</exception>
        public FocusData(int id, string text)
        {
            ID = id;
            // 在 C# 中的字符串，{ 需要转义，通过分割一对来避免歧义。 原 Regex: (.+?){(\d+)天}{(.+?)}(?:{(.+)})?(.+)?
            var pattern = "(.+?){" + "(\\d+)天}{" + "(.+?)}(?:{" + "(.+)})?(.+)?";
            try
            {
                var match = Regex.Match(text, pattern);
                // Groups[0] 是匹配成功部分的文本，应当等同于 text。
                // 从[1]开始才是括号匹配的数据
                // 是否以 * 开头
                BeginWithStar = match.Groups[1].Value.StartsWith("*");
                // 名称
                // 如果以 * 开头，则去掉 *
                if (BeginWithStar)
                    Name = match.Groups[1].Value[1..];
                else
                    Name = match.Groups[1].Value;
                // 天数
                Duration = int.Parse(match.Groups[2].Value);
                // 效果
                Effects = new string[] { match.Groups[3].Value };
                // 描述
                Descript = match.Groups[4].Value;
                // 备注
                Ps = match.Groups[5].Value;
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
