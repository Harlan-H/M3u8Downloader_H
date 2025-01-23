using System.Buffers;
using System.IO.Pipelines;
using System.Net.Http.Headers;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.Downloader.Extensions;

namespace M3u8Downloader_H.Downloader.MediaDownloads
{
    internal class MediaDownloader(HttpClient httpClient) : DownloaderBase(httpClient)
    {
        private readonly HttpClient httpClient = httpClient;

        protected async Task SetVideoSize(IStreamInfo streamInfo, CancellationToken cancellationToken = default)
        { 
            if (streamInfo.FileSize is null)
            {
                long fileSize = await httpClient.TryGetContentLengthAsync(streamInfo.Url.OriginalString, _headers, cancellationToken) ?? throw new InvalidDataException("获取视频大小失败");
                streamInfo.SetFileSize(fileSize);
            }
        }

        public override async Task DownloadAsync(IStreamInfo streamInfo, CancellationToken cancellationToken = default)
        {
            await base.DownloadAsync(streamInfo,cancellationToken);
            DialogProgress.SetDownloadStatus(false);

            await SetVideoSize(streamInfo, cancellationToken);

            string mediaPath = Path.Combine(_cachePath, $"{DownloadParam.VideoName}.{streamInfo.MediaType}.tmp");

            RangeHeaderValue rangeHeaderValue;
            FileInfo fileInfo = new(mediaPath);
            if (fileInfo.Exists)
                rangeHeaderValue = new RangeHeaderValue(fileInfo.Length, streamInfo.FileSize);
            else
                rangeHeaderValue = new RangeHeaderValue(0, streamInfo.FileSize);

            await DownloadAsynInternal(streamInfo, _headers, rangeHeaderValue, mediaPath, cancellationToken);
        }

        protected override void UpdateProgress(long total, long? filesize)
        {
            DialogProgress?.Report(total / (double)filesize!);
        }

    }
}
