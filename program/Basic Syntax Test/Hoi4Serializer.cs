using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoi4ReaderWriter
{
    public class Hoi4Serializer
    {
        readonly Type MainType;
        public Hoi4Serializer(Type T)
        {
            MainType = T ?? throw new ArgumentNullException();
        }
        public object? Deserialize(Stream stream)
        {
            var o = Activator.CreateInstance(MainType);
            if (o == null) { return null; }
            if (o is not IHoi4Serializable obj) { return null; }
            Hoi4Reader reader = new(stream);
            obj.ReadHoi4(reader);
            return obj;
        }
    }
}
