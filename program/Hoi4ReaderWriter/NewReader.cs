using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoi4ReaderWriter
{
    public class NewReader
    {
        int Index = 0;
        Node Root = new();
        public NewReader(ReadOnlySpan<char> stream)
        {
            GetNode(Root, ref Index, stream);
        }
        void GetNode(Node parent, ref int index, ReadOnlySpan<char> str)
        {
            var name = new StringBuilder();
            // 循环读取
            while (index < str.Length)
            {
                char c = str[index++];
                switch (c)
                {
                    // 添加字符
                    default: name.Append(c); break;

                    // 忽略空格或换行
                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t':
                        break;

                    // 如果是等号就准备赋值
                    case '=':
                        var doAssign = true;
                        var value = new StringBuilder();

                        // 去掉等号后分隔符
                        while (str[index] == ' ' || str[index] == '\t' || str[index] == '\r' || str[index] == '\n') { index++; }

                        // 读取到分号之后的有效字符
                        while (index < str.Length && doAssign)
                        {
                            c = str[index++];

                            switch (c)
                            {
                                // 如果等号后是变量就循环添加直到中断
                                default:
                                    value.Append(c); break;

                                // 如果赋值的值后遇到分隔符就完成赋值
                                case ' ':
                                case '\n':
                                case '\r':
                                case '\t':
                                    parent.Attributes.TryAdd(name.ToString(), value.ToString());
                                    name.Clear(); value.Clear();

                                    doAssign = false;
                                    break;

                                // 如果赋值后是 左大括号 就递归
                                case '{':

                                    var child = new Node();
                                    child.Name = name.ToString();
                                    name.Clear();

                                    parent.Children.Add(child);
                                    GetNode(child, ref index, str);

                                    doAssign = false;
                                    break;

                                // 如果赋值后是 " 就循环读取内部所有字符串
                                case '\"':
                                    while (index < str.Length && str[index] != '\"') // 跳过 " 字符本身
                                    {
                                        value.Append(str[index++]);
                                    }
                                    index++; // 这里要跳过结束的 " 不然会被遗留
                                    parent.Attributes.Add(name.ToString(), value.ToString());
                                    name.Clear(); value.Clear();

                                    doAssign = false;
                                    break;
                            }
                        }
                        break;

                    // 对于 左大括号 的收敛
                    case '}':
                        if (name.Length > 0) { parent.Value = name.ToString(); }
                        return;
                }
            }
        }
    }
    class Node
    {
        public string Name = "";
        public List<Node> Children = new();
        public Dictionary<string, string> Attributes = new();
        public string? Value;
    }
}
