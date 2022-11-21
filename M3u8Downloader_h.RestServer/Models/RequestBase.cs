using System.Text.Json.Serialization;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class RequestBase : IValidate
    {
        [JsonPropertyName("name")]
        public string? VideoName { get; set; }
        public string? SavePath { get; set; }

        [JsonPropertyName("plugin")]
        public string? PluginKey { get; set; }
        public IDictionary<string,string>? Headers { get; set; }
    }
}
