using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using M3u8Downloader_H.Exceptions;

namespace M3u8Downloader_H.Utils
{
    public static class PathEx
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

        public static void CreateDirectory(string dirPath, bool skipExist = true)
        {
            DirectoryInfo directoryInfo = new(dirPath);
            if(directoryInfo.Exists)
            {
                if (skipExist) return;

                throw new Exception("相同缓存目录存在下载线程停止\n如果你想继续下载可以通过修改设置中的相同缓存\n或者删除当前此条内容同时添加新的下载");
            }
            directoryInfo.Create();
        }

        public static string GetFileFullPathWithoutExtension(string path)
        {
            var baseFileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var baseDirPath = Path.GetDirectoryName(path);
            return Path.Combine(baseDirPath!, baseFileNameWithoutExtension);
        }
    }
}
