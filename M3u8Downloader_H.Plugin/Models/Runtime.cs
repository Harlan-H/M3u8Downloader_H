using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Plugin.Models
{
    public class Runtime
    {
        public string EntryPoint { get; set; } = default!;
        public string EntryType { get; set; } = default!;
        public bool HasUi { get; set; } = false;
        public bool HasDownload { get; set; } = false;
    }
}
