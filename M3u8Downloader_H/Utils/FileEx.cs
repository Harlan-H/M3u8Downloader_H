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
        public static void EnsureFileNotExist(string filefullpath)
        {
            FileInfo fileInfo = new(filefullpath);
            if (fileInfo.Exists && fileInfo.Length > 0)
            {
                throw new FileExistsException($"【{filefullpath}】文件已经存在，请修改名称后再次尝试");
            }
            
        }
    }
}
