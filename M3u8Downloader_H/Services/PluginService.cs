using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.Services
{
    public class PluginService
    {
        private readonly string _pluginDirPath =
#if DEBUG
            "e:/desktop/Plugins/";
#else
             Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
#endif
        private readonly Dictionary<string, Type> _pluginDict;
        public PluginService()
        {
            _pluginDict = new();
        }

        public void Load()
        {
            try
            {
                DirectoryInfo directoryInfo = new(_pluginDirPath);
                foreach (var item in directoryInfo.EnumerateFiles("M3u8Downloader_H.*.plugin.dll", SearchOption.TopDirectoryOnly))
                {
                    Type? type = LoadLibrary(item.FullName);
                    string key = item.Name.Normalize(@"M3u8Downloader_H\.(.*?)\.plugin");
                    if (type is not null && !string.IsNullOrWhiteSpace(key))
                        _pluginDict.Add(key, type);
                }
            }
            catch (DirectoryNotFoundException)
            {

            }
        }

        private Type? LoadLibrary(string path)
        {
            Type[] exportTypes = Assembly.LoadFrom(path).GetExportedTypes();
            return exportTypes.Where(i => i.GetInterface(nameof(IPluginBuilder)) != null).FirstOrDefault();
        }

        public IPluginBuilder? this[string key]
        {
            get
            {
                if (_pluginDict.TryGetValue(key, out Type? type))
                    return (IPluginBuilder?)Activator.CreateInstance(type);
                return null;
            }
        }

        public IEnumerable<string> Keys => _pluginDict.Keys;

    }
}
