using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.vlive.plugin.Readers;
using M3u8Downloader_H.vlive.plugin.Streams;

namespace M3u8Downloader_H.vlive.plugin
{
    public class Class1 : IPluginBuilder
    {
        public IM3u8FileInfoStreamService? CreateM3U8FileInfoStreamService() => null;

        public IM3uFileReader? CreateM3u8FileReader() => new M3UFileReader();

        public IM3u8UriProvider? CreateM3u8UriProvider() => new StreamClient();

        public IDownloadService? CreatePluginService() => null;

        public void SetAttributeReader(IAttributeReaderManager attributeReader)
        {
        }
    }
}