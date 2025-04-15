using System.Collections.Generic;
using System.Net.Http;

namespace M3u8Downloader_H.Common.Extensions
{
    public static class HttpRequestMessageExtension
    {
        public static void AddHeaders(this HttpRequestMessage httpRequestMessage, IEnumerable<KeyValuePair<string, string>>? headers)
        {
            if (headers is null)
                return;

            foreach (var item in headers)
            {
                if (!string.IsNullOrWhiteSpace(item.Key) && !string.IsNullOrWhiteSpace(item.Value))
                    httpRequestMessage.Headers.TryAddWithoutValidation(item.Key, item.Value);
            }
        }
    }
}
