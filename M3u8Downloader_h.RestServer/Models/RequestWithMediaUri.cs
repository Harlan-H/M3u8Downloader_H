using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.RestServer.Attributes;
using System.Text.Json.Serialization;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class RequestWithMediaUri : RequestBase
    {
        public bool IsVideoStream { get; set; } = true;

        [Required(ExceptionMsg = "video必须填写不能为空")]
        public MediaInfo Video { get; set; } = default!;
        public MediaInfo? Audio { get; set; }
    }

    internal class MediaInfo
    {
        [Required(ExceptionMsg = "url不能为空")]
        public Uri Url { get; set; } = default!;

        public long? FileSize { get; set; }
    }

    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(RequestWithMediaUri))]
    internal partial class RequestWithMediaUriContext : JsonSerializerContext;
}
