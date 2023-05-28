using Root_Dir_Ini;
RootDir steamRoot = new RootDir("Steam", null);
var steamPath = steamRoot.GetDir();
if (steamPath == null) { return; }

string[] dirs = new string[]{
@"steamapps\common\Hearts of Iron IV\history\countries",
@"steamapps\workshop\content\394360\1100200730\history\countries"
};

var output = @"已使用的三字母国策名 - 包含主mod.md";
StreamWriter writer = new(output);
int num = 0;
foreach (var dir in dirs)
{
    var files = Directory.GetFiles(Path.Combine(steamPath,dir));

    writer.WriteLine($"\n# {dir}");

    foreach (var file in files)
    {
        writer.WriteLine(Path.GetFileNameWithoutExtension(file));
        num++;
    }
}

writer.Close();

Console.WriteLine($"{num} done. input any to exit.");
Console.Read();
