using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Tool
{
    /// <summary>
    /// 数组字符串
    /// </summary>
    public class ArrayString
    {
        /// <summary>
        /// 元素分隔符
        /// </summary>
        public static char Splitter = ',';
        /// <summary>
        /// 将数组转换为单一字段
        /// </summary>
        /// <param name="elements">字符串数组</param>
        /// <returns></returns>
        public static string Writer(string[] elements)
        {
            var splitmark = Splitter + " ";
            var sb = new StringBuilder();
            foreach (var e in elements) 
            {
                sb.Append(e.ToString() + splitmark); 
            }
            var str = sb.ToString().Trim();
            if (str.EndsWith(","))
            { 
                str = str.Substring(0, str.Length - 1);
            }

            return str;
        }
        /// <summary>
        /// 将字段转换为字符串数组
        /// </summary>
        /// <param name="str">字段</param>
        /// <returns></returns>
        public static string[] Reader(string str)
        {
            return str.Split(Splitter).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }
    }
}
