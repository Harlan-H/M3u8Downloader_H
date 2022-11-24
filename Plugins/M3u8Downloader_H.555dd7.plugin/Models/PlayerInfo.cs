using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H._555dd7.plugin.Models
{
    internal class PlayerInfo
    {
        [JsonProperty("url")]
        public string Url { get; set; } = default!;
    }
}
