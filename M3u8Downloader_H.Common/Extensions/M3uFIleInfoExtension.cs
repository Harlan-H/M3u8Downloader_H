using M3u8Downloader_H.Abstractions.M3u8;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Common.Extensions
{
    public static class M3uFIleInfoExtension
    {
        extension(IM3uFileInfo m3UFileInfo)
        {
            public bool IsFile => m3UFileInfo.MediaFiles.Any(m => m.Uri.IsFile);

            public bool IsCrypted => m3UFileInfo.Key is not null;

            public bool IsEmpty => m3UFileInfo.MediaFiles is null || m3UFileInfo.MediaFiles?.Count == 0;

            public bool IsVod()
                => !string.IsNullOrWhiteSpace(m3UFileInfo!.PlaylistType) && m3UFileInfo.PlaylistType == "VOD";
            
        }
    }
}
