using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Core;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.Extensions;
using System.Collections.Generic;
using System.Linq;
using M3u8Downloader_H.M3U8.Infos;
using M3u8Downloader_H.Core.M3uCombiners;
using M3u8Downloader_H.M3U8;
using M3u8Downloader_H.M3U8.Extensions;

namespace M3u8Downloader_H.Services
{
    public class DownloadService : IDisposable
    {
        private readonly VideoDownloadClient videoDownloadClient = new(Http.Client);
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


        public static async Task<M3UFileInfo> GetM3U8FileInfo(Uri url, IEnumerable<KeyValuePair<string, string>>? Headers, string? content = default!, M3UKeyInfo? m3UKeyInfo = default!, CancellationToken cancellationToken = default)
        {
            M3UFileReader m3UReader = new();
            if (content is not null)
                return m3UReader.GetM3u8FileInfo(url, content);
            else if (url.IsFile)
            {
                string ext = Path.GetExtension(url.OriginalString).Trim('.');
                return m3UReader.GetM3u8FileInfo(ext, url);
            }
            else
                return await m3UReader.GetM3u8FileInfo(Http.Client, url, Headers, cancellationToken);
        }

        public static M3UFileInfo GetM3U8FileInfo(string content, Uri url)
        {
            M3UFileInfo m3UFileInfo = new M3UFileReader()
                                            .GetM3u8FileInfo(url, content);
            return m3UFileInfo.MediaFiles != null && m3UFileInfo.MediaFiles.Any()
                    ? m3UFileInfo
                    : throw new InvalidDataException($"'{url.OriginalString}' 没有包含任何数据");
        }

        public async Task LiveDownloadAsync(
            Uri uri,
            M3UFileInfo m3UFileInfo,
            IEnumerable<KeyValuePair<string, string>>? Headers,
            string filePath,
            string fileFullName,
            Func<IProgress<double>> getProgress,
            CancellationToken cancellationToken = default
            )
        {
            await EnsureThrottlingAsync(cancellationToken);

            try
            {
                void ClearCallBack(M3UFileInfo m3UFileinfo)
                {
                    if (!settingService.IsCleanUp) return;

                    foreach (var file in Directory.EnumerateFiles(filePath))
                        File.Delete(file);

                }

                await videoDownloadClient.DownloaderFactory.DownloadLiveAsync(settingService.Pluginitem?.FilePath!,
                                                            uri,
                                                            m3UFileInfo,
                                                            filePath,
                                                            fileFullName, // $"{filePath}-{DateTime.Now:yyyyMMddHHmmss}",
                                                            settingService.RecordDuration,
                                                            getProgress(),
                                                            Headers ?? settingService.Headers.ToDictionary(),
                                                            ClearCallBack,
                                                            settingService.SkipRequestError,
                                                            settingService.ForcedMerger,
                                                            cancellationToken);


                RemoveCacheDirectory(filePath, false);
            }
            finally
            {
                Interlocked.Decrement(ref _concurrentDownloadCount);
            }
        }

        public async Task DownloadAsync(
            M3UFileInfo m3UFileInfo,
            IEnumerable<KeyValuePair<string, string>>? Headers,
            string filePath,
            Func<IProgress<double>> getProgress,
            CancellationToken cancellationToken = default)
        {
            await EnsureThrottlingAsync(cancellationToken);

            try
            {
                int count = 0;
                while(true)
                {
                    try
                    {
                        await videoDownloadClient.DownloaderFactory.DownloadVodAsync(settingService.Pluginitem?.FilePath!,
                                                                m3UFileInfo,
                                                                filePath,
                                                                settingService.MaxThreadCount,
                                                                settingService.Timeouts,
                                                                getProgress(),
                                                                Headers ?? settingService.Headers.ToDictionary(),
                                                                settingService.SkipRequestError,
                                                                cancellationToken);
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


        public async Task VideoMerge(M3UFileInfo m3UFileInfo, string fileFullPath, string videoName, bool isFile)
        {
            if (m3UFileInfo.MediaFiles.Count < 2) return;

            await CombinerFactory.VideoMerge(m3UFileInfo, fileFullPath, videoName, isFile, settingService.ForcedMerger);

            RemoveCacheDirectory(fileFullPath);
        }


        private void RemoveCacheDirectory(string filePath, bool recursive = true)
        {
            try
            {
                if (settingService.IsCleanUp)
                    Directory.Delete(filePath, recursive);
            }
            catch (DirectoryNotFoundException)
            {

            }
        }


        public async Task ConvertToMp4(string OriginvideoName, string videoName, IProgress<double> progress,CancellationToken cancellationToken =default)
        {
            string fileExtension = Path.GetExtension(OriginvideoName).Trim('.');
            if (!(fileExtension != "mp4" && settingService.SelectedFormat == "mp4")) return;

            await CombinerFactory.Converter(OriginvideoName, settingService.SelectedFormat, videoName, true, progress, cancellationToken);
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
