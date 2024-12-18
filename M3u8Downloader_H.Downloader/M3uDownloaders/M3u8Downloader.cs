using System.Net.Http.Headers;
using System.Security.Cryptography;
using M3u8Downloader_H.Common.Interfaces;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Downloader.Extensions;
using M3u8Downloader_H.Downloader.Utils;

namespace M3u8Downloader_H.Downloader.M3uDownloaders
{
    internal class M3u8Downloader : IM3uDownloader
    {
        private readonly object balanceLock = new();
        private int downloadedCount;
        private double recordDuration;
        private int CurIndex = -1;

        public ILog? Log { get; set; }
        public HttpClient HttpClient { get; set; } = default!;
        public IEnumerable<KeyValuePair<string, string>>? Headers { get; set; } = default;
        public IProgress<double> Progress { get; set; } = default!;
        public IProgress<long> DownloadRate { get; set; } = default!;
        public int RetryCount { get; set; } = default!;

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(10);
        public M3u8Downloader()
        {

        }

        public virtual ValueTask Initialization(CancellationToken cancellationToken) => ValueTask.CompletedTask;


        public async ValueTask DownloadMapInfoAsync(M3UMediaInfo? m3UMapInfo, string savePath, bool skipRequestError = false, CancellationToken cancellationToken = default)
        {
            if (m3UMapInfo is null)
                return;

            string mediaPath = Path.Combine(savePath, m3UMapInfo.Title);
            FileInfo fileInfo = new(mediaPath);
            if (fileInfo.Exists && fileInfo.Length > 0)
                return;

            bool isSuccessful = await DownloadAsynInternal(m3UMapInfo.Uri, Headers, m3UMapInfo.RangeValue, mediaPath, skipRequestError, cancellationToken);
            if (isSuccessful == false)
                throw new InvalidOperationException($"获取map失败,地址为:{m3UMapInfo.Uri.OriginalString}");
            Log?.Info("fmp4格式视频,获取map信息完成");
        }

        public async Task Start(M3UFileInfo m3UFileInfo, int taskNumber, string filePath, int reserve0, bool skipRequestError = false, CancellationToken cancellationToken = default)
        {
            Task[] Tasks = new Task[taskNumber];
            try
            {
                for (int i = 0; i < taskNumber; i++)
                {
                    Tasks[i] = DownloadCallBack(m3UFileInfo, filePath, Headers, skipRequestError, cancellationToken);
                }

                Log?.Info("{0}条线程已开启", taskNumber);
                await Task.WhenAll(Tasks);
            }
            finally
            {
                foreach (Task item in Tasks)
                {
                    item?.Dispose();
                }
                Log?.Info("{0}条线程已停止",taskNumber);
            }

            if (skipRequestError == false && downloadedCount != m3UFileInfo.MediaFiles.Count)
                throw new DataMisalignedException($"下载数量不完整,完成数{downloadedCount}/{m3UFileInfo.MediaFiles.Count}");
        }


        public async Task<double> Start(M3UFileInfo m3UFileInfo, string savePath, int reserve0, bool skipRequestError = false, CancellationToken cancellationToken = default)
        {
            foreach (var mediaFile in m3UFileInfo.MediaFiles)
            {
                string mediaPath = Path.Combine(savePath, mediaFile.Title);
                bool isSuccessful = await DownloadAsynInternal(mediaFile.Uri, Headers, mediaFile.RangeValue, mediaPath, skipRequestError, cancellationToken);
                if(isSuccessful)
                {
                    recordDuration += mediaFile.Duration;
                    Progress.Report(recordDuration);
                }
            }
            return recordDuration;
        }


        private async Task DownloadCallBack(M3UFileInfo m3UFileInfo, string savePath, IEnumerable<KeyValuePair<string, string>>? headers, bool skipRequestError, CancellationToken token)
        {
            int localIndex = 0;
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

                M3UMediaInfo mediaInfo = m3UFileInfo.MediaFiles[localIndex];
                string mediaPath = Path.Combine(savePath, mediaInfo.Title);
                FileInfo fileInfo = new(mediaPath);
                if (fileInfo.Exists && fileInfo.Length > 0)
                {
                    _ = Interlocked.Increment(ref downloadedCount);
                    continue;
                }


                bool isSuccessful = await DownloadAsynInternal(mediaInfo.Uri, headers, mediaInfo.RangeValue, mediaPath, skipRequestError, token);
                if(isSuccessful)
                {
                    _ = Interlocked.Increment(ref downloadedCount);
                    Progress.Report(downloadedCount / (double)m3UFileInfo.MediaFiles.Count);
                }
            }
        }

        protected async Task<bool> DownloadAsynInternal(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, RangeHeaderValue? rangeHeaderValue, string mediaPath, bool skipRequestError, CancellationToken token)
        {
            bool IsSuccessful = false;
            for (int i = 0; i < RetryCount; i++)
            {
                try
                {
                    using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
                    cancellationTokenSource.CancelAfter(TimeOut);
                    (Stream tmpstream, string contentType) = await HttpClient.GetResponseContentAsync(uri, headers, rangeHeaderValue, token);
                    using Stream stream = DownloadAfter(new HandleImageStream(tmpstream, DownloadRate), contentType, token);

                    await WriteToFileAsync(mediaPath, stream, token);
                    IsSuccessful = true;
                    break;
                }
                catch (OperationCanceledException) when (!token.IsCancellationRequested)
                {
                    Log?.Warn("{0} 请求超时，重试第{1}次", uri.OriginalString, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
                catch (AggregateException ex) when(ex.InnerException is not InvalidDataException)
                {
                    if (ex.InnerException is CryptographicException)
                    {
                        throw new CryptographicException("解密失败,请确认key,iv是否正确");
                    }
                    Log?.Warn("{0} 遇到异常:{0},重试第{1}次", uri.OriginalString, ex.Message, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
                catch (IOException)
                {
                    Log?.Warn("{0} 遇到io异常，重试第{1}次", uri.OriginalString, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
                catch (HttpRequestException) when (skipRequestError)
                {
                    Log?.Warn("{0} 请求失败,以跳过错误，重试第{1}次", uri.OriginalString, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
            }
            if (!IsSuccessful)
                DeleteFileWhenTimeOut(mediaPath);
            return IsSuccessful;
        }

        //参数传入带类型的值 在其他操作上来判断是否要调用这个基类
        protected virtual Stream DownloadAfter(Stream stream, string contentType, CancellationToken cancellationToken)
        {
            HandleImageStream handleImageStream =(HandleImageStream)stream;
            Task t = handleImageStream.InitializePositionAsync(2000, cancellationToken);
            t.Wait(cancellationToken);
            return handleImageStream;
        }

        protected static async Task WriteToFileAsync(string file, Stream stream, CancellationToken token)
        {
            using FileStream fileobject = File.Create(file);
            await stream.CopyToAsync(fileobject, token);
        }

        protected void DeleteFileWhenTimeOut(string file)
        {
            FileInfo fileInfo = new(file);
            if (fileInfo.Exists && fileInfo.Length > 0)
            {
                fileInfo.Delete();
                Log?.Warn("重试达到上限，删除未完成的文件: {0}", file);
            }
        }
    }
}
