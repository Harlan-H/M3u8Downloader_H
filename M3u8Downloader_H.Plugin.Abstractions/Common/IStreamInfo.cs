using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Abstractions.Common
{
    public interface IStreamInfo
    {
        Uri Url { get; }
        string MediaType { get; }
        string? Codec { get; }
        long? FileSize { get; }

        void SetFileSize(long? fileSize);
        void SetMediaType(string mediaType);
    }
}
