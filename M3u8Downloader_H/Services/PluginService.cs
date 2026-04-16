using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins;
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
        private readonly PluginClient pluginClient;
        public IReadOnlyList<PluginHandle> GetAllPlugins => pluginClient.Plugins;
        public List<PluginHandle> GetAllActivePlugins = [];

        public event Action<PluginHandle>? PluginEnabled;
        public event Action<PluginHandle>? PluginDisabled;

        public PluginService()
        {
            pluginClient = PluginClient.Instance;
            pluginClient.PluginPath = _pluginDirPath;
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
            if (p.PluginManifest.Enabled) return;

            p.Toggle(true);

            GetAllActivePlugins.Add(p);
            PluginEnabled?.Invoke(p);
        }

        public void Disable(PluginHandle p)
        {
            if (!p.PluginManifest.Enabled) return;

            p.Toggle(false);

            GetAllActivePlugins.Remove(p);
            PluginDisabled?.Invoke(p);
            p.Unload();
        }

        public void Load()
        {
            var fileInfo =  new FileInfo(Path.Combine(_pluginConfigPath, "Plugin.dat"));
            if(!fileInfo.Exists) 
                return;

            pluginClient.LoadFromConfig(fileInfo.OpenRead());
            GetAllActivePlugins = [.. GetAllPlugins.Where(p => p.PluginManifest.Enabled)];
        }

        public IPluginEntry? this[string key] 
            => GetAllActivePlugins.FirstOrDefault(p => p.PluginManifest.Key.Equals(key))?.Load();
    }
}
