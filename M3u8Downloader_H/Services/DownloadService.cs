using System;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Extensions;
using System.Linq;
using M3u8Downloader_H.Core.DownloaderSources;
using M3u8Downloader_H.Core.DownloaderManagers;
using M3u8Downloader_H.Utils;

namespace M3u8Downloader_H.Services
{
    public class DownloadService : IDisposable
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly SettingsService settingService;

        private int _concurrentDownloadCount;
        public DownloadService(SettingsService settingService)
        {
            this.settingService = settingService;
        }

        private async Task EnsureThrottlingAsync(CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                while (_concurrentDownloadCount >= settingService.MaxConcurrentDownloadCount)
                    await Task.Delay(1000, cancellationToken);

                Interlocked.Increment(ref _concurrentDownloadCount);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async ValueTask GetM3u8FileInfo(IDownloadManager downloadManager,CancellationToken cancellationToken)
        {
            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(settingService.Timeouts));
            await downloadManager
                .WithHeaders(settingService.Headers.ToDictionary())
                .GetM3U8FileInfo(cancellationTokenSource.Token);
        }


        public async Task DownloadAsync(
            IDownloaderSource downloaderSource,
            DownloadRateSource downloadRate,
            CancellationToken cancellationToken = default)
        {
            await EnsureThrottlingAsync(cancellationToken);

            downloaderSource
                .WithDownloadRate(downloadRate)
                .WithTaskNumber(settingService.MaxThreadCount)
                .WithRetryCount(settingService.RetryCount)
                .WithSkipRequestError(settingService.SkipRequestError)
                .WithSkipDirectoryExist(settingService.SkipDirectoryExist)
                .WithSavePath(settingService.SavePath)
                .WithForceMerge(settingService.ForcedMerger)
                .WithMaxRecordDuration(settingService.RecordDuration)
                .WithCleanUp(settingService.IsCleanUp)
                .WithFormats(settingService.SelectedFormat)
                .WithHeaders(settingService.Headers.ToDictionary());

            try
            {
                downloadRate.Run();
                using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(settingService.Timeouts));
                await downloaderSource.DownloadAsync(cancellationTokenSource!.Token);
            }
            finally
            {
                Interlocked.Decrement(ref _concurrentDownloadCount);
            }
            
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
