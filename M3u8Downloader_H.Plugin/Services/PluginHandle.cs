using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Plugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Plugin.Services
{
    public class PluginHandle(string PluginPath, PluginManifest pluginManifest)
    {
        public PluginManifest PluginManifest => pluginManifest;
        public IPluginEntry? Instance { get; private set; }
        public PluginLoadContext LoadContext { get; set; } = default!;

        public void Toggle(bool Enable)
        {
            PluginManifest.Enabled = Enable;
        }

        public IPluginEntry Load()
        {
            var dirPath = Path.Combine(PluginPath, pluginManifest.DirecotryPath);
            var dllPath = Path.Combine(dirPath, pluginManifest.Entry);

            var alc = new PluginLoadContext(dirPath);
            var asm = alc.LoadFromAssemblyPath(dllPath);

            Type type = asm.GetExportedTypes().FirstOrDefault(i => typeof(IPluginEntry).IsAssignableFrom(i))
                ?? throw new InvalidDataException("未实现IPluginEntry接口无法开启");

            var instance = Activator.CreateInstance(type);
            if (instance is null || instance is not IPluginEntry PluginInstance)
                throw new InvalidDataException("没有默认的构造函数");

            Instance = PluginInstance;
            LoadContext = alc;
            return PluginInstance;
        }


        public void Unload()
        {
            Instance = null;
            LoadContext.Unload();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }


    }
}
