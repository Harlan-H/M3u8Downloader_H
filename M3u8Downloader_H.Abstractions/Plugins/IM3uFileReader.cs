using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.Abstractions.Plugins
{
    public interface IM3uFileReader
    {
        void InitAttributeReade(IAttributeReaderCollection readers);

        IM3uFileInfo GetM3u8FileInfo(Stream stream);
    }
}
