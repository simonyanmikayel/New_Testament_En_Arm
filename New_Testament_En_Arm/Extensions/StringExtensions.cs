
using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Extensions
{
    /// <summary>
    /// Helpers for working with strings and string representations.
    /// </summary>
    public static class StringExtensions
    {
        public static string RepeatString(this string s, int n)
        {
            return new StringBuilder(s.Length * n)
                        .AppendJoin(s, new string[n + 1])
                        .ToString();
        }
    }
}
