using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.Abstractions.Downloader
{
    public interface IDownloader
    {
        Task StartDownload(Action<int> StateAction,IDialogProgress dialogProgress, CancellationToken cancellationToken);
    }
}
