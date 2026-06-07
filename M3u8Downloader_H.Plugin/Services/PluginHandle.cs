using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Abstractions.Plugins.Window;
using M3u8Downloader_H.Plugin.Models;
using M3u8Downloader_H.Plugin.Models.Context;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;

namespace M3u8Downloader_H.Plugin.Services
{
    public class PluginHandle
    {
        private readonly Dictionary<string, MemoryStream> _assemblies = [];

        private PluginLoadContext LoadContext = default!;
        private Assembly Assembly = default!;
        private IPluginEntry pluginEntry = default!;

        private IWindowPlugin? WindowInstance = default;

        public PluginManifest PluginManifest { get; private set; } = default!;


        public async Task<PluginManifest> LoadManifest(ZipArchive zip)
        {
            var manifestFile = zip.GetEntry("manifest.json") ?? throw new InvalidDataException("没有manifest.json文件,无法加载插件");

            using var manifestSteam = await manifestFile.OpenAsync();
            var manifest = JsonSerializer.Deserialize(manifestSteam, PluginManifestContext.Default.PluginManifest)
                    ?? throw new InvalidDataException("manifest反序列失败");

            PluginManifest = manifest;
            return manifest;
        }

        public async Task LoadAssembils(ZipArchive zip)
        {
            foreach (var entry in zip.Entries)
            {
                if(entry.FullName.EndsWith(".dll"))
                {
                    using var s = await entry.OpenAsync();
                    var ms = new MemoryStream();
                    await s.CopyToAsync(ms);
                    ms.Position = 0;
                    _assemblies[entry.Name] = ms;
                }
            }
        }

        public bool CanHandleDownload(Uri url)
             => pluginEntry.CanHandle(url);


        public void LoadLibrary()
        {
            if (pluginEntry is not null)
                return;

            LoadContext = new PluginLoadContext(_assemblies);
            Assembly = LoadContext.LoadFromStream(_assemblies[PluginManifest.Runtime.EntryPoint]);
            var type  = Assembly.GetType(PluginManifest.Runtime.EntryType)
                    ?? throw new InvalidDataException($"{PluginManifest.Runtime.EntryPoint} 未实现IPluginEntry接口无法开启");

            var instance = Activator.CreateInstance(type);
            if (instance is null || instance is not IPluginEntry ipluginEntryInstance)
                throw new InvalidDataException($"{PluginManifest.Runtime.EntryPoint} 继承IWindowPlugin接口的类没有默认的构造函数");

            pluginEntry = ipluginEntryInstance;
        }

        public IWindowPlugin LoadUI(IServiceCollection service,IMemoryCache memoryCache)
        {
            var instance = pluginEntry.CreateWindoPlugin();
            if (instance is null || instance is not IWindowPlugin windowInstance)
                throw new InvalidDataException("继承IWindowPlugin接口的类没有默认的构造函数");


            service.AddSingleton<IPluginStorage>(new PluginStorage(PluginManifest.Key));
            service.AddSingleton<ICacheService>(new CacheService(memoryCache,PluginManifest.Key));
 
            WindowInstance = windowInstance;
            return windowInstance;
        }

        public IDownloadPlugin? LoadDownload()
        {
            //LoadLibrary();

            var instance = pluginEntry.CreateDownloadPlugin();
            if (instance is null || instance is not IDownloadPlugin downloadInstance)
                throw new InvalidDataException("继承IDownloadService结构的类没有默认的构造函数");

            return downloadInstance;
        }


        public void Unload()
        {
            if (WindowInstance is not null)
                WindowInstance = null;

            LoadContext?.Unload();
            _assemblies.Clear();
        }
    }
}
