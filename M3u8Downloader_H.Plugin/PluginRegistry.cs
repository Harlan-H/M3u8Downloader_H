using M3u8Downloader_H.Plugin.Models;
using M3u8Downloader_H.Plugin.Services;
using System.IO.Compression;
using System.Text.Json;

namespace M3u8Downloader_H.Plugin.PluginClients
{
    public partial class PluginRegistry
    {
        private FileInfo pluginConfigFileInfo = default!;
        private Dictionary<string, PluginState> _states = [];
        private readonly List<PluginHandle> plugins  = [];
        public IReadOnlyList<PluginHandle> Plugins => plugins;
        public string PluginPath { get; set; } = default!;
        public string PluginConfigPath { get; set; } = default!;


        private PluginRegistry()
        {
            
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


        public void LoadFromConfig()
        {
            pluginConfigFileInfo = new(Path.Combine(PluginConfigPath, "Plugin.dat"));
            if (pluginConfigFileInfo.Exists)
            {
                var states = JsonSerializer.Deserialize(pluginConfigFileInfo.OpenRead(), PluginStateContext.Default.DictionaryStringPluginState);
                if (states is not null)
                    _states = states;
            }

            var dir = new DirectoryInfo(PluginPath);
            foreach (var fileinfo in dir.EnumerateFiles("*.zip"))
            {
                var handle = new PluginHandle(Register, GetStateByKey);
                using var zip = ZipFile.OpenRead(fileinfo.FullName);
                handle.LoadManifest(zip);
                handle.LoadAssembils(zip);
                plugins.Add(handle);
            }

//             if(_states.Count > 0)
//                 JsonSerializer.Serialize(pluginConfigFileInfo.OpenWrite(), _states, PluginStateContext.Default.DictionaryStringPluginState);
        }
    }


    public partial class PluginRegistry
    {
        private readonly static PluginRegistry instance = new();
        public static PluginRegistry Instance => instance;
    }
}
