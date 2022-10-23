using M3u8Downloader_H.Core.DownloaderSources;
using M3u8Downloader_H.M3U8.AttributeReaders;
using M3u8Downloader_H.M3U8.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        IDownloadManager InitM3u8Reader();
        ValueTask GetM3U8FileInfo(CancellationToken cancellationToken);
        IDownloaderSource Build();
    }
}
