using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Downloader.Extensions;
using M3u8Downloader_H.Downloader.M3uDownloaders;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Plugin.PluginManagers;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Downloader.MediaDownloads;


namespace M3u8Downloader_H.Downloader
{
    public class DownloaderClient(HttpClient httpClient, IPluginManager? PluginManager, ILog log, IDownloadParamBase DownloadParam)
    {
        public IDownloaderSetting DownloaderSetting { get; set; } = default!;
        public IDialogProgress DialogProgress { get; set; } = default!;
        public M3UFileInfo M3UFileInfo { get; set; } = default!;
        public Func<CancellationToken, Task<M3UFileInfo>> GetLiveFileInfoFunc { get; set; } = default!;

        public M3uDownloaders.DownloaderBase M3u8Downloader
        {
            get
            {
                M3uDownloaders.DownloaderBase _m3u8downloader;
                if (!M3UFileInfo.IsVod())
                {
                    LiveM3uDownloader liveM3UDownloader = new(httpClient)
                    {
                        GetLiveFileInfoFunc = GetLiveFileInfoFunc
                    };
                    _m3u8downloader = liveM3UDownloader;
                }
                else if (M3UFileInfo.Key is not null)
                    _m3u8downloader = new CryptM3uDownloader(httpClient, M3UFileInfo);
                else if (PluginManager?.PluginService is not null)
                    _m3u8downloader = new PluginM3u8Downloader(PluginManager?.PluginService!,httpClient, M3UFileInfo);
                else
                    _m3u8downloader = new M3u8Downloader(httpClient);

                _m3u8downloader.DownloadParam = DownloadParam;
                _m3u8downloader.Log = log;
                _m3u8downloader.DownloaderSetting = DownloaderSetting;
                _m3u8downloader.DialogProgress = DialogProgress;
                return _m3u8downloader;
            }
        }

        public MediaDownloads.DownloaderBase MediaDownloader
        {
            get
            {
                IMediaDownloadParam mediaDownloadParam = (IMediaDownloadParam)DownloadParam;
                MediaDownloads.DownloaderBase mediaDownloader;
                if (mediaDownloadParam.IsVideoStream)
                    mediaDownloader = new MediaDownloader(httpClient);
                else
                    mediaDownloader = new LiveVideoDownloader(httpClient);

                mediaDownloader.DownloadParam = mediaDownloadParam;
                mediaDownloader.Log = log;
                mediaDownloader.DownloaderSetting = DownloaderSetting;
                mediaDownloader.DialogProgress = DialogProgress;
                return mediaDownloader;
            }
        }
    }
}
