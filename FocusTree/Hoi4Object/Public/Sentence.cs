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
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using FocusTree.Tool.Data;
using IFormattable = FocusTree.Tool.Data.IFormattable;

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

        #region ==== 变量获取器 ====

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
        public Types? ValueType
        {
            get 
            { 
                var type = GetEnumValue<Types>(Main.ValueType);
                return type == null ? null : (Types)type;
            }
            set { Main.ValueType = value == null ? string.Empty : value.Value.ToString(); }
        }
        /// <summary>
        /// 执行对象
        /// </summary>
        public string Value
        {
            get { return Main.Value; }
            set { Main.Value = value; }
        }
        public Types? TriggerType
        {
            get
            {
                var type = GetEnumValue<Types>(Main.TriggerType);
                return type == null ? null : (Types)type;
            }
            set { Main.TriggerType = value == null ? string.Empty : value.Value.ToString(); }
        }
        /// <summary>
        /// 动作触发者
        /// </summary>
        public List<string> Trigger
        { 
            get { return ArrayString.Reader(Main.Trigger).ToList(); }
            set { Main.Trigger = ArrayString.Writer(value.ToArray()); }
        }

        #endregion

        #region ==== 构造函数 ====

        /// <summary>
        /// 
        /// </summary>
        /// <param name="motion">执行动作，不可为空</param>
        /// <param name="valueType">值类型</param>
        /// <param name="value">执行值</param>
        /// <param name="triggerType">触发者类型</param>
        /// <param name="trigger">动作触发者</param>
        /// <param name="subSentences">子句</param>
        /// <returns></returns>
        public Sentence(
            Motions motion,
            Types? valueType,
            string value,
            Types? triggerType,
            string[] trigger,
            List<Sentence> subSentences
            )
        {
            Main = new(
                motion.ToString(),
                valueType == null ? string.Empty : valueType.Value.ToString(),
                value ?? string.Empty,
                triggerType == null ? string.Empty : triggerType.Value.ToString(),
                trigger == null ? string.Empty : ArrayString.Writer(trigger)
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

            //==== 读取主句属性 ====//
            var motion = reader.GetAttribute("Motion");
            var typePair = reader.GetAttribute("Type");
            var valuePair = reader.GetAttribute("Value");
            Main.Motion = motion ?? string.Empty;
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
                    Sentence sentence = new();
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

            writer.WriteAttributeString("Motion", Main.Motion);
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
                    Main.ValueType = match.Groups[1].Value;
                    Main.TriggerType = match.Groups[2].Value;
                    return;
                }
            }
            Main.ValueType = Main.TriggerType = string.Empty;
        }
        private string WriteValuePair()
        {
            return $"({Main.Value}),({Main.Trigger})";
        }

        private void ReadValuePair(string pair)
        {
            if (pair != null)
            {
                var match = Regex.Match(pair.Trim(), "\\((.*)\\),\\((.*)\\)");
                if (match.Success)
                {
                    Main.Value = match.Groups[1].Value;
                    Main.Trigger = match.Groups[2].Value;
                    return;
                }
            }
            Main.Value = Main.Trigger = string.Empty;
        }

        #endregion

        public new string ToString()
        {
            string result = $"Motion= {Main.Motion}, " +
                $"ValueType= {Main.ValueType}, " +
                $"Value= {Main.Value}, " +
                $"TriggerType= {Main.TriggerType}, " +
                $"Trigger= {Main.Trigger}";
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
        /// 值类型
        /// </summary>
        public string ValueType;
        /// <summary>
        /// 执行值
        /// </summary>
        public string Value;
        /// <summary>
        /// 触发者类型
        /// </summary>
        public string TriggerType;
        /// <summary>
        /// 动作触发者
        /// </summary>
        public string Trigger;
        public SentenceAttribute(string motion, string valueType, string value, string triggerType, string trigger)
        {
            Motion = motion;
            ValueType = valueType;
            Value = value;
            TriggerType = triggerType;
            Trigger = trigger;
        }
    }
}
