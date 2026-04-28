using M3u8Downloader_H.Plugin.Models.Local;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace M3u8Downloader_H.Plugin.Models
{
    public class PluginManifest
    {
        public string Key { get; set; } = default!;

        [JsonPropertyName("Manifest")]
        public BasicInfo BasicInfo { get; set; } = default!;
        public Release Release { get; set; } = default!;
        public Runtime Runtime { get; set; } = default!;
    }

    [JsonSerializable(typeof(PluginManifest))]
    public partial class PluginManifestContext : JsonSerializerContext;
}
