using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.Plugin
{
    public interface IDownloadService
    {
        /// <summary>
        /// 初始化函数，在没有进行任何下载之前，第一个调用的函数
        /// </summary>
        /// <param name="headers">附带的请求头，如果有的话</param>
        /// <param name="m3UFileInfo">m3u8的数据</param>
        /// <param name="cancellationToken">取消的token</param>
        /// <returns>没有返回内容</returns>
        Task Initialize(IEnumerable<KeyValuePair<string, string>>? headers, M3UFileInfo m3UFileInfo, CancellationToken cancellationToken);


        /// <summary>
        /// 当下载的数据接收到之后，会请求此函数，以便做后续处理
        /// 例如 当某些数据他没有采用通用的加密方式，而是使用了自己指定的一些加密方式，即可实现此函数来进一步处理
        /// 如果不需要处理此函数 可以直接将参数stream直接返回即可
        /// </summary>
        /// <param name="stream">请求到的数据流</param>
        /// <param name="cancellationToken">取消的token</param>
        /// <returns>返回处理后的数据</returns>
        Stream HandleData(Stream stream, CancellationToken cancellationToken);
    }
}
