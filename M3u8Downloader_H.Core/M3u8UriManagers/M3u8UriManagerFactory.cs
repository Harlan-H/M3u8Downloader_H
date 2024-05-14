using M3u8Downloader_H.Plugin;
using System.Collections.Generic;
using System.Net.Http;

namespace M3u8Downloader_H.Core.M3u8UriManagers
{
    internal class M3u8UriManagerFactory
    {
        public static IM3u8UriManager CreateM3u8UriManager(IM3u8UriProvider? iM3U8UriProvider,IEnumerable<KeyValuePair<string, string>>? headers)
        {
            if (iM3U8UriProvider is not null)
                return new PluginM3u8UriManager(iM3U8UriProvider,  headers);
            else
                return new M3u8UriManager();

        }
    }
}
