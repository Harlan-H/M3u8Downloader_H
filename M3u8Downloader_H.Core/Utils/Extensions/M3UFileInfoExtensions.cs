using System.IO;
using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.Core.Utils.Extensions
{
    public static class M3UFileInfoExtensions
    {
        public static bool IsVod(this M3UFileInfo m3UFileInfo)
        {
            return !string.IsNullOrWhiteSpace(m3UFileInfo!.PlaylistType) && m3UFileInfo.PlaylistType == "VOD";
        }

    }
}
