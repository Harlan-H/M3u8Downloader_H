using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Core.M3uDownloaders;
using M3u8Downloader_H.Core.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.DownloaderSources
{
    internal class DownloadLiveSource : DownloaderSource
    {
        private bool _firstTimeToRun = true;
        private readonly int _downloadStatus = 1;

        private void AddMedias(M3UFileInfo m3UFileinfo)
        {
            if (ReferenceEquals(m3UFileinfo, M3UFileInfo))
                return;

            foreach (var item in m3UFileinfo.MediaFiles)
            {
                M3UFileInfo.MediaFiles.Add(item);
            }
        }

        private async ValueTask<M3UFileInfo> GetM3U8FileInfoAsync(CancellationToken cancellationToken = default)
        {
            if (_firstTimeToRun)
            {
                M3UFileInfo.MediaFiles.GenerateTitle();
                _firstTimeToRun = false;
                return M3UFileInfo!;
            }

            return await GetLiveFileInfos(Url, Headers, cancellationToken);
        }

        public override async Task DownloadAsync(CancellationToken cancellationToken = default)
        {
            await base.DownloadAsync(cancellationToken);
            M3u8Downloader m3U8Downloader = CreateDownloader();
            m3U8Downloader.HttpClient = HttpClient;
            m3U8Downloader.Progress = LiveProgress;
            m3U8Downloader.DownloadRate = _downloadRate;
            m3U8Downloader.Headers = Headers;

            SetStatusDelegate(_downloadStatus);
            
            FileInfo fileInfo = new(VideoFullName);
            //当文件存在且大小不为零得情况下说明是旧得任务重新开始得
            if (!(fileInfo.Exists && fileInfo.Length > 0))
            {
                await m3U8Downloader.Initialization(cancellationToken);
                await m3U8Downloader.DownloadMapInfoAsync(M3UFileInfo.Map, VideoFullPath, _skipRequestError, cancellationToken);
            }

            M3UFileInfo previousMediaInfo = await GetM3U8FileInfoAsync(cancellationToken);
            while (true)
            {
                var duration = await m3U8Downloader.Start(previousMediaInfo, VideoFullPath, 0, _skipRequestError, cancellationToken);
                AddMedias(previousMediaInfo);

                if (previousMediaInfo.IsVod() || duration > _maxRecordDuration)
                    break;

                await Task.Delay(TimeSpan.FromSeconds(previousMediaInfo.MediaFiles.Last().Duration), cancellationToken);

                try
                {
                    previousMediaInfo = await GetM3u8FileInfos(Url, Headers, previousMediaInfo, cancellationToken);
                }
                catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
                {
                    break;
                }
            }

            await Converter(false, cancellationToken);
           
            RemoveCacheDirectory(VideoFullPath);
        }

        private async Task<M3UFileInfo> GetLiveFileInfos(Uri url, IEnumerable<KeyValuePair<string, string>>? Headers, CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    return await M3uReader.GetM3u8FileInfo(url,  Headers, true, cancellationToken);
                }
                catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound && i < 3)
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
                    //当newMediaInfos数量为0 说明新的数据 跟旧的数据完全一致  则延迟上次最后一项数据的Duration
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
