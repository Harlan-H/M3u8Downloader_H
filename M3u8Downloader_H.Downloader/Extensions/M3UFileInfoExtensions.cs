using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.Downloader.Extensions
{
    public static class M3UFileInfoExtensions
    {
        public static bool IsVod(this M3UFileInfo m3UFileInfo)
        {
            return !string.IsNullOrWhiteSpace(m3UFileInfo!.PlaylistType) && m3UFileInfo.PlaylistType == "VOD";
        }

    }
}
