using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.FrameWork;
using M3u8Downloader_H.Messages;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.Plugin.Models.Context;
using M3u8Downloader_H.Plugin.Services;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels.Components;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace M3u8Downloader_H.ViewModels.Menus
{
    public partial class AdvancedViewModel : ViewModelBase
    {
        private readonly WindowContext windowContext = new(Http.Instance, Notifications);
        public static SnackbarManager Notifications { get; } = new SnackbarManager("AdvancedWindowHost", TimeSpan.FromSeconds(5));

        public ObservableCollection<PluginNavItem> PluginNavItems { get; } = [];

        [ObservableProperty]
        public partial Control CurrentView { get; set; } = default!;

        [ObservableProperty]
        public partial PluginNavItem Seleted { get; set; } = default!;

        public AdvancedViewModel(PluginManager pluginManager,IMemoryCache memoryCache)
        {
            pluginManager.PluginEnabled += PluginService_PluginEnabled;
            pluginManager.PluginDisabled += PluginService_PluginDisabled;
            WeakReferenceMessenger.Default.Send(new GetAppComandServiceMessage(windowContext));
            windowContext.MemoryCacheSercie = memoryCache;
            pluginManager.InitActivePlugin();
        }

        private void PluginService_PluginEnabled(PluginHandle obj)
        {
            if (!obj.PluginManifest.Runtime.HasUi)
                return;


            PluginNavItems.Add(new PluginNavItem(obj, windowContext));
        }

        private void PluginService_PluginDisabled(PluginHandle obj)
        {
            if (!obj.PluginManifest.Runtime.HasUi)
                return;

            var item = PluginNavItems.FirstOrDefault(p => p == obj);
            if (item is null)
                return;

            item.Dispose();
            PluginNavItems.Remove(item);
        }

        partial void OnSeletedChanged(PluginNavItem value) 
        {
            if (value is null)
            {
                CurrentView = null!;
                return;
            }

            try
            {
                CurrentView = value.GetView();
            }catch(Exception e)
            {
                Notifications.Info(e.Message);
            }
        }
    }
}
