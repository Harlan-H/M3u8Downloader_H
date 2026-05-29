using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace M3u8Downloader_H.Common.DownloadPrams
{
    public class M3uFileInfoSource(Uri url, M3uType m3UType) : IM3uFileInfoSource
    {
        public Uri RequestUrl { get; } = url;
        public IM3uFileInfo? M3uFile { get; set; }
        public M3uType Type { get; } = m3UType;
        public string Extension => Path.GetExtension(M3uFile?.MediaFiles?.First().Uri.AbsolutePath) ?? string.Empty;
        public string CachePath => Type.ToString();

        public M3uFileInfoSource(Uri Url) : this(Url,M3uType.VIDEO)
        {

        }

        public M3uFileInfoSource(IM3uFileInfo m3UFileInfo) : this(default!, M3uType.VIDEO)
        {
            M3uFile = m3UFileInfo;
        }

        public M3uFileInfoSource(Uri requestUrl, IM3uFileInfo? m3uFile) : this(requestUrl, M3uType.VIDEO)
        {
            M3uFile = m3uFile;
        }
    }
}
