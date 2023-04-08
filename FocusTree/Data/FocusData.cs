using FocusTree.Hoi4Object.Public;
using FocusTree.Tool;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace FocusTree.Data
{
    /// <summary>
    /// 国策节点
    /// </summary>
    public class FocusNode
    {
        #region ==== 基本变量 ====

        public FocusData Data;
        /// <summary>
        /// 国策效果
        /// </summary>
        public List<Sentence> Effects = new();
        /// <summary>
        /// 依赖组
        /// </summary>
        public List<HashSet<int>> Requires = new();
        /// <summary>
        /// （自动生成）子链接
        /// </summary>
        public HashSet<int> Links = new();
        /// <summary>
        /// （自动生成）元坐标
        /// </summary>
        public Vector2 MetaPoint;
        /// <summary>
        /// 原始效果语句
        /// </summary>
        public List<string> RawEffect = new();

        #endregion

        #region ==== 变量获取器 ====

        /// <summary>
        /// 节点ID
        /// </summary>
        public int ID
        {
            get { return int.Parse(Data.ID); }
            set { Data.ID = value.ToString(); }
        }
        /// <summary>
        /// 国策名称
        /// </summary>
        public string Name
        {
            get { return Data.Name; }
            set { Data.Name = value; }
        }
        /// <summary>
        /// 实施国策所需的天数
        /// </summary>
        public int Duration
        {
            get { return int.Parse(Data.Duration); } 
            set { Data.Duration = value.ToString(); }
        }
        /// <summary>
        /// 国策描述
        /// </summary>
        public string Descript
        {
            get { return Data.Descript; }
            set { Data.Descript = value; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Ps
        {
            get { return Data.Ps; }
            set { Data.Ps = value; }
        }

        #endregion

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
        public void ReadXml(XmlReader reader)
        {
            Effects = new();
            Requires = new();

            //==== 读取 Data ====//
            var id = reader.GetAttribute("ID");
            if (id == null) { throw new Exception("缺失必要的节点id"); }
            var name = reader.GetAttribute("Name");
            var star = reader.GetAttribute("Star");
            var duration = reader.GetAttribute("Duration");
            var descript = reader.GetAttribute("Description");
            var ps = reader.GetAttribute("Ps.");
            Data = new(
                id, 
                name ?? string.Empty,
                star ?? "false", 
                duration ?? "0",
                descript ?? string.Empty,
                ps ?? string.Empty
                );

            while ( reader.Read())
            {
                if (reader.Name == "Node" && reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }
                //==== 读取 Effects ====//
                if (reader.Name == "Effects")
                {
                    //ReadEffects(reader);
                }
                //==== 读取 Requires ====//
                if (reader.Name == "Requires")
                {
                    ReadRequires(reader);
                }
                if (reader.Name == "RawEffects")
                {
                    ReadRawEffects(reader);
                }
            }
        }
        public void ReadEffects(XmlReader reader)
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
                    Effects.Add(sentence);
                }
            } while (reader.Read());
            throw new Exception("[2304060212] 读取 Effects 时未能找到结束标签");
        }
        public void ReadRequires(XmlReader reader)
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
        public void ReadRawEffects(XmlReader reader)
        {
            if (reader.ReadToDescendant("Effect") == false) { return; }
            do
            {
                if (reader.Name == "RawEffects" && reader.NodeType == XmlNodeType.EndElement) { return; }
                if (reader.Name == "Effect" && reader.NodeType == XmlNodeType.Element)
                {
                    reader.Read();
                    RawEffect.Add(reader.Value);
                }
            } while (reader.Read());
            throw new Exception("[2304082217] 读取 RawEffects 时未能找到结束标签");
        }
        public void WriteXml(XmlWriter writer)
        {
            // <Node>
            writer.WriteStartElement("Node");

            writer.WriteAttributeString("ID", Data.ID);
            writer.WriteAttributeString("Name", Data.Name);
            writer.WriteAttributeString("Star", Data.BeginWithStar);
            writer.WriteAttributeString("Duration", Data.Duration);
            writer.WriteAttributeString("Descript", Data.Descript);
            writer.WriteAttributeString("Ps.", Data.Ps);

            // <RawEffects>
            writer.WriteStartElement("RawEffects");
            foreach(var effect in Data.Effects)
            {
                writer.WriteElementString("Effect", effect);
            }
            // </RawEffects>
            writer.WriteEndElement();

            // <Effects>
            writer.WriteStartElement("Effects");
            foreach (var sentence in Effects)
            {
                sentence.WriteXml(writer);
            }
            // </Effects>
            writer.WriteEndElement();

            // <Requires>
            writer.WriteStartElement("Requires");
            foreach (var require in Requires)
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
    }

    [XmlRoot("OldNode")]
    /// <summary>
    /// 国策节点数据
    /// </summary>
    public struct FocusData
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        [XmlAttribute("ID")]
        public string ID;
        /// <summary>
        /// 国策名称
        /// </summary>
        [XmlIgnore]
        public string Name;
        /// <summary>
        /// 字段是否以 * 开头
        /// </summary>
        [XmlIgnore]
        public string BeginWithStar;
        /// <summary>
        /// 实施国策所需的天数
        /// </summary>
        [XmlIgnore]
        public string Duration;
        /// <summary>
        /// 国策效果
        /// </summary>
        //[Obsolete("旧格式过渡为新格式的存储")]
        [XmlElement("Effect")]
        public string[] Effects = new string[0];
        /// <summary>
        /// 国策描述
        /// </summary>
        [XmlIgnore]
        public string Descript;
        /// <summary>
        /// 备注
        /// </summary>
        [XmlIgnore]
        public string Ps;
        public FocusData(string id, string name, string beginWithStar, string duration, string descript, string ps)
        {
            ID = id;
            Name = name;
            BeginWithStar = beginWithStar;
            Duration = duration;
            Descript = descript;
            Ps = ps;
        }
    }
}
