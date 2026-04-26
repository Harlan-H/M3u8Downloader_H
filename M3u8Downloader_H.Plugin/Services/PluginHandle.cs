using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Abstractions.Plugins.Window;
using M3u8Downloader_H.Plugin.Models;
using M3u8Downloader_H.Plugin.PluginClients;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;

namespace M3u8Downloader_H.Plugin.Services
{
    public class PluginHandle(Action<PluginManifest,bool> registerAction,Func<string,PluginState> stateFunc)
    {
        private readonly Dictionary<string, MemoryStream> _assemblies = [];

        private PluginLoadContext LoadContext = default!;
        private Assembly Assembly = default!;
        private IPluginEntry pluginEntry = default!;

        private IWindowPlugin? WindowInstance = default;

        public PluginManifest PluginManifest { get; private set; } = default!;

        public bool Enable 
        {
            get => stateFunc(PluginManifest.Key).Enabled;
            set => stateFunc(PluginManifest.Key).Enabled = value;
        }

        public void LoadManifest(ZipArchive zip)
        {
            var manifestFile = zip.GetEntry("manifest.json");
            if (manifestFile is null)
                return;

            using var manifestSteam = manifestFile.Open();
            var manifest = JsonSerializer.Deserialize(manifestSteam, PluginManifestContext.Default.PluginManifest);
            if (manifest is null)
                return;

            PluginManifest = manifest;
            registerAction(manifest,true);
        }

        public void LoadAssembils(ZipArchive zip)
        {
            foreach (var entry in zip.Entries)
            {
                if(entry.FullName.EndsWith(".dll"))
                {
                    using var s = entry.Open();
                    var ms = new MemoryStream();
                    s.CopyTo(ms);
                    ms.Position = 0;
                    _assemblies[entry.Name] = ms;
                }
            }
        }

        public bool CanHandleDownload(Uri url)
             => pluginEntry.CanHandle(url);


        private void LoadLibrary()
        {
            if (pluginEntry is not null)
                return;

            LoadContext = new PluginLoadContext(_assemblies);
            Assembly = LoadContext.LoadFromStream(_assemblies[PluginManifest.Runtime.EntryPoint]);
            var type  = Assembly.GetType(PluginManifest.Runtime.EntryType)
                    ?? throw new InvalidDataException("未实现IPluginEntry接口无法开启");

            var instance = Activator.CreateInstance(type);
            if (instance is null || instance is not IPluginEntry ipluginEntryInstance)
                throw new InvalidDataException("继承IWindowPlugin接口的类没有默认的构造函数");

            pluginEntry = ipluginEntryInstance;
        }

        public IWindowPlugin LoadUI()
        {
            LoadLibrary();

            var instance = pluginEntry.CreateWindoPlugin();
            if (instance is null || instance is not IWindowPlugin windowInstance)
                throw new InvalidDataException("继承IWindowPlugin接口的类没有默认的构造函数");

            WindowInstance = windowInstance;
            return windowInstance;
        }

        public IDownloadPlugin? LoadDownload()
        {
            LoadLibrary();

            var instance = pluginEntry.CreateDownloadPlugin();
            if (instance is null || instance is not IDownloadPlugin downloadInstance)
                throw new InvalidDataException("继承IDownloadService结构的类没有默认的构造函数");

            return downloadInstance;
        }


        public void Unload()
        {
            if (WindowInstance is not null)
                WindowInstance = null;

            registerAction(PluginManifest, false);
            LoadContext?.Unload();
            _assemblies.Clear();
        }
    }
}
