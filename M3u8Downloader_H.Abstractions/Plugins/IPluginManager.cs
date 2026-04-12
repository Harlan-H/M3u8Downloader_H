namespace M3u8Downloader_H.Abstractions.Plugins
{
    public interface IPluginManager
    {
        IM3uFileReader? M3UFileReaderInterface { get; }
        IDownloadService? PluginService { get; }
    }
}
