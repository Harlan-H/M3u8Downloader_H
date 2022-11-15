using M3u8Downloader_H.RestServer.Attributes;
using System.Text.Json.Serialization;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class RequestWithURI : RequestBase
    {
        [JsonPropertyName("url")]
        [Required(ExceptionMsg = "url不能为空")]
        public Uri Url { get; set; } = default!;

        [Contained(new[] { "AES-128", "AES-192", "AES-256" }, ExceptionMsg = "不可用的key方法,必须是AES-128,AES-192,AES-256其中之一")]
        public string Method { get; set; } = "AES-128";
        public string? Key { get; set; }
        public string? Iv { get; set; }
    }
}
