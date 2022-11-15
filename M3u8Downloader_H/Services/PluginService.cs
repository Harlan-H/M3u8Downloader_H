using System.Collections.Generic;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.Plugin.PluginClients;

namespace M3u8Downloader_H.Services
{
    public class PluginService
    {
        private readonly string _pluginDirPath =
#if DEBUG
            "e:/desktop/Plugins/";
#else
             Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
#endif
        private readonly PluginClient pluginClient;
        public PluginService()
        {
            pluginClient = PluginClient.Instance; 
        }

        public void Load() => pluginClient.Load(_pluginDirPath);

        public IPluginBuilder? this[string? key] => pluginClient.CreatePluginBuilder(key); 

        public IEnumerable<string> Keys => pluginClient.Keys;

    }
}
