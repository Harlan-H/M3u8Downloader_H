using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.Downloader.M3uDownloaders
{
    internal class M3u8Downloader(HttpClient httpClient) : DownloaderBase(httpClient)
    {
        private readonly Lock balanceLock = new();
        private readonly Lock countLock = new();
        private int downloadedCount;
        private int CurIndex = -1;

        public virtual ValueTask Initialization(CancellationToken cancellationToken = default) => ValueTask.CompletedTask;


        public override async Task DownloadAsync(IM3uFileInfo m3UFileInfo,  CancellationToken cancellationToken = default)
        {
            await base.DownloadAsync(m3UFileInfo, cancellationToken);
            DialogProgress.SetDownloadStatus(false);

            await Initialization(cancellationToken);
            await DownloadMapInfoAsync(m3UFileInfo.Map, cancellationToken);

            Task[] Tasks = new Task[DownloaderSetting.MaxThreadCount];
            try
            {
                for (int i = 0; i < DownloaderSetting.MaxThreadCount; i++)
                {
                    Tasks[i] = DownloadCallBack(m3UFileInfo, _cachePath, _headers,  cancellationToken);
                }

                Log?.Info("{0}条线程已开启", DownloaderSetting.MaxThreadCount);
                await Task.WhenAll(Tasks);

                Log?.Info("下载完成");
            }
            finally
            {
                foreach (Task item in Tasks)
                {
                    item?.Dispose();
                }
                Log?.Info("{0}条线程已停止", DownloaderSetting.MaxThreadCount);
            }

            if (DownloaderSetting.SkipRequestError == false && downloadedCount != m3UFileInfo.MediaFiles.Count)
                throw new DataMisalignedException($"下载数量不完整,完成数{downloadedCount}/{m3UFileInfo.MediaFiles.Count}");
        }

        private async Task DownloadCallBack(IM3uFileInfo m3UFileInfo, string savepath, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken token)
        {
            int localIndex;
            while (true)
            {
                lock (balanceLock)
                {
                    localIndex = ++CurIndex;
                }

                if (localIndex >= m3UFileInfo.MediaFiles.Count)
                {
                    break;
                }

                IM3uMediaInfo mediaInfo = m3UFileInfo.MediaFiles[localIndex];
                string mediaPath = Path.Combine(savepath, mediaInfo.Title);
                FileInfo fileInfo = new(mediaPath);
                if (fileInfo.Exists && fileInfo.Length > 0)
                {
                    _ = Interlocked.Increment(ref downloadedCount);
                    continue;
                }


                bool isSuccessful = await DownloadAsynInternal(mediaInfo, headers, mediaPath, token);
                if (isSuccessful)
                {
                    lock (countLock)
                    {
                        downloadedCount++;
                        DialogProgress.Report(downloadedCount / (double)m3UFileInfo.MediaFiles.Count);
                    }
                }
            }
        }
    }
}
