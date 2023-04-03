//public Dictionary<string, Attribute> AtrributeCatalog = new();
using Hoi4Object.IO;

while (true)
{
    FormatRawEffectSentence.Unformattable.Clear();
    Console.Clear();
    var sentences = GetSentences();
    foreach (string sentence in sentences)
    {
        var result = FormatRawEffectSentence.Formatter(sentence, out string formatted);
        if (result == true)
        {
            Console.WriteLine($"{formatted} <= {sentence}");
        }
    }
    Console.WriteLine();
    Console.WriteLine("----- 无法格式化 -----");
    foreach (string str in FormatRawEffectSentence.Unformattable)
    {
        Console.WriteLine(str);
    }
    Console.ReadKey();
}

string[] GetSentences()
{
    List<string> result = new();
    FileStream file = new("sentences.txt", FileMode.Open);
    StreamReader reader = new StreamReader(file);
    while (reader.Peek() != -1) 
    {
        result.Add(reader.ReadLine());
    }
    reader.Close();
    file.Close();
    return result.ToArray();
}