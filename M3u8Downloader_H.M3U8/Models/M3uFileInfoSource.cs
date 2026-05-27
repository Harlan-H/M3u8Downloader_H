using M3u8Downloader_H.Abstractions.M3u8;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.M3U8.Models
{
    public enum M3uType
    {
        VIDEO,
        AUDIO,
        SUBTITLE
    }

    public class M3uFileInfoSource(Uri Url, M3uType m3UType)
    {
        public Uri Requests { get; } = Url;
        public IM3uFileInfo? M3uFile { get; set; }
        public M3uType Type { get; } = m3UType;
        public bool IsEmpty => M3uFile is null;

        public M3uFileInfoSource(Uri Url) : this(Url, M3uType.VIDEO)
        {

        }
    }
}
