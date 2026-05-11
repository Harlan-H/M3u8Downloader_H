using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using M3u8Downloader_H.Plugin.Models.Local;

namespace M3u8Downloader_H.Plugin.Models.Online
{
    public class PluginPackage
    {
        public string Key { get; set; } = default!;

        public List<PluginVersion> Versions { get; set; } = default!;
    }

    public class PluginVersion
    {
        [JsonPropertyName("Manifest")]
        public OnlineBasicInfo BasicInfo { get; set; } = default!;
        public OnlineRelease Release { get; set; } = default!;
        public Runtime Runtime { get; set; } = default!;
    }

    [JsonSerializable(typeof(PluginPackage))]
    public partial class PluginPackageContext : JsonSerializerContext;
}
