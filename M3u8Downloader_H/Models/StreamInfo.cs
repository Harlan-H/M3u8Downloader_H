using System;
using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.Models
{
    public class StreamInfo(Uri url, string mediaType = "video") : IStreamInfo
    {
        public Uri Url { get; } = url;

        public string MediaType { get; private set; } = mediaType;

        public string? Codec { get; private set; }

        public long? FileSize { get; private set; }

        void IStreamInfo.SetFileSize(long? fileSize) =>  FileSize = fileSize;

        void IStreamInfo.SetMediaType(string mediaType)
        {
            if (string.IsNullOrWhiteSpace(MediaType))
                return;

            if (mediaType.StartsWith("video/",StringComparison.CurrentCultureIgnoreCase))
            {
                MediaType = "video";
            }
            else if (mediaType.StartsWith("audio/", StringComparison.CurrentCultureIgnoreCase))
            {
                MediaType = "audio";
            }
        }
    }
}
