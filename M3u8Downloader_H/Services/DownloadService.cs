using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Extensions;
using System.Linq;
using M3u8Downloader_H.M3U8.Infos;
using M3u8Downloader_H.M3U8;
using M3u8Downloader_H.M3U8.Extensions;
using M3u8Downloader_H.Core.DownloaderSources;
using M3u8Downloader_H.Core.DownloaderManagers;

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
            await downloadManager
                .WithHeaders(settingService.Headers.ToDictionary())
                .GetM3U8FileInfo(cancellationToken);
        }


        public static M3UFileInfo GetM3U8FileInfo(string content, Uri url)
        {
            M3UFileInfo m3UFileInfo = new M3UFileReader().GetM3u8FileInfo(url, content);
            return m3UFileInfo.MediaFiles != null && m3UFileInfo.MediaFiles.Any()
                    ? m3UFileInfo
                    : throw new InvalidDataException($"'{url.OriginalString}' 没有包含任何数据");
        }

        public async Task DownloadAsync(
            IDownloaderSource downloaderSource,
            CancellationToken cancellationToken = default)
        {
            await EnsureThrottlingAsync(cancellationToken);

            downloaderSource
                .WithTaskNumber(settingService.MaxThreadCount)
                .WithTimeout(settingService.Timeouts)
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
                int count = 0;
                while(true)
                {
                    try
                    {
                        await downloaderSource.DownloadAsync(cancellationToken);
                        break;
                    }
                    catch (DataMisalignedException) when (count < settingService.RetryCount)
                    {
                        await Task.Delay(3000, cancellationToken);
                        count++;
                        continue;
                    }
                }
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
