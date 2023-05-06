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
            while (reader.Read())
            {

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
