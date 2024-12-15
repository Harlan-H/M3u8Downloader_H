using M3u8Downloader_H.Plugin.Extensions;
using System.Reflection;

namespace M3u8Downloader_H.Plugin.PluginClients
{
    public partial class PluginClient
    {
        private static readonly string _filterStr = "M3u8Downloader_H.*.plugin.dll";
        private static readonly string _pluginKeyRegex = @"M3u8Downloader_H\.(.*?)\.plugin";
        private readonly Dictionary<string, Type> _pluginDict = [];
        private readonly FileSystemWatcher watcher = new();
        public IEnumerable<string> Keys => _pluginDict.Keys;

        public string PluginPath { get; set; } = default!;

        private PluginClient()
        {

        }

        public void Init()
        {
            watcher.Path = PluginPath;
            watcher.Filter = _filterStr;
            watcher.NotifyFilter = NotifyFilters.FileName;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Name)) return;

            string key = e.Name!.Normalize(_pluginKeyRegex);
            if (_pluginDict.ContainsKey(key))
            {
                _pluginDict.Remove(key);
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Name)) return;

            LoadFile(e.FullPath, e.Name!);
        }

        public void Load()
        {
            try
            {
                DirectoryInfo directoryInfo = new(PluginPath);
                foreach (var item in directoryInfo.EnumerateFiles(_filterStr, SearchOption.TopDirectoryOnly))
                {
                    LoadFile(item.FullName, item.Name);
                }
                watcher.EnableRaisingEvents = true;
            }
            catch (DirectoryNotFoundException)
            {

            }
        }

        private void LoadFile(string fullPath,string fileName)
        {
            Type? type = LoadLibrary(fullPath);
            string key = fileName.Normalize(_pluginKeyRegex);
            if (type is not null && !string.IsNullOrWhiteSpace(key))
                _pluginDict.Add(key, type);
        }


        private static Type? LoadLibrary(string path)
        {
            try
            {
                Type[] exportTypes = Assembly.LoadFile(path).GetExportedTypes();
                return exportTypes.FirstOrDefault(i => typeof(IPluginBuilder).IsAssignableFrom(i));
            }catch(FileLoadException)
            {
                return null;
            }
        }

        public Type? GetPluginType(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            if (_pluginDict.TryGetValue(key, out Type? type))
                return type;
            return null;
        }
    }


    public partial class PluginClient
    {
        private readonly static PluginClient instance = new();
        public static PluginClient Instance => instance;
    }
}
