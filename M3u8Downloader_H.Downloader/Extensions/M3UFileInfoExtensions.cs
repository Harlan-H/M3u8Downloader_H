using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.Downloader.Extensions
{
    public static class M3UFileInfoExtensions
    {
        public static bool IsVod(this IM3uFileInfo m3UFileInfo)
        {
            return !string.IsNullOrWhiteSpace(m3UFileInfo!.PlaylistType) && m3UFileInfo.PlaylistType == "VOD";
        }

    }
}
