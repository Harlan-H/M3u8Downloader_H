using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using M3u8Downloader_H.Exceptions;

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

        public static string GetMd5(this string s)
        {
            using SHA256 md5 = SHA256.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(s));

            StringBuilder sb = new();
            for (int i = 0; i < data.Length; i++)
                sb.Append(data[i].ToString("X2"));

            return sb.ToString();
        }

        public static byte[] ToHex(this string s)
        {
            s = s.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? s[2..] : s;
            return Convert.FromHexString(s);
        }

    }
}
