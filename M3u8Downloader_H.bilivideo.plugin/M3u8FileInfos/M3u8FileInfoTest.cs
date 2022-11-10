using M3u8Downloader_H.Plugin.Extensions;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.bilivideo.plugin.M3u8FileInfos
{
    //这个类是示范代码
    internal class M3u8FileInfoTest : IM3u8FileInfoService
    {
        /// <summary>
        /// 再请求之前可以做的一些操作
        /// </summary>
        /// <param name="uri">请求的url</param>
        /// <param name="headers">请求头</param>
        /// <returns>HttpRequestMessage 自己实现可以参考CreateDefaultBeforeRequest的代码</returns>
        public HttpRequestMessage BeforeRequest(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers = null)
        {
            return this.CreateDefaultBeforeRequest(uri, headers);
        }


        /// <summary>
        /// 请求到数据之后的处理
        /// </summary>
        /// <param name="stream">原始的数据流</param>
        /// <param name="cancellationToken">取消的令牌</param>
        /// <returns>如果不处理直接返回stream,处理可以返回任意继承了stream的类</returns>
        public Stream PostRequest(Stream stream, CancellationToken cancellationToken = default)
        {
            return stream;
        }
    }
}
