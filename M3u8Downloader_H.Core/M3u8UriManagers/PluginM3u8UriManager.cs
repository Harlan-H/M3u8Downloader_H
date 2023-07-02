using M3u8Downloader_H.Core.Utils.Extensions;
using M3u8Downloader_H.Plugin;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.M3u8UriManagers
{
    internal class PluginM3u8UriManager  : IM3u8UriManager
    {
        private readonly IM3u8UriProvider m3U8UriProvider;
        private readonly HttpClient httpClient;
        private readonly IEnumerable<KeyValuePair<string, string>>? headers;

        public PluginM3u8UriManager(IM3u8UriProvider m3U8UriProvider,HttpClient httpClient, IEnumerable<KeyValuePair<string, string>>? headers)
        {
            this.m3U8UriProvider = m3U8UriProvider;
            this.httpClient = httpClient;
            this.headers = headers;
        }

        public Task<Uri> GetM3u8UriAsync(Uri uri, int reserve0, CancellationToken cancellationToken)
        {
            try
            {
                return m3U8UriProvider.GetM3u8UriAsync(httpClient, uri, headers, cancellationToken);
            }catch(OperationCanceledException) when(!cancellationToken.IsCancellationRequested) 
            {
                throw new TimeoutException($"访问 {uri.OriginalString} 超时");
            }      
        }
    }
}
