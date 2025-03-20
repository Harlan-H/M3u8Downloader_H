using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Downloader.M3uDownloaders;

namespace M3u8Downloader_H.Downloader
{
    public class DownloaderLocal(ILog log, IDownloadParamBase DownloadParam)
    {
        public IDialogProgress DialogProgress { get; set; } = default!;

        public OnlyDecryptDownloader M3u8Downloader
        {
            get
            {
                OnlyDecryptDownloader _m3u8downloader = new(null!)
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
