using System.Net;
using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.Downloader.MediaDownloads
{
    internal class LiveVideoDownloader(HttpClient httpClient) : DownloaderBase(httpClient)
    {
        private bool updated = false;
        public override async Task DownloadAsync(IStreamInfo streamInfo, CancellationToken cancellationToken = default)
        {
            await base.DownloadAsync(streamInfo, cancellationToken);
            DialogProgress.SetDownloadStatus(true);
            updated = false;

            Log?.Info("直播录制开始");
            string mediaPath = Path.Combine(_cachePath, streamInfo.Title);

            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(DownloaderSetting.RecordDuration));
            try
            {
                await DownloadAsynInternal(streamInfo, _headers, null, () => File.Create(mediaPath), cancellationTokenSource.Token);
            }catch(OperationCanceledException) when(cancellationTokenSource.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                Log?.Info("已录制{0},录制结束", TimeSpan.FromSeconds(DownloaderSetting.RecordDuration).ToString());
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound && updated)
            {
                Log?.Info("地址返回404,直播可能已经结束");
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
