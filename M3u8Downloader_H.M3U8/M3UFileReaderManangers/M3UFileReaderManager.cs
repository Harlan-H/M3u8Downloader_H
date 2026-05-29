using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.M3U8.Models;
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
        internal IEnumerable<KeyValuePair<string, string>>? _headers => context.DownloadParam.Headers ?? context.DownloaderSetting.Headers;

        public async Task<IM3uFileInfo> GetM3u8FileInfo(CancellationToken cancellationToken = default)
            => await GetM3u8FileInfo(DownloadParam.RequestUrl, cancellationToken);

        public async Task<IM3uFileInfo> GetM3u8FileInfo(string url, CancellationToken cancellationToken = default)
            => await GetM3u8FileInfo(new Uri(url), cancellationToken);

        public async Task<IM3uFileInfo> GetM3u8FileInfo(Uri requestUrl, CancellationToken cancellationToken = default)
        {

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    return await GetM3u8FileInfoInternal(requestUrl, _headers, cancellationToken);
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

        public IList<IM3uFileInfoSource>? AutoHandleM3uFileInfo(IM3uFileInfo m3UFileInfo)
        {
            List<IM3uFileInfoSource> m3UFileInfoSources = [];
            if (m3UFileInfo.Streams is null && m3UFileInfo.MediaFiles.Any())
            {
                m3UFileInfoSources.Add(new M3uFileInfoSource(m3UFileInfo));
                return m3UFileInfoSources;

            }else if (m3UFileInfo.Streams is not null && m3UFileInfo.Streams.Any())
            {
                var stream = m3UFileInfo.Streams.Count > 1 ? m3UFileInfo.Streams.OrderByDescending(s => s.Bandwidth).First() : m3UFileInfo.Streams.Single();
                m3UFileInfoSources.Add(new M3uFileInfoSource(stream.Uri));

                if (stream.Audio is not null)
                {
                    var audios = m3UFileInfo.Medias?.Where(a => a.Type.ToUpper().Equals("AUDIO")).ToList();
                    if(audios is not null)
                    {
                        var medias = audios.Where(m => m.GroupId == stream.Audio).ToArray();
                        if (medias.Length > 1)
                            return null;

                        var audio = medias.Single();
                        m3UFileInfoSources.Add(new M3uFileInfoSource(audio.Uri, M3uType.AUDIO));
                    }
                }
                
                if (stream.Subtitles is not null)
                {
                    var subtitls = m3UFileInfo.Medias?.Where(s => s.Type.ToUpper().Equals("SUBTITLES")).ToList();
                    if (subtitls is not null)
                    {
                        var subtitlArr = subtitls.Where(m => m.GroupId == stream.Subtitles).ToArray();
                        if (subtitlArr.Length > 1)
                            return null;

                        var subtile = subtitls.Single();
                        m3UFileInfoSources.Add(new M3uFileInfoSource(subtile.Uri, M3uType.SUBTITLE));
                    }
                }
            }
            return m3UFileInfoSources;
        }

        protected async Task<IM3uFileInfo> GetM3u8FileInfoInternal(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            await using Stream stream = await httpClientWrap.GetStreamAsync(uri, headers, cancellationToken);
            IM3uFileInfo m3uFileInfo = await M3u8FileReader.GetM3u8FileInfo(uristream);
//             if (m3uFileInfo.Streams != null && m3uFileInfo.Streams.Any())
//             {
//                 IM3uStreamInfo m3UStreamInfo = m3uFileInfo.Streams.Count > 1 ? m3uFileInfo.Streams.OrderByDescending(s => s.Bandwidth).First() : m3uFileInfo.Streams.First();
//                 return await GetM3u8FileInfoInternal(m3UStreamInfo.Uri, headers, cancellationToken);
//             }
            return m3uFileInfo;
        }
    }
}
