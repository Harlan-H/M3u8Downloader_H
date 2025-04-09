using System;
using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.Common.DownloadPrams
{
    public class StreamInfo(Uri url,string title, string mediaType) : IStreamInfo
    {
        public Uri Url { get; } = url;

        public string MediaType { get; private set; } = mediaType;

        public string? Codec { get; private set; }

        public long? FileSize { get; private set; }

        public string Title { get; private set; } = title;
        public void SetFileSize(long? fileSize) => FileSize = fileSize;
    }
}
