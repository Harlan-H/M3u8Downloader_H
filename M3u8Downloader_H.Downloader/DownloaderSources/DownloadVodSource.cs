using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Downloader.M3uDownloaders;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.Downloader.DownloaderSources
{
    internal class DownloadVodSource(HttpClient httpClient, IDownloadService? downloadService) : DownloaderSource(downloadService)
    {
        private readonly HttpClient _httpClient = httpClient;

        public override async Task DownloadAsync(Action<bool> IsLiveAction, CancellationToken cancellationToken = default)
        {
            await base.DownloadAsync(IsLiveAction, cancellationToken);

            M3u8Downloader m3U8Downloade = CreateDownloader();
            m3U8Downloade.TimeOut = TimeSpan.FromSeconds( Settings.Timeouts);
            m3U8Downloade.HttpClient = _httpClient;
            m3U8Downloade.Progress = DownloadParams.VodProgress;
            m3U8Downloade.DownloadRate = DownloadRate;
            m3U8Downloade.Headers = Headers;
            m3U8Downloade.RetryCount = Settings.RetryCount;

            IsLiveAction.Invoke(false);
            await m3U8Downloade.Initialization(cancellationToken);
            await m3U8Downloade.DownloadMapInfoAsync(M3UFileInfo.Map, DownloadParams.VideoFullPath, Settings.SkipRequestError, cancellationToken);
            await m3U8Downloade.Start(M3UFileInfo, Settings.MaxThreadCount, DownloadParams.VideoFullPath, 0, Settings.SkipRequestError, cancellationToken);

        }
    }
}
