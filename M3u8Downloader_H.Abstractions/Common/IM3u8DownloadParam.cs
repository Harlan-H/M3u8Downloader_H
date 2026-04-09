using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.Abstractions.Common
{
    public interface IM3u8DownloadParam : IDownloadParamBase
    {
        Uri RequestUrl { get; }
        IM3uKeyInfo? M3UKeyInfo { get; }

    }
}
