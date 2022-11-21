using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.bilivideo.plugin.M3u8FileInfos
{
    //这个类是示范代码
    internal class M3u8FileInfoTest : IM3u8UriProvider
    {
        /// <summary>
        /// 再请求之前可以做的一些操作
        /// 可以选择直接请求数据，也可以进行一些别得操作
        /// </summary>
        /// <param name="httpClient">http实例</param>
        /// <param name="uri">请求url</param>
        /// <param name="headers">请求头</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>返回实际拿到得uri</returns>
        public Task<Uri> GetM3u8UriAsync(HttpClient httpClient, Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(uri);
        }
    }
}
