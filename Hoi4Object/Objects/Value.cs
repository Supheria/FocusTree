using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoi4Object.Objects
{
    internal class Value : StateAtrribute
    {
        bool PositiveAdd;
        string[] StringColors = new string[]
        {
            Color.Black.Name,
            Color.Green.Name,
            Color.Orange.Name,
            Color.Red.Name
        };
        public static string _CHANGE_VALUE_NULL_ = "/CHANGE_VALUE_NULL/";
        /// <summary>
        /// 修改值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="hasPercent"></param>
        /// <returns></returns>
        public virtual string[] EditValue(string value, bool hasPercent)
        {
            value = hasPercent ? value + "%" : value;
            if (int.Parse(value) > 0)
            {
                return new string[]
                {
                StringColors[0],
                $"{Name}{_NAME_VALUE_SPLITTER_}",
                PositiveAdd ? StringColors[1] : StringColors[2],
                value
                };
            }
            else if (int.Parse(value) < 0)
            {
                return new string[]
                {
                StringColors[0],
                $"{Name}{_NAME_VALUE_SPLITTER_}",
                PositiveAdd ? StringColors[1] : StringColors[2],
                value
                };
            }
            return new string[]
            {
            StringColors[0],
            Name,
            StringColors[3],
            _CHANGE_VALUE_NULL_
            };
        }
    }
}
