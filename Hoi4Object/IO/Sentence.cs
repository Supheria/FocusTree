using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using static Hoi4Object.IO.PublicSign;

namespace Hoi4Object.IO
{
    public class Sentence
    {
        #region ==== 基本变量 ====

        /// <summary>
        /// 执行动作
        /// </summary>
        [XmlAttribute("Motion")]
        string Motion = Motions.None.ToString();
        /// <summary>
        /// 执行对象类型
        /// </summary>
        [XmlAttribute("Type")]
        string Type = Types.None.ToString();
        /// <summary>
        /// 执行对象
        /// </summary>
        [XmlAttribute("Object")]
        string Object = "null";
        /// <summary>
        /// 附带数据
        /// </summary>
        [XmlAttribute("Data")]
        string Data = "null";
        /// <summary>
        /// 触发动作的国家
        /// </summary>
        [XmlAttribute("TriggerState")]
        string TriggerState = "null";
        /// <summary>
        /// 动作受施的国家
        /// </summary>
        [XmlAttribute("SufferState")]
        string SufferState = "null";
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
        /// <param name="object">有可用性的执行对象，不可为空</param>
        /// <param name="data">附带数据</param>
        /// <param name="triggerState">触发动作的国家</param>
        /// <param name="sufferState">动作受施的国家</param>
        /// <param name="subSentences">子句</param>
        /// <returns></returns>
        public Sentence(
            Motions motion,
            Types type,
            string @object,
            string? data,
            string? triggerState,
            string? sufferState,
            List<Sentence>? subSentences
            )
        {
            Motion = motion.ToString();
            Type = type.ToString();
            Object = @object;
            Data = data == null ? "null" : data;
            TriggerState = triggerState == null ? "null" : triggerState;
            SufferState = sufferState == null ? "null" : sufferState;
            SubSentences = subSentences == null ? new() : subSentences;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="motion">执行动作，不可为空</param>
        /// <param name="availabilityObject">有可用性的执行对象，不可为空</param>
        /// <param name="data">附带数据</param>
        /// <param name="triggerState">触发动作的国家</param>
        /// <param name="sufferState">动作受施的国家</param>
        /// <param name="subSentences">子句</param>
        /// <returns></returns>
        public Sentence(Motions motion,
            AvailabilityObjects availabilityObject,
            string? data,
            string? triggerState,
            string? sufferState,
            List<Sentence>? subSentences
            )
        {
            Motion = motion.ToString();
            Type = Types.Availability.ToString();
            Object = availabilityObject.ToString();
            Data = data == null ? "null" : data;
            TriggerState = triggerState == null ? "null" : triggerState;
            SufferState = sufferState == null ? "null" : sufferState;
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
            var @object = reader.GetAttribute("Object");
            var data = reader.GetAttribute("Data");
            var triggerState = reader.GetAttribute("TriggerState");
            var sufferState = reader.GetAttribute("SufferState");
            Motion = motion == null ? Motions.None.ToString() : motion;
            Type = type == null ? Types.None.ToString() : type;
            Object = @object == null ? "null" : @object;
            Data = data == null ? "null" : data;
            TriggerState = triggerState == null ? "null" : triggerState;
            SufferState = sufferState == null ? "null" : sufferState;

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
            writer.WriteAttributeString("Object", Object);
            writer.WriteAttributeString("Data", Data);
            writer.WriteAttributeString("TriggerState", TriggerState);
            writer.WriteAttributeString("SufferState", SufferState);

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
    }
}
