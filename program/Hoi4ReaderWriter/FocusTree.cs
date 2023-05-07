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
    public class TriggerableBlock
    {

    }
    public class MTTH_Block
    {
        protected float Base;
        protected float Add;
        protected float Factor;
    }
    public class Country : MTTH_Block
    {
        
    }
    public class Modifier
    {

    }
}
