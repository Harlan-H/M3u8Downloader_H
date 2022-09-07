using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Core.M3u8Analyzers;
using M3u8Downloader_H.Core.M3uCombiners;
using M3u8Downloader_H.Core.Utils.Extensions;
using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.Core.M3uDownloaders
{
    public class DownloaderFactory
    {
        private readonly HttpClient httpClient;
        private readonly AnalyzerClient analyzerClient;

        public DownloaderFactory(HttpClient httpClient, AnalyzerClient analyzerClient)
        {
            this.httpClient = httpClient;
            this.analyzerClient = analyzerClient;
        }

        private M3u8Downloader CreateDownloader(string pluginPath, M3UFileInfo m3UFileInfo, IProgress<double> progress, IEnumerable<KeyValuePair<string, string>>? headers = default)
        {
            return !string.IsNullOrWhiteSpace(pluginPath)
                ? new PluginM3u8Downloader(pluginPath, httpClient, headers, m3UFileInfo, progress)
                : m3UFileInfo.Key is not null
                ? new CryptM3uDownloader(httpClient, headers, m3UFileInfo, progress)
                : new M3u8Downloader(httpClient, headers, progress);
        }

        public async ValueTask DownloadVodAsync(string pluginPath, M3UFileInfo m3UFileInfo, string filePath, int threadnum,int timeouts, IProgress<double> progress, IEnumerable<KeyValuePair<string, string>>? headers = default, bool skipRequestError = false, CancellationToken cancellationToken = default)
        {
            M3u8Downloader m3U8Downloader = CreateDownloader(pluginPath, m3UFileInfo, progress, headers);
            m3U8Downloader.TimeOut = timeouts * 1000;

            await m3U8Downloader.Initialization(cancellationToken);
            await m3U8Downloader.DownloadMapInfoAsync(m3UFileInfo.Map, filePath, skipRequestError, cancellationToken);
            await m3U8Downloader.Start(m3UFileInfo, threadnum, filePath, 0, skipRequestError, cancellationToken);
        }


        public async ValueTask DownloadLiveAsync(string pluginPath, Uri uri, M3UFileInfo m3UFileInfo, string filePath, string videoName, double maxRecordDuration, IProgress<double> progress, IEnumerable<KeyValuePair<string, string>>? headers, Action<M3UFileInfo> actionCallback, bool skipRequestError = false, bool ForceMerge = false, CancellationToken cancellationToken = default)
        {
            M3u8Downloader m3U8Downloader = CreateDownloader(pluginPath, m3UFileInfo, progress, headers);
            await m3U8Downloader.Initialization(cancellationToken);

            using M3uCombiner m3UCombiner = new(filePath, videoName, FileMode.Append);
            m3UCombiner.Initialization();

            FileInfo fileInfo = new(videoName);
            //当文件存在且大小不为零得情况下说明是旧得任务重新开始得
            if (!(fileInfo.Exists && fileInfo.Length > 0))
            {
                await m3U8Downloader.DownloadMapInfoAsync(m3UFileInfo.Map, filePath, skipRequestError, cancellationToken);
                await m3UCombiner.MegerVideoHeader(m3UFileInfo.Map);
            }

            M3UFileInfo previousMediaInfo = m3UFileInfo;
            double durationCount = 0;
            while (true)
            {
                var duration = await m3U8Downloader.Start(previousMediaInfo, filePath, 0, skipRequestError, cancellationToken);

                //获取得到任何数据都会实时进行合并，同时删除缓存
                await m3UCombiner.Start(previousMediaInfo, ForceMerge);
                actionCallback(previousMediaInfo);

                durationCount += (double)duration;
                if (previousMediaInfo.IsVod() || durationCount > maxRecordDuration)
                    break;

                await Task.Delay(TimeSpan.FromSeconds(previousMediaInfo.MediaFiles.Last().Duration), cancellationToken);

                try
                {
                    previousMediaInfo = await analyzerClient.GetM3u8FileInfos(uri, headers, previousMediaInfo, cancellationToken);
                }
                catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
                {
                    break;
                }
            }
        }
    }
}
