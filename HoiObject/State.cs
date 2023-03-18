using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace HoiObject
{
    public class State : HoiObject, IXmlSerializable, IActionable
    {
        /// <summary>
        /// 静态属性（国家名, <属性名, 数值修正记录>
        /// </summary>
        Dictionary<string, Dictionary<string, List<int>>> StaticAttributes;

        Dictionary<string, Dictionary<string, >>

        #region ==== xml序列化 ====

        public XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(XmlReader reader)
        {

        }
        public void WriteXml(XmlWriter writer)
        {

        }

        #endregion
    }
}
