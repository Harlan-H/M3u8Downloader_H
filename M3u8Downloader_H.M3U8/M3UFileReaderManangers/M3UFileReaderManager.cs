using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.M3UFileReaderManangers
{
    public class M3UFileReaderManager(IDownloadContext context, IM3uFileReader M3u8FileReader)
    {
        private readonly IHttpClientWrapper httpClientWrap = context.HttpClient;
        internal IM3u8DownloadParam DownloadParam => (IM3u8DownloadParam)context.DownloadParam;
        internal IEnumerable<KeyValuePair<string, string>>? headers => context.DownloadParam.Headers ?? context.DownloaderSetting.Headers;


        public async Task<IM3uFileInfo> GetM3u8FileInfo(TimeSpan timeouts, CancellationToken cancellationToken = default)
        {
            return await GetM3u8FileInfoInternal(DownloadParam.RequestUrl, DownloadParam.Headers ?? context.DownloaderSetting.Headers, cancellationToken);
        }

        public async Task<IM3uFileInfo> GetM3u8FileInfo(CancellationToken cancellationToken = default)
        {

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    return await GetM3u8FileInfoInternal(DownloadParam.RequestUrl, headers, cancellationToken);
                }
                catch (TimeoutException ex)
                {
                    context.Log?.Warn("{0}，重试第{1}次", ex.Message, i + 1);
                    await Task.Delay(2000, cancellationToken);
                    continue;
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    context.Log?.Warn("获取m3u8信息超过{0}秒，重试第{1}次", context.DownloaderSetting.Timeouts, i + 1);
                    await Task.Delay(2000, cancellationToken);
                    continue;
                }
            }

            throw new InvalidOperationException($"'{DownloadParam.RequestUrl.OriginalString}' 请求失败，请检查网络是否可以访问");
        }

        protected async Task<IM3uFileInfo> GetM3u8FileInfoInternal(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            await using Stream stream = await httpClientWrap.GetStreamAsync(uri, headers, cancellationToken);
           // await using var timeoutStream = new TimeoutStream(stream, new M3u8Downloader_H.Utilities.Models.TimeOutOptions(context.DownloaderSetting.Timeouts));
            IM3uFileInfo m3uFileInfo = await M3u8FileReader.GetM3u8FileInfo(stream);
            if (m3uFileInfo.Streams != null && m3uFileInfo.Streams.Any())
            {
                IM3uStreamInfo m3UStreamInfo = m3uFileInfo.Streams.Count > 1 ? m3uFileInfo.Streams.OrderByDescending(s => s.Bandwidth).First() : m3uFileInfo.Streams.First();
                return await GetM3u8FileInfoInternal(m3UStreamInfo.Uri, headers, cancellationToken);
            }
            return m3uFileInfo;
        }
    }
}
