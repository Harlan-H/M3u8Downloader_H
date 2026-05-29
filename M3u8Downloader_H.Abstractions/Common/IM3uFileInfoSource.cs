using M3u8Downloader_H.Abstractions.M3u8;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace M3u8Downloader_H.Abstractions.Common
{
    public enum M3uType
    {
        [Description("视频")]
        VIDEO,

        [Description("音频")]
        AUDIO,

        [Description("字幕")]
        SUBTITLE
    }

    public interface IM3uFileInfoSource
    {
        Uri RequestUrl { get; }
        IM3uFileInfo? M3uFile { get; set; }
        M3uType Type { get; }
        string Extension { get; }
        string CachePath { get; }
    }
}
