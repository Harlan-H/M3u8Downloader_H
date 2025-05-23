using System.Net;
using System.Net.Http.Headers;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.Downloader.Utils;

namespace M3u8Downloader_H.Downloader.MediaDownloads
{
    internal class MediaDownloader(HttpClient httpClient) : DownloaderBase(httpClient)
    {
        private readonly HttpClient httpClient = httpClient;

        protected async Task SetVideoSize(IStreamInfo streamInfo, CancellationToken cancellationToken = default)
        { 
            if (streamInfo.FileSize is null)
            {
                try
                {
                    long fileSize = await httpClient.TryGetContentLengthAsync(streamInfo.Url.OriginalString, _headers, cancellationToken) ?? throw new InvalidDataException("获取视频大小失败");
                    streamInfo.SetFileSize(fileSize);
                    Log?.Info("获得文件大小是:{0}", new FileSize(fileSize).ToString());
                }
                catch (HttpRequestException ex) 
                {
                    if(ex.StatusCode == HttpStatusCode.Forbidden)
                    {
                        throw new HttpRequestException("请设置referer后再次尝试", ex, ex.StatusCode);
                    }
                    throw new HttpRequestException("获得文件大小失败",ex,ex.StatusCode);
                }
            }
        }

        public override async Task DownloadAsync(IStreamInfo streamInfo, CancellationToken cancellationToken = default)
        {
            await base.DownloadAsync(streamInfo,cancellationToken);
            DialogProgress.SetDownloadStatus(false);

            Log?.Info("【{0}】开始下载", streamInfo.Title);
            await SetVideoSize(streamInfo, cancellationToken);

            string mediaPath = Path.Combine(_cachePath, streamInfo.Title);

            RangeHeaderValue rangeHeaderValue;
            FileInfo fileInfo = new(mediaPath);
            if (fileInfo.Exists && fileInfo.Length == streamInfo.FileSize) 
            {
                Log?.Info("【{0}】已经下载完成,跳过下载", streamInfo.Title);
                return;
            }
            else if (fileInfo.Exists && fileInfo.Length < streamInfo.FileSize)
            {
                rangeHeaderValue = new RangeHeaderValue(fileInfo.Length, streamInfo.FileSize);
            }
            else
                rangeHeaderValue = new RangeHeaderValue(0, streamInfo.FileSize);
            
            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(DownloaderSetting.Timeouts));
            Log?.Info("当前超时时间是【{0}】秒,请不要设置过小以免造成过早退出", DownloaderSetting.Timeouts);

            await DownloadAsynInternal(streamInfo, _headers, rangeHeaderValue, () => fileInfo.Open(FileMode.Append), cancellationTokenSource.Token);
            Log?.Info("【{0}】下载完成", streamInfo.Title);
        }

        protected override void UpdateProgress(long total, long? filesize)
        {
            DialogProgress?.Report(total / (double)filesize!);
        }

    }
}
