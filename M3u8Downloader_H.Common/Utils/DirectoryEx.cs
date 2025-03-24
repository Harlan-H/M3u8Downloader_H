using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Common.Utils
{
    public static class DirectoryEx
    {
        public static void DeleteCache(string path)
        {
            DirectoryInfo directory = new(path);
            if (!directory.Exists) 
                return;

            directory.Delete(true);
        }
    }
}
