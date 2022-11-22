using M3u8Downloader_H.Common.Utils;
using Newtonsoft.Json;

namespace M3u8Downloader_H.bilibili.plugin.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    internal class LiveRoomInfos
    {
        [JsonProperty("roomInitRes.code")]
        public int Code { get; set; }

        [JsonProperty("roomInitRes.message")]
        public string Message { get; set; } = string.Empty;

        [JsonProperty("roomInitRes.data.playurl_info.playurl.stream")]
        public IList<StreamInfo> StreamInfos { get; set; } = default!;
    }

    [JsonConverter(typeof(JsonPathConverter))]
    internal class StreamInfo
    {
        [JsonProperty("protocol_name")]
        public string ProtocolName { get; set; } = string.Empty;

        [JsonProperty("format")]
        public IList<CodecInfo> CodecInfos { get; set; } = default!;
    }

    [JsonConverter(typeof(JsonPathConverter))]
    internal class CodecInfo
    {
        [JsonProperty("format_name")]
        public string FormatName { get; set; } = string.Empty;

        [JsonProperty("codec[0].base_url")]
        public string BaseUrl { get; set; } = string.Empty;

        [JsonProperty("codec[0].url_info[0].host")]
        public string HostUrl { get; set; } = string.Empty;

        [JsonProperty("codec[0].url_info[0].extra")]
        public string Extra { get; set; } = string.Empty;
    }
}
