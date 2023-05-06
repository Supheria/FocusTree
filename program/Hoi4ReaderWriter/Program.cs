// See https://aka.ms/new-console-template for more information
//#define local_tab_timer
using Hoi4ReaderWriter;
using System.Text;

Console.WriteLine("Hello, World!");

#if local_tab_timer
var fs = new FileStream("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\FILES\\test reader writer\\bulgaria.txt", FileMode.Open);
Reader reader = new(fs);
FileStream file = new("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\FILES\\test reader writer\\local tab.txt", FileMode.Create);
StreamWriter writer = new(file);
int TabTimes = 0;
while (reader.Read())
{
    if (reader.Type == Reader.ElementTypes.EndElement) { TabTimes--; continue; }
    StringBuilder sb = new();
    for (int i = 0; i < TabTimes; i++)
    {
        sb.Append('\t');
    }
    sb.Append(reader.Name);
    writer.Write(sb.ToString() + '\n');
    writer.Flush();
    if (reader.Type == Reader.ElementTypes.StartElement) { TabTimes++; }
}
#else
var fs = new FileStream("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\FILES\\test reader writer\\bulgaria.txt", FileMode.Open);
Reader reader = new(fs);
FileStream file = new("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\FILES\\test reader writer\\out put.txt", FileMode.Create);
StreamWriter writer = new(file);
while (reader.Read())
{
    if (reader.Type == Reader.ElementTypes.EndElement) { continue; }
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
#endif
//fs.Close();
//fs = new FileStream("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\FILES\\test reader writer\\bulgaria.txt", FileMode.Open);
//file = new("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\FILES\\test reader writer\\tab test.txt", FileMode.Create);
//writer = new(file);
//int TabTimes = 0;
//reader = new(fs);
//while (reader.Read())
//{
//    if (reader.Type == Reader.ElementTypes.EndElement) { TabTimes--; continue; }
//    StringBuilder sb = new();
//    for (int i = 0; i < TabTimes; i++)
//    {
//        sb.Append('\t');
//    }
//    sb.Append(reader.Name);
//    writer.Write(sb.ToString() + '\n');
//    writer.Flush();
//    if (reader.Type == Reader.ElementTypes.StartElement) { TabTimes++; }
//    var a = sb.ToString();
//}
//MemoryStream TrimedStream = new();

//int currentByte;
//while ((currentByte = fs.ReadByte()) != -1)
//{
//    if (currentByte == '#')
//    {
//        while ((currentByte = fs.ReadByte()) != '\n') ;
//    }
//    if (currentByte == '=')
//    {
//        TrimedStream.WriteByte((byte)currentByte);
//        while (IsBlank(currentByte = fs.ReadByte())) ;
//        if (currentByte == -1)
//        {
//            throw new EndOfStreamException();
//        }
//        TrimedStream.WriteByte((byte)currentByte);
//        while (!IsBlank(currentByte = fs.ReadByte()))
//        {
//            TrimedStream.WriteByte((byte)currentByte);
//        }
//        TrimedStream.WriteByte((byte)'\n');
//    }
//    else if (!IsBlank(currentByte))
//    {
//        TrimedStream.WriteByte((byte)currentByte);
//    }
//}

//FileStream file = new("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\FILES\\test reader writer\\out put.txt", FileMode.Create);
//file.Write(TrimedStream.ToArray());
//bool IsBlank(int c)
//{
//    return c == '\n' || c == ' ' || c == '\t' || c == '\r';
//}