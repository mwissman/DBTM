using System;
using System.Text.RegularExpressions;

namespace Tests.Domain
{
    public static class StringExtensions
    {
        public static string FixNewlines(this string stringToFix)
        {
            return Regex.Replace(stringToFix, @"\r\n?|\n", Environment.NewLine);
        }
    }
}