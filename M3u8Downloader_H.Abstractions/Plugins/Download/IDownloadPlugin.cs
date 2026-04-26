using M3u8Downloader_H.Abstractions.Models;

namespace M3u8Downloader_H.Abstractions.Plugins.Download
{
    public interface IDownloadPlugin
    {
        /// <summary>
        /// 当你实现了IM3uFileReader接口那在此方法里直接返回你自己的实例
        /// 例如 new MyM3uFileReader(m3UFileReader,downloadContext);
        /// </summary>
        /// <param name="m3UFileReader"></param>
        /// <param name="downloadContext"></param>
        /// <returns></returns>
        IM3uFileReader? CreateM3uFileReader(IM3uFileReader m3UFileReader,IDownloadContext downloadContext);


        /// <summary>
        /// 和上面类似
        /// 例如： new MyDownloadService(downloadService,downloadContext);
        /// </summary>
        /// <param name="downloadService"></param>
        /// <param name="downloadContext"></param>
        /// <returns></returns>
        IDownloadService? CreateDownloadService(IDownloadService downloadService, IDownloadContext downloadContext);
    }
}
