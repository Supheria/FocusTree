//#define FORMAT_TEST
#define RAW_EFFECTS
using FocusTree.Data.Hoi4Object;
using System.Numerics;
using System.Xml;

namespace FocusTree.Data.Focus
{
    /// <summary>
    /// 国策节点
    /// </summary>
    public class FocusNode
    {
        #region ==== 基本变量 ====

        public FocusData Data;
        /// <summary>
        /// 依赖组
        /// </summary>
        public List<HashSet<int>> Requires = new();
        /// <summary>
        /// （自动生成）子链接
        /// </summary>
        public HashSet<int> Links = new();
        /// <summary>
        /// 原始效果语句
        /// </summary>
        public List<string> RawEffects = new();

        #endregion

        #region ==== 变量获取器 ====

        /// <summary>
        /// 国策效果
        /// </summary>
        public List<string> Effects
        {
            get { return effects.Select(x => x.ToString()).ToList(); }
            set { value.ForEach(x => effects.Add(Sentence.FromString(x))); }
        }
        public List<Sentence> effects = new();
        /// <summary>
        /// 栅格化坐标（nullable 给初始化节点位置用的，读取存储文件后不会为null，可以忽略）
        /// </summary>
        public Point LatticedPoint;
        /// <summary>
        /// 节点ID
        /// </summary>
        public int ID;
        /// <summary>
        /// 国策名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 实施国策所需的天数
        /// </summary>
        public int Duration;
        /// <summary>
        /// 国策描述
        /// </summary>
        public string Descript;
        /// <summary>
        /// 备注
        /// </summary>
        public string Ps;
        /// <summary>
        /// 字段是否以 * 开头
        /// </summary>
        public bool BeginWithStar;

        #endregion

        #region ==== 构造函数 ====

        /// <summary>
        /// 用于序列化
        /// </summary>
        public FocusNode()
        {
        }

        public FocusNode(FocusData data)
        {
            Data = data;
        }

        #endregion

        #region ==== 序列化方法 ====

        public void ReadXml(XmlReader reader)
        {
            Effects = new();
            Requires = new();

            //==== 读取 Data ====//
            ID = int.Parse(reader.GetAttribute("ID"));
            if (ID == null) { throw new Exception("缺失必要的节点id"); }
            Name = reader.GetAttribute("Name");
            BeginWithStar = bool.Parse(reader.GetAttribute("Star"));
            Duration = int.Parse(reader.GetAttribute("Duration"));
            Descript = reader.GetAttribute("Descript");
            Ps = reader.GetAttribute("Ps.");
            //LatticedPoint = Point.reader.GetAttribute("Point");

            while (reader.Read())
            {
                if (reader.Name == "Node" && reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }
                //==== 读取 Effects ====//
                if (reader.Name == "Effects")
                {
                    ReadEffects(reader);
                }
                //==== 读取 Requires ====//
                if (reader.Name == "Requires")
                {
                    ReadRequires(reader);
                }
                //==== 读取 RawEffects ====//
                if (reader.Name == "RawEffects")
                {
                    ReadRawEffects(reader);
                    FormatRawEffects();
                }
            }
        }
        [Obsolete("临时使用，作为转换语句格式的过渡")]
        private void FormatRawEffects()
        {
            foreach (var raw in RawEffects)
            {
                Program.testInfo.total++;
                if (!FormatRawEffectSentence.Formatter(raw, out var formattedList))
                {
#if RAW_EFFECTS
                    Program.testInfo.erro++;
                    Program.testInfo.good = Program.testInfo.total - Program.testInfo.erro;
                    Program.testInfo.InfoText += $"{ID}. {raw}\n";
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
        private void ReadRequires(XmlReader reader)
        {
            if (reader.ReadToDescendant("Require") == false) { return; }
            do
            {
                if (reader.Name == "Requires" && reader.NodeType == XmlNodeType.EndElement) { return; }
                if (reader.Name == "Require" && reader.NodeType == XmlNodeType.Element)
                {
                    reader.Read();
                    Requires.Add(ArrayString.Reader(reader.Value).Select(x => int.Parse(x)).ToHashSet());
                }
            } while (reader.Read());
            throw new Exception("[2302191020] 读取 Requires 时未能找到结束标签");
        }
        /// <summary>
        /// 读取原始效果语句
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="Exception"></exception>
        private void ReadRawEffects(XmlReader reader)
        {
            if (reader.ReadToDescendant("Effect") == false) { return; }
            do
            {
                if (reader.Name == "RawEffects" && reader.NodeType == XmlNodeType.EndElement) { return; }
                if (reader.Name == "Effect" && reader.NodeType == XmlNodeType.Element)
                {
                    reader.Read();
                    RawEffects.Add(reader.Value);
                }
            } while (reader.Read());
            throw new Exception("[2304082217] 读取 RawEffects 时未能找到结束标签");
        }
        public void WriteXml(XmlWriter writer)
        {
            // <Node>
            writer.WriteStartElement("Node");

            writer.WriteAttributeString("ID", ID.ToString());
            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeString("Star", BeginWithStar.ToString());
            writer.WriteAttributeString("Duration", Duration.ToString());
            writer.WriteAttributeString("Descript", Descript.ToString());
            writer.WriteAttributeString("Ps.", Ps);
            writer.WriteAttributeString("Point", LatticedPoint.ToString());
#if RAW_EFFECTS
            // <RawEffects>
            writer.WriteStartElement("RawEffects");
            foreach (var effect in RawEffects)
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
            foreach (var require in Requires)
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
