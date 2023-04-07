using FocusTree.Hoi4Object.IO.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FocusTree.Hoi4Object.Public.PublicSign;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.InteropServices;
using FocusTree.Tool;

namespace FocusTree.Hoi4Object.Public
{
    public class Sentence
    {
        #region ==== 基本变量 ====

        /// <summary>
        /// 主句属性
        /// </summary>
        SentenceAttribute Main;
        /// <summary>
        /// 子句
        /// </summary>
        public List<Sentence> SubSentences { get; private set; } = new();

        #endregion

        #region ==== 属性获取器 ====

        /// <summary>
        /// 执行动作
        /// </summary>
        public Motions? Motion
        {
            get
            {
                var motion = GetEnumValue<Motions>(Main.Motion);
                return motion == null ? null : (Motions)motion;
            }
            set { Main.Motion = value == null ? string.Empty : value.Value.ToString(); }
        }
        /// <summary>
        /// 执行对象类型
        /// </summary>
        public Types? Type
        {
            get 
            { 
                var type = GetEnumValue<Types>(Main.Type);
                return type == null ? null : (Types)type;
            }
            set { Main.Type = value == null ? string.Empty : value.Value.ToString(); }
        }
        /// <summary>
        /// 执行对象
        /// </summary>
        public string Value
        {
            get { return Main.Value; }
            set { Main.Value = value; }
        }
        /// <summary>
        /// 动作触发者
        /// </summary>
        public List<string> Trigger
        { 
            get { return Main.Trigger.ToList(); }
            set { Main.Trigger = value.ToArray(); }
        }

        #endregion

        #region ==== 构造函数 ====

        /// <summary>
        /// 
        /// </summary>
        /// <param name="motion">执行动作，不可为空</param>
        /// <param name="type">执行对象类型</param>
        /// <param name="value">执行值</param>
        /// <param name="trigger">动作触发者</param>
        /// <param name="subSentences">子句</param>
        /// <returns></returns>
        public Sentence(
            Motions motion,
            Types? type,
            string? value,
            string[]? trigger,
            List<Sentence>? subSentences
            )
        {
            Main = new(
                motion.ToString(),
                type == null ? string.Empty : type.Value.ToString(),
                value ?? string.Empty,
                trigger ?? Array.Empty<string>()
                );
            SubSentences = subSentences ?? new();
        }
        /// <summary>
        /// 用于序列化
        /// </summary>
        public Sentence()
        {
        }

        #endregion

        #region ==== 序列化方法 ====

        public void ReadXml(XmlReader reader)
        {
            SubSentences = new();

            var motion = reader.GetAttribute("Motion");
            var type = reader.GetAttribute("Type");
            var value = reader.GetAttribute("Value");
            var trigger = reader.GetAttribute("Trigger");
            Main.Motion = motion ?? string.Empty;
            Main.Type = type ?? string.Empty;
            Main.Value = value ?? string.Empty;
            Main.Trigger = trigger == null ? Array.Empty<string>() : ArrayString.Reader(trigger);
            
            if (reader.ReadToDescendant("Sentence") == false) { return; }
            // 进入子句后直到遇到结束标签结束
            do
            {
                if (reader.Name == "Sentence")
                {
                    if (reader.NodeType == XmlNodeType.EndElement) { return; }
                    Sentence sentence = new();
                    sentence.ReadXml(reader);
                    SubSentences.Add(sentence);
                }
            } while (reader.Read());
        }
        public void WriteXml(XmlWriter writer)
        {
            //==== 序列化主句 ====//

            writer.WriteStartElement("Sentence");

            writer.WriteAttributeString("Motion", Main.Motion);
            writer.WriteAttributeString("Type", Main.Type);
            writer.WriteAttributeString("Value", Main.Value);
            writer.WriteAttributeString("Trigger", ArrayString.Writer(Main.Trigger));

            //==== 序列化子句 ====//

            if (SubSentences.Count > 0)
            {
                foreach (var subsentence in SubSentences)
                {
                    subsentence.WriteXml(writer);
                }
            }

            writer.WriteEndElement();
        }

        #endregion

        public new string ToString()
        {
            string result =  $"Motion= {Main.Motion}, " +
                $"Type= {Main.Type}, " +
                $"Value= {Main.Value}, " +
                $"Trigger= {ArrayString.Writer(Trigger.ToArray())}";
            foreach (var sentence in SubSentences)
            {
                result += "\n  Sub: " + sentence.ToString();
            }
            return result ;
        }
    }

    public struct SentenceAttribute
    {
        /// <summary>
        /// 执行动作
        /// </summary>
        public string Motion;
        /// <summary>
        /// 执行对象类型
        /// </summary>
        public string Type;
        /// <summary>
        /// 执行数据
        /// </summary>
        public string Value;
        /// <summary>
        /// 动作触发者
        /// </summary>
        public string[] Trigger;
        public SentenceAttribute(string motion, string type, string value, string[] trigger)
        {
            Motion = motion;
            Type = type;
            Value = value;
            Trigger = trigger;
        }
    }
}
