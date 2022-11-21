using M3u8Downloader_H.bilivideo.plugin.Readers;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.bilivideo.plugin
{
    public class Class1 : IPluginBuilder
    {
        public IM3u8UriProvider? CreateM3u8UriProvider() => null;

        public IM3uFileReader? CreateM3u8FileReader() => null;


        public IDownloadService? CreatePluginService() => null;

        public void SetAttributeReader(IAttributeReaderManager attributeReader)
        {
            attributeReader.Set("#EXTINF", new MediaAttributeReader());
        }
    }
}