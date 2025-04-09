using System.Net;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Downloader.Extensions;

namespace M3u8Downloader_H.Downloader.M3uDownloaders
{
    internal class LiveM3uDownloader(HttpClient httpClient): DownloaderBase(httpClient)
    {
        private static readonly Random _rand = Random.Shared;
        private static readonly TimeSpan _timeoutTimeSpan = TimeSpan.FromSeconds(3);
        private bool _firstTimeToRun = true;
        private IM3uFileInfo? _m3uFileInfo;
        private float recordDuration;
        private long _index;

        public Func<TimeSpan,CancellationToken, Task<IM3uFileInfo>> GetLiveFileInfoFunc { get; set; } = default!;

        private void AddMedias(IM3uFileInfo m3UFileinfo)
        {
            if (ReferenceEquals(m3UFileinfo, _m3uFileInfo))
                return;

            foreach (var item in m3UFileinfo.MediaFiles)
            {
                _m3uFileInfo!.MediaFiles.Add(item);
            }
        }

        public List<IM3uMediaInfo> RenameTitle(IEnumerable<M3UMediaInfo> m3UMediaInfos)
        {
            if (m3UMediaInfos is null)
                throw new InvalidDataException("无效的直播数据流");

            foreach (var item in m3UMediaInfos)
            {
                item.Title = $"{++_index}.tmp";
            }
            return m3UMediaInfos.Cast<IM3uMediaInfo>().ToList();
        }

        private async ValueTask<IM3uFileInfo> GetM3U8FileInfoAsync(CancellationToken cancellationToken = default)
        {
            if (_firstTimeToRun)
            {
                RenameTitle(_m3uFileInfo!.MediaFiles.Cast<M3UMediaInfo>());
                _firstTimeToRun = false;
                return _m3uFileInfo!;
            }


            var m3uFileInfo = await GetLiveFileInfos(cancellationToken);
            RenameTitle(m3uFileInfo.MediaFiles.Cast<M3UMediaInfo>());
            return m3uFileInfo;
        }

        public override async Task DownloadAsync(IM3uFileInfo m3UFileInfo,  CancellationToken cancellationToken = default)
        {
            await base.DownloadAsync(m3UFileInfo,  cancellationToken);
            DialogProgress.SetDownloadStatus(true);

            Log?.Info("直播录制开始");
            _m3uFileInfo ??= m3UFileInfo;

            await DownloadMapInfoAsync(m3UFileInfo.Map, cancellationToken);

            IM3uFileInfo previousMediaInfo = await GetM3U8FileInfoAsync(cancellationToken);
            while (true)
            {
                var duration = await Start(previousMediaInfo, _headers, cancellationToken);
                AddMedias(previousMediaInfo);

                if (previousMediaInfo.IsVod() || duration > DownloaderSetting.RecordDuration)
                {
                    Log?.Info("已录制{0},录制结束", TimeSpan.FromSeconds(duration).ToString());
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(previousMediaInfo.MediaFiles.Last().Duration), cancellationToken);

                try
                {
                    previousMediaInfo = await GetM3u8FileInfos(previousMediaInfo, cancellationToken);
                }
                catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
                {
                    break;
                }
            }
        }

        public async Task<double> Start(IM3uFileInfo m3UFileInfo, IEnumerable<KeyValuePair<string, string>>? Headers, CancellationToken cancellationToken = default)
        {
            foreach (var mediaFile in m3UFileInfo.MediaFiles)
            {
                string mediaPath = Path.Combine(_cachePath, mediaFile.Title);
                bool isSuccessful = await DownloadAsynInternal(mediaFile, Headers,  mediaPath,  cancellationToken);
                if(isSuccessful)
                {
                    recordDuration += mediaFile.Duration;
                    DialogProgress.Report(recordDuration);
                }
            }
            return recordDuration;
        }

        private async Task<M3UFileInfo> GetLiveFileInfos(CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    IM3uFileInfo m3UFileInfo = await GetLiveFileInfoFunc(_timeoutTimeSpan, cancellationToken);
                    return (M3UFileInfo)m3UFileInfo;
                }
                catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound && i < 4)
                {
                    Log?.Warn("获取直播数据失败,网页返回代码:{0},返回内容:{1},正在进行第{2}次重试", e.StatusCode, e.Message, i + 1);
                    await Task.Delay(2000, cancellationToken);
                    continue;
                }
            }

            throw new InvalidOperationException($"请求失败，请检查网络是否可以访问");
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
        public async Task<IM3uFileInfo> GetM3u8FileInfos(IM3uFileInfo oldM3u8FileInfo, CancellationToken cancellationToken = default)
        {
            bool IsEnded = true;
            M3UFileInfo m3ufileinfo = default!;
            var oldMediafile = oldM3u8FileInfo.MediaFiles.Cast<M3UMediaInfo>().Last();
            for (int i = 0; i < 5; i++)
            {
                m3ufileinfo = await GetLiveFileInfos(cancellationToken);
                //判断新的内容里 上次最后一条数据所在的位置，同时跳过那条数据 取出剩下所有内容
                IEnumerable<M3UMediaInfo> newMediaInfos = m3ufileinfo.MediaFiles.Cast<M3UMediaInfo>().Skip(m => m == oldMediafile);
                if (!newMediaInfos.Any())
                {
                    //当newMediaInfos数量为0 说明新的数据 跟旧的数据完全一致  则随机延迟上次某项数据的Duration
                    int index = _rand.Next(m3ufileinfo.MediaFiles.Count);
                    double delayTime = m3ufileinfo.MediaFiles[index].Duration;
                    Log?.Info("本次获取数据和上次完全一致等待{0}秒后继续重试", delayTime);
                    await Task.Delay(TimeSpan.FromSeconds(delayTime), cancellationToken);
                    continue;
                }
                Log?.Info("获取到新的直播数据有{0}个", newMediaInfos.Count());
                //重新修改成新的名称，方便中途停止及后续合并
                m3ufileinfo.MediaFiles = RenameTitle(newMediaInfos);
                IsEnded = false;
                break;
            }
            if (IsEnded)
            {
                Log?.Info("5次请求数据全部一致,判定直播结束");
                throw new HttpRequestException("直播结束", null, System.Net.HttpStatusCode.NotFound);
            }
            return m3ufileinfo;
        }

    }
}
