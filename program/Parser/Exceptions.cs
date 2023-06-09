using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Parser
{
    public class Exceptions
    {
        static string LogPath { get; set; } = "ex.log";
        static Exceptions()
        {
            using var file = new FileStream(LogPath, FileMode.Create);
            using var logger = new StreamWriter(file);
            logger.Close();
        }
        private static void Append(string message)
        {
            if(!File.Exists(LogPath)) { return; }
            using var file = new FileStream(LogPath, FileMode.Append);
            using var logger = new StreamWriter(file);
            logger.WriteLine(message);
            logger.Close();
        }
        public static void UnknownError(Element element)
        {
            Append($"unknown error at line({element.Line}), column({element.Column})");
        }
        public static void UnexpectedKey(Element element) 
        {
            Append($"unexpected key at line({element.Line}), column({element.Column})");
        }
        public static void UnexpectedOperator(Element element) 
        {
            Append($"unexpected operator at line({element.Line}), column({element.Column})");
        }
        public static void UnexpectedValue(Element element)
        {
            Append($"unexpected value at line({element.Line}), column({element.Column})");
        }
        public static void UnexpectedArrayType(Element element)
        {
            Append($"unexpected array type at line({element.Line}), column({element.Column})");
        }
        public static void UnexpectedArraySyntax(Element element) 
        {
            Append($"unexpected array syntax at line({element.Line}), column({element.Column})");
        }
        public static void Exception(string message) 
        {
            Append(message);
        }
    }
}
