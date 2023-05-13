using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoi4ReaderWriter
{
    public class FocusTree : IHoi4Serializable
    {
        public void ReadHoi4(Hoi4Reader reader)
        {
            Console.WriteLine("\nReadHoi4 in FocusTree!\n");
            FileStream file = new("FocusTree deserialize test.txt", FileMode.Create);
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
        }
    }

    public struct FocusTreeInfo
    {
        string Id = string.Empty;
        Country country = new();
        Point ContinuousFocusPosition = new();
        //Point 
        public FocusTreeInfo() { }
    }
    public interface IMtthOperatable
    {
        float Base { get; set; }
        float Add { get; set; }
        float Factor { get; set; }
    }
    public class Country : IMtthOperatable
    {
        public float Base { get; set; }
        public float Add { get; set; }
        public float Factor { get; set; }
        Modifier modifier { get; set; }
    }
    public class Modifier : IMtthOperatable
    {
        public float Base { get; set; }
        public float Add { get; set; }
        public float Factor { get; set; }
        public Trigger[] triggers { get; set; }
    }
    public class Trigger
    {

    }
}
