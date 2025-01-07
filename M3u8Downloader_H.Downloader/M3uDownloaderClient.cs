using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Downloader.Extensions;
using M3u8Downloader_H.Downloader.M3uDownloaders;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Plugin.PluginManagers;
using M3u8Downloader_H.Abstractions.Common;


namespace M3u8Downloader_H.Downloader
{
    public class M3uDownloaderClient(HttpClient httpClient, IPluginManager? PluginManager, IDownloadParam DownloadParam)
    {
        private DownloaderBase? _m3u8downloader;

        public IDownloaderSetting DownloaderSetting { get; set; } = default!;
        public IDialogProgress DialogProgress { get; set; } = default!;
        public M3UFileInfo M3UFileInfo { get; set; } = default!;
        public DownloaderBase M3u8Downloader
        {
            get
            {
                if(_m3u8downloader is not null)
                    return _m3u8downloader;

                if (!M3UFileInfo.IsVod())
                    _m3u8downloader = new LiveM3uDownloader(httpClient);
                else if(M3UFileInfo.MediaFiles.Any(m => m.Uri.IsFile))
                    _m3u8downloader = new NullDownloader(httpClient);
                else if (M3UFileInfo.Key is not null)
                    _m3u8downloader = new CryptM3uDownloader(httpClient, M3UFileInfo);
                else if (PluginManager?.PluginService is not null)
                    _m3u8downloader = new PluginM3u8Downloader(PluginManager?.PluginService!,httpClient, M3UFileInfo);
                else
                    _m3u8downloader = new M3u8Downloader(httpClient);

                _m3u8downloader.DownloadParam = DownloadParam;
                _m3u8downloader.DownloaderSetting = DownloaderSetting;
                _m3u8downloader.DialogProgress = DialogProgress;
                return _m3u8downloader;
            }
        }
    }
}
