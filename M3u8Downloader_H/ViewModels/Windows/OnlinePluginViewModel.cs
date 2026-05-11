using CommunityToolkit.Mvvm.ComponentModel;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.Plugin.PluginClients;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels.Components;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public partial class OnlinePluginViewModel : ViewModelBase
    {
        private readonly PluginRepository pluginRepository;
        private readonly PluginRegistry pluginRegistry;

        public ObservableCollection<PluginOnlineItem> PluginOnlineItems { get; } = [];

        [ObservableProperty]
        public partial LoadState State { get; set; } = LoadState.NotLoaded;

        [ObservableProperty]
        public partial string ErrorString { get; set; } = string.Empty;

        public OnlinePluginViewModel()
        {
            pluginRegistry = PluginRegistry.Instance;
            pluginRepository = new PluginRepository(Http.Instance.GetClient());
        }

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
