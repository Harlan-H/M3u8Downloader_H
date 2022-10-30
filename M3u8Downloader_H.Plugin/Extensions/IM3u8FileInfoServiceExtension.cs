using M3u8Downloader_H.Extensions;

namespace M3u8Downloader_H.Plugin.Extensions
{
    public  static class IM3u8FileInfoServiceExtension
    {
        public static HttpRequestMessage CreateDefaultBeforeRequest(this IM3u8FileInfoService M3U8InfoService,Uri uri, IEnumerable<KeyValuePair<string, string>>? headers = null)
        {
            HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.AddHeaders(headers);
            return request;
        }

    }
}
