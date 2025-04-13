using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Downloader;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Settings;
using M3u8Downloader_H.Combiners;
using M3u8Downloader_H.Common.Models;
using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.Downloader;

namespace M3u8Downloader_H.Core.Downloads
{
    public partial class MediaDownloader : IDownloader
    {
        private readonly IMediaDownloadParam mediaDownloadParam;
        private readonly IDownloaderSetting downloaderSetting;
        private readonly ILog log;
        private DownloaderClient m3UDownloaderClient = default!;
        private M3uCombinerClient m3UCombinerClient = default!;

        private bool _isDownloaded = false;

        public MediaDownloader(IMediaDownloadParam mediaDownloadParam, IDownloaderSetting downloaderSetting,ILog log)
        {
            this.mediaDownloadParam = mediaDownloadParam;
            this.downloaderSetting = downloaderSetting;
            this.log = log;
        }

        public async ValueTask StartDownload(Action<int> StateAction, IDialogProgress dialogProgress, CancellationToken cancellationToken)
        {
            StateAction.Invoke((int)DownloadStatus.Enqueued);
            using var acquire = dialogProgress.Acquire();

            await DownloadAsync(dialogProgress, cancellationToken);

            await MergeAsync(dialogProgress, cancellationToken);

            IMergeSetting mergeSetting = (IMergeSetting)downloaderSetting;
            if (mergeSetting.IsCleanUp)
                DirectoryEx.DeleteCache(mediaDownloadParam.CachePath);
        }

        private async ValueTask DownloadAsync(IDialogProgress downloadProgress, CancellationToken cancellationToken)
        {
            if (_isDownloaded)
                return;

            m3UDownloaderClient.DialogProgress = downloadProgress;
            foreach (var media in mediaDownloadParam.Medias)
            {
                await m3UDownloaderClient.MediaDownloader.DownloadAsync(media, cancellationToken);
            }
            _isDownloaded = true;
        }

        private async ValueTask MergeAsync(IDialogProgress downloadProgress, CancellationToken cancellationToken)
        {
            await m3UCombinerClient.FFmpeg.ConvertToMp4(mediaDownloadParam.Medias, downloadProgress, cancellationToken);
        }

    }

    public partial class MediaDownloader
    {
        public static MediaDownloader CreateMediaDownloader(
            HttpClient httpClient,
            IMediaDownloadParam m3U8DownloadParam,
            IDownloaderSetting downloaderSetting,
            ILog logger)
        {
            MediaDownloader mediaDownloader = new(m3U8DownloadParam, downloaderSetting, logger)
            {
                m3UDownloaderClient = new DownloaderClient(httpClient, null, logger, m3U8DownloadParam, downloaderSetting),
                m3UCombinerClient = new M3uCombinerClient(logger, m3U8DownloadParam, (IMergeSetting)downloaderSetting)
            };
            return mediaDownloader;
        }
    }
}
