using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Models;

namespace M3u8Downloader_H.Plugin.Models.Context
{
    public class DownloadContext(IHttpClientWrapper httpClient,
        ILog log,
        IDownloadParamBase downloadParamBase,
        IDownloaderSetting downloaderSetting
            ) : IDownloadContext
    {
        public IHttpClientWrapper HttpClient { get; private set; } = httpClient;

        public ILog Log { get; private set; } = log;

        public IDownloadParamBase DownloadParam { get; private set; } = downloadParamBase;

        public IDownloaderSetting DownloaderSetting { get; private set; } = downloaderSetting;
    }
}
