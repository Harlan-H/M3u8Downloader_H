using System;
using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.Common.M3u8Infos
{
    public class M3UStreamInfo : IM3uStreamInfo
    {
        public int? ProgramId { get; set; }

        public int? Bandwidth { get; set; }

        public string Codecs { get; set; } = default!;

        public string Resolution { get; set; } = default!;

        public Uri Uri { get; set; } = default!;
    }
}