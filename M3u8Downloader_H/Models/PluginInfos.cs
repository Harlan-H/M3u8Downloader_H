using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace M3u8Downloader_H.Models
{
    [Serializable]
    [XmlRoot(ElementName = "config")]
    public class PluginInfos
    {
        [XmlArray("plugins")]
        public List<PluginItem> PluginItems { get; set; } = default!;
    }

    [Serializable]
    [XmlType(TypeName = "pluginItem")]
    public partial class PluginItem
    {
        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; } = default!;

        [XmlAttribute(AttributeName = "filepath")]
        public string FilePath { get; set; } = default!;
    }

    public partial class PluginItem : IEquatable<PluginItem>
    {
        public bool Equals(PluginItem? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return StringComparer.OrdinalIgnoreCase.Equals(other.Title, Title) &&
                   StringComparer.OrdinalIgnoreCase.Equals(other.FilePath, FilePath);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((PluginItem)obj);
        }

        public override int GetHashCode() => HashCode.Combine(
            StringComparer.OrdinalIgnoreCase.GetHashCode(Title),
            StringComparer.OrdinalIgnoreCase.GetHashCode(FilePath));
    }
}
