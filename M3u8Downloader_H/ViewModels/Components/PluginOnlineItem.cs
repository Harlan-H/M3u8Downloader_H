using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.Common.Models;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Plugin.Models;
using M3u8Downloader_H.Plugin.Models.Online;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels.Components
{
    public partial class PluginOnlineItem(Func<CancellationToken,Task> downloadFunc, OnlinePluginManifest onlinePluginManifest, PluginState? pluginState) : ObservableObject
    {
        private bool _installed = false;
        public string Title => onlinePluginManifest.BasicInfo.Title;
        public string Desc => onlinePluginManifest.BasicInfo.Description;
        public string Author => onlinePluginManifest.BasicInfo.Author;
        public Version Version => onlinePluginManifest.Release.Version;
        public Version? LocalVersion => pluginState?.CurrentVersion;
        public DateTime Time =>  onlinePluginManifest.BasicInfo.Time;

        [ObservableProperty]
        public partial LoadState State { get; set; }

        [ObservableProperty]
        public partial string ErrorString { get; set; }

        public bool CanInstallPlugin => _installed  == false && LocalVersion is null;

        [RelayCommand(CanExecute = nameof(CanInstallPlugin))]
        private async Task InstallPlugin()
        {
            try
            {
                
                using CancellationTokenSource cancellationTokenSource = new();
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
                State = LoadState.Loading;
                await downloadFunc.Invoke(cancellationTokenSource.Token);
                _installed = true;
                InstallPluginCommand.NotifyCanExecuteChanged();
                State = LoadState.Loaded;
            }
            catch (Exception ex) 
            {
                State = LoadState.Failed;
                ErrorString = ex.ToString();
            }
        }

        public bool CanUpdatePlugin => _installed == false && LocalVersion is not null && Version > LocalVersion;

        [RelayCommand(CanExecute = nameof(CanUpdatePlugin))]
        private async Task UpdatePlugin()
        {
            try
            {
                using CancellationTokenSource cancellationTokenSource = new();
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));
                State = LoadState.Loading;
                await downloadFunc.Invoke(cancellationTokenSource.Token);
                _installed = true;
                UpdatePluginCommand.NotifyCanExecuteChanged();
                State = LoadState.Loaded;
            }
            catch (Exception ex)
            {
                State = LoadState.Failed;
                ErrorString = ex.ToString();
            }
        }
    }
}
