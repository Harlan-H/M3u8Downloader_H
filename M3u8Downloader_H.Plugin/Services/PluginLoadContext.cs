using System.Reflection;
using System.Runtime.Loader;

namespace M3u8Downloader_H.Plugin.Services
{
    public class PluginLoadContext(string pluginPath) : AssemblyLoadContext(isCollectible: true)
    {
        private readonly AssemblyDependencyResolver _resolve = new(pluginPath);

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (assemblyName.Name!.StartsWith("Avalonia") ||
                assemblyName.Name!.StartsWith("CommunityToolkit.Mvvm") ||
                assemblyName.Name!.StartsWith("M3u8Downloader_H.Common") ||
                assemblyName.Name!.StartsWith("M3u8Downloader_H.Abstractions"))
                return null;

            var path = _resolve.ResolveAssemblyToPath(assemblyName);
            return path is not null ? LoadFromAssemblyPath(path) : null;
        }
    }
}
