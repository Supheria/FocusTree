using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace FocusTree.IO
{
    public struct CsvFocusData
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        [XmlAttribute("ID")]
        public int Id { get; } = 0;
        /// <summary>
        /// 国策名称
        /// </summary>
        public string Name { get; } = "";
        /// <summary>
        /// 字段是否以 * 开头
        /// </summary>
        public bool BeginWithStar { get; } = false;

        /// <summary>
        /// 实施天数
        /// </summary>
        public int Duration { get; } = 0;
        /// <summary>
        /// 国策效果
        /// </summary>
        public string RawEffectsCohesion { get; } = "";
        /// <summary>
        /// 国策描述
        /// </summary>
        public string Description { get; } = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Ps { get; } = "";

        public List<HashSet<int>> Requires { get; } = new();

        /// <summary>
        /// 从文本中解析 FocusData(国测数据)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text">文本</param>
        /// <exception cref="Exception">正则匹配异常</exception>
        public CsvFocusData(int id, string text)
        {
            Id = id;

            try
            {
                var match = Regex.Match(text, "(.+?){" + "(\\d+)天}{" + "(.+?)}(?:{" + "(.+)})?(.+)?");
                // Groups[0] 是匹配成功部分的文本，应当等同于 text。
                // 从[1]开始才是括号匹配的数据
                // 是否以 * 开头
                BeginWithStar = match.Groups[1].Value.StartsWith("*");
                // 如果以 * 开头，则去掉 *
                Name = BeginWithStar ? match.Groups[1].Value[1..] : match.Groups[1].Value;
                // 天数
                Duration = int.Parse(match.Groups[2].Value);
                // 效果
                RawEffectsCohesion = match.Groups[3].Value;
                // 描述
                Description = match.Groups[4].Value;
                // 备注
                Ps = match.Groups[5].Value;
            }
            catch (Exception ex)
            {
                throw new(
                    $"正则匹配时发生了异常。\n" +
                    $"试图解析的内容: {text}\n" +
                    $"异常信息: {ex.Message}");
            }
        }
    }
}
