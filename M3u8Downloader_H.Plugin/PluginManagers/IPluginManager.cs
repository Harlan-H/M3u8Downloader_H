using M3u8Downloader_H.M3U8.AttributeReaders;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.Plugin.PluginManagers
{
    public interface IPluginManager
    {
        IDictionary<string, IAttributeReader> AttributeReaders { get; }
        IDownloadService? PluginService { get; }
        IM3u8FileInfoService? M3U8FileInfoService { get; }
    }
}
