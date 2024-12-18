using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.Common.Interfaces;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.Extensions;
using M3u8Downloader_H.M3U8.M3UFileReaders;
using M3u8Downloader_H.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.M3UFileReaderManangers
{
    public class M3UFileReaderManager(IM3uFileReader? M3UFileReader, HttpClient httpClient, IDictionary<string, IAttributeReader>? attributeReaders = default!) : IM3UFileInfoMananger
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly M3UFileReaderWithStream _m3UFileReaderWithStream = M3UFileReaderFactory.CreateM3UFileReader(M3UFileReader, attributeReaders);

        public TimeSpan TimeOuts { get; set; } = TimeSpan.FromSeconds(10);
        public ILog? Log { get; set; }

        public async Task<M3UFileInfo> GetM3u8FileInfo(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationTokenSource.CancelAfter(TimeOuts);
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var m3u8FileInfo = await GetM3u8FileInfoInternal(uri, headers, cancellationTokenSource.Token);
                    return checkM3u8FileInfo(m3u8FileInfo, uri);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    Log?.Warn("获取m3u8信息超过{0}秒，重试第{1}次", TimeOuts.Seconds, i + 1);
                    await Task.Delay(2000, cancellationTokenSource.Token);
                    continue;
                }
            }

            throw new InvalidOperationException($"'{uri.OriginalString}' 请求失败，请检查网络是否可以访问");
        }


        public M3UFileInfo GetM3u8FileInfo(string ext, Uri uri)
        {
            M3UFileInfo m3UFileInfo = ext switch
            {
                "xml" => new M3UFileReaderWithXml().GetM3u8FileInfo(uri),
                "json" => new M3UFileReaderWithJson().GetM3u8FileInfo(uri),
                "" => new M3UFileReaderWithDirectory().GetM3u8FileInfo(uri,(Stream)null!),
                "m3u8" => _m3UFileReaderWithStream.GetM3u8FileInfo(uri),
                _ => throw new InvalidOperationException("请确认是否为.m3u8或.json或.xml或文件夹"),
            };
            return checkM3u8FileInfo(m3UFileInfo, uri);
        }

        private M3UFileInfo checkM3u8FileInfo(M3UFileInfo m3u8FileInfo,Uri uri)
        {
            if (m3u8FileInfo.MediaFiles != null && m3u8FileInfo.MediaFiles.Any())
                return m3u8FileInfo;
            else
                throw new InvalidDataException($"'{uri.OriginalString}' 没有包含任何数据"); 
        }

        protected virtual Task<(Uri?,Stream)> GetM3u8FileStreamAsync(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            return _httpClient.GetStreamAndUriAsync(uri, headers, cancellationToken);
        }


        protected async Task<M3UFileInfo> GetM3u8FileInfoInternal(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            (Uri? requestUrl, Stream stream) = await _httpClient.GetStreamAndUriAsync(uri, headers, cancellationToken);
            M3UFileInfo m3uFileInfo = _m3UFileReaderWithStream.GetM3u8FileInfo(requestUrl ?? uri, stream);
            if (m3uFileInfo.Streams != null && m3uFileInfo.Streams.Any())
            {
                M3UStreamInfo m3UStreamInfo = m3uFileInfo.Streams.Count > 1 ? m3uFileInfo.Streams.OrderByDescending(s => s.Bandwidth).First() : m3uFileInfo.Streams.First();
                return await GetM3u8FileInfoInternal(m3UStreamInfo.Uri, headers, cancellationToken);
            }
            return m3uFileInfo;
        }
    }
}
