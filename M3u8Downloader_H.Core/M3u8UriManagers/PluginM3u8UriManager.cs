using M3u8Downloader_H.Plugin;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.M3u8UriManagers
{
    internal class PluginM3u8UriManager(IM3u8UriProvider m3U8UriProvider, IEnumerable<KeyValuePair<string, string>>? headers) : IM3u8UriManager
    {
        private readonly IM3u8UriProvider m3U8UriProvider = m3U8UriProvider;
        private readonly IEnumerable<KeyValuePair<string, string>>? headers = headers;
        public bool Completed { get; set; } = false;

        public Task<Uri> GetM3u8UriAsync(Uri uri, int reserve0, CancellationToken cancellationToken)
        {
            try
            {
                var url = m3U8UriProvider.GetM3u8UriAsync(uri, headers, cancellationToken);
                Completed = true;
                return url;
            }
            catch(OperationCanceledException) when(!cancellationToken.IsCancellationRequested) 
            {
                throw new TimeoutException($"访问 {uri.OriginalString} 超时");
            }      
        }
    }
}
