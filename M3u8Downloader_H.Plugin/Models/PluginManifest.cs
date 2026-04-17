using System.Text.Json.Serialization;

namespace M3u8Downloader_H.Plugin.Models
{
    public class PluginManifest
    {
        public string Key { get; set; } = default!;
        public string Title { get; set; } = default!;

        [JsonPropertyName("dir")]
        public string DirectoryPath { get; set; } = default!;

        public string Entry { get; set; } = default!;
        public string Descriptor { get; set; } = default!;
        public string Auth { get; set; } = default!;
        public Version Version { get; set; } = default!;
        public bool HasUi {  get; set; } = false;
        public bool HasDownload { get; set; } = false;
        public bool Enabled { get; set; } = false;
    }

    [JsonSerializable(typeof(IList<PluginManifest>))]
    public partial class PluginContext : JsonSerializerContext;

}
