#define DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoi4ReaderWriter
{
    public class Reader
    {
        public enum ElementTypes
        {
            Element,
            StartElement,
            EndElement
        }
        public string Name { get; private set; } = string.Empty;
        public ElementTypes Type { get; private set; }
        public string Value { get; private set; } = string.Empty;
        Stack<string> StartElements = new();
        public int TabTimes { get; private set; } = 0;
        /// <summary>
        /// 裁剪流
        /// </summary>
        MemoryStream TrimedStream = new();
        public Reader(Stream rawStream)
        {
            ReadTrimedStream(rawStream);
#if DEBUG
            FileStream file = new("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\FILES\\test reader writer\\class out put.txt", FileMode.Create);
            file.Write(TrimedStream.ToArray());
            foreach (var c in TrimedStream.ToArray())
            {
                //Console.Write((char)c);
            }
#endif
        }
        private bool IsBlank(int c)
        {
            return c == '\n' || c == ' ' || c == '\t' || c == '\r';
        }

        #region ==== 初始化裁剪流 ====

        private void ReadTrimedStream(Stream stream)
        {
            int currentByte;
            while ((currentByte = stream.ReadByte()) != -1)
            {
                if (currentByte == '#')
                {
                    while ((currentByte = stream.ReadByte()) != '\n') ;
                }
                if (currentByte == '=')
                {
                    TrimedStream.WriteByte((byte)currentByte);
                    while (IsBlank(currentByte = stream.ReadByte())) ;
                    if (currentByte == -1)
                    {
                        throw new EndOfStreamException("No any value.");
                    }
                    TrimedStream.WriteByte((byte)currentByte);
                    while ((currentByte = stream.ReadByte()) != '\n')
                    {
                        if (currentByte == '\r' || currentByte == -1) { break; }
                        if (currentByte == '#')
                        {
                            while (stream.ReadByte() != '\n') ;
                            break;
                        }
                        TrimedStream.WriteByte((byte)currentByte);
                    }
                }
                else if (!IsBlank(currentByte))
                {
                    TrimedStream.WriteByte((byte)currentByte);
                }
                else
                {
                    TrimedStream.WriteByte((byte)' ');
                }
            }
            TrimedStream.Position = 0;
        }

        #endregion

        public bool Read()
        {
            int currentByte;
            while ((currentByte = TrimedStream.ReadByte()) == ' ') ;
            if (currentByte == -1) { return false; }
            if (currentByte == '}')
            {
                Name = StartElements.Pop();
                Type = ElementTypes.EndElement;
                Value = string.Empty;
                TabTimes = StartElements.Count;
                return true;
            }
            StringBuilder sb = new();
            TabTimes = StartElements.Count;
            do
            {
                sb.Append((char)currentByte);
            } while ((currentByte = TrimedStream.ReadByte()) != '=' && currentByte != '<' && currentByte != '>');
            if (sb.Length == 0) { throw new InvalidDataException("No Element was found."); }
            sb.Append((char)currentByte); // = or < or >
            Name = sb.ToString()[..^1].Trim();
            while ((currentByte = TrimedStream.ReadByte()) == ' ') ;
            if (currentByte == '{')
            {
                if (SearchForEqualSign())
                {
                    StartElements.Push(Name);
                    TrimedStream.ReadByte();
                    Type = ElementTypes.StartElement;
                    Value = string.Empty;
                    return true;
                }
                // enum condition
                Type = ElementTypes.Element;
                sb = new(sb.ToString().Substring(sb.Length - 1, 1));
                while ((currentByte = TrimedStream.ReadByte()) != '}')
                {
                    if (currentByte == ' ') 
                    {
                        while ((currentByte = TrimedStream.ReadByte()) == ' ') ;
                        if (currentByte == '}') { break; }
                        sb.Append('|');
                        continue; 
                    }
                    sb.Append((char)currentByte);
                }
                //TrimedStream.Position--;
                Value = sb.ToString();
                return true;
            }
            Type = ElementTypes.Element;
            //sb.Clear();
            sb = new(sb.ToString().Substring(sb.Length - 1, 1));
            // "... ..."
            if (currentByte == '\"')
            {
                while ((currentByte = TrimedStream.ReadByte()) != '\"')
                {
                    sb.Append((char)currentByte);
                }
            }
            else
            {
                if (currentByte == ' ')
                {
                    while ((currentByte = TrimedStream.ReadByte()) == ' ') ;
                }
                do
                {
                    sb.Append((char)currentByte);
                } while ((currentByte = TrimedStream.ReadByte()) != ' ' && currentByte != '}');
                if (currentByte == '}')
                {
                    TrimedStream.Position--;
                }
            }
            Value = sb.ToString();
            return true;
        }
        private bool SearchForEqualSign()
        {
            var streamPositionCache = TrimedStream.Position;
            int currentByte;
            bool result = false;
            while ((currentByte = TrimedStream.ReadByte()) != '}')
            {
                if (currentByte == '=' || currentByte == '<' || currentByte == '>') 
                {
                    result = true;
                    break;
                }
            }
            TrimedStream.Position = streamPositionCache;
            return result;
        }
    }
}
