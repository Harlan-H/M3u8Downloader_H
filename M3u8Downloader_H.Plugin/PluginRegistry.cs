using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.Plugin.Models;
using M3u8Downloader_H.Plugin.Services;
using System.IO.Compression;
using System.Text.Json;

namespace M3u8Downloader_H.Plugin.PluginClients
{
    public partial class PluginRegistry
    {
        private readonly string _pluginDirPath = StorageSpaceManager.GetPluginPath();
        private readonly string _pluginConfigPath = Path.Combine(StorageSpaceManager.GetConfigPath(), "Plugin.dat");

        private Dictionary<string, PluginState> _states = [];
        private readonly List<PluginHandle> plugins  = [];
        public IReadOnlyList<PluginHandle> Plugins => plugins;


        private PluginRegistry()
        {
            Directory.CreateDirectory(_pluginDirPath);
        }


        public PluginState? TryGetPluginStateByKey(string key)
        {
            _states.TryGetValue(key, out var state);
            return state;
        }

        private void Register(PluginManifest manifest,bool isRegister = true)
        {
            if (isRegister && !_states.ContainsKey(manifest.Key))
            {
                _states[manifest.Key] = new PluginState
                {
                    Key = manifest.Key,
                    Enabled = false,
                    CurrentVersion = manifest.Release.Version
                };
            }else if(isRegister == false) {
                _states.Remove(manifest.Key);
            }
        }

        private PluginState GetStateByKey(string key)
            => _states[key];


        public void LoadConfig()
        {
            var pluginConfigFileInfo = new FileInfo(_pluginConfigPath);
            if (pluginConfigFileInfo.Exists)
            {
                var states = JsonSerializer.Deserialize(pluginConfigFileInfo.OpenRead(), PluginStateContext.Default.DictionaryStringPluginState);
                if (states is not null)
                    _states = states;
            }
        }

        public void LoadPlugins()
        {
            var dir = new DirectoryInfo(_pluginDirPath);
            foreach (var fileinfo in dir.EnumerateFiles("*.zip"))
            {
                var handle = new PluginHandle(fileinfo, Register, GetStateByKey);
                using var zip = ZipFile.OpenRead(fileinfo.FullName);
                handle.LoadManifest(zip);
                handle.LoadAssembils(zip);
                plugins.Add(handle);
            }

//              if(_states.Count > 0)
//                  JsonSerializer.Serialize(pluginConfigFileInfo.OpenWrite(), _states, PluginStateContext.Default.DictionaryStringPluginState);
        }
    }


    public partial class PluginRegistry
    {
        private readonly static PluginRegistry instance = new();
        public static PluginRegistry Instance => instance;
    }
}
