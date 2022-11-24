using M3u8Downloader_H._555dd7.plugin.Streams;
using M3u8Downloader_H.Plugin;
using System.Net.WebSockets;
using System.Security.Cryptography;

namespace M3u8Downloader_H._555dd7.plugin
{
    public class Class1 : IPluginBuilder
    {
        public IM3u8FileInfoStreamService? CreateM3U8FileInfoStreamService() => null;

        public IM3uFileReader? CreateM3u8FileReader() => null;

        public IM3u8UriProvider? CreateM3u8UriProvider() => new StreamClient();

        public IDownloadService? CreatePluginService() => null;

        public void SetAttributeReader(IAttributeReaderManager attributeReader)
        {
            
        }
    }
}