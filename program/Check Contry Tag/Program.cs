using Root_Dir_Ini;
RootDir steamRoot = new RootDir("Steam", null);
var steamPath = steamRoot.GetDir();
if (steamPath == null) { return; }

Console.WriteLine("intput three letters to check: ");
string? toCheck = Console.ReadLine();
while (toCheck != null && toCheck != string.Empty)
{
    Check(toCheck);
    toCheck = Console.ReadLine();
}
void Check(string toCheck)
{
    string[] dirs = new string[]{
        @"steamapps\common\Hearts of Iron IV\common\country_tags",
        @"steamapps\workshop\content\394360\1100200730\history\countries",
    };
    var output = @"checked_tags.md";
    StreamWriter writer = new(output);
    int num = 0;

}



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
