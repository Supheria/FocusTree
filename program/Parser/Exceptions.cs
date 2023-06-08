using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class Exceptions
    {
        public static void UnknownError(Element element) { }
        public static void UnexpectedKey(Element element) { }
        public static void UnexpectedOperator(Element element) { }
        public static void UnexpectedValue(Element element) { }
        public static void UnexpectedArrayType(Element element) { }
        public static void UnexpectedArraySyntax(Element element) { }
        public static void Exception(string message) { }
    }
}
