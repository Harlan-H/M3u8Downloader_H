using M3u8Downloader_H.bilivideo.plugin.Readers;
using M3u8Downloader_H.M3U8.AttributeReaderManager;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.bilivideo.plugin
{
    public class Class1 : IPluginBuilder
    {
        public IDownloadService? CreatePluginService() => null;

        public void SetAttributeReader(IAttributeReaderManager attributeReader)
        {
            attributeReader.Set("#EXTINF", new MediaAttributeReader());
        }
    }
}