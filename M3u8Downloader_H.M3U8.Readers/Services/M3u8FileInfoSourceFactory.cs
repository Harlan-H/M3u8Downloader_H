using M3u8Downloader_H.M3U8.AttributeReaders;
using M3u8Downloader_H.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.Readers.Services
{
    public class M3u8FileInfoSourceFactory
    {
        public static IM3u8FileInfoSource CreateM3u8FileInfoSoucr(IM3u8FileInfoService? m3U8FileInfoService, HttpClient httpClient, IDictionary<string, IAttributeReader>? attributeReaders = null)
        {
            if (m3U8FileInfoService is not null)
                return new PluginM3u8FileInfoSource(m3U8FileInfoService, httpClient, attributeReaders);
            else
                return new M3u8FileInfoSource(httpClient, attributeReaders);
        }
    }
}
