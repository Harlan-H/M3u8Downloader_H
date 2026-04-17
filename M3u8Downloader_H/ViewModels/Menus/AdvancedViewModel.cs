using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using M3u8Downloader_H.FrameWork;
using M3u8Downloader_H.Messages;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Plugin.Services;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels.Components;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace M3u8Downloader_H.ViewModels.Menus
{
    public partial class AdvancedViewModel : ViewModelBase
    {
        private readonly WindowContext windowContext = new(Http.Client, Notifications);
        public static SnackbarManager Notifications { get; } = new SnackbarManager("AdvancedWindowHost", TimeSpan.FromSeconds(5));

        public ObservableCollection<PluginNavItem> PluginNavItems { get; } = [];

        [ObservableProperty]
        public partial Control CurrentView { get; set; } = default!;

        [ObservableProperty]
        public partial PluginNavItem Seleted { get; set; } = default!;

        public AdvancedViewModel(PluginService pluginService)
        {
            pluginService.PluginEnabled += PluginService_PluginEnabled;
            pluginService.PluginDisabled += PluginService_PluginDisabled;
            WeakReferenceMessenger.Default.Send(new GetAppComandServiceMessage(windowContext));
            pluginService.InitActivePlugin();
        }

        private void PluginService_PluginEnabled(PluginHandle obj)
        {
            if (!obj.PluginManifest.HasUi)
                return;

            PluginNavItems.Add(new PluginNavItem(obj));
        }

        private void PluginService_PluginDisabled(PluginHandle obj)
        {
            if (!obj.PluginManifest.HasUi)
                return;

            var item = PluginNavItems.FirstOrDefault(p => p == obj);
            if (item is null)
                return;

            PluginNavItems.Remove(item);
        }

        partial void OnSeletedChanged(PluginNavItem oldValue, PluginNavItem newValue) 
        {
            if (oldValue == newValue)
                return;

            try
            {
                CurrentView = newValue.GetView(windowContext);
            }catch(Exception e)
            {
                Notifications.Notify(e.Message);
            }
        }
    }
}
