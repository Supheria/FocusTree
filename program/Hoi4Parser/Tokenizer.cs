using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoi4Parser
{
#if WIN32
    using size_t = UInt32;
#else
    using size_t = UInt64;
#endif
    public class Tokenizer
    {
        public static List<int> Delimiter = new() { '\t', ' ', '\n', '\r', '#', '=', '>', '<', '}', '{', '"', -1 };
        public static List<int> Blank = new() { '\t', ' ', '\n', '\r' };
        public static List<int> EndLine = new() { '\n', '\r', -1 };
        public static List<int> Marker = new() { '=', '>', '<', '}', '{' };
        public static int Note = '#';
        public static int Quote = '"';

        size_t Line;
        size_t Column;
        readonly Stream Buffer;

        public Tokenizer(Stream stream)
        {
            Buffer = stream;
            if (ReadByte() != 0xEF || ReadByte() != 0xBB || ReadByte() != 0xBF) // fin.get() will return -1 if meet EOF
            {
                Buffer.Seek(0, SeekOrigin.Begin);
                Column = 0;
            }
        }


        private int ReadByte()
        {
            Column++;
            return Buffer.ReadByte();
        }
    }
}
