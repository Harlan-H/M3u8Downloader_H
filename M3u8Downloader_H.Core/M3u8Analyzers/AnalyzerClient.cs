using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Core.Utils.Extensions;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.M3U8;
using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.Core.M3u8Analyzers
{
    public class AnalyzerClient
    {
        private readonly HttpClient http;
        private readonly M3UFileReader m3UFileReader;

        public AnalyzerClient(HttpClient http)
        {
            this.http = http;
            m3UFileReader = new M3UFileReader();
        }

        private static M3UFileInfo CheckM3UFileInfo(M3UFileInfo m3UFileInfo, Uri uri)
        {
            return m3UFileInfo.MediaFiles != null && m3UFileInfo.MediaFiles.Any()
                    ? m3UFileInfo
                    : throw new InvalidDataException($"'{uri.OriginalString}' 没有包含任何数据");
        }

        private async Task<M3UFileInfo> Read(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            Stream stream = uri.IsFile ? File.OpenRead(uri.OriginalString) : await http.GetStreamAsync(uri, headers, cancellationToken);
            M3UFileInfo m3uFileInfo = m3UFileReader.Read(uri, stream);
            if(m3uFileInfo.Streams != null && m3uFileInfo.Streams.Any())
            {
                M3UStreamInfo m3UStreamInfo = m3uFileInfo.Streams.Count > 1 ? m3uFileInfo.Streams.OrderByDescending(s => s.Bandwidth).First() : m3uFileInfo.Streams.First();
                return await Read(m3UStreamInfo.Uri, headers, cancellationToken);
            }
            return m3uFileInfo;
        }

        public M3UFileInfo ReadFromContent(Uri uri, string content)
        {
            M3UFileInfo m3UFileInfo = m3UFileReader.Read(uri, content);
            return CheckM3UFileInfo(m3UFileInfo, uri);
        }

        private async ValueTask<M3UFileInfo> ReadFromFile(Uri uri)
        {
            string ext = Path.GetExtension(uri.OriginalString).Trim('.');
            M3UFileInfo m3UFileInfo = ext switch
            {
                "xml" => (M3UFileInfo)AnalyzerFactory.CreateXmlAnalyzer(uri),
                "json" => (M3UFileInfo)AnalyzerFactory.CreateJsonAnalyzer(uri),
                //字符串为空 说明是文件夹
                "" => (M3UFileInfo)AnalyzerFactory.CreateDirectoryAnalyzer(uri),
                "m3u8" => await Read(uri, null),
                _ => throw new InvalidOperationException("请确认是否为.m3u8或.json或.xml或.ts或文件夹"),
            };
            return CheckM3UFileInfo(m3UFileInfo, uri);
        }

        public async ValueTask<M3UFileInfo> GetM3u8FileInfos(Uri url, IEnumerable<KeyValuePair<string, string>>? Headers, string? content = default!, M3UKeyInfo? m3UKeyInfo = default!, CancellationToken cancellationToken = default)
        {
            var m3u8fileinfo  = content != null
                ? ReadFromContent(url, content)
                : url.IsFile
                    ? await ReadFromFile(url)
                    : await GetM3u8FileInfos(url, Headers, cancellationToken);

            if (m3UKeyInfo is not null)
                m3u8fileinfo.Key = m3UKeyInfo;

            return m3u8fileinfo;
        }

        private async Task<M3UFileInfo> GetM3u8FileInfos(Uri url, IEnumerable<KeyValuePair<string, string>>? Headers, CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var m3u8FileInfo = await Read(url, Headers, cancellationToken);
                    return CheckM3UFileInfo(m3u8FileInfo, url);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(2000, cancellationToken);
                    continue;
                }
            }

            throw new InvalidOperationException($"'{url.OriginalString}' 请求失败，请检查网络是否可以访问");
        }


        private async Task<M3UFileInfo> GetLiveFileInfos(Uri url, IEnumerable<KeyValuePair<string, string>>? Headers, CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    return await GetM3u8FileInfos(url, Headers, cancellationToken);
                }
                catch(HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound && i < 3)
                {
                    await Task.Delay(2000, cancellationToken);
                    continue;
                }
            }

            throw new InvalidOperationException($"'{url.OriginalString}' 请求失败，请检查网络是否可以访问");
        }

        /// <summary>
        /// 获取新得m3u8得同时与原始得m3u8数据进行比较，返回与原始数据不同得新数据
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="Headers">请求头</param>
        /// <param name="oldM3u8FileInfo">原始数据</param>
        /// <param name="cancellationToken">token</param>
        /// <returns>新得m3u8数据</returns>
        /// <exception cref="HttpRequestException">当5次数据都是重复，则判定直播结束</exception>
        public async Task<M3UFileInfo> GetM3u8FileInfos(Uri url, IEnumerable<KeyValuePair<string, string>>? Headers, M3UFileInfo oldM3u8FileInfo, CancellationToken cancellationToken = default)
        {
            bool IsEnded = true;
            M3UFileInfo m3ufileinfo = default!;
            var oldMediafile = oldM3u8FileInfo.MediaFiles.Last();
            for (int i = 0; i < 5; i++)
            {
                m3ufileinfo = await GetLiveFileInfos(url, Headers, cancellationToken);
                //判断新的内容里 上次最后一条数据所在的位置，同时跳过那条数据 取出剩下所有内容
                IEnumerable<M3UMediaInfo> newMediaInfos = m3ufileinfo.MediaFiles.Skip(m => m == oldMediafile).ToList();
                if (!newMediaInfos.Any())
                {
                    //当计数为0 说明新的数据 跟旧的数据完全一致  则延迟上次最后一项数据的Duration
                    double delayTime = m3ufileinfo.MediaFiles.Last().Duration;
                    await Task.Delay(TimeSpan.FromSeconds(delayTime), cancellationToken);
                    continue;
                }
                //防止缓存视频出现重名的问题，重新生成新的名称
                m3ufileinfo.MediaFiles = newMediaInfos.GenerateTitle();
                IsEnded = false;
                break;
            }
            if (IsEnded)
                throw new HttpRequestException("直播结束", null, System.Net.HttpStatusCode.NotFound);
            return m3ufileinfo;
        }
    }
}
