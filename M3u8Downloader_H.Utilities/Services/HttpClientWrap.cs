using M3u8Downloader_H.Utilities.Extensions;
using M3u8Downloader_H.Utilities.Models;
using System.Net.Http.Headers;

namespace M3u8Downloader_H.Utilities.Services
{
    public sealed  class HttpClientWrap(
        HttpClient httpClient,
        TimeOutOptions options)
    {
        public HttpClientWrap(HttpClient httpClient ,long timeout) : this(httpClient,new TimeOutOptions(timeout)) { }

        public HttpClientWrap(HttpClient httpClient) : this(httpClient, new TimeOutOptions(30)) { }

        private async Task<HttpResponseMessage> CheckTimeout(Task<HttpResponseMessage> responseTask,CancellationToken cancellationToken)
        {
            var timeoutTask = Task.Delay(options.HeaderTimeout, cancellationToken);
            var completed = await Task.WhenAny(responseTask, timeoutTask);
            if (completed != responseTask)
                throw new TimeoutException($"连接超时 {options.HeaderTimeout} 秒");

            return  await responseTask;
        }

        public async Task<Stream> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            var responseTask = httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            var response = await CheckTimeout(responseTask, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }


        public async ValueTask<HttpResponseMessage> HeadAsync(HttpRequestMessage httpRequestMessage,  CancellationToken cancellationToken = default)
        {
            var responseTask = httpClient.SendAsync(httpRequestMessage, cancellationToken);

            var response = await CheckTimeout(responseTask,cancellationToken);
            response.EnsureSuccessStatusCode();

            return response;;
        }

        public async ValueTask<long?> TryGetContentLengthAsync(string requestUri, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, requestUri);
            request.AddHeaders(headers);

            using var resp = await HeadAsync(request, cancellationToken);
            return resp.Content.Headers.ContentLength;
        }



        public async Task<Stream> GetStreamAsync(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, RangeHeaderValue? rangeHeaderValue, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage httpRequest = new(HttpMethod.Get, uri);
            httpRequest.AddHeaders(headers);
            if (rangeHeaderValue != null)
            {
                httpRequest.Headers.Range = rangeHeaderValue;
            }
            var stream = await SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            return await Task.FromResult<Stream>(new TimeoutStream(stream, options));
        }

        public async Task<Stream> GetStreamAsync(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.AddHeaders(headers);

            return await SendAsync(request,  cancellationToken);
        }


        public async Task<byte[]> GetByteArrayAsync(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.AddHeaders(headers);

            await using var resp =  await SendAsync(request, cancellationToken);
            await using MemoryStream memoryStream = new();
            await resp.CopyToAsync(memoryStream,cancellationToken);
            return memoryStream.ToArray();
        }

    }
}
