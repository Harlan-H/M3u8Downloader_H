using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Extensions
{
    internal static class UriExtension
    {
        public static string GetHostName(this Uri uri)
        {
            return uri.Host.Split('.')[^2];
        }


    }
}
