using Caliburn.Micro;
using M3u8Downloader_H.ViewModels.Windows;

namespace M3u8Downloader_H.ViewModels.Menus
{
    class ConverterViewModel : Screen
    {
        public BindableCollection<Screen> SubWindows { get; } =
        [
            new M3u8ConverterViewModel() { DisplayName = "M3U8"},
            new DirConverterViewModel() { DisplayName = "文件夹" },
            new MediaConverterViewModel() { DisplayName = "音视频"}
        ];
    }
}
