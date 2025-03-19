using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Downloader.M3uDownloaders;

namespace M3u8Downloader_H.Downloader
{
    internal class DownloaderLocal(ILog log, IDownloadParamBase DownloadParam)
    {
        public IDialogProgress DialogProgress { get; set; } = default!;
        public M3UFileInfo M3UFileInfo { get; set; } = default!;

        public DownloaderBase M3u8Downloader
        {
            get
            {
                DownloaderBase _m3u8downloader = new OnlyDecryptDownloader(M3UFileInfo, null!)
                {
                    DownloadParam = DownloadParam,
                    Log = log,
                    DialogProgress = DialogProgress
                };
                return _m3u8downloader;
            }
        }
    }
}
