using System.Reflection;
using System.Runtime.Loader;

namespace M3u8Downloader_H.Plugin.Services
{
    public class PluginLoadContext(Dictionary<string, MemoryStream> _assemblies) : AssemblyLoadContext(true)
    {
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (_assemblies.TryGetValue(assemblyName.Name + ".dll", out var memory))
            {
                return LoadFromStream(memory);
            }
            return null; // 用主程序
        }
    }
}
