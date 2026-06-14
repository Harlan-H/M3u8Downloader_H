using M3u8Downloader_H.RestServer.Attributes;
using System.Text.Json.Serialization;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class RequestWithGetM3u8FileInfo : IValidate
    {
        [Required(ExceptionMsg = "content不能为空")]
        public string Content { get; set; } = default!;

        [JsonPropertyName("baseurl")]
        public Uri? Url { get; set; }
    }

    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(RequestWithGetM3u8FileInfo))]
    internal partial class RequestWithGetM3u8FileInfoContext : JsonSerializerContext;
}
