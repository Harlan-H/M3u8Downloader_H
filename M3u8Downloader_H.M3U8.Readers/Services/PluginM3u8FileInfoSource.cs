using M3u8Downloader_H.M3U8.AttributeReaders;
using M3u8Downloader_H.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.Readers.Services
{
    internal class PluginM3u8FileInfoSource : M3u8FileInfoSource
    {
        private readonly IM3u8FileInfoService m3U8FileInfoService;

        public PluginM3u8FileInfoSource(IM3u8FileInfoService m3U8FileInfoService , HttpClient httpClient, IDictionary<string, IAttributeReader>? attributeReaders = null) : base(httpClient, attributeReaders)
        {
            this.m3U8FileInfoService = m3U8FileInfoService;
        }

        protected override HttpRequestMessage BeforeRequest(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers)
        {
            return m3U8FileInfoService.BeforeRequest(uri, headers);
        }

        protected override Stream PostRequest(Stream stream)
        {
            return m3U8FileInfoService.PostRequest(stream);
        }
    }
}
