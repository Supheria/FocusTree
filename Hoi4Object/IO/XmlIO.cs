using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Hoi4Object.IO
{
    internal class XmlIO
    {
        public static void SaveSentence(string path, Sentence sentence)
        {
            try
            {
                var file = File.Create(path);
                var writer = new XmlSerializer(typeof(Sentence));
                writer.Serialize(file, sentence);
                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[2303051316]无法保存文件。\n{ex.Message}");
            }
        }
        public static Sentence? LoadGraph(string path)
        {
            try
            {
                var file = File.OpenRead(path);
                var reader = new XmlSerializer(typeof(Sentence));
                var graph = reader.Deserialize(file) as Sentence;
                file.Close();
                return graph;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[2303051302]不受支持的文件。\n{ex.Message}");
                return null;
            }
        }
    }
}
