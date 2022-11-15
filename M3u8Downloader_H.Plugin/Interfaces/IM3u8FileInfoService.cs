namespace M3u8Downloader_H.Plugin
{
    public interface IM3u8FileInfoService
    {
        /// <summary>
        /// 请求之前做的操作，例如需要post请求，或者参数是加密的都可以在此函数操作
        /// 如果不需要处理此函数可以使用this.CreateDefaultBeforeRequest()来创建默认的处理方法
        /// </summary>
        /// <param name="uri">请求地址</param>
        /// <param name="headers">请求头</param>
        /// <returns>返回供httpclient使用的请求头</returns>
        HttpRequestMessage BeforeRequest(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers = null);

        /// <summary>
        /// 处理请求之后的操作，例如返回的m3u8数据是加密的必须解密才能看到m3u8的字符串信息
        /// 如果不需要处理可以直接返回stream
        /// 处理之后可以返回MemoryStream
        /// </summary>
        /// <param name="stream">请求到的数据</param>
        /// <param name="cancellationToken">取消的令牌</param>
        /// <returns>返回处理后的数据</returns>
        Stream PostRequest(Stream stream, CancellationToken cancellationToken = default);
    }
}
