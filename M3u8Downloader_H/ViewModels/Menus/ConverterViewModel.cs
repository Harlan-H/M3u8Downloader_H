using Avalonia.Platform;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace M3u8Downloader_H.ViewModels.Menus
{
    public partial class ConverterViewModel : ViewModelBase
    {
        public ObservableCollection<ViewModelBase> SubWindows { get; }

        public ConverterViewModel(SettingsService settingsService)
        {
            SubWindows =
            [
                new M3u8ConverterViewModel(settingsService) { Title = "M3U8" },
                new MediaConverterViewModel(settingsService) { Title = "长视频" }
            ];

        }

    }
}
