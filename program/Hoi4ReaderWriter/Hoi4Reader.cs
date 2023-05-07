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
        /// 裁剪好的字节缓冲区
        /// </summary>
        readonly byte[] TrimedBuffer;
        /// <summary>
        /// 当前所在缓冲区位置
        /// </summary>
        int BufferPosition = 0;

        #region ==== 初始化裁剪缓冲区 ====

        public Hoi4Reader(Stream rawStream)
        {
            var buffer = ReadTrimedBuffer(rawStream);
            var length = buffer.Length;
            bool isBOM = length > 3 && buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF; // has UTF-8 BOM head
            length = isBOM ? length - 3 : length;
            TrimedBuffer = new byte[length];
            for (int i = 0; i < length; i++)
            {
                TrimedBuffer[i] = buffer[isBOM ? i + 3 : i];
            }
#if DEBUG
            FileStream file = new(@"C:\Users\Non_E\Documents\GitHub\FocusTree\FocusTree\modding analysis\test\trimed stream.txt", FileMode.Create);
            file.Write(TrimedBuffer.ToArray());
#endif
        }
        private byte[] ReadTrimedBuffer(Stream stream)
        {
            MemoryStream buffer = new();
            bool lastByteIsBlank = false; 
            int currentByte;
            while ((currentByte = stream.ReadByte()) != -1)
            {
                if (currentByte == '#')
                {
                    while ((currentByte = stream.ReadByte()) != '\n') ;
                }
                if (currentByte == '\n' || currentByte == ' ' || currentByte == '\t' || currentByte == '\r')
                {
                    if (!lastByteIsBlank)
                    {
                        lastByteIsBlank = true;
                        buffer.WriteByte((byte)' ');
                    }
                    continue;
                }
                buffer.WriteByte((byte)currentByte);
                lastByteIsBlank = false;
            }
            buffer.WriteByte((byte)' ');
            return buffer.ToArray();
        }
        
        #endregion

        /// <summary>
        /// 逐节点向下读取
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public bool Read()
        {
            while (TrimedBuffer[BufferPosition] == ' ')
            {
                if (++BufferPosition >= TrimedBuffer.Length) { return false; }
            }
            if (TrimedBuffer[BufferPosition] == '}')
            {
                BufferPosition++;
                Type = ElementTypes.EndElement;
                Name = StartElements.Pop();
                Value = string.Empty;
                TabTimes = StartElements.Count;
            }
            else
            {
                Type = ElementTypes.Element;
                TabTimes = StartElements.Count;
                var sign = ReadName();

                if (TrimedBuffer[BufferPosition] == '{')
                {
                    BufferPosition++;
                    if (SearchForAssignmentWithinBrace())
                    {
                        StartElements.Push(Name);
                        Value = string.Empty;
                    }
                    else { ReadBraceAssignment(sign); }
                }
                else { ReadAssignment(sign); }
            }
            return true;
        }
        /// <summary>
        /// 读取节点名称
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        private byte ReadName()
        {
            StringBuilder sb = new();
            bool hasBlank = false;
            while (TrimedBuffer[BufferPosition] != '=' && TrimedBuffer[BufferPosition] != '<' && TrimedBuffer[BufferPosition] != '>')
            {
                if (TrimedBuffer[BufferPosition] != ' ')
                {
                    if (hasBlank) { throw new InvalidDataException("Element name cannot contain blank."); }
                    sb.Append((char)TrimedBuffer[BufferPosition]);
                }
                else { hasBlank = true; }
                if (++BufferPosition >= TrimedBuffer.Length) { throw new InvalidDataException("Unexpected end of assignment."); }
            }
            if (sb.Length == 0) { throw new InvalidDataException("No Element Name was found."); }
            var sign = TrimedBuffer[BufferPosition];
            do
            {
                if (++BufferPosition >= TrimedBuffer.Length) { throw new InvalidDataException("Unexpected end of assignment."); }
            } while (TrimedBuffer[BufferPosition] == ' ');
            Name = sb.ToString();
            return sign;
        }
        /// <summary>
        /// 在花括号内预查找是否有赋值语句（ = or < or > ）
        /// </summary>
        /// <returns></returns>
        private bool SearchForAssignmentWithinBrace()
        {
            var tempPostion = BufferPosition;
            bool result = false;
            while (TrimedBuffer[tempPostion] != '}')
            {
                if (TrimedBuffer[tempPostion] == '=' || TrimedBuffer[tempPostion] == '<' || TrimedBuffer[tempPostion] == '>')
                {
                    result = true;
                    break;
                }
                if (++tempPostion >= TrimedBuffer.Length) { throw new InvalidDataException("Unexpected end of brace."); }
            }
            return result;
        }
        /// <summary>
        /// 读取花括号赋值节点（枚举组合赋值 或 赋空值）
        /// </summary>
        private void ReadBraceAssignment(byte assignmentSign)
        {
            StringBuilder sb = new(((char)assignmentSign).ToString()); // = or < or >
            bool hasBlank = false;
            while (TrimedBuffer[BufferPosition] != '}')
            {
                if (TrimedBuffer[BufferPosition] == ' ')
                {
                    hasBlank = true;
                    BufferPosition++;
                    continue;
                }
                if (hasBlank)
                {
                    sb.Append(',');
                    hasBlank = false;
                }
                sb.Append((char)TrimedBuffer[BufferPosition]);
                BufferPosition++;
            }
            BufferPosition++;
            Value = sb.ToString();
        }
        /// <summary>
        /// 读取赋值节点（ = or < or > ）
        /// </summary>
        /// <param name="assignmentSign"></param>
        /// <exception cref="InvalidDataException"></exception>
        private void ReadAssignment(byte assignmentSign)
        {
            StringBuilder sb = new(((char)assignmentSign).ToString()); // = or < or >
            if (TrimedBuffer[BufferPosition] == '\"')
            {
                // "... ..." or "UTF-8" condition
                if (++BufferPosition >= TrimedBuffer.Length) { throw new InvalidDataException("Unexpected end of quote."); }
                int startPos = BufferPosition;
                int length = 0;
                bool doTransfer = false;
                while (TrimedBuffer[BufferPosition] != '\"' || (doTransfer && TrimedBuffer[BufferPosition] == '\"')) 
                {
                    length++;
                    if (doTransfer) { doTransfer = false; }
                    if (TrimedBuffer[BufferPosition] == '\\') { doTransfer = true; }
                    if (++BufferPosition >= TrimedBuffer.Length) { throw new InvalidDataException("Unexpected end of quote."); }
                }
                BufferPosition++;
                sb.Append(Encoding.UTF8.GetString(TrimedBuffer, startPos, length));
                Value = sb.ToString();
            }
            else
            {
                while (TrimedBuffer[BufferPosition] != ' ' && TrimedBuffer[BufferPosition] != '}')
                {
                    sb.Append((char)TrimedBuffer[BufferPosition++]);
                }
                Value = sb.ToString();
            }
        }
    }
}
