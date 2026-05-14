using CommunityToolkit.Mvvm.ComponentModel;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.Plugin.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels.Components;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public partial class OnlinePluginViewModel(PluginManager pluginManager) : ViewModelBase
    {
        private readonly PluginRepository pluginRepository = new(() => Http.Instance.GetClient());
        private readonly PluginRegistry pluginRegistry = pluginManager.RegistryClient;

        public ObservableCollection<PluginOnlineItem> PluginOnlineItems { get; } = [];

        [ObservableProperty]
        public partial LoadState State { get; set; } = LoadState.NotLoaded;

        [ObservableProperty]
        public partial string ErrorString { get; set; } = string.Empty;

        public async Task InitPluginItem()
        {
            using CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
            var pluginManifests = await pluginRepository.InitPluginManifest(cancellationTokenSource.Token);
            foreach (var item in pluginManifests)
            {
                var pluginState = pluginRegistry.TryGetPluginStateByKey(item.Key);
                PluginOnlineItems.Add(
                    new PluginOnlineItem(token => pluginRepository.DownloadPlugin(item.Release.DownloadUrl,token),
                            item, pluginState));
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
                ErrorString = ex.ToString();
            }
        }
    }
}
