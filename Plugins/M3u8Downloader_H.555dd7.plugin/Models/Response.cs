using Newtonsoft.Json;

namespace M3u8Downloader_H._555dd7.plugin.Models
{
    internal class Response
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; } = default!;

        [JsonProperty("message")]
        public string? Msg { get; set; }
    }
}
