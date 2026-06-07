using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.Plugin.Models;
using M3u8Downloader_H.Plugin.Utils;
using System.Text.Json;

namespace M3u8Downloader_H.Plugin.Services
{
    public partial class PluginRegistry
    {
        private readonly DebounceDispatcher debounceDispatcher = new();
        private readonly string _pluginDirPath = StorageSpaceManager.GetPluginPath();
        private readonly string _pluginConfigPath = Path.Combine(StorageSpaceManager.GetConfigPath(), "Plugin.dat");
        private Dictionary<string, PluginState> _states = [];
        private readonly List<PluginHandle> plugins  = [];
        public IReadOnlyList<PluginHandle> Plugins => plugins;

        public PluginRegistry()
        {
            Directory.CreateDirectory(_pluginDirPath);
        }

        public void Load()
        {
            var pluginConfigFileInfo = new FileInfo(_pluginConfigPath);
            if (pluginConfigFileInfo.Exists)
            {
                using var fileStream = pluginConfigFileInfo.OpenRead();
                var states = JsonSerializer.Deserialize(fileStream, PluginStateContext.Default.DictionaryStringPluginState);
                if (states is not null)
                    _states = states;
            }
        }

        public void Save()
        {
            var pluginConfigFileInfo = new FileInfo(_pluginConfigPath);
            using var fileStream = pluginConfigFileInfo.Create();
            JsonSerializer.Serialize(fileStream, _states, PluginStateContext.Default.DictionaryStringPluginState);
        }

        public PluginState? TryGetPluginStateByKey(string key)
        {
            _states.TryGetValue(key, out var state);
            return state;
        }

        public PluginState GetPluginStateByKey(string key)
                => _states[key];


        public void UpdateVersionByKey(string key,Version version)
        {
            if(_states.TryGetValue(key, out var state))
            {
                state.CurrentVersion = version;
            }
        }


        public bool IsEnable(PluginManifest pluginManifest) 
            => _states[pluginManifest.Key].Enabled;


        public void Toggle(PluginManifest pluginManifest, bool enable)
        {
            _states[pluginManifest.Key].Enabled = enable;
        }

        public void Register(PluginManifest manifest,string pluginFullName)
        {
            if (!_states.ContainsKey(manifest.Key))
            {
                _states[manifest.Key] = new PluginState
                {
                    Key = manifest.Key,
                    Enabled = false,
                    FullName = pluginFullName,
                    CurrentVersion = manifest.Release.Version
                };
            }
        }

        public PluginState? UnRegister(PluginManifest manifest)
        {
            if(_states.TryGetValue(manifest.Key,out PluginState? state))
            {
                _states.Remove(manifest.Key);
                return state;
            }
            return null;
        }

    }

}
