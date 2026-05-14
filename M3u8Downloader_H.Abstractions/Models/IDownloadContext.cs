using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3uDownloaders;


namespace M3u8Downloader_H.Abstractions.Models
{
    public interface IDownloadContext
    {
        HttpClient HttpClient { get; }
        ILog Log { get; }
        IDownloadParamBase DownloadParam { get; }
        IDownloaderSetting DownloaderSetting { get; }
    }
}
