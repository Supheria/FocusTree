﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml;
//using System.Xml.Schema;
//using System.Xml.Serialization;

//namespace Hoi4Object.IO
//{
//    public class FocusEffects : IXmlSerializable
//    {
//        public Dictionary<int, List<Sentence>> EffectGroups = new();
//        public XmlSchema GetSchema()
//        {
//            return null;
//        }
//        public void ReadXml(XmlReader reader)
//        {
//            do
//            {
//                if (reader.Name == "Node" && reader.NodeType == XmlNodeType.Element)
//                {
//                    var sId = reader.GetAttribute("ID");
//                    if (sId == null) { continue; }
//                    int id = int.Parse(sId);
//                    var effects = ReadEffects(reader);
//                    if (effects != null)
//                    {
//                        EffectGroups[id] = effects;
//                    }
//                }
//            } while (reader.Read());
//        }
//        private List<Sentence>? ReadEffects(XmlReader reader)
//        {
//            List<Sentence> effects = new();
//            // 子节点探针
//            if (reader.ReadToDescendant("Sentence") == false) { return null; }
//            do
//            {
//                if (reader.Name == "Node" && reader.NodeType == XmlNodeType.EndElement)
//                {
//                    break;
//                }
//                if (reader.Name == "Sentence")
//                {
//                    Sentence sentence = new();
//                    sentence.ReadXml(reader);
//                    effects.Add(sentence);
//                }
//            } while (reader.Read());
//            return effects;
//        }
//        public void WriteXml(XmlWriter writer)
//        {
//            foreach (var effectGroup in EffectGroups)
//            {
//                writer.WriteStartElement("Node");
//                writer.WriteAttributeString("ID", effectGroup.Key.ToString());
//                foreach (var sentence in effectGroup.Value)
//                {
//                    sentence.WriteXml(writer);
//                }
//                writer.WriteEndElement();
//            }
//        }
//    }
//}
