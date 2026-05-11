using M3u8Downloader_H.Plugin.Models.Local;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Plugin.Models.Online
{
    public class OnlineBasicInfo : BasicInfo
    {
        public Uri Repo { get; set; } = default!;
        public DateTime Time { get; set; } = default!;
    }
}
