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
        [XmlElement("Effects")]
        public string[] Effects = null;
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
        public string GetEffects()
        {
            if (Effects == null)
            {
                return "";
            }
            string effects = "";
            for (int i = 0; i < Effects.Length; i++)
            {
                effects += Effects[i] + "\n";
            }
            return effects;
        }
        public void SetEffects(string effects)
        {
            Effects = effects.Split('\n');
        }

        ///// <summary>
        ///// 分割效果
        ///// </summary>
        ///// <returns></returns>
        //private string[] GetEffects(string str, string fileName, int id)
        //{
        //    bool hasStr = false;
        //    List<string> effects = new();
        //    //var reg = "\\W(\\w+)\\W([+|-]\\d+)((?:%)?)\\W"; // 原： \W(\w+)\W([+|-]\d+)((?:%)?)\W
        //    //var reg = "([\\u4e00-\\u9fa5]+\\W?[+|-]\\d+%?)"; // (\w+)\W?[+|-]\d+%?
        //    //var reg2 = "((增加)?(添加)?\\d+个\\w+)"; // ((?:增加)?\d+个\w+)
        //    var reg3 = "(\\d+x\\d+%?\\w+：\\w+)"; // (\d+x\d+%?\w+：\w+)
        //    var reg4 = "(减少\\d.?\\d\\w+\\d+%?\\w+)"; // (减少\d.?\d\w+\d+%?\w+)
        //    var reg5 = "[\\u4e00-\\u9fa5]+[（].+[）]"; // XX（）
        //    var reg6 = "((（[\\u4e00-\\u9fa5]+）)?获得[\\u4e00-\\u9fa5]+，其效果为（[^（]+）。?)"; // 获得...，其效果为（...）
        //    var reg7 = "(获得[\\u4e00-\\u9fa5]+：[\\u4E00-\\u9FA50-9]+。?)"; // 获得...：...
        //    var reg8 = "((（[\\u4e00-\\u9fa5]+）)?获得\\d+(单位|个){1}[\\u4e00-\\u9fa5]+(，\\d+(单位|个){1}[\\u4e00-\\u9fa5]+)*。?)"; // 获得x个/单位...(，x个/单位...)*
        //    var reg9 = "((（[\\u4e00-\\u9fa5]+）)?(触发事件|完成国策)\\W?(.+”|.+\")+。?)"; // 触发事件/完成国策
        //    var reg10 = "((（[\\u4e00-\\u9fa5]+）)?[\\u4e00-\\u9fa5]+对[\\u4e00-\\u9fa5]+：[+|-]\\d+%?。?)"; // ...对...：+/-...
        //    var reg11 = "(^([\\u4e00-\\u9fa5]+：)?[\\u4e00-\\u9fa5（）]+：([+|-]\\d+%?|[\\u4e00-\\u9fa5]+))(，[\\u4e00-\\u9fa5]+：([+|-]\\d+%?|[\\u4e00-\\u9fa5]+))*"; // 争取复权生机追加效果：（蜘蛛风洞）我国对其进攻修正：+10%，我国对其防御加成：+10%
        //    var reg12 = "以.+取代.+(?:，以|。以)"; // 以...取代...
        //    var matches =
        //        // Regex.Matches(str, reg).Union(
        //        //Regex.Matches(str, reg2).Union(
        //        Regex.Matches(str, reg3).Union(
        //        Regex.Matches(str, reg4)).Union(
        //        Regex.Matches(str, reg5)).Union(
        //        Regex.Matches(str, reg6)).Union(
        //        Regex.Matches(str, reg7)).Union(
        //        Regex.Matches(str, reg8)).Union(
        //        Regex.Matches(str, reg9)).Union(
        //        Regex.Matches(str, reg10)).Union(
        //        Regex.Matches(str, reg11)).Union(
        //        Regex.Matches(str, reg12)).ToArray();
        //    if (matches.Length > 0)
        //    {
        //        foreach (Match match in matches)
        //        {
        //            if (match.Success)
        //            {
        //                effects.Add(match.Groups[1].Value);

        //                hasStr = true;
        //            }
        //        }
        //    }
        //    string testStr = "";
        //    effects.ForEach(x => testStr += x);
        //    if (testStr.Length != str.Length)
        //    {
        //        Program.testInfo.InfoText += $"×{fileName}[{id}] - {str}\n";
        //        Program.testInfo.erro++;
        //    }
        //    else if (testStr != str)
        //    {
        //        Program.testInfo.InfoText += $"?{fileName}[{id}] - {str}\n";
        //        Program.testInfo.differ++;
        //    }
        //    else
        //    {
        //        Program.testInfo.InfoText += $"√{fileName}[{id}] - {str}\n";
        //        Program.testInfo.good++;
        //    }
        //    Program.testInfo.total++;
        //    return effects.ToArray();
        //}
    }
}
