// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;

[DllImport("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\x64\\Debug\\Hoi4 Parser.dll", EntryPoint = "parse", SetLastError = true)]
static extern void parse(char[] path);

try
{
    string a = "test";
    var c = a.ToCharArray();
    parse(c);
}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString());
}
Console.Read();