namespace M3u8Downloader_H.Abstractions.Plugins
{
    public interface IPluginManager
    {
        IM3u8FileInfoStreamService? M3U8FileInfoStreamService { get; }
        IM3uFileReader? M3UFileReaderInterface { get; }
        IDictionary<string, IAttributeReader> AttributeReaders { get; }
        IDownloadService? PluginService { get; }
    }
}
