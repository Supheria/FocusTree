using Hoi4ReaderWriter;
using System.Text;

//var fs = new FileStream("bulgaria.txt", FileMode.Open);
//Hoi4Reader reader = new(fs);
//FileStream file = new("output.txt", FileMode.Create);
//StreamWriter writer = new(file);
//while (reader.Read())
//{
//    if (reader.Type == Hoi4Reader.ElementTypes.EndElement) { continue; }
//    StringBuilder sb = new();
//    for (int i = 0; i < reader.TabTimes; i++)
//    {
//        sb.Append('\t');
//    }
//    sb.Append(reader.Name);
//    sb.Append(reader.Value);
//    writer.Write(sb.ToString() + '\n');
//    writer.Flush();
//}

var file = File.ReadAllText(@"trimed stream.txt");

Node root = new Node();
int i = 0;
GetNode(root, ref i, file.AsSpan());

Console.WriteLine();

void GetNode(Node parent, ref int index, ReadOnlySpan<char> str)
{
    var sb = new StringBuilder();
    // 循环读取
    while (index < str.Length)
    {
        char c = str[index++];
        switch (c)
        {
            // 添加字符
            default: sb.Append(c); break;

            // 忽略空格或换行
            case ' ':
            case '\n':
            case '\r':
            case '\t':
                break;

            // 如果是等号就准备赋值
            case '=':
                var w2 = true;
                var sb2 = new StringBuilder();

                // 去掉等号后分隔符
                while (str[index] == ' ' || str[index] == '\t' || str[index] == '\r' || str[index] == '\n') { index++; }

                // 读取到分号之后的有效字符
                while (index < str.Length && w2)
                {
                    c = str[index++];

                    switch (c)
                    {
                        // 如果等号后是变量就循环添加直到中断
                        default:
                            sb2.Append(c); break;

                        // 如果赋值的值后遇到分隔符就完成赋值
                        case ' ':
                        case '\n':
                        case '\r':
                        case '\t':
                            parent.Attributes.TryAdd(sb.ToString(), sb2.ToString());
                            sb.Clear(); sb2.Clear();

                            w2 = false;
                            break;

                        // 如果赋值后是 左大括号 就递归
                        case '{':

                            var child = new Node();
                            child.Name = sb.ToString();
                            sb.Clear();

                            parent.Children.Add(child);
                            GetNode(child, ref index, str);

                            w2 = false;
                            break;

                        // 如果赋值后是 " 就循环读取内部所有字符串
                        case '\"':
                            while (index < str.Length && str[index] != '\"') // 跳过 " 字符本身
                            {
                                sb2.Append(str[index++]);
                            }
                            index++; // 这里要跳过结束的 " 不然会被遗留
                            parent.Attributes.Add(sb.ToString(), sb2.ToString());
                            sb.Clear(); sb2.Clear();

                            w2 = false;
                            break;
                    }
                }
                break;

            // 对于 左大括号 的收敛
            case '}':
                if (sb.Length > 0) { parent.Value = sb.ToString(); }
                return;
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