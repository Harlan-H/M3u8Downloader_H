namespace M3u8Downloader_H.Plugin
{
    public interface IM3u8UriProvider
    {
        /// <summary>
        /// 获取m3u8得数据流,此函数可以做解析网站得操作,只要最终返回m3u8得数据流即可
        /// 当你实现此函数得时候需要注意得是
        /// 这个请求数据他可能会循环请求,因为某些m3u8得数据流是需要请求两次得
        /// 所以当你实现此函数得时候,需要注意一下是否被重复调用了,如果是m3u8得文件流被重复请求可以使用
        /// httpClient.GetStreamAsync(uri, headers, cancellationToken) 函数发起对m3u8得请求
        /// </summary>
        /// <param name="httpClient">http实例</param>
        /// <param name="uri">请求地址</param>
        /// <param name="headers">请求头</param>
        /// <param name="cancellationToken">取消的令牌</param>
        /// <returns>返回m3u8得地址</returns>
        Task<Uri> GetM3u8UriAsync(HttpClient httpClient, Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default);
    }
}
