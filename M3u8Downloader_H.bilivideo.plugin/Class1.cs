using M3u8Downloader_H.bilibili.plugin.Lives;
using M3u8Downloader_H.bilibili.plugin.Readers;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.bilibili.plugin
{
    public class Class1 : IPluginBuilder
    {
        public IM3u8UriProvider? CreateM3u8UriProvider() => new LiveClient();

        public IM3u8FileInfoStreamService? CreateM3U8FileInfoStreamService() => null;

        public IM3uFileReader? CreateM3u8FileReader() => null;

        public IDownloadService? CreatePluginService() => null;

        public void SetAttributeReader(IAttributeReaderManager attributeReader)
        {
            attributeReader.Set("#EXTINF", new MediaAttributeReader());
        }


    }
}