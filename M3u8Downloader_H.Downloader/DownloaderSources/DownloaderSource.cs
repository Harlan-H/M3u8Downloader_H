using M3u8Downloader_H.Combiners.Interfaces;
using M3u8Downloader_H.Common.Interfaces;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Downloader.M3uDownloaders;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.Settings.Models;

namespace M3u8Downloader_H.Downloader.DownloaderSources
{
    internal abstract class DownloaderSource(IDownloadService? downloadService) : IDownloaderSource
    {
        private bool _firstTimeToRun = true;
        private readonly IDownloadService? downloadService = downloadService;
        public IEnumerable<KeyValuePair<string, string>>? Headers { get; set; } = default!;
        public M3UFileInfo M3UFileInfo { get; set; } = default!;
        public IDownloadParams DownloadParams { get; set; } = default!;
        public ISettings Settings { get; set; } = default!;
        public IProgress<long> DownloadRate { get ; set ; } = default!;

        public Func< Uri,IEnumerable<KeyValuePair<string, string>>?, CancellationToken, Task<M3UFileInfo>> GetLiveFileInfoFunc { get;  set ; } = default!;
        public ILog? Log { get ; set ; }

        protected M3u8Downloader CreateDownloader()
        {
            M3u8Downloader  m3U8Downloader = downloadService is not null
                ? new PluginM3u8Downloader(downloadService, M3UFileInfo)
                : M3UFileInfo.Key is not null
                ? new CryptM3uDownloader(M3UFileInfo)
                : new M3u8Downloader();
            m3U8Downloader.Log = Log;   
            return m3U8Downloader;
        }

        public virtual Task DownloadAsync(Action<bool> IsLiveAction,CancellationToken cancellationToken = default)
        {
            if (_firstTimeToRun)
            {
                CreateDirectory(DownloadParams.VideoFullPath);
                _firstTimeToRun = false;
            }
            
            return Task.CompletedTask;
        }

        protected void CreateDirectory(string dirPath)
        {
            DirectoryInfo directoryInfo = new(dirPath);
            if (directoryInfo.Exists)
            {
                Log?.Info("找到缓存目录:{0},开始继续下载", dirPath);
                return;
            }
            directoryInfo.Create();
            Log?.Info("创建缓存目录:{0}", dirPath);
        }

    }
}
