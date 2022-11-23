using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.vlive.plugin.Models
{
    internal class InkeyInfos
    {
        [JsonProperty("inkey")]
        public string Inkey { get; set; } = default!;
    }
}
