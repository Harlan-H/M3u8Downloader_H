using Avalonia.Controls;
using M3u8Downloader_H.Abstractions.Models;


namespace M3u8Downloader_H.Abstractions.Plugins.Window
{
    public interface IWindowPlugin
    {
        void InitializeWindow(IWindowContext windowContext);
        Control CreateMainView();
    }
}
