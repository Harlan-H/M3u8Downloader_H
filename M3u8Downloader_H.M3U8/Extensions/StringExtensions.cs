using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace M3u8Downloader_H.M3U8.Extensions
{
    public static class StringExtensions
    {
        public static string GetMd5(this string s)
        {
            using SHA256 md5 = SHA256.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            return Convert.ToHexString(data);
        }

        public static byte[] ToHex(this string s)
        {
            s = s.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? s[2..] : s;
            return Convert.FromHexString(s);
        }
    }
}
