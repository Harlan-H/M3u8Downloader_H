using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.Plugin.Services;
using System.IO.Compression;

namespace M3u8Downloader_H.Plugin
{
    public class PluginManager
    {
        private readonly string _pluginDirPath = StorageSpaceManager.GetPluginPath();
        private readonly List<PluginHandle> plugins = [];
        private List<PluginHandle> _activePlugins = [];
        public event Action<PluginHandle>? PluginEnabled;
        public event Action<PluginHandle>? PluginDisabled;

        public IReadOnlyList<PluginHandle> AllPlugins => plugins;
        public PluginRegistry RegistryClient { get; } = new PluginRegistry();


        public void InitActivePlugin()
        {
            foreach (var item in _activePlugins)
            {
                PluginEnabled?.Invoke(item);
            }
        }

        public void Load()
        {
            RegistryClient.Load();
            LoadPlugins();
            foreach (var item in plugins.Where(p => RegistryClient.IsEnable(p.PluginManifest)))
            {
                item.LoadLibrary();
                _activePlugins.Add(item);
            }
        }

        public void DelPlugins(PluginHandle pluginHandle)
        {
            Disable(pluginHandle);
            pluginHandle.Unload();
            var state = RegistryClient.UnRegister(pluginHandle.PluginManifest);
            if(state is not null)
                File.Delete(state.FullName);
        }

        private void LoadPlugins()
        {
            var dir = new DirectoryInfo(_pluginDirPath);
            foreach (var fileinfo in dir.EnumerateFiles("*.zip"))
            {
                var handle = new PluginHandle();
                using var zip = ZipFile.OpenRead(fileinfo.FullName);
                var manifest = handle.LoadManifest(zip);
                handle.LoadAssembils(zip);
                plugins.Add(handle);
                RegistryClient.Register(manifest, fileinfo.FullName);
            }
        }

        public void Enable(PluginHandle p)
        {
            if (RegistryClient.IsEnable(p.PluginManifest)) return;

            RegistryClient.Toggle(p.PluginManifest, true);

            _activePlugins.Add(p);
            PluginEnabled?.Invoke(p);
            p.LoadLibrary();
        }

        public void Disable(PluginHandle p)
        {
            if (!RegistryClient.IsEnable(p.PluginManifest)) return;

            RegistryClient.Toggle(p.PluginManifest, false);

            PluginDisabled?.Invoke(p);
            _activePlugins.Remove(p);
            p.Unload();
        }

        public IDownloadPlugin? CreateDownloadPlugin(Uri url)
        {
            if (url is not null)
            {
                var plugin = _activePlugins.Where(p => p.PluginManifest.Runtime.HasDownload).FirstOrDefault(p => p.CanHandleDownload(url));
                return plugin?.LoadDownload();
            }
            return null;
        }
    }
}
