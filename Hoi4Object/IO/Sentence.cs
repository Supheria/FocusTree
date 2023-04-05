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
    public class Sentence : IXmlSerializable
    {
        #region ==== 基本变量 ====

        /// <summary>
        /// 执行动作
        /// </summary>
        [XmlAttribute("Motion")]
        string Motion;
        /// <summary>
        /// 执行对象类型
        /// </summary>
        [XmlAttribute("Type")]
        string Type;
        /// <summary>
        /// 执行对象
        /// </summary>
        [XmlAttribute("Object")]
        string Object;
        /// <summary>
        /// 附带数据
        /// </summary>
        [XmlAttribute("Data")]
        string Data;
        /// <summary>
        /// 触发动作的国家
        /// </summary>
        [XmlAttribute("TriggerState")]
        string TriggerState;
        /// <summary>
        /// 动作受施的国家
        /// </summary>
        [XmlAttribute("SufferState")]
        string SufferState;
        /// <summary>
        /// 子句
        /// </summary>
        List<Sentence> SubSentences;

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
        private Sentence()
        {
        }

        #endregion

        // -- 序列化工具 --
        //static XmlSerializer MainStructure_Serial = new(typeof(SentenceStructure));
        static XmlSerializer Sentence_Serial = new(typeof(Sentence));
        static XmlSerializerNamespaces NullXmlNameSpace = new(new XmlQualifiedName[] { new XmlQualifiedName("", "") });
        // -- 序列化方法 --
        /// <summary>
        /// 序列化预留方法，默认返回 null
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(XmlReader reader)
        {
            SubSentences = new();
            do 
            {
                if (reader.NodeType != XmlNodeType.Element) { continue; }
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                if (reader.Name == "Sentence")
                {
                    ReadSentence(reader, this);
                }
                if (reader.Name == "SubSentence")
                {
                    SubSentences.Add(ReadSentence(reader));
                }
            } while (reader.Read());

        }
        private static Sentence ReadSentence(XmlReader reader, Sentence? sentence = null)
        {
            sentence = sentence == null ? new() : sentence;
            var motion = reader.GetAttribute("Motion");
            sentence.Motion = motion == null ? Motions.None.ToString() : motion;
            var type = reader.GetAttribute("Type");
            sentence.Type = type == null ? Types.None.ToString() : type;
            var @object = reader.GetAttribute("Object");
            sentence.Object = @object == null ? "null" : @object;
            var data = reader.GetAttribute("Data");
            sentence.Data = data == null ? "null" : data;
            var triggerState = reader.GetAttribute("TriggerState");
            sentence.TriggerState = triggerState == null ? "null" : triggerState;
            var sufferState = reader.GetAttribute("SufferState");
            sentence.SufferState = sufferState == null ? "null" : sufferState;
            return sentence;
        }
        public void WriteXml(XmlWriter writer)
        {
            //==== 序列化基本结构 ====//
            WriteSentence(writer, this);

            //==== 序列化子句 ====//

            foreach (var subsentence in SubSentences)
            {
                writer.WriteStartElement("SubSentence");
                WriteSentence(writer, subsentence);
                writer.WriteEndElement();
            }
        }
        private static void WriteSentence(XmlWriter writer, Sentence sentence)
        {
            writer.WriteAttributeString("Motion", sentence.Motion);
            writer.WriteAttributeString("Type", sentence.Type);
            writer.WriteAttributeString("Object", sentence.Object);
            writer.WriteAttributeString("Data", sentence.Data);
            writer.WriteAttributeString("TriggerState", sentence.TriggerState);
            writer.WriteAttributeString("SufferState", sentence.SufferState);
        }
    }
}
