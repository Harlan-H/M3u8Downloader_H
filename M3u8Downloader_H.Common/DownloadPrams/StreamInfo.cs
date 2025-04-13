using System;
using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.Common.DownloadPrams
{
    public class StreamInfo : IStreamInfo
    {
        public Uri Url { get; }

        public string MediaType { get; private set; }

        public string? Codec { get; private set; }

        public long? FileSize { get; private set; }

        public string Title { get; set; }
        public void SetFileSize(long fileSize)
        {
            if (fileSize < 1000)
                throw new InvalidDataException($"传入的文件大小过小,文件大小是{fileSize}");

            FileSize = fileSize;
        }

        public StreamInfo(Uri url, string title, string mediaType)
        {
            Url = url;
            Title = title;
            MediaType = mediaType;
        }

        public StreamInfo(Uri url, string title, string mediaType,long? filesize)
        {
            Url = url;
            Title = title;
            MediaType = mediaType;
            FileSize = filesize;
        }
    }
}
