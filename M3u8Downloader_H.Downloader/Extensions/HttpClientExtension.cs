using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Common.Extensions;

namespace M3u8Downloader_H.Downloader.Extensions
{
    internal static class HttpClientExtension
    {
        public static async Task<(Stream, string)> GetResponseContentAsync(this HttpClient httpClient, HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default )
        { 
            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            response.EnsureSuccessStatusCode();

            var contentType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return (stream, contentType);
        }

        public static async Task<(Stream, string)> GetResponseContentAsync(this HttpClient httpClient, Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, RangeHeaderValue? rangeHeaderValue, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage httpRequest = new(HttpMethod.Get, uri);
            httpRequest.AddHeaders(headers);
            if(rangeHeaderValue != null)
            {
                httpRequest.Headers.Range = rangeHeaderValue;
            }

            return await httpClient.GetResponseContentAsync(httpRequest, cancellationToken);
        }
    }
}
