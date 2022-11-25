using M3u8Downloader_H.Common.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.douyin.plugin.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class StreamInfos
    {
        [JsonProperty("app.initialState.roomStore.roomInfo.room.stream_url.hls_pull_url_map.FULL_HD1")]
        public string Url { get; set; } = default!;
    }
}
