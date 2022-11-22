namespace M3u8Downloader_H.Plugin.PluginManagers
{
    public interface IPluginManager
    {
        IM3u8UriProvider? M3U8UriProvider { get; }
        IM3u8FileInfoStreamService? M3U8FileInfoStreamService { get; }
        IM3uFileReader? M3UFileReaderInterface { get; }
        IDictionary<string, IAttributeReader> AttributeReaders { get; }
        IDownloadService? PluginService { get; }
    }
}
