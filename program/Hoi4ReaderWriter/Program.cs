using Hoi4ReaderWriter;
using System.Text;

var fs = new FileStream("bulgaria.txt", FileMode.Open);
Hoi4Reader reader = new(fs);
FileStream file = new("output.txt", FileMode.Create);
StreamWriter writer = new(file);
while (reader.Read())
{
    if (reader.Type == Hoi4Reader.ElementTypes.EndElement) { continue; }
    StringBuilder sb = new();
    for (int i = 0; i < reader.TabTimes; i++)
    {
        sb.Append('\t');
    }
    sb.Append(reader.Name);
    sb.Append(reader.Value);
    writer.Write(sb.ToString() + '\n');
    writer.Flush();
}