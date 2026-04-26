using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.Plugin.PluginClients;
using M3u8Downloader_H.Plugin.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace M3u8Downloader_H.Services
{
    public class PluginService
    {
        private readonly string _pluginDirPath = StorageSpaceManager.GetPluginPath();
        private readonly string _pluginConfigPath = StorageSpaceManager.GetConfigPath();
        private readonly PluginRegistry pluginClient;
        public IReadOnlyList<PluginHandle> GetAllPlugins => pluginClient.Plugins;
        public List<PluginHandle> GetAllActivePlugins = [];

        public event Action<PluginHandle>? PluginEnabled;
        public event Action<PluginHandle>? PluginDisabled;

        public PluginService()
        {
            pluginClient = PluginRegistry.Instance;
            pluginClient.PluginPath = _pluginDirPath;
            pluginClient.PluginConfigPath = _pluginConfigPath;
        }

        public void InitActivePlugin()
        {
            foreach (var item in GetAllActivePlugins)
            {
                PluginEnabled?.Invoke(item);
            }
        }

        public void Enable(PluginHandle p)
        {
            if (p.Enable) return;

            p.Enable = true;

            GetAllActivePlugins.Add(p);
            PluginEnabled?.Invoke(p);
        }

        public void Disable(PluginHandle p)
        {
            if (!p.Enable) return;

            p.Enable = false;

            PluginDisabled?.Invoke(p);
            GetAllActivePlugins.Remove(p);
        }

        public void Load()
        {
            pluginClient.LoadFromConfig();
            GetAllActivePlugins = [.. GetAllPlugins.Where(p => p.Enable)];
        }
            
        public IDownloadPlugin? CreateDownloadPlugin(string? key,Uri url)
        {
            if(!string.IsNullOrWhiteSpace(key))
            {
                var plugin = GetAllActivePlugins.FirstOrDefault(p => p.PluginManifest.Key.Equals(key));
                if (plugin is null || plugin.PluginManifest.Runtime.HasDownload == false)
                    return null;

                var canHandle = plugin.CanHandleDownload(url);
                return canHandle ? plugin.LoadDownload() : null;
            }
            else if (url is not null)
            {
                var plugin = GetAllActivePlugins.FirstOrDefault(p => p.CanHandleDownload(url));
                return plugin?.LoadDownload();
            }else
            {
                return null;
            }
        }
    }
}
