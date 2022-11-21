using M3u8Downloader_H.Common.Extensions;


namespace M3u8Downloader_H.Plugin.Extensions
{
    public  static class IM3u8FileInfoServiceExtension
    {
        /// <summary>
        /// 当实现IM3u8FileInfoService接口之后
        /// 使用this.CreateDefaultBeforeRequest()来获取此函数的调用
        /// </summary>
        /// <param name="M3U8InfoService"></param>
        /// <param name="uri"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static HttpRequestMessage CreateDefaultBeforeRequest(this IM3u8UriProvider M3U8InfoService,Uri uri, IEnumerable<KeyValuePair<string, string>>? headers = null)
        {
            HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.AddHeaders(headers);
            return request;
        }

    }
}
