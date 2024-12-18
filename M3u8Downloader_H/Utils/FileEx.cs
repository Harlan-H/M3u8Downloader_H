using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using M3u8Downloader_H.Exceptions;
using Newtonsoft.Json.Linq;

namespace M3u8Downloader_H.Utils
{
    internal static class FileEx
    {
        private static readonly string[] formats = { "ts", "m4s", "mp4" };
        public static void EnsureFileNotExist(string filename)
        {
            foreach (string item in formats)
            {
                string filePath = filename + '.' + item;
                FileInfo fileInfo = new(filePath);
                if (fileInfo.Exists && fileInfo.Length > 0)
                {
                    throw new FileExistsException($"【{filename}】文件已经存在，请修改名称后再次尝试");
                }
            }
        }
    }
}
