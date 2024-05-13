using M3u8Downloader_H.Combiners.Interfaces;
using M3u8Downloader_H.Common.Interfaces;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Settings.Models;

namespace M3u8Downloader_H.Downloader.DownloaderSources
{
    public interface IDownloaderSource
    {
        ILog? Log { get; set; }
        ISettings Settings { get; set; }
        IProgress<long> DownloadRate { get; set; }
        IEnumerable<KeyValuePair<string, string>>? Headers { get; set; }
        M3UFileInfo M3UFileInfo { get; set; }
        IDownloadParams DownloadParams { get; set; }
        Func<Uri, IEnumerable<KeyValuePair<string, string>>?, CancellationToken, Task<M3UFileInfo>> GetLiveFileInfoFunc { get; set; }
        Task DownloadAsync(Action<bool> IsLiveAction, CancellationToken cancellationToken = default);

    }
}
