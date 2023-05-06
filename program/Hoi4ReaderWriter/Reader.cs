#define DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoi4ReaderWriter
{
    public class Hoi4Reader
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        public enum ElementTypes
        {
            Element,
            //StartElement,
            EndElement
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; private set; } = string.Empty;
        /// <summary>
        /// 节点类型
        /// </summary>
        public ElementTypes Type { get; private set; }
        /// <summary>
        /// 节点值（StartElement&EndElement类型节点值为 string.Empty)
        /// </summary>
        public string Value { get; private set; } = string.Empty;
        /// <summary>
        /// 记录节点由内外顺序
        /// </summary>
        Stack<string> StartElements = new();
        /// <summary>
        /// 测试用 - 当前节点缩进级别
        /// </summary>
        public int TabTimes { get; private set; } = 0;
        /// <summary>
        /// 裁剪流
        /// </summary>
        MemoryStream TrimedStream = new();

        #region ==== 初始化裁剪流 ====

        public Hoi4Reader(Stream rawStream)
        {
            ReadTrimedStream(rawStream);
#if DEBUG
            FileStream file = new("trimed stream.txt", FileMode.Create);
            file.Write(TrimedStream.ToArray());
#endif
        }
        private void ReadTrimedStream(Stream stream)
        {
            bool lastIsBlank = false;
            int currentByte;
            while ((currentByte = stream.ReadByte()) != -1)
            {
                if (currentByte == '#')
                {
                    while ((currentByte = stream.ReadByte()) != '\n') ;
                }
                if (currentByte == '\n' || currentByte == ' ' || currentByte == '\t' || currentByte == '\r')
                {
                    if (!lastIsBlank)
                    {
                        lastIsBlank = true;
                        TrimedStream.WriteByte((byte)' ');
                    }
                    continue;
                }
                TrimedStream.WriteByte((byte)currentByte);
                lastIsBlank = false;
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
                Type = ElementTypes.EndElement;
                Name = StartElements.Pop();
                Value = string.Empty;
                TabTimes = StartElements.Count;
                return true;
            }
            Type = ElementTypes.Element;
            TabTimes = StartElements.Count;
            StringBuilder sb = new();
            bool hasBlank = false;
            do
            {
                if (currentByte != ' ')
                {
                    if (currentByte == -1) { throw new InvalidDataException("No Element Name was found."); }
                    if (hasBlank) { throw new InvalidDataException("Element Name cannot contain blank."); }
                    sb.Append((char)currentByte);
                }
                else { hasBlank = true; }
            } while ((currentByte = TrimedStream.ReadByte()) != '=' && currentByte != '<' && currentByte != '>');
            if (sb.Length == 0) { throw new InvalidDataException("No Element Name was found."); }
            sb.Append((char)currentByte); // = or < or >
            Name = sb.ToString()[..^1].Trim();
            while ((currentByte = TrimedStream.ReadByte()) == ' ') ;
            if (currentByte == '{')
            {
                if (SearchForAssignmentWithinBrace())
                {
                    //Type = ElementTypes.StartElement;
                    StartElements.Push(Name);
                    TrimedStream.ReadByte();
                    Value = string.Empty;
                    return true;
                }
                // enum condition
                Type = ElementTypes.Element;
                sb = new(sb.ToString().Substring(sb.Length - 1, 1)); // new since = or < or >
                hasBlank = false;
                while ((currentByte = TrimedStream.ReadByte()) != '}')
                {
                    if (currentByte == ' ') 
                    {
                        hasBlank = true;
                        continue; 
                    }
                    if (hasBlank)
                    {
                        sb.Append(',');
                        hasBlank = false;
                    }
                    sb.Append((char)currentByte);
                }
                Value = sb.ToString();
                return true;
            }
            //Type = ElementTypes.Element;
            sb = new(sb.ToString().Substring(sb.Length - 1, 1)); // new since = or < or >
            // "... ..." or "UTF-8" condition
            if (currentByte == '\"')
            {
                var buffer = new byte[1024];
                int realLength = 0;
                while ((currentByte = TrimedStream.ReadByte()) != '\"')
                {
                    buffer[realLength++] = (byte)currentByte;
                }
                sb.Append(Encoding.UTF8.GetString(buffer, 0, realLength));
                Value = sb.ToString();
                return true;
            }
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
            Value = sb.ToString();
            return true;
        }
        /// <summary>
        /// 在花括号内预查找是否有赋值语句（ = or < or > ）
        /// </summary>
        /// <returns></returns>
        private bool SearchForAssignmentWithinBrace()
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
