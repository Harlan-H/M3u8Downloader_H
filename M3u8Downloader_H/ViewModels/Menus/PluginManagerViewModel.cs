using CommunityToolkit.Mvvm.ComponentModel;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.ViewModels.Components;
using M3u8Downloader_H.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace M3u8Downloader_H.ViewModels.Menus
{
    public partial class PluginManagerViewModel : ViewModelBase
    {
        public ObservableCollection<ViewModelBase> Tabs { get; }

        [ObservableProperty]
        public partial ViewModelBase SelectedTab { get; set; } 


        public PluginManagerViewModel(PluginService pluginService)
        {
            Tabs =
            [
                new LocalPluginViewModel(pluginService) { Title = "本地插件" },
                new OnlinePluginViewModel() {Title = "在线插件"}
            ];

            SelectedTab = Tabs[0];
        }


        partial void OnSelectedTabChanged(ViewModelBase value)
        {
            if (value is OnlinePluginViewModel online)
            {
                _ = online.EnsureLoadedAsync();
            }
        }
    }

}
