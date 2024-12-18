using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using M3u8Downloader_H.Exceptions;

namespace M3u8Downloader_H.Utils
{
    internal static class PathEx
    {
        //生成唯一hash值,只要地址不改变hash就不会变
        public static string GenerateFileNameWithoutExtension(Uri? uri, string? fileName)
        {
            string tmpFileName;
            if (!string.IsNullOrWhiteSpace(fileName))
                tmpFileName = EscapeFileName(fileName);
            else if (uri is not null)
                tmpFileName = Convert.ToHexString(MD5.HashData( Encoding.UTF8.GetBytes(uri.OriginalString)));
            else
                tmpFileName = Guid.NewGuid().ToString("N");
            return tmpFileName;
        }

        public static string EscapeFileName(string fileName) =>
        Path.GetInvalidFileNameChars().Append('.').Aggregate(fileName, (current, invalidChar) => current.Replace(invalidChar, '_'));
    }
}
