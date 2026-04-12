using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.Abstractions.Plugins
{
    public interface IM3uFileReader
    {
        IAttributeReaderCollection CreateAttributeReaderCollection();
        IM3uFileInfo Read(Stream stream);
    }
}
