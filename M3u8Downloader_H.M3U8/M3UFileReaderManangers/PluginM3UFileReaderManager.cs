using M3u8Downloader_H.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.M3UFileReaderManangers
{
    internal class PluginM3UFileReaderManager : M3UFileReaderManager
    {
        private readonly IM3u8FileInfoStreamService m3U8FileInfoService;
        private readonly HttpClient httpClient;

        public PluginM3UFileReaderManager(IM3u8FileInfoStreamService m3U8FileInfoService, IM3uFileReader? M3UFileReader, HttpClient httpClient, IDictionary<string, IAttributeReader>? attributeReaders = null) : base(M3UFileReader,httpClient, attributeReaders)
        {
            this.m3U8FileInfoService = m3U8FileInfoService;
            this.httpClient = httpClient;
        }

        protected override async Task<(Uri?, Stream)> GetM3u8FileStreamAsync(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            return await m3U8FileInfoService.GetM3u8FileStreamAsync(httpClient, uri, headers, cancellationToken);
        }
    }
}
