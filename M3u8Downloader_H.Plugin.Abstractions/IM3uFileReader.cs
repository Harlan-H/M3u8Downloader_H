using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.Plugin
{
    public interface IM3uFileReader
    {
        M3UFileInfo Read(Uri baseUri, Stream stream);
    }
}
