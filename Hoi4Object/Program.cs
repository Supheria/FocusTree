//public Dictionary<string, Attribute> AtrributeCatalog = new();
using Hoi4Object.IO;

while (true)
{
    FormatRawEffectSentence.Unformattable.Clear();
    Console.Clear();
    var sentences = GetSentences();
    FocusEffects effects = new();
    List<Sentence> sentencesList = new();
    foreach (string sentence in sentences)
    {
        if (FormatRawEffectSentence.Formatter(sentence, out Sentence? formatted))
        {
            sentencesList.Add(formatted);
            //XmlIO.SaveSentence("test.txt", formatted);
        }
    }
    effects.EffectGroups.Add(10, new());
    effects.EffectGroups.Add(12, sentencesList);
    XmlIO.SaveObject<FocusEffects>("test.txt", effects);
    Console.WriteLine("----- 无法格式化 -----");
    foreach (string str in FormatRawEffectSentence.Unformattable)
    {
        Console.WriteLine(str);
    }
    Console.ReadKey();
    var a = XmlIO.LoadObject<FocusEffects>("test.txt");
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