using System.Security.Cryptography;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Downloader.Extensions;
using M3u8Downloader_H.Downloader.Utils;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Extensions;


namespace M3u8Downloader_H.Downloader.M3uDownloaders
{
    public abstract class DownloaderBase(HttpClient httpClient)
    {
        private bool _firstTimeToRun = true;

        internal IDownloadParamBase DownloadParam { get; set; } = default!;
        internal IDownloaderSetting DownloaderSetting { get; set; } = default!;
        internal ILog Log { get; set; } = default!;
        internal IDialogProgress DialogProgress {  get; set; } = default!;       

        protected IEnumerable<KeyValuePair<string, string>>? _headers => DownloadParam.Headers ?? DownloaderSetting.Headers;
        protected string _cachePath => DownloadParam.GetCachePath();

        public async ValueTask DownloadMapInfoAsync(M3UMediaInfo? m3UMapInfo, CancellationToken cancellationToken = default)
        {
            if (m3UMapInfo is null)
                return;

            string mediaPath = Path.Combine(_cachePath, m3UMapInfo.Title);
            FileInfo fileInfo = new(mediaPath);
            if (fileInfo.Exists && fileInfo.Length > 0)
                return;

            bool isSuccessful = await DownloadAsynInternal(m3UMapInfo, _headers, mediaPath, DownloaderSetting.SkipRequestError, cancellationToken);
            if (isSuccessful == false)
                throw new InvalidOperationException($"获取map失败,地址为:{m3UMapInfo.Uri.OriginalString}");
            Log?.Info("fmp4格式视频,获取map信息完成");
        }


        public virtual Task DownloadAsync(M3UFileInfo m3UFileInfo, CancellationToken cancellationToken = default)
        {
            if (_firstTimeToRun)
            {
                CreateDirectory(_cachePath);
                _firstTimeToRun = false;
            }

            return Task.CompletedTask;
        }


        protected async Task<bool> DownloadAsynInternal(M3UMediaInfo m3UMediaInfo, IEnumerable<KeyValuePair<string,string>>? headers, string mediaPath, bool skipRequestError, CancellationToken token)
        {
            bool IsSuccessful = false;
            for (int i = 0; i < DownloaderSetting.RetryCount; i++)
            {
                try
                {
                    using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
                    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(DownloaderSetting.Timeouts));
                    Stream tmpstream = await httpClient.GetResponseContentAsync(m3UMediaInfo.Uri, headers, m3UMediaInfo.RangeValue, token);
                    using Stream stream = DownloadAfter(new HandleStreamInternal(tmpstream, DialogProgress), string.Empty, token);

                    await WriteToFileAsync(mediaPath, stream, token);
                    IsSuccessful = true;
                    break;
                }
                catch (OperationCanceledException) when (!token.IsCancellationRequested)
                {
                    Log?.Warn("{0} 请求超时，重试第{1}次", m3UMediaInfo.Uri.OriginalString, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
                catch (AggregateException ex) when (ex.InnerException is not InvalidDataException)
                {
                    if (ex.InnerException is CryptographicException)
                    {
                        throw new CryptographicException("解密失败,请确认key,iv是否正确");
                    }
                    Log?.Warn("{0} 遇到异常:{0},重试第{1}次", m3UMediaInfo.Uri.OriginalString, ex.Message, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
                catch (IOException)
                {
                    Log?.Warn("{0} 遇到io异常，重试第{1}次", m3UMediaInfo.Uri.OriginalString, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
                catch (HttpRequestException) when (skipRequestError)
                {
                    Log?.Warn("{0} 请求失败,以跳过错误，重试第{1}次", m3UMediaInfo.Uri.OriginalString, i + 1);
                    await Task.Delay(2000, token);
                    continue;
                }
            }
            if (!IsSuccessful)
                DeleteFileWhenTimeOut(mediaPath);
            return IsSuccessful;
        }

        protected virtual Stream DownloadAfter(Stream stream, string contentType, CancellationToken cancellationToken = default)
        {
            HandleStreamInternal handleImageStream = (HandleStreamInternal)stream;
            Task t = handleImageStream.InitializePositionAsync(2000, cancellationToken);
            t.Wait(cancellationToken);
            return handleImageStream;
        }

        protected static async Task WriteToFileAsync(string file, Stream stream, CancellationToken token = default)
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
