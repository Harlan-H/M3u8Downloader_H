using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Progress.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.Interfaces
{
    public interface IDownloader
    {
        Task StartDownload(IProgressManager progressManager, CancellationToken cancellationToken);
    }
}
