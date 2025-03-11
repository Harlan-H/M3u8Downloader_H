using Caliburn.Micro;
using M3u8Downloader_H.ViewModels.Windows;

namespace M3u8Downloader_H.ViewModels.Menus
{
    class ConverterViewModel : Screen
    {
        public BindableCollection<Screen> SubWindows { get; } = [];

        public ConverterViewModel()
        {
            SubWindows.Add(new M3u8ConverterViewModel() { DisplayName = "M3U8"});
            SubWindows.Add(new DirConverterViewModel() { DisplayName = "文件夹" });
            SubWindows.Add(new MediaConverterViewModel() { DisplayName = "音视频"});
        }
    }
}
