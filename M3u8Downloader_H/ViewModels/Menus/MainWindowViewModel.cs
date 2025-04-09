using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.ViewModels.FrameWork;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.RestServer;
using Caliburn.Micro;
using System.Threading;
using PropertyChanged;
using System.Security.Principal;
using M3u8Downloader_H.ViewModels.Windows;
using M3u8Downloader_H.Common.Models;

namespace M3u8Downloader_H.ViewModels.Menus
{
    public class MainWindowViewModel : Screen
    {
        private readonly HttpListenService httpListenService = HttpListenService.Instance;
        private readonly SettingsService settingsService;
        private readonly PluginService pluginService;
        private readonly M3u8WindowViewModel m3U8WindowViewModel;

        public ISnackbarMessageQueue Notifications { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));
        public BindableCollection<DownloadViewModel> Downloads { get; } = [];
        public BindableCollection<Screen> SubWindows { get; } = [];

        [DoNotNotify]
        public IList<DownloadViewModel> SelectedDownloads { get; set; } = Array.Empty<DownloadViewModel>();

        public int? HttpServicePort { get; set; }

        public MainWindowViewModel(SettingsService settingsService, PluginService pluginService)
        {
            this.settingsService = settingsService;
            this.pluginService = pluginService;
            m3U8WindowViewModel = new M3u8WindowViewModel(settingsService, pluginService, Notifications) { DisplayName = "M3U8", EnqueueDownloadAction = EnqueueDownload };
            SubWindows.Add(m3U8WindowViewModel);
            SubWindows.Add(new MediaWindowViewModel(settingsService, Notifications) {DisplayName="长视频" ,EnqueueDownloadAction = EnqueueDownload });
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            _ = Task.Run(() =>
            {
                settingsService.Load();
                settingsService.Validate();
                settingsService.UpdateConcurrentDownloadCount();

                pluginService.Load();
                WindowsPrincipal windowsPrincipal = new(WindowsIdentity.GetCurrent());
                if(windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    httpListenService.Run(i => HttpServicePort = i);
                    httpListenService.Initialization(m3U8WindowViewModel.ProcessM3u8Download, m3U8WindowViewModel.ProcessM3u8Download);
                }
            }, cancellationToken);

            return base.OnInitializeAsync(cancellationToken);
        }


        public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in Downloads)
                item.OnCancel();

            settingsService.Save();
            return base.CanCloseAsync(cancellationToken);
        }


        private void EnqueueDownload(DownloadViewModel download)
        {
            var existingDownloads = Downloads.Where(d =>  d == download ).FirstOrDefault();
            if (existingDownloads is not null)
            {
                Notifications.Enqueue($"{download.VideoName} 已经在下载列表中!");
                return;
            }
           
            download.OnStart();
            Downloads.Insert(0, download);
        }


        public void RestartDownloads()
        {
            foreach (var item in SelectedDownloads)
            {
                item.OnRestart();
            }
        }

        public void StopDownloads()
        {
            foreach (var item in SelectedDownloads)
            {
                item.OnCancel();
            }
        }

        public async void RemoveDownloads()
        {
            DeleteDialogViewModel deleteDialogViewModel = DeleteDialogViewModel.CreateDeleteDialogViewModel(SelectedDownloads.Count);
            DeleteDialogResult? deleteDialogResult = await DialogManager.ShowDialogAsync(deleteDialogViewModel);
            if (deleteDialogResult is null)
                return;

            if (deleteDialogResult.DialogResult == false)
                return;

            foreach (var download in SelectedDownloads)
            {
                download.OnCancel();
                Downloads.Remove(download);
                if (deleteDialogResult.IsDeleteCache)
                     download.DeleteCache();
            }
        }
     

        public void RemoveInactiveDownloads()
        {
            var inactiveDownloads = Downloads.Where(c => !c.IsActive).ToArray();
            Downloads.RemoveRange(inactiveDownloads);
        }

        public void RemoveSuccessDownloads()
        {
            var successDownloads = Downloads.Where(c => c.Status == DownloadStatus.Completed).ToArray();
            Downloads.RemoveRange(successDownloads);
        }

        public void RemoveFailedDownloads()
        {
            var failedDownloads = Downloads.Where(c => c.Status == DownloadStatus.Failed).ToArray();
            Downloads.RemoveRange(failedDownloads);
        }

        public void RestartFailedDownloads()
        {
            var failedDownloads = Downloads.Where(c => c.Status == DownloadStatus.Failed);
            foreach (var failedDownload in failedDownloads)
                failedDownload.OnRestart();
        }

        public void CopyUrl(DownloadViewModel download) => Clipboard.SetText(download.RequestUrl.OriginalString);

        public void CopyTitle(DownloadViewModel download) => Clipboard.SetText(download.VideoName);

        public void CopyFailReason(DownloadViewModel download) => Clipboard.SetText(download.FailReason);

        public void CopyLogs(DownloadViewModel download) => Clipboard.SetText(download.Log.CopyLog());

    }
}
