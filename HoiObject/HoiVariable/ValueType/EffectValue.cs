using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoiObject.HoiVariable.ValueType
{
    internal struct EffectValue : IValueType
    {

        public Dictionary<string, IValueType> Value { get; private set; }
        public EffectValue(Dictionary<string, IValueType> value) 
        {
            Value = value; 
        }
        public string[] Add(IValueType IOther)
        {
            foreach (var item in Value)
            {

            }
        }
        public string[] Sub(IValueType IOhter)
        {

        }
    }
}
