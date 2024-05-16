namespace M3u8Downloader_H.Plugin
{
    public interface IM3u8FileInfoStreamService
    {
        /// <summary>
        /// 获取m3u8得数据流
        /// 当你实现此函数得时候需要注意得是
        /// 这个请求数据他可能会循环请求,因为某些m3u8得数据流是需要请求两次得
        /// 所以当你实现此函数得时候,需要注意一下是否被重复调用了
        /// 如果需要默认得处理函数可以使用下面得方式,同时引入M3u8Downloader_H.Common这个dll
        /// httpClient.GetStreamAndUriAsync(uri, headers, cancellationToken);
        /// </summary>
        /// <param name="uri">请求地址</param>
        /// <param name="headers">请求头</param>
        /// <param name="cancellationToken">取消的令牌</param>
        /// <returns>返回得到得m3u8数据流</returns>
        Task<(Uri?,Stream)> GetM3u8FileStreamAsync(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default);
    }
}
