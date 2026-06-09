using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Settings;
using M3u8Downloader_H.Combiners;
using M3u8Downloader_H.Common.Models;
using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.Core.Interfaces;
using M3u8Downloader_H.Downloader;
using M3u8Downloader_H.Progress.Extensions;
using M3u8Downloader_H.Progress.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.Downloads
{
    public partial class MediaDownloader(IDownloadContext downloadContext) : IDownloader
    {
        private readonly IMediaDownloadParam mediaDownloadParam = (IMediaDownloadParam)downloadContext.DownloadParam;
        private readonly IDownloaderSetting downloaderSetting = downloadContext.DownloaderSetting;
        private readonly ILog log = downloadContext.Log;
        private DownloaderClient m3UDownloaderClient = default!;
        private M3uCombinerClient m3UCombinerClient = default!;

        private bool _isDownloaded = false;

        public async Task StartDownload(IProgressManager progressManager, CancellationToken cancellationToken)
        {
            await DownloadAsync(progressManager, cancellationToken);

            await MergeAsync(progressManager, cancellationToken);

            IMergeSetting mergeSetting = (IMergeSetting)downloaderSetting;
            if (mergeSetting.IsCleanUp)
                DirectoryEx.DeleteCache(mediaDownloadParam.CachePath);
        }

        private async Task DownloadAsync(IProgressManager progressManager, CancellationToken cancellationToken)
        {
            if (_isDownloaded)
                return;

            var hanlder = mediaDownloadParam.IsVideoStream ? progressManager.CreateLiveHandler() : progressManager.CreateVodHandler();
            using var acquire = hanlder.Acquire();

            m3UDownloaderClient.DialogProgress = hanlder.ToReporter();
            foreach (var media in mediaDownloadParam.Medias)
            {
                await m3UDownloaderClient.MediaDownloader.DownloadAsync(media, cancellationToken);
            }
            _isDownloaded = true;
            hanlder.Clear();
        }

        private async Task MergeAsync(IProgressManager progressManager, CancellationToken cancellationToken)
        {
            var hanlder = progressManager.CreateMergerHandler();
            using var acquire = hanlder.Acquire();

            await m3UCombinerClient.FFmpeg.ConvertToMp4(mediaDownloadParam.Medias, hanlder.ToReporter(), cancellationToken);
            hanlder.Clear();
        }

    }

    public partial class MediaDownloader
    {
        public static MediaDownloader CreateMediaDownloader(IDownloadContext context)
        {
            MediaDownloader mediaDownloader = new(context)
            {
                m3UDownloaderClient = new DownloaderClient(context,null),
                m3UCombinerClient = new M3uCombinerClient(context.Log, context.DownloadParam, (IMergeSetting)context.DownloaderSetting)
            };
            return mediaDownloader;
        }
    }
}
