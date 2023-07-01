#define FORMAT_TEST
#define RAW_EFFECTS
using FocusTree.Data.Hoi4Object;
using System.Xml;

namespace FocusTree.Data.Focus
{
    /// <summary>
    /// 国策节点控制类
    /// </summary>
    public class FocusNode
    {
        #region ==== 国策信息 ====

        public FocusData FData;
        /// <summary>
        /// 国策效果
        /// </summary>
        public List<string> Effects
        {
            get => _effects.Select(x => x.ToString()).ToList();
            set => value.ForEach(x => _effects.Add(Hoi4Sentence.FromString(x)));
        }
        private readonly List<Hoi4Sentence> _effects = new();


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
            FData = new();
        }
        public void ReadXml(XmlReader reader)
        {
            Effects = new();
            FData = new()
            {
                Id = int.Parse(reader.GetAttribute("ID") ?? throw new ArgumentException()),
                Name = reader.GetAttribute("Name") ?? FData.Name,
                BeginWithStar = bool.Parse(reader.GetAttribute("Star") ?? "false"),
                Duration = int.Parse(reader.GetAttribute("Duration") ?? "0"),
                Description = reader.GetAttribute("Description") ?? FData.Description,
                Ps = reader.GetAttribute("Ps.") ?? FData.Ps,
            };
            var pair = ArrayString.Reader(reader.GetAttribute("Point"));
            if (pair is not { Length: 2 })
                FData.LatticedPoint = new(0, 0);
            else
                FData.LatticedPoint = new(int.Parse(pair[0]), int.Parse(pair[1]));

            while (reader.Read())
            {
                if (reader.Name == "Node" && reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }

                switch (reader.Name)
                {
#if FORMAT_TEST
                    //==== 读取 Effects ====//
                    case "Effects":
                        ReadEffects(reader);
                        break;
#endif
                    //==== 读取 Requires ====//
                    case "Requires":
                        ReadRequires(reader, FData.Requires);
                        break;
                    //==== 读取 RawEffects ====//
                    case "RawEffects":
                        ReadRawEffects(reader, FData.RawEffects);
                        break;
                }
            }
#if FORMAT_TEST
            FormatRawEffects(FData.RawEffects, FData.Id);
#endif
        }
        [Obsolete("临时使用，作为转换语句格式的过渡")]
        private void FormatRawEffects(List<string> rawEffects, int id)
        {
            foreach (var raw in rawEffects)
            {
                Program.TestInfo.Total++;
                if (!FormatRawEffectSentence.Formatter(raw, out var formattedList))
                {
#if RAW_EFFECTS
                    Program.TestInfo.Error++;
                    Program.TestInfo.Good = Program.TestInfo.Total - Program.TestInfo.Error;
                    Program.TestInfo.Append($"{id}. {raw}");
#endif
                    continue;
                }
                foreach (var formatted in formattedList)
                {
                    _effects.Add(formatted);
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
                switch (reader.Name)
                {
                    case "Effects" when reader.NodeType == XmlNodeType.EndElement:
                        return;
                    case "Sentence":
                        {
                            Hoi4Sentence sentence = new();
                            sentence.ReadXml(reader);
                            _effects.Add(sentence);
                            break;
                        }
                }
            } while (reader.Read());
            throw new("[2304060212] 读取 Effects 时未能找到结束标签");
        }

        /// <summary>
        /// 读取节点依赖
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="requires"></param>
        /// <exception cref="Exception"></exception>
        private static void ReadRequires(XmlReader reader, ICollection<HashSet<int>> requires)
        {
            if (reader.ReadToDescendant("Require") == false) { return; }
            do
            {
                switch (reader.Name)
                {
                    case "Requires" when reader.NodeType == XmlNodeType.EndElement:
                        return;
                    case "Require" when reader.NodeType == XmlNodeType.Element:
                        reader.Read();
                        requires.Add(ArrayString.Reader(reader.Value).Select(int.Parse).ToHashSet());
                        break;
                }
            } while (reader.Read());
            throw new("[2302191020] 读取 Requires 时未能找到结束标签");
        }

        /// <summary>
        /// 读取原始效果语句
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="rawEffects"></param>
        /// <exception cref="Exception"></exception>
        private static void ReadRawEffects(XmlReader reader, ICollection<string> rawEffects)
        {
            if (reader.ReadToDescendant("Effect") == false) { return; }
            do
            {
                switch (reader.Name)
                {
                    case "RawEffects" when reader.NodeType == XmlNodeType.EndElement:
                        return;
                    case "Effect" when reader.NodeType == XmlNodeType.Element:
                        reader.Read();
                        rawEffects.Add(reader.Value);
                        break;
                }
            } while (reader.Read());
            throw new("[2304082217] 读取 RawEffects 时未能找到结束标签");
        }
        public void WriteXml(XmlWriter writer)
        {
            // <Node>
            writer.WriteStartElement("Node");

            writer.WriteAttributeString("ID", FData.Id.ToString());
            writer.WriteAttributeString("Name", FData.Name);
            writer.WriteAttributeString("Star", FData.BeginWithStar.ToString());
            writer.WriteAttributeString("Duration", FData.Duration.ToString());
            writer.WriteAttributeString("Description", FData.Description);
            writer.WriteAttributeString("Ps.", FData.Ps);
            var point = FData.LatticedPoint;
            writer.WriteAttributeString("Point", ArrayString.Writer(new string[] { point.Col.ToString(), point.Row.ToString() }));
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
            FormatRawEffects(FData.RawEffects, FData.Id);
            // <Effects>
            writer.WriteStartElement("Effects");
            foreach (var sentence in _effects)
            {
                sentence.WriteXml(writer);
            }
            // </Effects>
            writer.WriteEndElement();
#endif
            // <Requires>
            writer.WriteStartElement("Requires");
            foreach (var require in FData.Requires.Where(require => require.ToArray().Length > 0))
            {
                // <Require>
                writer.WriteElementString("Require", ArrayString.Writer(require.Select(x => x.ToString()).ToArray()));
                // </Require>
            }
            // </Requires>
            writer.WriteEndElement();

            // </Node>
            writer.WriteEndElement();
        }

#endregion
    }
}
