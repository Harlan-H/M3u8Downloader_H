using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.Abstractions.Plugins
{
    public interface IM3uFileReader
    {
        M3UFileInfo Read(Uri baseUri, Stream stream);
    }
}
