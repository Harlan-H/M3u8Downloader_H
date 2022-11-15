using M3u8Downloader_H.RestServer.Attributes;
using System.Text.Json.Serialization;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class RequestWithContent : RequestBase
    {
        [Required(ExceptionMsg = "content不能为空")]
        public string Content { get; set; } = default!;

        [JsonPropertyName("baseurl")]
        public Uri? Url { get; set; }
    }
}
