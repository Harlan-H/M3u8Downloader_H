using System.Text.Json.Serialization;
using M3u8Downloader_H.RestServer.Attributes;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class RequestBase : IValidate
    {
        [JsonPropertyName("name")]
        public string VideoName { get; set; } = default!;

        public string SavePath { get; set; } = string.Empty;

        public IDictionary<string,string>? Headers { get; set; }
    }
}
