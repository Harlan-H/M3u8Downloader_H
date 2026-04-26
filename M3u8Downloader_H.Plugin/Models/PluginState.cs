using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace M3u8Downloader_H.Plugin.Models
{
    public class PluginState
    {
        public string Key { get; set; } = default!;
        public bool Enabled { get; set; }
        public Version CurrentVersion { get; set; } = default!;
        public DateTime LastLoadedTime { get; set; } = DateTimeOffset.Now.LocalDateTime;

        public string LoadMode { get; set; } = "memory";
    }

    [JsonSerializable(typeof(Dictionary<string, PluginState>))]
    public partial class PluginStateContext : JsonSerializerContext;
}
