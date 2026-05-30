using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Downloader.Utils;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace M3u8Downloader_H.Downloader.M3uDownloaders
{
    public class SubtitleDownloader : IDownloadService
    {
        private readonly IHttpClientWrapper httpClientWrap;
        private readonly IDownloadService downloadService;
        private readonly IDownloadContext context;

        public Func<Stream, CancellationToken, Task<Stream>> HandleDataFunc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<string, Stream, CancellationToken, Task> WriteToFileFunc { get => downloadService.WriteToFileFunc; set => throw new NotImplementedException(); }
        public Func<IM3uMediaInfo, IEnumerable<KeyValuePair<string, string>>?, string, CancellationToken, Task<bool>> DownloadM3uMediaInfoFunc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SubtitleDownloader(IDownloadService downloadService, IDownloadContext context)
        {
            this.downloadService = downloadService;
            this.context = context;
            httpClientWrap = context.HttpClient;
            downloadService.DownloadM3uMediaInfoFunc = DownloadM3uMediaInfo;
        }

        public ValueTask Initialization(IM3uFileInfoSource m3UFileInfoSource, CancellationToken cancellationToken = default)
                => downloadService.Initialization(m3UFileInfoSource, cancellationToken);

        public Task StartDownload(IM3uFileInfoSource m3UFileInfo, CancellationToken cancellationToken = default)
            => downloadService.StartDownload(m3UFileInfo, cancellationToken);


        public async Task<bool> DownloadM3uMediaInfo(IM3uMediaInfo m3UMediaInfo, IEnumerable<KeyValuePair<string, string>>? headers, string mediaPath, CancellationToken cancellationToken = default)
        {
            bool IsSuccessful = false;
            for (int i = 0; i < context.DownloaderSetting.RetryCount; i++)
            {
                try
                {
                    Stream tmpstream = await httpClientWrap.GetStreamAsync(m3UMediaInfo.Uri, headers, m3UMediaInfo.RangeValue, cancellationToken);
                    await WriteToFileFunc(mediaPath, tmpstream, cancellationToken);

                    IsSuccessful = true;
                    break;
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    context.Log?.Warn("请求超时,重试第{1}次 {0}", m3UMediaInfo.Uri.OriginalString, i + 1);
                    await Task.Delay(2000, cancellationToken);
                    continue;
                }
                catch (TimeoutException e)
                {
                    context.Log?.Warn("{1},重试第{2}次 {0}",  m3UMediaInfo.Uri.OriginalString, e.Message, i + 1);
                    await Task.Delay(2000, cancellationToken);
                    continue;
                }
                catch (AggregateException ex) when (ex.InnerException is not InvalidDataException)
                {
                    if (ex.InnerException is CryptographicException)
                    {
                        throw new CryptographicException("解密失败,请确认key,iv是否正确");
                    }
                    context.Log?.Warn("遇到异常:{1},重试第{2}次 {0}", m3UMediaInfo.Uri.OriginalString, ex.Message, i + 1);
                    await Task.Delay(2000, cancellationToken);
                    continue;
                }
                catch (IOException ioex)
                {
                    context.Log?.Warn("遇到io异常{1}，重试第{2}次 {0}", m3UMediaInfo.Uri.OriginalString, ioex.Message, i + 1);
                    await Task.Delay(2000, cancellationToken);
                    continue;
                }
                catch (HttpRequestException) when (context.DownloaderSetting.SkipRequestError)
                {
                    context.Log?.Warn("请求失败,以跳过错误，重试第{1}次 {0}", m3UMediaInfo.Uri.OriginalString, i + 1);
                    await Task.Delay(2000, cancellationToken);
                    continue;
                }
            }
            return IsSuccessful;
        }
    }
}
