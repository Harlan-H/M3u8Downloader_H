using M3u8Downloader_H.Abstractions.Plugins;
using System.Reflection;
using System.Text.RegularExpressions;

namespace M3u8Downloader_H.Plugin.PluginClients
{
    public partial class PluginClient
    {
        private static readonly string _filterStr = "M3u8Downloader_H.*.plugin.dll";
        private readonly Dictionary<string, Type> _pluginDict = [];
        public IEnumerable<string> Keys => _pluginDict.Keys;

        public string PluginPath { get; set; } = default!;

        private PluginClient()
        {

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
            }
            catch (DirectoryNotFoundException)
            {

            }
        }

        private void LoadFile(string fullPath,string fileName)
        {
            Type? type = LoadLibrary(fullPath);
            string key = _pluginKeyRegex().Match(fileName).Groups[1].Value;
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

        [GeneratedRegex(@"M3u8Downloader_H\.(.*?)\.plugin", RegexOptions.Compiled)]
        private static partial Regex _pluginKeyRegex();
    }


    public partial class PluginClient
    {
        private readonly static PluginClient instance = new();
        public static PluginClient Instance => instance;
    }
}
