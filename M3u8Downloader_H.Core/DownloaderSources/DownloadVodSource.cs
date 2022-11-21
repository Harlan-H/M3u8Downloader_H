using M3u8Downloader_H.Core.M3uDownloaders;
using M3u8Downloader_H.Common.M3u8Infos;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.DownloaderSources
{
    internal class DownloadVodSource : DownloaderSource
    {
        private readonly int _downloadStatus = 2;
        public override async Task DownloadAsync(CancellationToken cancellationToken = default)
        {
            await base.DownloadAsync(cancellationToken);

            M3u8Downloader m3U8Downloade = CreateDownloader();
            m3U8Downloade.TimeOut = _timeouts * 1000;
            m3U8Downloade.HttpClient = HttpClient;
            m3U8Downloade.Progress = VodProgress;
            m3U8Downloade.DownloadRate = _downloadRate;
            m3U8Downloade.Headers = Headers;

            SetStatusDelegate(_downloadStatus);
            await m3U8Downloade.Initialization(cancellationToken);
            await m3U8Downloade.DownloadMapInfoAsync(M3UFileInfo.Map, VideoFullPath, _skipRequestError, cancellationToken);
            await m3U8Downloade.Start(M3UFileInfo, _taskNumber, VideoFullPath, 0, _skipRequestError, cancellationToken);

            await Converter(false, cancellationToken);

            RemoveCacheDirectory(VideoFullPath);
        }
    }
}
