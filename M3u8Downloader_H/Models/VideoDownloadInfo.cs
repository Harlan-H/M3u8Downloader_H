using Stylet;
using System;

namespace M3u8Downloader_H.Models
{
    public class VideoDownloadInfo : PropertyChangedBase, ICloneable
    {
        public string RequestUrl { get; set; } = default!;
        public string VideoName { get; set; } = default!;

        public string? Method { get; set; } = default!;
        public string? Key { get; set; } = default!;
        public string? Iv { get; set; } = default!;

        public VideoDownloadInfo()
        {

        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
