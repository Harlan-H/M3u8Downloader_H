using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.Downloader.MediaDownloads
{
    internal class LiveVideoDownloader(HttpClient httpClient) : DownloaderBase(httpClient)
    {
        public override async Task DownloadAsync(IStreamInfo streamInfo, CancellationToken cancellationToken = default)
        {
            await base.DownloadAsync(streamInfo, cancellationToken);
            DialogProgress.SetDownloadStatus(true);

            string mediaPath = Path.Combine(_cachePath, $"{DownloadParam.VideoName}.{streamInfo.MediaType}.tmp");
            await DownloadAsynInternal(streamInfo, _headers, null, mediaPath, cancellationToken);
        }

        protected override void UpdateProgress(long total, long? filesize)
        {
            DialogProgress.Report(-0.00001);
        }
    }
}
