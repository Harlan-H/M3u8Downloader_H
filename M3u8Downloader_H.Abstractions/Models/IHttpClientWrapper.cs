using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace M3u8Downloader_H.Abstractions.Models
{
    public interface IHttpClientWrapper
    {
        Task<Stream> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
        ValueTask<HttpResponseMessage> HeadAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default);
        ValueTask<long?> TryGetContentLengthAsync(string requestUri, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default);
        Task<Stream> GetStreamAsync(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, RangeHeaderValue? rangeHeaderValue, CancellationToken cancellationToken = default);
        Task<Stream> GetStreamAsync(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default);
        Task<byte[]> GetByteArrayAsync(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers = default, CancellationToken cancellationToken = default);
    }
}
