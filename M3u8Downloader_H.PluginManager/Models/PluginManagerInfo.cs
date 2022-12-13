using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.PluginManager.Models
{
    public class PluginManagerInfo
    {
        public IList<PluginInfo> Plugins { get; set; } = default!;
    }

    public class PluginInfo
    {
        public string Title { get; set; } = default!;
        public string Version { get; set; } = default!;
        public string  RequiredVersion { get; set; } = default!;
        public string FileName { get; set; } = default!;
    }
}
