using M3u8Downloader_H.M3U8.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.DownloaderSources
{
    public interface IDownloaderSource
    {
        IDownloaderSource WithTaskNumber(int number);
        IDownloaderSource WithTimeout(int timeout);
        IDownloaderSource WithSavePath(string savepath);
        IDownloaderSource WithMaxRecordDuration(double duration);
        IDownloaderSource WithSkipRequestError(bool skip);
        IDownloaderSource WithForceMerge(bool merge);
        IDownloaderSource WithFormats(string format);
        IDownloaderSource WithCleanUp(bool isCleanUp);
        IDownloaderSource WithSkipDirectoryExist(bool skipexist);
        IDownloaderSource WithHeaders(IEnumerable<KeyValuePair<string, string>>? headers);

        Task DownloadAsync(CancellationToken cancellationToken = default);

    }
}
