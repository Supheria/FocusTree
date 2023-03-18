using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoiObject.HoiVariable.ValueType
{
    public interface IValueType
    {
        bool PositiveAdd { get; }
        VariableTags Tag { get; }
        /// <summary>
        /// 加运算
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        (string, Color)[] Add(uint value, string name);
        /// <summary>
        /// 减运算
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        (string, Color)[] Sub(uint value, string name);
    }
}