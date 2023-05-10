using Hoi4ReaderWriter;
using System.Text;

var fs = new FileStream(@"C:\Users\Non_E\Documents\GitHub\FocusTree\FocusTree\modding analysis\test\00_on_actions.txt", FileMode.Open);
//Hoi4Serializer serializer = new(typeof(FocusTree));
//var tree = serializer.Deserialize(fs);
Hoi4Reader reader = new(fs);
FileStream file = new(@"C:\Users\Non_E\Documents\GitHub\FocusTree\FocusTree\modding analysis\test\output.txt", FileMode.Create);
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
Console.WriteLine();