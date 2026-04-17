using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.M3U8.Extensions;
using M3u8Downloader_H.M3U8.M3UFileReaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.M3UFileReaderManangers
{
    public class M3UFileReaderManager(IDownloadContext context, IM3uFileReader M3u8FileReader)
    {
        internal IM3u8DownloadParam DownloadParam => (IM3u8DownloadParam)context.DownloadParam;
        internal IEnumerable<KeyValuePair<string, string>>? headers => context.DownloadParam.Headers ?? context.DownloaderSetting.Headers;

        internal TimeSpan TimeOuts { get; set; } = TimeSpan.FromSeconds(15);

        public async Task<IM3uFileInfo> GetM3u8FileInfo(TimeSpan timeouts, CancellationToken cancellationToken = default)
        {
            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationTokenSource.CancelAfter(timeouts);

            return await GetM3u8FileInfoInternal(DownloadParam.RequestUrl, DownloadParam.Headers ?? context.DownloaderSetting.Headers, cancellationTokenSource.Token);
        }

        public async Task<IM3uFileInfo> GetM3u8FileInfo(CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < 5; i++)
            {
                using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cancellationTokenSource.CancelAfter(TimeOuts);
                try
                {
                    return await GetM3u8FileInfoInternal(DownloadParam.RequestUrl, headers, cancellationTokenSource.Token);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    context.Log?.Warn("获取m3u8信息超过{0}秒，重试第{1}次", TimeOuts.Seconds, i + 1);
                    await Task.Delay(2000, cancellationTokenSource.Token);
                    continue;
                }
            }

            throw new InvalidOperationException($"'{DownloadParam.RequestUrl.OriginalString}' 请求失败，请检查网络是否可以访问");
        }

        protected async Task<IM3uFileInfo> GetM3u8FileInfoInternal(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            Stream stream = await context.HttpClient.GetStreamAsync(uri, headers, cancellationToken);
            IM3uFileInfo m3uFileInfo = M3u8FileReader.GetM3u8FileInfo(stream);
            if (m3uFileInfo.Streams != null && m3uFileInfo.Streams.Any())
            {
                IM3uStreamInfo m3UStreamInfo = m3uFileInfo.Streams.Count > 1 ? m3uFileInfo.Streams.OrderByDescending(s => s.Bandwidth).First() : m3uFileInfo.Streams.First();
                return await GetM3u8FileInfoInternal(m3UStreamInfo.Uri, headers, cancellationToken);
            }
            return m3uFileInfo;
        }
    }
}
