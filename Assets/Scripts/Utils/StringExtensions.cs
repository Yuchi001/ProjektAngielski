using System.Text.RegularExpressions;

namespace Utils
{
    public static class StringExtensions
    {
        public static string SplitCamelCase(this string current)
        {
            return Regex.Replace(current, "(\\B[A-Z])", " $1");
        }
    }
}