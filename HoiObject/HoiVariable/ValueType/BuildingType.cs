using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HoiObject.HoiVariable.ValueType
{
    public struct BuildingValue : IValueType
    {
        public bool PositiveAdd { get; private set; }
        public VariableTags Tag { get; init; }
        public BuildingValue(bool positiveAdd)
        {
            Tag = VariableTags.Building;
            PositiveAdd = positiveAdd;
        }
        public (string, Color)[] Add(uint value, string name)
        {
            return new (string, Color)[] { 
                ($"增加{value}个{name}", PositiveAdd ? Color.Green : Color.Red)
            };
        }
        public (string, Color)[] Sub(uint value, string name)
        {
            return new (string, Color)[]{ 
                ($"移除{value}个{name}", PositiveAdd ? Color.Red : Color.Green)
            };
        }
    }
}
