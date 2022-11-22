using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Core.DownloaderSources;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.DownloaderManagers
{
    public interface IDownloadManager
    {
        string VideoFullPath { get; }
        string VideoFullName { get; }
        IDownloadManager WithM3u8Content(string content);
        IDownloadManager WithM3u8FileInfo(M3UFileInfo fileinfo);
        IDownloadManager WithKeyInfo(M3UKeyInfo m3UKeyInfo);
        IDownloadManager WithVodProgress(IProgress<double> action);
        IDownloadManager WithLiveProgress(IProgress<double> action);
        IDownloadManager WithStatusAction(Action<int> action);
        IDownloadManager WithHeaders(IEnumerable<KeyValuePair<string, string>>? headers);
        Task GetM3U8FileInfo(CancellationToken cancellationToken);
        IDownloaderSource Build();
    }
}
