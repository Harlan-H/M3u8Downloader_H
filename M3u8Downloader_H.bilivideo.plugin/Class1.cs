using M3u8Downloader_H.bilivideo.plugin.Readers;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.bilivideo.plugin
{
    public class Class1 : IPluginBuilder
    {
        public IM3u8FileInfoService? CreateM3u8FileInfoService() => null;

        public IDownloadService? CreatePluginService() => null;

        public void SetAttributeReader(IAttributeReaderManager attributeReader)
        {
            attributeReader.Set("#EXTINF", new MediaAttributeReader());
        }
    }
}