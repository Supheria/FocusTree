using System.Text.RegularExpressions;
using System.Xml;
using static FocusTree.Data.Hoi4Object.PublicSign;

namespace FocusTree.Data.Hoi4Object
{
    public class Hoi4Sentence
    {
        #region ==== 基本变量 ====

        /// <summary>
        /// 主句属性
        /// </summary>
        Hoi4SentenceStruct Main;
        /// <summary>
        /// 子句
        /// </summary>
        public List<Hoi4Sentence> SubSentences { get; private set; } = new();

        #endregion

        #region ==== 变量获取器 ====

        /// <summary>
        /// 执行动作
        /// </summary>
        public Motions Motion { get => Main.Motion; set => Main.Motion = value; }
        ///// <summary>
        ///// 执行对象类型
        ///// </summary>
        //public Types? ValueType
        //{
        //    get
        //    {
        //        var type = GetEnumValue<Types>(Main.ValueType);
        //        return type == null ? null : (Types)type;
        //    }
        //    set { Main.ValueType = value == null ? string.Empty : value.Value.ToString(); }
        //}
        ///// <summary>
        ///// 执行对象
        ///// </summary>
        //public string Value
        //{
        //    get { return Main.Value; }
        //    set { Main.Value = value; }
        //}
        public Types TriggerType { get => Main.TriggerType; set => Main.TriggerType = value; }
        /// <summary>
        /// 动作触发者
        /// </summary>
        public List<string> Triggers { get => Main.Triggers.ToList(); set => value.ToArray(); }

        #endregion

        #region ==== 构造函数 ====

        /// <summary>
        /// 
        /// </summary>
        /// <param name="motion">执行动作，不可为空</param>
        /// <param name="valueType">值类型</param>
        /// <param name="value">执行值</param>
        /// <param name="triggerType">触发者类型</param>
        /// <param name="triggers">动作触发者</param>
        /// <param name="subSentences">子句</param>
        /// <returns></returns>
        public Hoi4Sentence(
            Motions motion,
            Types valueType,
            string value,
            Types triggerType,
            string[] triggers,
            List<Hoi4Sentence> subSentences
            )
        {
            Main = new()
            {
                Motion = motion,
                ValueType = valueType,
                Value = value ?? string.Empty,
                TriggerType = triggerType,
                Triggers = triggers ?? Array.Empty<string>()
            };
            SubSentences = subSentences ?? new();
        }
        /// <summary>
        /// 用于序列化
        /// </summary>
        public Hoi4Sentence()
        {
            Main = new();
        }

        #endregion

        #region ==== 序列化方法 ====

        public void ReadXml(XmlReader reader)
        {
            SubSentences = new();

            //==== 读取主句属性 ====//
            Main.Motion = (Motions)GetEnumValue<Motions>(reader.GetAttribute("Motion"));
            var typePair = reader.GetAttribute("Type");
            var valuePair = reader.GetAttribute("Value");
            ReadTypePair(typePair);
            ReadValuePair(valuePair);

            //==== 尝试查找并读取子句====//
            if (reader.ReadToDescendant("Sentence") == false) { return; }
            // 进入子句后直到遇到结束标签结束
            do
            {
                if (reader.Name == "Sentence")
                {
                    if (reader.NodeType == XmlNodeType.EndElement) { return; }
                    Hoi4Sentence sentence = new();
                    sentence.ReadXml(reader);
                    SubSentences.Add(sentence);
                }
            } while (reader.Read());
        }
        public void WriteXml(XmlWriter writer)
        {
            //==== 序列化主句 ====//

            // <Sentence>
            writer.WriteStartElement("Sentence");

            writer.WriteAttributeString("Motion", Main.Motion.ToString());
            writer.WriteAttributeString("Type", WriteTypePair());
            writer.WriteAttributeString("Value", WriteValuePair());

            //==== 序列化子句 ====//

            if (SubSentences.Count > 0)
            {
                foreach (var subsentence in SubSentences)
                {
                    subsentence.WriteXml(writer);
                }
            }

            // </Sentence>
            writer.WriteEndElement();
        }

        private string WriteTypePair()
        {
            return $"({Main.ValueType}),({Main.TriggerType})";
        }

        private void ReadTypePair(string pair)
        {
            if (pair != null)
            {
                var match = Regex.Match(pair.Trim(), "\\((.*)\\),\\((.*)\\)");
                if (match.Success)
                {
                    Main.ValueType = (Types)GetEnumValue<Types>(match.Groups[1].Value);
                    Main.TriggerType = (Types)GetEnumValue<Types>(match.Groups[2].Value);
                    return;
                }
            }
            Main.ValueType = Main.TriggerType = Types.None;
        }
        private string WriteValuePair()
        {
            return $"({Main.Value}),({ArrayString.Writer(Main.Triggers)})";
        }

        private void ReadValuePair(string pair)
        {
            if (pair != null)
            {
                var match = Regex.Match(pair.Trim(), "\\((.*)\\),\\((.*)\\)");
                if (match.Success)
                {
                    Main.Value = match.Groups[1].Value;
                    Main.Triggers = ArrayString.Reader(match.Groups[2].Value);
                    return;
                }
            }
            Main.Value = string.Empty;
            Main.Triggers = new string[0];
        }

        #endregion

        #region ==== 拓展序列化方法 ====

        /// <summary>
        /// 转换为json字符串
        /// </summary>
        /// <returns></returns>
        public new string ToString()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 用json字符串生成
        /// </summary>
        /// <param name="jsonString"></param>
        public static Hoi4Sentence FromString(string jsonString)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
