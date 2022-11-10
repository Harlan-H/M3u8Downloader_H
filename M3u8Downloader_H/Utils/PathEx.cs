using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using M3u8Downloader_H.Exceptions;

namespace M3u8Downloader_H.Utils
{
    internal static class PathEx
    {

        public static string GenerateFileNameWithoutExtension(string? fileName)
        {
            string tmpFileName;
            if (string.IsNullOrWhiteSpace(fileName))
                tmpFileName = Guid.NewGuid().ToString("N");
            else
                tmpFileName = EscapeFileName(fileName);
            return tmpFileName;
        }

        public static string EscapeFileName(string fileName) =>
        Path.GetInvalidFileNameChars().Append('.').Aggregate(fileName, (current, invalidChar) => current.Replace(invalidChar, '_'));
    }
}
