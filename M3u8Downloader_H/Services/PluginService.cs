using System;
using System.Collections.Generic;
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
            pluginClient.PluginPath = _pluginDirPath;
            pluginClient.Init();
        }

        public void Load() => pluginClient.Load();

        public Type? this[string? key] => pluginClient.GetPluginType(key); 

        public IEnumerable<string> Keys => pluginClient.Keys;

    }
}
