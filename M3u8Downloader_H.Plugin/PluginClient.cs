using M3u8Downloader_H.Plugin.Extensions;
using System.Reflection;

namespace M3u8Downloader_H.Plugin.PluginClients
{
    public partial class PluginClient
    {
        private readonly Dictionary<string, Type> _pluginDict = new();
        public IEnumerable<string> Keys => _pluginDict.Keys;

        private PluginClient()
        {

        }

        public void Load(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = new(path);
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

        private static Type? LoadLibrary(string path)
        {
            try
            {
                Type[] exportTypes = Assembly.LoadFrom(path).GetExportedTypes();
                return exportTypes.FirstOrDefault(i => typeof(IPluginBuilder).IsAssignableFrom(i));
            }catch(FileLoadException)
            {
                return null;
            }
        }

        public IPluginBuilder? CreatePluginBuilder(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            if (_pluginDict.TryGetValue(key, out Type? type))
                return (IPluginBuilder?)Activator.CreateInstance(type);
            return null;
        }
    }


    public partial class PluginClient
    {
        private readonly static PluginClient instance = new();
        public static PluginClient Instance => instance;
    }
}
