using M3u8Downloader_H.Abstractions.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.Interfaces
{
    public interface IDownloader
    {
        Task StartDownload(Action<int> StateAction,IDialogProgress dialogProgress, CancellationToken cancellationToken);
    }
}
