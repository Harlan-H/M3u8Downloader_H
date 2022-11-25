using M3u8Downloader_H.douyin.plugin.Lives;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.douyin.plugin
{
    public class Class1 : IPluginBuilder
    {
        public IM3u8FileInfoStreamService? CreateM3U8FileInfoStreamService() => null;

        public IM3uFileReader? CreateM3u8FileReader() => null;

        public IM3u8UriProvider? CreateM3u8UriProvider() => new LiveClient();

        public IDownloadService? CreatePluginService() => null;

        public void SetAttributeReader(IAttributeReaderManager attributeReader)
        {
           
        }
    }
}