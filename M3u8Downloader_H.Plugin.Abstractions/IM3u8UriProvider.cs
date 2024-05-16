namespace M3u8Downloader_H.Plugin
{
    public interface IM3u8UriProvider
    {
        /// <summary>
        /// 获取m3u8得数据流,此函数可以做解析网站得操作,只要最终返回m3u8得数据流即可
        /// </summary>
        /// <param name="uri">请求地址</param>
        /// <param name="headers">请求头</param>
        /// <param name="cancellationToken">取消的令牌</param>
        /// <returns>返回m3u8得地址</returns>
        Task<Uri> GetM3u8UriAsync(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default);
    }
}
