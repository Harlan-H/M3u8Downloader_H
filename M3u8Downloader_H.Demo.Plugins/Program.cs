using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Abstractions.Plugins.Window;
using Avalonia;

namespace M3u8Downloader_H.Demo.Plugins
{
    public class Program : IPluginEntry
    {
        public IDownloadPlugin CreateDownloadPlugin() => null!;

        public IWindowPlugin CreateWindowPlugin()
        {
            return new Gui();
        }
    }
}
