using M3u8Downloader_H.Abstractions.M3u8;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.M3U8.Models
{
    public class M3UMediaManifest : IM3uMediaManifest
    {
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;

        public string GroupId { get; set; } = default!;

        public string AutoSelect { get; set; } = default!;

        public string Language { get; set; } = string.Empty;

        public string Default { get; set; } = default!;

        public string Channels { get; set; } = default!;

        public Uri Uri { get; set; } = default!;
    }
}
