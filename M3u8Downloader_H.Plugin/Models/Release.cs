using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Plugin.Models
{
    public class Release
    {
        public Version Version { get; set; } = default!;
        public Version MinAppVersion { get; set; } = default!;
    }
}
