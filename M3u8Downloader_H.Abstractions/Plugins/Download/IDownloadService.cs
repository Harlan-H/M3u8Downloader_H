using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.Abstractions.Plugins.Download
{
    /// <summary>
    /// 此接口为m3u8下载核心接口
    /// 如果你需要实现自己的下载逻辑需要实现这个接口
    /// </summary>
    public interface IDownloadService
    {
        /// <summary>
        /// 初始化函数，在没有进行任何下载之前，第一个调用的函数
        /// </summary>
        /// <param name="headers">附带的请求头，如果有的话</param>
        /// <param name="m3UFileInfo">m3u8的数据</param>
        /// <param name="cancellationToken">取消的token</param>
        /// <returns>没有返回内容</returns>
        ValueTask Initialization(CancellationToken cancellationToken = default);


        /// <summary>
        /// 开始下载函数 等于所有下载的入口函数
        /// </summary>
        /// <param name="m3UFileInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartDownload(IM3uFileInfo m3UFileInfo, CancellationToken cancellationToken = default);

        /// <summary>
        /// 内部实际工作的方法
        /// </summary>
        /// <param name="m3UMediaInfo"></param>
        /// <param name="headers"></param>
        /// <param name="mediaPath"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> DownloadM3uMediaInfo(IM3uMediaInfo m3UMediaInfo, IEnumerable<KeyValuePair<string, string>>? headers, string mediaPath, CancellationToken cancellationToken = default);


        /// <summary>
        /// 当下载的数据接收到之后，会请求此函数，以便做后续处理
        /// 例如 当某些数据他没有采用通用的加密方式，而是使用了自己指定的一些加密方式，即可实现此函数来进一步处理
        /// 如果不需要处理此函数 可以直接将参数stream直接返回即可
        /// </summary>
        /// <param name="stream">请求到的数据流</param>
        /// <param name="cancellationToken">取消的token</param>
        /// <returns>返回处理后的数据</returns>
        Func<Stream, CancellationToken, Stream> HandleDataFunc { get; set; }


        /// <summary>
        /// 最终要写入的文件处理方法
        /// </summary>
        Func<string, Stream, CancellationToken, Task> WriteToFileFunc { get; set; }
    }
}
