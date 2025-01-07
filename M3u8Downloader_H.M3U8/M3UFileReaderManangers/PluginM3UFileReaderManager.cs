using M3u8Downloader_H.Abstractions.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.M3UFileReaderManangers
{
    internal class PluginM3UFileReaderManager(IM3u8FileInfoStreamService m3U8FileInfoService,  HttpClient httpClient) : M3UFileReaderManager(httpClient)
    {
        protected override async Task<Stream> GetM3u8FileStreamAsync(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default) 
            => await m3U8FileInfoService.GetM3u8FileStreamAsync(uri, headers, cancellationToken);
    }
}
