
using System.Reflection;

namespace Test_Cs
{
    internal class Helper
    {
        public static Type? MakeType(string classNamePath)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            return asm.GetType(classNamePath);
        }
        public static string MakeClassName(string value)
        {
            return char.ToUpper(value[0]) + value.Substring(1);
        }
    }
}
