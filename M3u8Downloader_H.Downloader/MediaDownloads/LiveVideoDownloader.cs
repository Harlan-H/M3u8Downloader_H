using System.Threading;
using M3u8Downloader_H.Abstractions.Common;
using Newtonsoft.Json.Linq;

namespace M3u8Downloader_H.Downloader.MediaDownloads
{
    internal class LiveVideoDownloader(HttpClient httpClient) : DownloaderBase(httpClient)
    {
        public override async Task DownloadAsync(IStreamInfo streamInfo, CancellationToken cancellationToken = default)
        {
            await base.DownloadAsync(streamInfo, cancellationToken);
            DialogProgress.SetDownloadStatus(true);
            DialogProgress.IncProgressNum(true);

            string mediaPath = Path.Combine(_cachePath, $"{DownloadParam.VideoName}.{streamInfo.MediaType}");

            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(DownloaderSetting.RecordDuration));
            try
            {
                await DownloadAsynInternal(streamInfo, _headers, null, () => File.Create(mediaPath), cancellationTokenSource.Token);
            }catch(OperationCanceledException) when(cancellationTokenSource.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                Log?.Info("已录制{0},录制结束", TimeSpan.FromSeconds(DownloaderSetting.RecordDuration).ToString());
            }
        }

        protected override void UpdateProgress(long total, long? filesize)
        {

        }
    }
}
