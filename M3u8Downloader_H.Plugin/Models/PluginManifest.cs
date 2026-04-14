using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace M3u8Downloader_H.Plugin.Models
{
    public partial class PluginManifest
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string DirecotryPath { get; set; }
        public string Entry { get; set; }
        public string Descriptor { get; set; }
        public string Auth { get; set; }
        public Version Version { get; set; }
        public bool Enabled { get; set; } = false;
    }

    public partial class PluginManifest
    {
       


    }

    [JsonSerializable(typeof(IList<PluginManifest>))]
    public partial class PluginContext : JsonSerializerContext;

}
