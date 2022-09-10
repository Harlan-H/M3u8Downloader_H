using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Extensions
{
    public static class HttpClientExtension
    {
        public static async Task<Stream> GetStreamAsync(this HttpClient httpClient, HttpRequestMessage httpRequestMessage, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if(ensureSuccess) response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }

        public static async Task<Stream> GetStreamAsync(this HttpClient httpClient,Uri uri, IEnumerable<KeyValuePair<string, string>> headers = default,CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.AddHeaders(headers);

            return await httpClient.GetStreamAsync(request, true, cancellationToken);
        }
        
        public static async Task<Stream> GetStreamAsync(this HttpClient httpClient,string uri, IEnumerable<KeyValuePair<string, string>> headers = default, CancellationToken cancellationToken = default)
        {
            return await httpClient.GetStreamAsync(new Uri(uri, UriKind.Absolute), headers, cancellationToken);
        }

        public static async Task<byte[]> GetByteArrayAsync(this HttpClient httpClient, HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }

        public static async Task<byte[]> GetByteArrayAsync(this HttpClient httpClient, Uri uri, IEnumerable<KeyValuePair<string, string>> headers = default, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.AddHeaders(headers);

            return await httpClient.GetByteArrayAsync(request, cancellationToken);
        }

        public static async Task<byte[]> GetByteArrayAsync(this HttpClient httpClient, string uri, IEnumerable<KeyValuePair<string, string>> headers = default, CancellationToken cancellationToken = default)
        {
            return await httpClient.GetByteArrayAsync(new Uri(uri, UriKind.Absolute), headers, cancellationToken);
        }
    }
}
