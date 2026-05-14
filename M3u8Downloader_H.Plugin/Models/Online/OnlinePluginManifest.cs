using M3u8Downloader_H.Plugin.Models.Local;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace M3u8Downloader_H.Plugin.Models.Online
{
    public class OnlinePluginManifest
    {
        public string Key { get; set; } = default!;

        [JsonPropertyName("Manifest")]
        public OnlineBasicInfo BasicInfo { get; set; } = default!;
        public OnlineRelease Release { get; set; } = default!;
        public Runtime Runtime { get; set; } = default!;
    }

    public class OnlinePlugin
    {
        [JsonPropertyName("plugins")]
        public List<OnlinePluginManifest> OnlinePluginManifests { get; set; } = default!;
    } 

    [JsonSerializable(typeof(OnlinePlugin))]
    public partial class OnlinePluginManifestContext : JsonSerializerContext;
}
