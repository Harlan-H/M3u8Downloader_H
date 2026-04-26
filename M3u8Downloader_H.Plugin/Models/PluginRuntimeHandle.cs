using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Plugin.Services;

namespace M3u8Downloader_H.Plugin.Models
{
    public class PluginRuntimeHandle
    {
        public string Key { get; set; } = default!;
        public PluginLoadContext LoadContext { get; set; } = default!;
        public IPluginEntry Instance { get; set; } = default!;
        public WeakReference WeakRef { get; set; } = default!;
    }
}
