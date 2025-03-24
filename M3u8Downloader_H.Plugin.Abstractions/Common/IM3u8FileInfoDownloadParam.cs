using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.Abstractions.Common
{
    public interface IM3u8FileInfoDownloadParam : IDownloadParamBase
    {
        IM3uFileInfo M3UFileInfos { get; }
    }
}
