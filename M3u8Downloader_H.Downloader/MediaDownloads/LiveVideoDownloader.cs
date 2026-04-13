using System.Net;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Models;

namespace M3u8Downloader_H.Downloader.MediaDownloads
{
    public class LiveVideoDownloader(IDownloadContext downloadContext) : DownloaderBase(downloadContext)
    {
        private bool updated = false;
        private readonly IDownloadContext downloadContext = downloadContext;
        public override async Task DownloadAsync(IStreamInfo streamInfo, CancellationToken cancellationToken = default)
        {
            await base.DownloadAsync(streamInfo, cancellationToken);
            DialogProgress.SetDownloadStatus(true);
            updated = false;

            downloadContext.Log?.Info("直播录制开始");
            string mediaPath = Path.Combine(_cachePath, streamInfo.Title);

            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationTokenSource.CancelAfter(downloadContext.DownloaderSetting.RecordDuration);
            try
            {
                await DownloadAsynInternal(streamInfo, _headers, null, () => File.Create(mediaPath), cancellationTokenSource.Token);
            }catch(OperationCanceledException) when(cancellationTokenSource.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                downloadContext.Log?.Info("已录制{0},录制结束", downloadContext.DownloaderSetting.RecordDuration);
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound && updated)
            {
                downloadContext.Log?.Info("地址返回404,直播可能已经结束");
            }
        }

        protected override void UpdateProgress(long total, long? filesize)
        {
            if(updated is false && total > 0)
            {
                DialogProgress.IncProgressNum(true);
                updated = true;
            }
        }
    }
}
