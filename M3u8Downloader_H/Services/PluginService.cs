using System;
using System.Collections.Generic;
using M3u8Downloader_H.Plugin.PluginClients;
using System.IO;


namespace M3u8Downloader_H.Services
{
    public class PluginService
    {
        private readonly string _pluginDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
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
