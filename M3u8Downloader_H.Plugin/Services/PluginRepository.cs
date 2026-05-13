using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.Plugin.Models.Online;
using System.Text.Json;

namespace M3u8Downloader_H.Plugin.Services
{
    public partial class PluginRepository
    {
        private static readonly Uri RepositoryUri = new("https://raw.githubusercontent.com/Harlan-H/M3u8Downloader_H.Plugins/refs/heads/master/data/plugins.json");
        private static readonly string PackUrl = "https://raw.githubusercontent.com/Harlan-H/M3u8Downloader_H.Plugins/refs/heads/master/data/plugins/%s.json";
        private static readonly string _pluginTempPath = StorageSpaceManager.GetTempPath();
        private static readonly string _pluginDirPath = StorageSpaceManager.GetPluginPath();
        private readonly HttpClient httpClient;
        private OnlinePlugin? onlinePlugin;

        public PluginRepository(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            Directory.CreateDirectory(_pluginTempPath);
        }

        public async Task<List<OnlinePluginManifest>> InitPluginManifest(CancellationToken cancellationToken)
        {
            if (onlinePlugin is not null )
                return onlinePlugin.OnlinePluginManifests;

            var resp = await httpClient.GetStringAsync(RepositoryUri, cancellationToken);

            onlinePlugin = JsonSerializer.Deserialize(resp,
                OnlinePluginManifestContext.Default.OnlinePlugin)
                ?? throw new InvalidDataException("获取插件数据失败");

            return onlinePlugin.OnlinePluginManifests;
        }

        public async Task<PluginPackage> GetPluginPackByKey(string key, CancellationToken cancellationToken)
        {
            Uri packUri = new(string.Format(PackUrl,key));

            var resp = await httpClient.GetStringAsync(packUri, cancellationToken);
            
            var pluginPackage = JsonSerializer.Deserialize(resp, PluginPackageContext.Default.PluginPackage) 
                   ?? throw new InvalidDataException($"获取【{key}】插件数据失败");
            return pluginPackage;
        }

        public async Task DownloadPlugin(Uri url, CancellationToken cancellationToken) 
        {
            var resp = await httpClient.GetStreamAsync(url, cancellationToken);

            var downloadFileName = Path.Combine(_pluginTempPath, url.Segments.Last()) + ".download";
            await WriteToFileAsync(downloadFileName, resp, cancellationToken);

            var targetFileName = Path.Combine(_pluginDirPath, url.Segments.Last());
            File.Move(downloadFileName, targetFileName,true);

            static async Task WriteToFileAsync(string file, Stream stream, CancellationToken token = default)
            {
                using FileStream fileStream = File.Create(file, 81920);
                await stream.CopyToAsync(fileStream, token);
            }
        }
    }

}
