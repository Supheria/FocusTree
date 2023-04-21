//#define FORMAT_TEST
#define RAW_EFFECTS
using FocusTree.Data.Hoi4Object;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Xml;

namespace FocusTree.Data.Focus
{
    /// <summary>
    /// 国策节点控制类
    /// </summary>
    public class FocusNode
    {
        #region ==== 节点信息 ====

        #endregion

        #region ==== 国策信息 ====

        public FocusData FData;
        /// <summary>
        /// 国策效果
        /// </summary>
        public List<string> Effects
        {
            get { return effects.Select(x => x.ToString()).ToList(); }
            set { value.ForEach(x => effects.Add(Sentence.FromString(x))); }
        }
        public List<Sentence> effects = new();
        

        #endregion

        public FocusNode(FocusData fData)
        {
            FData = fData;
        }

        #region ==== 序列化方法 ====

        /// <summary>
        /// 用于序列化
        /// </summary>
        public FocusNode()
        {
        }
        public void ReadXml(XmlReader reader)
        {
            Effects = new();
            List<HashSet<int>> requires = new();
            List<string> rawEffects = new();

            //==== 读取 Data ====//
            var id = reader.GetAttribute("ID");
            if (id == null) { throw new Exception("缺失必要的节点id"); }
            var name = reader.GetAttribute("Name");
            var star = reader.GetAttribute("Star");
            var duration = reader.GetAttribute("Duration");
            var descript = reader.GetAttribute("Descript");
            var ps = reader.GetAttribute("Ps.");
            var pair = ArrayString.Reader(reader.GetAttribute("Point"));
            Point latticedPoint = new();
            if (pair == null || pair.Length != 2) { latticedPoint = new(0, 0); }
            else { latticedPoint = new(int.Parse(pair[0]), int.Parse(pair[1])); }

            while (reader.Read())
            {
                if (reader.Name == "Node" && reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                //==== 读取 Effects ====//
                if (reader.Name == "Effects")
                {
                    ReadEffects(reader);
                }
                //==== 读取 Requires ====//
                if (reader.Name == "Requires")
                {
                    ReadRequires(reader, ref requires);
                }
                //==== 读取 RawEffects ====//
                if (reader.Name == "RawEffects")
                {
                    ReadRawEffects(reader, ref rawEffects);
                }
            }
            FData = new(
                int.Parse(id),
                name ?? string.Empty,
                bool.Parse(star),
                int.Parse(duration),
                descript ?? string.Empty,
                ps ?? string.Empty,
                latticedPoint,
                rawEffects,
                requires
                );
            FormatRawEffects(rawEffects, int.Parse(id));
        }
        [Obsolete("临时使用，作为转换语句格式的过渡")]
        private void FormatRawEffects(List<string> rawEffects, int id)
        {
            foreach (var raw in rawEffects)
            {
                Program.testInfo.total++;
                if (!FormatRawEffectSentence.Formatter(raw, out var formattedList))
                {
#if RAW_EFFECTS
                    Program.testInfo.erro++;
                    Program.testInfo.good = Program.testInfo.total - Program.testInfo.erro;
                    Program.testInfo.InfoText += $"{id}. {raw}\n";
#endif
                    continue;
                }
                foreach (var formatted in formattedList)
                {
                    effects.Add(formatted);
                }
            }
        }
        /// <summary>
        /// 读取效果
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="Exception"></exception>
        private void ReadEffects(XmlReader reader)
        {
            // 子节点探针
            if (reader.ReadToDescendant("Sentence") == false) { return; }
            do
            {
                if (reader.Name == "Effects" && reader.NodeType == XmlNodeType.EndElement) { return; }
                if (reader.Name == "Sentence")
                {
                    Sentence sentence = new();
                    sentence.ReadXml(reader);
                    effects.Add(sentence);
                }
            } while (reader.Read());
            throw new Exception("[2304060212] 读取 Effects 时未能找到结束标签");
        }
        /// <summary>
        /// 读取节点依赖
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="Exception"></exception>
        private void ReadRequires(XmlReader reader, ref List<HashSet<int>> requires)
        {
            if (reader.ReadToDescendant("Require") == false) { return; }
            do
            {
                if (reader.Name == "Requires" && reader.NodeType == XmlNodeType.EndElement) { return; }
                if (reader.Name == "Require" && reader.NodeType == XmlNodeType.Element)
                {
                    reader.Read();
                    requires.Add(ArrayString.Reader(reader.Value).Select(x => int.Parse(x)).ToHashSet());
                }
            } while (reader.Read());
            throw new Exception("[2302191020] 读取 Requires 时未能找到结束标签");
        }
        /// <summary>
        /// 读取原始效果语句
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="Exception"></exception>
        private void ReadRawEffects(XmlReader reader, ref List<string> rawEffects)
        {
            if (reader.ReadToDescendant("Effect") == false) { return; }
            do
            {
                if (reader.Name == "RawEffects" && reader.NodeType == XmlNodeType.EndElement) { return; }
                if (reader.Name == "Effect" && reader.NodeType == XmlNodeType.Element)
                {
                    reader.Read();
                    rawEffects.Add(reader.Value);
                }
            } while (reader.Read());
            throw new Exception("[2304082217] 读取 RawEffects 时未能找到结束标签");
        }
        public void WriteXml(XmlWriter writer)
        {
            // <Node>
            writer.WriteStartElement("Node");

            writer.WriteAttributeString("ID", FData.ID.ToString());
            writer.WriteAttributeString("Name", FData.Name);
            writer.WriteAttributeString("Star", FData.BeginWithStar.ToString());
            writer.WriteAttributeString("Duration", FData.Duration.ToString());
            writer.WriteAttributeString("Descript", FData.Descript.ToString());
            writer.WriteAttributeString("Ps.", FData.Ps);
            var point = FData.LatticedPoint;
            writer.WriteAttributeString("Point", ArrayString.Writer(new string[] { point.X.ToString(), point.Y.ToString() }));
#if RAW_EFFECTS
            // <RawEffects>
            writer.WriteStartElement("RawEffects");
            foreach (var effect in FData.RawEffects)
            {
                writer.WriteElementString("Effect", effect);
            }
            // </RawEffects>
            writer.WriteEndElement();
#endif
#if FORMAT_TEST
            // <Effects>
            writer.WriteStartElement("Effects");
            foreach (var sentence in Data.Effects)
            {
                sentence.WriteXml(writer);
            }
            // </Effects>
            writer.WriteEndElement();
#endif
            // <Requires>
            writer.WriteStartElement("Requires");
            foreach (var require in FData.Requires)
            {
                if (require.ToArray().Length > 0)
                {
                    // <Require>
                    writer.WriteElementString("Require", ArrayString.Writer(require.Select(x => x.ToString()).ToArray()));
                    // </Require>
                }
            }
            // </Requires>
            writer.WriteEndElement();

            // </Node>
            writer.WriteEndElement();
        }

        #endregion
    }
}
