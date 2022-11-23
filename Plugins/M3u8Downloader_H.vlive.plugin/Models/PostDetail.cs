using M3u8Downloader_H.Common.Utils;
using Newtonsoft.Json;

namespace M3u8Downloader_H.vlive.plugin.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    internal class PostDetail
    {
        [JsonProperty("postDetail.post.officialVideo.liveChatId")]
        public string LiveChatId { get; set; } = default!;

        [JsonProperty("postDetail.post.officialVideo.vodId")]
        public string VodId { get; set; } = default!;
    }
}
