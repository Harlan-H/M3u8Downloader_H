using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace M3u8Downloader_H.Core.Utils.Extensions
{
    internal static class StringExtensions
    {
//         public static byte[] ToHexArray(this string s)
//         {
//             s = s.StartsWith("0x", true, null) ? s.Remove(0, 2) : s;
//             byte[] returnBytes = new byte[s.Length / 2];
//             for (int i = 0; i < returnBytes.Length; i++)
//             {
//                 returnBytes[i] = byte.Parse(s.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
//             }
// 
//             return returnBytes;
//         }

        public static string? NullIfWhiteSpace(this string s) =>
            !string.IsNullOrWhiteSpace(s)
                ? s
                : null;


        public static string GetMd5(this string s)
        {
            using SHA256 md5 = SHA256.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            return Convert.ToHexString(data);
        }

        public static byte[] ToHex(this string s)
        {
            s = s.StartsWith("0x", StringComparison.Ordinal) ? s[2..] : s;
            return Convert.FromHexString(s);
            //             byte[] returnBytes = new byte[s.Length / 2];
            //             for (int i = 0; i < returnBytes.Length; i++)
            //                 returnBytes[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
            //             return returnBytes;
        }
    }
}
