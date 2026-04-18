using M3u8Downloader_H.Plugin.Models;
using M3u8Downloader_H.Plugin.Services;
using System.Text.Json;

namespace M3u8Downloader_H.Plugin.PluginClients
{
    public partial class PluginClient
    {
        private readonly List<PluginHandle> plugins  = [];
        public IReadOnlyList<PluginHandle> Plugins => plugins;
        public string PluginPath { get; set; } = default!;

        private PluginClient()
        {

        }

        public void LoadFromConfig(Stream configStream)
        {
            var manifests = JsonSerializer.Deserialize(configStream, PluginContext.Default.IListPluginManifest);
            if (manifests is null)
                return;

            foreach (var manifest in manifests)
            {
                plugins.Add(new PluginHandle(PluginPath,manifest));
            }
        }
    }


    public partial class PluginClient
    {
        private readonly static PluginClient instance = new();
        public static PluginClient Instance => instance;
    }
}
