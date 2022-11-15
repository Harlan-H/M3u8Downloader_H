using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Plugin.Extensions
{
    internal static class StringExtension
    {
        public static string Normalize(this string s, string pattern)
        {
            return Regex.Match(s, pattern).Groups[1].Value;
        }
    }
}
