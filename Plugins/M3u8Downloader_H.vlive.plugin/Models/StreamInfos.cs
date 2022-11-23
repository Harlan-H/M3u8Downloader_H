using M3u8Downloader_H.Common.Utils;
using Newtonsoft.Json;

namespace M3u8Downloader_H.vlive.plugin.Models
{
    [JsonConverter(typeof(JsonPathConverter))]
    internal class StreamInfos
    {
        [JsonProperty("streams[0].keys[0].name")]
        public string Param { get; set; } = default!;

        [JsonProperty("streams[0].keys[0].value")]
        public string Value { get; set; } = default!;

        [JsonProperty("streams[0].videos")]
        public IList<VideoInfo> VideoInfos { get; set; } = default!;
    }

    [JsonConverter(typeof(JsonPathConverter))]
    internal class VideoInfo
    {
        [JsonProperty("bitrate.video")]
        public int Bitrate { get; set; }

        [JsonProperty("source")]
        public Uri Source { get; set; } = default!;

        [JsonProperty("template.body.bandwidth")]
        public int BandWidth { get; set; }

        [JsonProperty("template.body.format")]
        public string TsFormat { get; set; } = default!;

        [JsonProperty("template.body.version")]
        public string TsVersion { get; set; } = default!;

        [JsonProperty("template.body.mediaSequence")]
        public int MeidaSequence { get; set; } = default!;

        [JsonProperty("template.body.extInfos")]
        public int[] ExtInfos { get; set; } = default!;
    }
}
