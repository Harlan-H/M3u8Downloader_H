using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.ViewModels.Components;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public partial class OnlinePluginViewModel(PluginManager pluginManager) : ViewModelBase
    {
        public ObservableCollection<PluginOnlineItem> PluginOnlineItems { get; } = [];

        [ObservableProperty]
        public partial LoadState State { get; set; } = LoadState.NotLoaded;

        [ObservableProperty]
        public partial string ErrorString { get; set; } = string.Empty;

        public async Task InitPluginItem()
        {
            using CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
            var pluginManifests = await pluginManager.InitPluginManifest(cancellationTokenSource.Token);
            foreach (var item in pluginManifests)
            {
                PluginOnlineItems.Add( new PluginOnlineItem(pluginManager, item));
            }
        }



        public async Task EnsureLoadedAsync()
        {
            if (State == LoadState.Loaded)
                return;

            State = LoadState.Loading;

            try
            {
                await InitPluginItem();
                State = LoadState.Loaded;
            }
            catch(Exception ex)
            {
                State = LoadState.Failed;
                ErrorString = ex.Message;
            }
        }

        [RelayCommand]
        private void Explore(string str)
        {

            ProcessStartInfo processStartInfo = new(str)
            {
                UseShellExecute = true
            };
            Process.Start(processStartInfo);
        }
    }
}
