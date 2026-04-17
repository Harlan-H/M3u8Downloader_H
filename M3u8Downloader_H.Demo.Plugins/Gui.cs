using Avalonia.Controls;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins.Window;
using M3u8Downloader_H.Demo.Plugins.ViewModel;


namespace M3u8Downloader_H.Demo.Plugins
{
    public class Gui : IWindowPlugin
    {
        private IWindowContext windowContext = default!;
        public void InitializeWindow(IWindowContext windowContext)
        {
            this.windowContext = windowContext;
        }

        public Control CreateMainView()
        {
            return new MainWindowView()
            {
                DataContext = new MainWindowViewModel(windowContext)
            };
        }

    }
}
