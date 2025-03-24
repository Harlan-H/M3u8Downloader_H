using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace M3u8Downloader_H.Utils
{
    internal static class PathEx
    {
        //生成唯一hash值,只要地址不改变hash就不会变
        public static string GenerateFileNameWithoutExtension(Uri? uri)
        {
            if (uri is not null)
                return Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(uri.OriginalString)));
            return string.Empty;
        }

        public static string EscapeFileName(string fileName) =>
        Path.GetInvalidFileNameChars().Append('.').Aggregate(fileName, (current, invalidChar) => current.Replace(invalidChar, '_'));
    }
}
