
//#define test_single_file

using Hoi4ReaderWriter;
using System.Diagnostics;

#if test_single_file
var fileName = @"C:\Program Files\steam\steamapps\common\Hearts of Iron IV\interface\tutorialscreen.gui";
var fs = new FileStream(fileName, FileMode.Open);
Hoi4Reader reader = new(fs);
FileStream file = new(@"output.txt", FileMode.Create);
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
string Hoi4GameDir;
Console.Write("Input Hoi4 game root directory or enter for default: ");
var read = Console.ReadLine();
var DirIni = @"_dir.ini";
if (read != null && Directory.Exists(read))
{
    Hoi4GameDir = read;
}
else if (File.Exists(DirIni))
{
    StreamReader sr = new(new FileStream(DirIni, FileMode.Open));
    Hoi4GameDir = sr.ReadToEnd().Trim();
    sr.Close();
}
else
{
    Console.WriteLine("There is no default path. Input any to exit.");
    Console.ReadKey();
    return;
}
StreamWriter sw = new(new FileStream(DirIni, FileMode.Create));
sw.WriteLine(Hoi4GameDir);
sw.Close();

List<string> files = Directory.GetFiles(Hoi4GameDir, "*.txt", SearchOption.AllDirectories).ToList();
files.AddRange(Directory.GetFiles(Hoi4GameDir, "*.gfx", SearchOption.AllDirectories));
files.AddRange(Directory.GetFiles(Hoi4GameDir, "*.gui", SearchOption.AllDirectories));

var logPath = @"exception log.txt";
FileStream log = new(logPath, FileMode.Create);
StreamWriter logWriter = new(log);
logWriter.WriteLine(System.DateTime.Now.ToString("g"));
logWriter.WriteLine("Hoi4 Original Game Version 1.12.13");
logWriter.WriteLine();

int txtNumber, gfxNumber, guiNumber;
txtNumber = gfxNumber = guiNumber = 0;
for (int j = 0; j < files.Count; j++)
{
    var file = files[j];
    var ext = Path.GetExtension(file);
    if (ext == ".gfx")
    {
        gfxNumber++;
    }
    else if (ext == ".gui")
    {
        guiNumber++;
    }
    else { txtNumber++; }
    Console.Clear();
    Console.WriteLine(Hoi4GameDir + '\n');
    Console.WriteLine($"read {j + 1}/{files.Count} files");
    try
    {
        var fs = new FileStream(file, FileMode.Open);
        Hoi4Reader reader = new(fs);
        while (true)
        {
            try
            {
                if (!reader.Read()) { break; }
            }
            catch (Exception e)
            {
                fs.Close();
                throw new Exception(e.Message);
            }
        }
        fs.Close();
    }
    catch (Exception ex)
    {
        logWriter.WriteLine(file);
        logWriter.WriteLine(ex.Message);
        continue;
    }
}
logWriter.WriteLine($"\ntxt num {txtNumber}, gfx num {gfxNumber}, gui num {guiNumber}");
logWriter.Close();

using (Process pc = new())
{
    pc.StartInfo.FileName = "cmd.exe";

    pc.StartInfo.CreateNoWindow = true;
    pc.StartInfo.RedirectStandardError = true;

    pc.StartInfo.RedirectStandardInput = true;
    pc.StartInfo.RedirectStandardOutput = true;
    pc.StartInfo.UseShellExecute = false;
    pc.Start();

    pc.StandardInput.WriteLine("explorer " + $"\"{Directory.GetCurrentDirectory()}\\{logPath}\"");
    pc.StandardInput.WriteLine("exit");
    pc.StandardInput.AutoFlush = true;

    pc.WaitForExit();
    pc.Close();
}
#endif