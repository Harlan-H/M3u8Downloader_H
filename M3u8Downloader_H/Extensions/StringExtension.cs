using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace M3u8Downloader_H.Extensions
{
    public static class StringExtension
    {
        public static IEnumerable<KeyValuePair<string, string>>? ToDictionary(this string s)
        {
            IEnumerable<KeyValuePair<string, string>>? keyValuePairs = null;
            if (!string.IsNullOrWhiteSpace(s))
                keyValuePairs = s.Split(Environment.NewLine)
                                    .Select(h => h.Split(":", 2, StringSplitOptions.TrimEntries))
                                    .ToDictionary(r => r[0], r => r[1]);

            return keyValuePairs;
        }
    }
}
