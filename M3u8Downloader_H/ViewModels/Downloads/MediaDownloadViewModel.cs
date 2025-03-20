using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Combiners;
using M3u8Downloader_H.Downloader;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;

namespace M3u8Downloader_H.ViewModels
{
    public partial class MediaDownloadViewModel(SettingsService settingsService, SoundService soundService) : DownloadViewModel(settingsService, soundService)
    {
        private readonly SettingsService settingsService = settingsService;
        private DownloaderClient m3UDownloaderClient = default!;
        private M3uCombinerClient m3UCombinerClient = default!;

        private bool _isDownloaded = false;

        protected override async Task StartDownload(CancellationToken cancellationToken)
        {
            Status = DownloadStatus.Enqueued;
            downloadProgress ??= new(this);
            using var acquire = downloadProgress.Acquire();

            await DownloadAsync(downloadProgress, cancellationToken);

            await MergeAsync(downloadProgress, cancellationToken);

        }

        private async Task DownloadAsync(IDialogProgress downloadProgress, CancellationToken cancellationToken)
        {
            if (_isDownloaded)
                return;

            m3UDownloaderClient.DownloaderSetting = settingsService;
            m3UDownloaderClient.DialogProgress = downloadProgress;

            IMediaDownloadParam mediaDownloadParam = (IMediaDownloadParam)DownloadParam;
            foreach (var media in mediaDownloadParam.Medias)
            {
                await m3UDownloaderClient.MediaDownloader.DownloadAsync(media, cancellationToken);
            }
            _isDownloaded = true;
        }

        private async Task MergeAsync(IDialogProgress downloadProgress, CancellationToken cancellationToken)
        {
            m3UCombinerClient.DialogProgress = downloadProgress;

            IMediaDownloadParam mediaDownloadParam = (IMediaDownloadParam)DownloadParam;
            await m3UCombinerClient.FFmpeg.ConvertToMp4(mediaDownloadParam, cancellationToken);
        }
    }

    public partial class MediaDownloadViewModel
    {
        public static DownloadViewModel CreateDownloadViewModel(
            MediaDownloadParams m3U8DownloadParam)
        {
            MediaDownloadViewModel viewModel = IoC.Get<MediaDownloadViewModel>();
            viewModel.DownloadParam = m3U8DownloadParam;
            viewModel.RequestUrl = m3U8DownloadParam.Medias[0].Url;
            viewModel.VideoName = m3U8DownloadParam.VideoName;

            viewModel.m3UDownloaderClient = new DownloaderClient(Http.Client, null, viewModel.Log, m3U8DownloadParam);
            viewModel.m3UCombinerClient = new M3uCombinerClient(viewModel.Log, m3U8DownloadParam);

            return viewModel;
        }
    }
}
