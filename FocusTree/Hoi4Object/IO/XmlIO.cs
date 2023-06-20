using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Hoi4Object.IO
{
    internal class XmlIO
    {
        public static void SaveObject<T>(string path, IXmlSerializable obj)
        {
            var file = File.Create(path);
            var writer = new XmlSerializer(typeof(T));
            writer.Serialize(file, obj);
            file.Close();
        }
        public static T? LoadObject<T>(string path)
        {
            var file = File.OpenRead(path);
            var reader = new XmlSerializer(typeof(T));
            var obj = (T?)reader.Deserialize(file);
            file.Close();
            return obj;
        }
    }
}
