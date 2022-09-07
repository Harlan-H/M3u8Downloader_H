using System;

namespace M3u8Downloader_H.M3U8.Infos
{
    public class M3UStreamInfo
    {
        public int? ProgramId { get; set; }

        public int? Bandwidth { get; set; }

        public string Codecs { get; set; } = default!;

        public string Resolution { get; set; } = default!;

        public Uri Uri { get; set; } = default!;
    }
}