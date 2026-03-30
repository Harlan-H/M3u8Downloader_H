using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.FrameWork;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels.Downloads;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public partial class MediaWindowViewModel : ViewModelBase
    {
        private readonly SettingsService settingsService;
        private readonly ViewModelManager viewModelManager;
        private readonly SnackbarManager notification;

        public MediaDownloadInfo MediaDownloadInfo { get; } = new MediaDownloadInfo();
        public Action<DownloadViewModel> EnqueueDownloadAction { get; set; } = default!;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ProcessMediaDownloadCommand))]
        public partial bool IsBusy { get; private set; }

        public MediaWindowViewModel(SettingsService settingsService, ViewModelManager viewModelManager, SnackbarManager Notifications)
        {
            this.settingsService = settingsService;
            this.viewModelManager = viewModelManager;
            notification = Notifications;
        }

        private bool CanProcessMediaDownload() => !IsBusy;

        [RelayCommand(CanExecute = nameof(CanProcessMediaDownload))]
        private void ProcessMediaDownload(MediaDownloadInfo mediaDownloadInfo)
        {
            IsBusy = true;
            try
            {
                Uri VideoUri = MediaDownloadInfo.GetVideoRequestUri();
                Uri? AudioUri = MediaDownloadInfo.GetAudioRequestUri();
                MediaDownloadParams mediaDownloadParams = new(settingsService.SavePath, VideoUri, AudioUri, mediaDownloadInfo.VideoName, settingsService.Headers)
                {
                    IsVideoStream = mediaDownloadInfo.StreamIndex == 0
                };
                ProcessMediaDownload(mediaDownloadParams);

                mediaDownloadInfo.Reset(settingsService.IsResetAddress, settingsService.IsResetName);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                notification.Notify(e.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void ProcessMediaDownload(IMediaDownloadParam mediaDownloadParams)
        {
            FileEx.EnsureFileNotExist(mediaDownloadParams.VideoFullName);

            DownloadViewModel download = viewModelManager.CreateDownloadViewModel(mediaDownloadParams);
            if (download is null) return;

            EnqueueDownloadAction(download);
        }
    }
}
