using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Abstractions.Plugins.Window;
using M3u8Downloader_H.Plugin.Models;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;

namespace M3u8Downloader_H.Plugin.Services
{
    public class PluginHandle(string PluginPath, PluginManifest pluginManifest)
    {
        private PluginLoadContext LoadContext = default!;
        private Assembly Assembly = default!;

        private IWindowPlugin? WindowInstance = default!;
        private Type? downloadServiceType = default!;
        //private IPluginEntry? Instance = default!;

        public PluginManifest PluginManifest => pluginManifest;

        public void Toggle(bool Enable)
        {
            PluginManifest.Enabled = Enable;
        }

        private void LoadLibrary()
        {
            if (Assembly is not null)
                return;

            var dirPath = Path.Combine(PluginPath, pluginManifest.DirectoryPath);
            var dllPath = Path.Combine(dirPath, pluginManifest.Entry);

            LoadContext = new PluginLoadContext(dirPath);
            Assembly = LoadContext.LoadFromAssemblyPath(dllPath);
        }

//         public IPluginEntry Load()
//         {
//             if (Instance is not null)
//                 return Instance;
// 
//             var dirPath = Path.Combine(PluginPath, pluginManifest.DirectoryPath);
//             var dllPath = Path.Combine(dirPath, pluginManifest.Entry);
// 
//             var alc = new PluginLoadContext(dirPath);
//             var asm = alc.LoadFromAssemblyPath(dllPath);
// 
//             Type type = asm.GetExportedTypes().FirstOrDefault(i => typeof(IPluginEntry).IsAssignableFrom(i))
//                 ?? throw new InvalidDataException("未实现IPluginEntry接口无法开启");
// 
//             var instance = Activator.CreateInstance(type);
//             if (instance is null || instance is not IPluginEntry PluginInstance)
//                 throw new InvalidDataException("没有默认的构造函数");
// 
//             Instance = PluginInstance;
//             LoadContext = alc;
//             return Instance;
//         }


        public IWindowPlugin LoadUI()
        {
            LoadLibrary();

            Type type = Assembly.GetExportedTypes().FirstOrDefault(i => typeof(IWindowPlugin).IsAssignableFrom(i))
              ?? throw new InvalidDataException("未实现IWindowPlugin接口无法开启");

            var instance = Activator.CreateInstance(type);
            if (instance is null || instance is not IWindowPlugin windowInstance)
                throw new InvalidDataException("继承IWindowPlugin接口的类没有默认的构造函数");

            WindowInstance = windowInstance;
            return windowInstance;
        }

        public IDownloadPlugin LoadDownload()
        {
            LoadLibrary();

            downloadServiceType ??= Assembly.GetExportedTypes().FirstOrDefault(i => typeof(IDownloadPlugin).IsAssignableFrom(i))
                    ?? throw new InvalidDataException("未实现IDownloadService接口无法开启");

            var instance = Activator.CreateInstance(downloadServiceType);
            if (instance is null || instance is not IDownloadPlugin downloadInstance)
                throw new InvalidDataException("继承IDownloadService结构的类没有默认的构造函数");

            return downloadInstance;
        }


        public void Unload()
        {
            if (WindowInstance is not null)
                WindowInstance = null;

            if (downloadServiceType is not null)
                downloadServiceType = null;

            LoadContext.Unload();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
