using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.Common.Models;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.FrameWork;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels.Dialogs;
using M3u8Downloader_H.ViewModels.Downloads;
using M3u8Downloader_H.ViewModels.FrameWork;
using M3u8Downloader_H.ViewModels.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels.Menus
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly SettingsService settingsService;
        private readonly PluginService pluginService;
        private readonly ViewModelManager viewModelManager;
        private readonly M3u8WindowViewModel m3U8WindowViewModel;
        private readonly MediaWindowViewModel mediaWindowViewModel;
        private List<IDisposable> _disposables = [];
        public SnackbarManager Notifications { get; } = new SnackbarManager("MainWindowHost",TimeSpan.FromSeconds(5));
        

        public ObservableCollection<ViewModelBase> SubWindows { get; } = [];

        public ObservableCollection<DownloadViewModel> Downloads { get; } = [];

        [ObservableProperty]
        public partial string HttpServicePort { get; set; } = string.Empty;

        public MainWindowViewModel(SettingsService settingsService, PluginService pluginService)
        {
            this.settingsService = settingsService;
            this.pluginService = pluginService;
            viewModelManager = new(settingsService);
            m3U8WindowViewModel = new M3u8WindowViewModel(settingsService, pluginService, viewModelManager, Notifications) { Title = "M3U8", EnqueueDownloadAction = EnqueueDownload };
            SubWindows.Add(m3U8WindowViewModel);
            mediaWindowViewModel = new MediaWindowViewModel(settingsService, viewModelManager, Notifications) { Title = "长视频", EnqueueDownloadAction = EnqueueDownload };
            SubWindows.Add(mediaWindowViewModel);

            _disposables.Add(settingsService.WatchProperty(
                s => s.MaxConcurrentDownloadCount,
                () => ThrottlingSemaphore.Instance.MaxCount = settingsService.MaxConcurrentDownloadCount));

            _disposables.Add(settingsService.WatchProperty(
                s => s.ProxyAddress,
                () => HttpClient.DefaultProxy = string.IsNullOrWhiteSpace(settingsService.ProxyAddress) ? new WebProxy() : new WebProxy(settingsService.ProxyAddress)));
        }

        ~MainWindowViewModel()
        {
            foreach (var item in _disposables)
            {
                item.Dispose();
            }
        }



 
        private void EnqueueDownload(DownloadViewModel download)
        {
            var existingDownloads = Downloads.FirstOrDefault(d => d == download);
            if (existingDownloads is not null)
            {
                Notifications.Notify($"{download.VideoName} 已经在下载列表中!");
                return;
            }

            download.Start();
            Downloads.Insert(0, download);
        }

        [RelayCommand]
        private void RestartDownloads(IList selectedDownloads)
        {
            foreach (DownloadViewModel item in selectedDownloads)
            {
                item.RestartCommand.Execute(null);
            }
        }

        [RelayCommand]
        private void StopDownloads(IList selectedDownload)
        {
            foreach (DownloadViewModel item in selectedDownload)
            {
                item.CancelCommand.Execute(null);
            }
        }

        [RelayCommand]
        private async Task RemoveDownloads(IList selectedDownload)
        {
            var tempSelectDwonload = selectedDownload.Cast<DownloadViewModel>().ToList();
            DeleteDialogViewModel deleteDialogViewModel = DeleteDialogViewModel.CreateDeleteDialogViewModel(tempSelectDwonload.Count);
            DeleteDialogResult? deleteDialogResult = await DialogManager.ShowDialogAsync(deleteDialogViewModel);
            if (deleteDialogResult is null)
                return;

            if (deleteDialogResult.DialogResult == false)
                return;

            foreach (DownloadViewModel download in tempSelectDwonload)
            {
                download.CancelCommand.Execute(null);
                Downloads.Remove(download);
                if (deleteDialogResult.IsDeleteCache)
                    download.DeleteCache();
            }
        }

        [RelayCommand]
        private void RemoveInactiveDownloads()
        {

            var inactiveDownloads = Downloads.Where(c => !c.IsActive).ToArray();
            foreach(var inactiveDownload in inactiveDownloads)
                Downloads.Remove(inactiveDownload);
        }

        [RelayCommand]
        private void RemoveSuccessDownloads()
        {
            var successDownloads = Downloads.Where(c => c.Status == DownloadStatus.Completed).ToArray();
            foreach (var successDownload in successDownloads)
                Downloads.Remove(successDownload);
        }

        [RelayCommand]
        private void RemoveFailedDownloads()
        {
            var failedDownloads = Downloads.Where(c => c.Status == DownloadStatus.Failed).ToArray();
            foreach (var failedDownload in failedDownloads)
                Downloads.Remove(failedDownload);
        }

        [RelayCommand]
        private void RestartFailedDownloads()
        {
            var failedDownloads = Downloads.Where(c => c.Status == DownloadStatus.Failed).ToArray();
            foreach (var failedDownload in failedDownloads)
                failedDownload.RestartCommand.Execute(null);
        }

        [RelayCommand]
        private async Task CopyUrl(DownloadViewModel download)
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.Clipboard is not { } provider)
                return;

            await provider.SetTextAsync(download.RequestUrl.OriginalString);
            
        }

        [RelayCommand]
        private async Task CopyTitle(DownloadViewModel download)
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.Clipboard is not { } provider)
                return;


            await  provider.SetTextAsync(download.VideoName);
            
        }

        [RelayCommand]
        private async Task CopyLogs(DownloadViewModel download)
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
           desktop.MainWindow?.Clipboard is not { } provider)
                return;

            await provider.SetTextAsync(download.Log.CopyLog());       
        }
    }
}
