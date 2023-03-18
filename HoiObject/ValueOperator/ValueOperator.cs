using HoiObject.HoiVariable;
using HoiObject.HoiVariable.ValueType;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HoiObject.ValueOperator
{
    public static class ValueOperator
    {
        (string, Color)[] Add<T>(this T obj, ) where T : IValueType
        {
            switch(obj.Tag)
            {
                case ValueType.Building:
                    return new (string, Color)[] {
                        ($"增加{value}个{name}", PositiveAdd ? Color.Green : Color.Red)
            };
            }
        }
    }
}
