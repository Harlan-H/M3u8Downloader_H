using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.Abstractions.Plugins
{
    public interface IM3uFileReader
    {
        IM3uFileInfo Read(Uri baseUri, Stream stream);
    }
}
