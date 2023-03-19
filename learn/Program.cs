using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Drawing;

namespace ReflectionTest
{
    class MyClass
    {
        public int a;
        public int b;
        MyClass(int x, int y)
        {
            a = x;
            b = y;
        }
        public static int ShowData(int x, string m)
        {
            Console.WriteLine(" x = {0}, m = {1}", x, m);
            return 100;
        }
    }
    public class Progam
    {
        static void Main(string[] args)
        {
            Type ty = typeof(MyClass);
            MethodInfo[] info = ty.GetMethods();
            foreach (MethodInfo func in info)
            {
                if (func.Name.Equals("ShowData", StringComparison.Ordinal))
                {
                    object[] paramArray = { 0, 0 };
                    ParameterInfo[] parmInfo = func.GetParameters();
                    foreach (ParameterInfo param in parmInfo)
                    {
                        Type paramType = param.ParameterType;

                        if (paramType.Name == "Int32")
                        {
                            paramArray[0] = Convert.ToInt32("10000000");
                        }
                        else if (paramType.Name == "String")
                        {
                            paramArray[1] = Convert.ToString("hello world ");
                        }
                    }
                    object result = func.Invoke(null, paramArray);
                    Console.WriteLine(" result = {0}", (int)result);
                }
            }
            var strs = new string[] { "A", "B", "C" };
            string a = $"test: {string.Join(", ", strs)}";
            var str = Color.Orange.Name;
            var color = Color.FromName(str);
            Console.ReadKey();
        }
    }
}
