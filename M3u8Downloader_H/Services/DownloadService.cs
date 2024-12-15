using System;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Downloader.DownloaderSources;
using M3u8Downloader_H.Utils;

namespace M3u8Downloader_H.Services
{
    public class DownloadService(SettingsService settingService) : IDisposable
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly SettingsService settingService = settingService;

        private int _concurrentDownloadCount;

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


        public async Task DownloadAsync(
            IDownloaderSource downloaderSource,
            DownloadRateSource downloadRate,
            Action<bool> IsLive,
            CancellationToken cancellationToken = default)
        {
            await EnsureThrottlingAsync(cancellationToken);

            downloaderSource.Settings = settingService;
            downloaderSource.DownloadRate = downloadRate;

            try
            {
                downloadRate.Run();
                await downloaderSource.DownloadAsync(IsLive,cancellationToken);

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
