using System.Text.Json.Serialization;
using M3u8Downloader_H.RestServer.Attributes;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class RequestBase : IValidate
    {
        [JsonPropertyName("name")]
        public string VideoName { get; set; } = default!;

        [Required(ExceptionMsg = "savepath必须填写不能为空")]
        public string SavePath { get; set; } = default!;

        [JsonPropertyName("plugin")]
        public string? PluginKey { get; set; }
        public IDictionary<string,string>? Headers { get; set; }
    }
}
