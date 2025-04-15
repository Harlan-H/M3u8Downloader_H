using Caliburn.Micro;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.ViewModels.Windows;

namespace M3u8Downloader_H.ViewModels.Menus
{
    class ConverterViewModel : Screen
    {
        public ConverterViewModel(SettingsService settingsService)
        {
            SubWindows =
            [
                new M3u8ConverterViewModel(settingsService) { DisplayName = "M3U8" },
                new DirConverterViewModel(settingsService) { DisplayName = "文件夹" },
                new MediaConverterViewModel(settingsService) { DisplayName = "长视频" }
            ];

        }
        public BindableCollection<Screen> SubWindows { get; } 
    }
}
