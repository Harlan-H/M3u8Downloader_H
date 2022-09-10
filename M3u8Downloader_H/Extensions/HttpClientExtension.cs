using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Extensions
{
    internal static class HttpClientExtension
    {
        public static async Task<bool>  GetConnectStatus(this HttpClient httpClient, Uri uri,  CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(HttpMethod.Head, uri);
            HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
}
