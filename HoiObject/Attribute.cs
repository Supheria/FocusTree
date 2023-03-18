using HoiObject.HoiVariable.ValueType.ValueType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace HoiObject
{
    public abstract class Attribute : IXmlSerializable
    {
        public Attribute(IValueType data, VariableTags tag)
        {
            Data = data;
            Tag = tag;
        }
        public IValueType Data { get; init; }
        public VariableTags Tag { get; init; }
        public abstract string Add(IValueType IData);
        public abstract string Sub(IValueType IData);
        public abstract string Replace(Attribute IOhter);

        public abstract XmlSchema? GetSchema();
        public abstract void ReadXml(XmlReader reader);
        public abstract void WriteXml(XmlWriter writer);
    }
    //enum AttributeNames
    //{
    //    Stability,

    //}
    ////struct Attribute
    ////{
    ////    public string Name { get; }
    ////    public VariableTags Tag { get; }
    ////}
}
