using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoi4ReaderWriter
{
    internal interface IHoi4Serializable
    {
        void ReadHoi4(Hoi4Reader reader);
    }
}
