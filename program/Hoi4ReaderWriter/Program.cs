#define test_single_file
using Hoi4ReaderWriter;
using System.Text;

var b = new byte[]{17};
var a = Encoding.Unicode.GetBytes("1");

#if test_single_file
var fs = new FileStream(@"C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\modding analysis\\test\\germany.txt", FileMode.Open);
//Hoi4Serializer serializer = new(typeof(FocusTree));
//var tree = serializer.Deserialize(fs);
Hoi4Reader reader = new(fs);
FileStream file = new(@"C:\Users\Non_E\Documents\GitHub\FocusTree\FocusTree\modding analysis\test\output.txt", FileMode.Create);
StreamWriter writer = new(file);
try
{
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
    Console.WriteLine("done");
}
catch(Exception ex)
{
    Console.WriteLine("EXCEPTION: " + ex.Message);
}
#else
string[] dirs = new string[]{
    //@"C:\Program Files\steam\steamapps\common\Hearts of Iron IV\common",
    //@"C:\Program Files\steam\steamapps\common\Hearts of Iron IV\interface",
    //@"C:\Program Files\steam\steamapps\common\Hearts of Iron IV\events",
    @"C:\Program Files\steam\steamapps\common\Hearts of Iron IV\",
};

FileStream log = new(@"C:\Users\Non_E\Documents\GitHub\FocusTree\FocusTree\modding analysis\test\exception log.txt", FileMode.Create);
StreamWriter logWriter = new(log);
int txtNumber, gfxNumber;
txtNumber = gfxNumber = 0;
for(int i = 0; i < dirs.Length; i++)
{
    var dir = dirs[i];
    List<string> files = Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories).ToList();
    //List<string> files = Directory.GetFiles(dir, "*.gfx", SearchOption.AllDirectories).ToList();
    files.AddRange(Directory.GetFiles(dir, "*.gfx", SearchOption.AllDirectories));
    //fileTotalNumber += files.Count;
    //Console.WriteLine($"{i}/{dirs.Length} dir has done.");
    for (int j = 0; j < files.Count; j++)
    {
        var file = files[j];
        var ext = Path.GetExtension(file);
        if (ext == ".gfx")
        {
            gfxNumber++;
        }
        else { txtNumber++; }
        Console.Clear();
        Console.WriteLine($"read {j + 1}/{files.Count} files in dir {i + 1}/{dirs.Length}");
        try
        {
            ReadTest(file);
        }
        catch (Exception ex)
        {
            // md pattern
            //logWriter.WriteLine($">> {Path.GetDirectoryName(file)}\\\\**{Path.GetFileName(file)}**");
            //logWriter.WriteLine(">> *" + ex.Message + "*");
            logWriter.WriteLine(file);
            logWriter.WriteLine(ex.Message);
            continue;
        }
    }
}
logWriter.WriteLine($"\ntxt num {txtNumber}, gfx num {gfxNumber}");
Console.WriteLine("done.");
logWriter.Close();

void ReadTest(string filename)
{
    var fs = new FileStream(filename, FileMode.Open);
    //Hoi4Serializer serializer = new(typeof(FocusTree));
    //var tree = serializer.Deserialize(fs);
    Hoi4Reader reader = new(fs);
    //FileStream file = new(@"C:\Users\Non_E\Documents\GitHub\FocusTree\FocusTree\modding analysis\test\output.txt", FileMode.Create);
    //StreamWriter writer = new(file);
    while (true)
    {
        try
        {
            if(!reader.Read())
            {
                break;
            }
        }
        catch (Exception e)
        {
            fs.Close();
            throw new Exception(e.Message);
        }
    }
    //file.Close();
    fs.Close();
}
#endif
Console.WriteLine();