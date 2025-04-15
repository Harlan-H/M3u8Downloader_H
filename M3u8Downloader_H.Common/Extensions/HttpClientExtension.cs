using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Common.Extensions
{
    public static class HttpClientExtension
    {
        public static async Task<Stream> GetStreamAsync(this HttpClient httpClient, HttpRequestMessage httpRequestMessage, bool ensureSuccess = true, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if(ensureSuccess) response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }

        public static async Task<Stream> GetStreamAsync(this HttpClient httpClient,Uri uri, IEnumerable<KeyValuePair<string, string>>? headers = default,CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.AddHeaders(headers);

            return await httpClient.GetStreamAsync(request, true, cancellationToken);
        }

        public static async Task<(Uri?,Stream)> GetStreamAndUriAsync(this HttpClient httpClient, Uri uri, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.AddHeaders(headers);

            HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            Uri? requestUri = response.RequestMessage?.RequestUri;
            Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return (requestUri, stream);
        }

        public static async Task<Stream> GetStreamAsync(this HttpClient httpClient,string uri, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default)
        {
            return await httpClient.GetStreamAsync(new Uri(uri, UriKind.Absolute), headers, cancellationToken);
        }

        public static async Task<byte[]> GetByteArrayAsync(this HttpClient httpClient, HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }

        public static async Task<byte[]> GetByteArrayAsync(this HttpClient httpClient, Uri uri, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.AddHeaders(headers);

            return await httpClient.GetByteArrayAsync(request, cancellationToken);
        }

        public static async Task<byte[]> GetByteArrayAsync(this HttpClient httpClient, string uri, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default)
        {
            return await httpClient.GetByteArrayAsync(new Uri(uri, UriKind.Absolute), headers, cancellationToken);
        }

        public  static async Task<string> GetStringAsync(this HttpClient httpClient, HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }


        public static async Task<string> GetStringAsync(this HttpClient httpClient,Uri uri, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.AddHeaders(headers);

            return await httpClient.GetStringAsync(request, cancellationToken);
        }

        public static async Task<string> PostStringAsync(this HttpClient httpClient, Uri uri, string payload, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(HttpMethod.Post, uri);
            request.AddHeaders(headers);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        public static async Task<string> PostJsonAsync(this HttpClient httpClient, Uri uri, string payload, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(HttpMethod.Post, uri);
            request.AddHeaders(headers);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        public static async ValueTask<HttpResponseMessage> HeadAsync(this HttpClient httpClient, string requestUri, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, requestUri);
            request.AddHeaders(headers);

            return await httpClient.SendAsync(request, cancellationToken);
        }

        public static async ValueTask<long?> TryGetContentLengthAsync(this HttpClient httpClient, string requestUri, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default)
        {
            using var response = await httpClient.HeadAsync(requestUri, headers, cancellationToken);
            response.EnsureSuccessStatusCode();

            return response.Content.Headers.ContentLength;
        }

        public static async ValueTask<(long?,string?)> GetMediaLengthAndTypeAsync(this HttpClient httpClient, string requestUri, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default)
        {
            using var response = await httpClient.HeadAsync(requestUri, headers, cancellationToken);
            response.EnsureSuccessStatusCode();

            long? length = response.Content.Headers.ContentLength;
            string? mediaType = response.Content.Headers.ContentType?.MediaType;
            return (length,mediaType);
        }
    }
}
