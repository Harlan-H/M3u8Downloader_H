using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Extensions;
using M3u8Downloader_H.Downloader.Extensions;
using System.Buffers;
using System.Net.Http.Headers;


namespace M3u8Downloader_H.Downloader.MediaDownloads
{
    public abstract class DownloaderBase(HttpClient httpClient)
    {
        private bool _firstTimeToRun = true;
        internal IMediaDownloadParam DownloadParam { get; set; } = default!;
        internal IDownloaderSetting DownloaderSetting { get; set; } = default!;
        internal ILog Log { get; set; } = default!;
        internal IDialogProgress DialogProgress { get; set; } = default!;

        protected IEnumerable<KeyValuePair<string, string>>? _headers => DownloadParam.Headers ?? DownloaderSetting.Headers;

        protected string _cachePath => DownloadParam.GetCachePath();

        public virtual Task DownloadAsync(IStreamInfo streamInfo,CancellationToken cancellationToken = default)
        {
            if (_firstTimeToRun)
            {
                CreateDirectory(_cachePath);
                _firstTimeToRun = false;
            }

            return Task.CompletedTask;
        }


        protected async Task DownloadAsynInternal(IStreamInfo streamInfo, IEnumerable<KeyValuePair<string, string>>? headers, RangeHeaderValue? rangeHeaderValue, string mediaPath, CancellationToken token)
        {
            for (int i = 0; i < DownloaderSetting.RetryCount; i++)
            {
                try
                {
                    using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
                    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(DownloaderSetting.Timeouts));
                    
                    using Stream stream = await httpClient.GetResponseContentAsync(streamInfo.Url, headers, rangeHeaderValue, token);
                    using FileStream fileobject = File.Open(mediaPath, FileMode.Append);
                    await WriteToFileAsync(streamInfo, fileobject, stream, token);
                    break;
                }
                catch (OperationCanceledException) when (!token.IsCancellationRequested)
                {
                    Log?.Warn("{0} 请求超时，重试第{1}次", streamInfo.Url.OriginalString, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
                catch (IOException)
                {
                    Log?.Warn("{0} 遇到io异常，重试第{1}次", streamInfo.Url.OriginalString, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
                catch (HttpRequestException)
                {
                    Log?.Warn("{0} 请求失败,以跳过错误，重试第{1}次", streamInfo.Url.OriginalString, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
            }
        }


        protected abstract void UpdateProgress(long total, long? filesize);

        protected async Task WriteToFileAsync(IStreamInfo streamInfo, FileStream fileStream, Stream stream, CancellationToken token = default)
        {
            using var buffer = MemoryPool<byte>.Shared.Rent(0x10000);
            Memory<byte> memory = buffer.Memory;
            long totalBytes = 0L;
            int bytesCopied = 0;
            do
            {
                bytesCopied = await stream.ReadAsync(memory, token);
                if (bytesCopied > 0) await fileStream.WriteAsync(memory[..bytesCopied], token);

                totalBytes += bytesCopied;
                DialogProgress?.Report(bytesCopied);
                UpdateProgress(totalBytes, streamInfo.FileSize!);
            } while (bytesCopied > 0);
        }


        protected void CreateDirectory(string dirPath)
        {
            DirectoryInfo directoryInfo = new(dirPath);
            if (directoryInfo.Exists)
            {
                Log?.Info("找到缓存目录:{0},开始继续下载", dirPath);
                return;
            }
            directoryInfo.Create();
            Log?.Info("创建缓存目录:{0}", dirPath);
        }

    }
}
