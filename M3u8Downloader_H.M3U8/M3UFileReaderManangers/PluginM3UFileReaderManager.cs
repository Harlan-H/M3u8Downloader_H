using M3u8Downloader_H.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.M3UFileReaderManangers
{
    internal class PluginM3UFileReaderManager(IM3u8FileInfoStreamService m3U8FileInfoService, IM3uFileReader? M3UFileReader, HttpClient httpClient, IDictionary<string, IAttributeReader>? attributeReaders = null) : M3UFileReaderManager(M3UFileReader,httpClient, attributeReaders)
    {
        private readonly IM3u8FileInfoStreamService m3U8FileInfoService = m3U8FileInfoService;

        protected override async Task<(Uri?, Stream)> GetM3u8FileStreamAsync(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default) 
            => await m3U8FileInfoService.GetM3u8FileStreamAsync(uri, headers, cancellationToken);
    }
}
