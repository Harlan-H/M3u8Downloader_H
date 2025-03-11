using System;
using Caliburn.Micro;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Abstractions.Extensions;
using M3u8Downloader_H.Utils;
using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;
using System.Threading;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public class MediaWindowViewModel: Screen
    {
        private readonly SettingsService settingsService;
        private readonly ISnackbarMessageQueue notifications;

        public MediaDownloadInfo MediaDownloadInfo { get; } = new MediaDownloadInfo();
        public bool IsBusy { get; private set; }
        public Action<DownloadViewModel> EnqueueDownloadAction { get; set; } = default!;

        public MediaWindowViewModel(SettingsService settingsService, ISnackbarMessageQueue Notifications)
        {
            MediaDownloadInfo.NormalProcessDownloadAction = ProcessMediaDownload;
            this.settingsService = settingsService;
            notifications = Notifications;
        }


        public bool CanProcessMediaDownload => !IsBusy;
        public void ProcessMediaDownload(MediaDownloadInfo mediaDownloadInfo)
        {
            IsBusy = true;
            try
            {
                mediaDownloadInfo.DoProcess(settingsService);

                mediaDownloadInfo.Reset(settingsService.IsResetAddress, settingsService.IsResetName);
            }
            catch (Exception e)
            {
                notifications.Enqueue(e.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void ProcessMediaDownload(MediaDownloadParams mediaDownloadParams)
        {
            FileEx.EnsureFileNotExist(mediaDownloadParams.GetCachePath());

            DownloadViewModel download = MediaDownloadViewModel.CreateDownloadViewModel(mediaDownloadParams);
            if (download is null) return;

            EnqueueDownloadAction(download);
        }

    }
}
