using M3u8Downloader_H.Downloader.M3uDownloaders;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Downloader.MediaDownloads;
using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Downloader.Extensions;


namespace M3u8Downloader_H.Downloader
{
    public class DownloaderClient
    {
        private readonly HttpClient httpClient = default!;
        private readonly IPluginManager? pluginManager;
        private readonly ILog log;
        private readonly IDownloadParamBase downloadParam;
        private readonly IDownloaderSetting downloaderSetting = default!;

        public IDialogProgress DialogProgress { get; set; } = default!;
        public IM3uFileInfo M3UFileInfo { get; set; } = default!;
        public Func<CancellationToken, Task<IM3uFileInfo>> GetLiveFileInfoFunc { get; set; } = default!;

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
                else if (pluginManager?.PluginService is not null)
                    _m3u8downloader = new PluginM3u8Downloader(pluginManager?.PluginService!,httpClient, M3UFileInfo);
                else
                    _m3u8downloader = new M3u8Downloader(httpClient);

                _m3u8downloader.DownloadParam = downloadParam;
                _m3u8downloader.Log = log;
                _m3u8downloader.DownloaderSetting = downloaderSetting;
                _m3u8downloader.DialogProgress = DialogProgress;
                return _m3u8downloader;
            }
        }

        public MediaDownloads.DownloaderBase MediaDownloader
        {
            get
            {
                IMediaDownloadParam mediaDownloadParam = (IMediaDownloadParam)downloadParam;
                MediaDownloads.DownloaderBase mediaDownloader;
                if (mediaDownloadParam.IsVideoStream)
                    mediaDownloader = new MediaDownloader(httpClient);
                else
                    mediaDownloader = new LiveVideoDownloader(httpClient);

                mediaDownloader.DownloadParam = mediaDownloadParam;
                mediaDownloader.Log = log;
                mediaDownloader.DownloaderSetting = downloaderSetting;
                mediaDownloader.DialogProgress = DialogProgress;
                return mediaDownloader;
            }
        }

        public OnlyDecryptDownloader M3uDecrypter
        {
            get
            {
                OnlyDecryptDownloader _m3u8downloader = new()
                {
                    DownloadParam = downloadParam,
                    Log = log,
                    DialogProgress = DialogProgress
                };
                return _m3u8downloader;
            }
        }


        public DownloaderClient(HttpClient httpClient, IPluginManager? PluginManager, ILog log, IDownloadParamBase downloadParam, IDownloaderSetting downloaderSetting)
        {
            this.httpClient = httpClient;
            pluginManager = PluginManager;
            this.log = log;
            this.downloadParam = downloadParam;
            this.downloaderSetting = downloaderSetting;
        }

        public DownloaderClient(ILog log, IDownloadParamBase DownloadParam)
        {
            this.log = log;
            downloadParam = DownloadParam;
        }
    }
}
