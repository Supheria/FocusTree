using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HoiObject
{
    public static  class Action
    {
        public static void Require<T>(this T obj) where T : IActionable
        {

        }
        public static void Remove<T>(this T obj) where T : IActionable
        {

        }
    }
}
