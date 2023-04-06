using FocusTree.Hoi4Object.IO.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FocusTree.Hoi4Object.Public.PublicSign;
using System.Xml.Serialization;
using System.Xml;

namespace FocusTree.Hoi4Object.Public
{
    public class Sentence
    {
        #region ==== 基本变量 ====

        /// <summary>
        /// 执行动作
        /// </summary>
        [XmlAttribute("Motion")]
        string Motion = "null";
        /// <summary>
        /// 执行对象类型
        /// </summary>
        [XmlAttribute("Type")]
        string Type = "null";
        /// <summary>
        /// 执行对象
        /// </summary>
        [XmlAttribute("Value")]
        string Value = "null";
        /// <summary>
        /// 动作触发者
        /// </summary>
        [XmlAttribute("MotionTrigger")]
        string MotionTrigger = "null";
        /// <summary>
        /// 子句
        /// </summary>
        List<Sentence> SubSentences = new();

        #endregion

        #region ==== 构造函数 ====

        /// <summary>
        /// 
        /// </summary>
        /// <param name="motion">执行动作，不可为空</param>
        /// <param name="type">执行对象类型</param>
        /// <param name="value">执行值</param>
        /// <param name="motionTrigger">动作触发者</param>
        /// <param name="subSentences">子句</param>
        /// <returns></returns>
        public Sentence(
            Motions motion,
            Types? type,
            string value,
            string motionTrigger,
            List<Sentence> subSentences
            )
        {
            Motion = motion.ToString();
            Type = type == null ? "null" : type.ToString();
            Value = value == null ? "null" : value;
            MotionTrigger = motionTrigger == null ? "null" : motionTrigger;
            SubSentences = subSentences == null ? new() : subSentences;
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
            var motionTrigger = reader.GetAttribute("Trigger");
            Motion = motion == null ? "null" : motion;
            Type = type == null ? "null" : type;
            Value = value == null ? "null" : value;
            MotionTrigger = motionTrigger == null ? "null" : motionTrigger;

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
            writer.WriteAttributeString("Motion", Motion);
            writer.WriteAttributeString("Type", Type);
            writer.WriteAttributeString("Value", Value);
            writer.WriteAttributeString("Trigger", MotionTrigger);

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
            string result =  $"Motion= {Motion}, Type= {Type}, Value= {Value}, Trigger= {MotionTrigger}";
            foreach (var sentence in SubSentences)
            {
                result += "\n\tSub: " + sentence.ToString();
            }
            return result ;
        }
    }
}
