using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Downloader.Extensions;
using M3u8Downloader_H.Downloader.Utils;
using System.Security.Cryptography;

namespace M3u8Downloader_H.Downloader.M3uDownloaders
{
    internal class M3u8Downloader : IDownloadService
    {
        private readonly Lock balanceLock = new();
        private readonly Lock countLock = new();
        private readonly IDownloadContext context;
        private readonly IDialogProgress dialogProgress;
        private int downloadedCount;
        private int CurIndex = -1;

        private bool _firstTimeToRun = true;

        protected IEnumerable<KeyValuePair<string, string>>? _headers => context.DownloadParam.Headers ?? context.DownloaderSetting.Headers;
        protected string _cachePath => context.DownloadParam.CachePath;

        public Func<Stream, CancellationToken, Stream> HandleDataFunc { get; set; } = default!;
        public Func<string, Stream, CancellationToken, Task> WriteToFileFunc { get; set; } = default!;

        protected bool _isFmp4 = false;

        public M3u8Downloader(IDownloadContext context, IDialogProgress DialogProgress)
        {
            dialogProgress = DialogProgress;
            this.context = context;
            HandleDataFunc = HandleData;
            WriteToFileFunc = WriteToFileAsync;
        }

        public ValueTask Initialization(CancellationToken cancellationToken = default)
        {
            downloadedCount = 0;
            CurIndex = -1;

            if (_firstTimeToRun)
            {
                CreateDirectory(_cachePath);
                _firstTimeToRun = false;
            }

            return ValueTask.CompletedTask;
        }

        public async ValueTask DownloadMapInfoAsync(IM3uMediaInfo? m3UMapInfo, CancellationToken cancellationToken = default)
        {
            if (m3UMapInfo is null)
                return;

            _isFmp4 = true;
            string mediaPath = Path.Combine(_cachePath, m3UMapInfo.Title);
            FileInfo fileInfo = new(mediaPath);
            if (fileInfo.Exists && fileInfo.Length > 0)
                return;

            bool isSuccessful = await DownloadM3uMediaInfo(m3UMapInfo, _headers, mediaPath, cancellationToken);
            if (isSuccessful == false)
                throw new InvalidOperationException($"获取map失败,地址为:{m3UMapInfo.Uri.OriginalString}");
            context.Log?.Info("fmp4格式视频,获取map信息完成");
        }


        public async Task StartDownload(IM3uFileInfo m3UFileInfo,  CancellationToken cancellationToken = default)
        {
            dialogProgress.SetDownloadStatus(false);

            await DownloadMapInfoAsync(m3UFileInfo.Map, cancellationToken);

            Task[] Tasks = new Task[context.DownloaderSetting.MaxThreadCount];
            try
            {
                for (int i = 0; i < context.DownloaderSetting.MaxThreadCount; i++)
                {
                    Tasks[i] = DownloadCallBack(m3UFileInfo, _cachePath, _headers,  cancellationToken);
                }

                context.Log?.Info("{0}条线程已开启", context.DownloaderSetting.MaxThreadCount);
                await Task.WhenAll(Tasks);

                context.Log?.Info("下载完成");
            }
            finally
            {
                foreach (Task item in Tasks)
                {
                    item?.Dispose();
                }
                context.Log?.Info("{0}条线程已停止", context.DownloaderSetting.MaxThreadCount);
            }

            if (context.DownloaderSetting.SkipRequestError == false && downloadedCount != m3UFileInfo.MediaFiles.Count)
                throw new DataMisalignedException($"下载数量不完整,完成数{downloadedCount}/{m3UFileInfo.MediaFiles.Count}");
        }

        private async Task DownloadCallBack(IM3uFileInfo m3UFileInfo, string savepath, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken token)
        {
            int localIndex;
            while (true)
            {
                lock (balanceLock)
                {
                    localIndex = ++CurIndex;
                }

                if (localIndex >= m3UFileInfo.MediaFiles.Count)
                {
                    break;
                }

                IM3uMediaInfo mediaInfo = m3UFileInfo.MediaFiles[localIndex];
                string mediaPath = Path.Combine(savepath, mediaInfo.Title);
                FileInfo fileInfo = new(mediaPath);
                if (fileInfo.Exists && fileInfo.Length > 0)
                {
                    _ = Interlocked.Increment(ref downloadedCount);
                    continue;
                }


                bool isSuccessful = await DownloadM3uMediaInfo(mediaInfo, headers, mediaPath, token);
                if (isSuccessful)
                {
                    lock (countLock)
                    {
                        downloadedCount++;
                        dialogProgress.Report(downloadedCount / (double)m3UFileInfo.MediaFiles.Count);
                    }
                }
            }
        }

        public async Task<bool> DownloadM3uMediaInfo(IM3uMediaInfo m3UMediaInfo, IEnumerable<KeyValuePair<string, string>>? headers, string mediaPath, CancellationToken token)
        {
            bool IsSuccessful = false;
            for (int i = 0; i < context.DownloaderSetting.RetryCount; i++)
            {
                try
                {
                    using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
                    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(context.DownloaderSetting.Timeouts));

                    Stream tmpstream = await context.HttpClient.GetResponseContentAsync(m3UMediaInfo.Uri, headers, m3UMediaInfo.RangeValue, cancellationTokenSource.Token);
                    using Stream stream = HandleDataFunc(new HandleStreamInternal(tmpstream, dialogProgress),  cancellationTokenSource.Token);

                    await WriteToFileFunc(mediaPath, stream, cancellationTokenSource.Token);
                    IsSuccessful = true;
                    break;
                }
                catch (OperationCanceledException) when (!token.IsCancellationRequested)
                {
                    context.Log?.Warn("{0} 请求超时，重试第{1}次", m3UMediaInfo.Uri.OriginalString, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
                catch (AggregateException ex) when (ex.InnerException is not InvalidDataException)
                {
                    if (ex.InnerException is CryptographicException)
                    {
                        throw new CryptographicException("解密失败,请确认key,iv是否正确");
                    }
                    context.Log?.Warn("{0} 遇到异常:{0},重试第{1}次", m3UMediaInfo.Uri.OriginalString, ex.Message, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
                catch (IOException ioex)
                {
                    context.Log?.Warn("{0} 遇到io异常{1}，重试第{2}次", m3UMediaInfo.Uri.OriginalString, ioex.Message, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
                catch (HttpRequestException) when (context.DownloaderSetting.SkipRequestError)
                {
                    context.Log?.Warn("{0} 请求失败,以跳过错误，重试第{1}次", m3UMediaInfo.Uri.OriginalString, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
            }
            if (!IsSuccessful)
                DeleteFileWhenTimeOut(mediaPath);
            return IsSuccessful;
        }


        public Stream HandleData(Stream stream,  CancellationToken cancellationToken = default)
        {
            HandleStreamInternal handleImageStream = (HandleStreamInternal)stream;
            if (_isFmp4 is false)
            {
                Task t = handleImageStream.InitializePositionAsync(2000, cancellationToken);
                t.Wait(cancellationToken);
            }
            return handleImageStream;
        }

        public async Task WriteToFileAsync(string file, Stream stream, CancellationToken token = default)
        {
            using FileStream fileobject = File.Create(file);
            await stream.CopyToAsync(fileobject, token);
        }


        private void DeleteFileWhenTimeOut(string file)
        {
            FileInfo fileInfo = new(file);
            if (fileInfo.Exists && fileInfo.Length > 0)
            {
                fileInfo.Delete();
                context.Log?.Warn("重试达到上限，删除未完成的文件: {0}", file);
            }
        }

        private void CreateDirectory(string dirPath)
        {
            DirectoryInfo directoryInfo = new(dirPath);
            if (directoryInfo.Exists)
            {
                context.Log?.Info("找到缓存目录:{0},开始继续下载", dirPath);
                return;
            }
            directoryInfo.Create();
            context.Log?.Info("创建缓存目录:{0}", dirPath);
        }

    }
}
