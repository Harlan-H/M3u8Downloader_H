using M3u8Downloader_H.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace M3u8Downloader_H.bilivideo.plugin.Lives
{
    public readonly partial struct LiveId
    {
        private string Value { get; }

        public string Url => $"https://live.bilibili.com/{Value}";

        private LiveId(string value) => Value = value;

        public override string ToString() => Value;
    }

    public readonly partial struct LiveId
    {
        private static readonly Regex regex = new(@"live\.bilibili.*?/(\d+)(?:\?|$)", RegexOptions.Compiled);
        private static string? TryNormalize(string? liveIdOrUrl)
        {
            if (string.IsNullOrWhiteSpace(liveIdOrUrl))
                return null;

            var regularMatch = regex.Match(liveIdOrUrl).Groups[1].Value;
            if (!string.IsNullOrWhiteSpace(regularMatch))
                return regularMatch;

            return null;
        }

        public static LiveId? TryParse(string? liveIdOrUrl) => TryNormalize(liveIdOrUrl)?.Pipe(r => new LiveId(r));

        public static LiveId Parse(string liveIdOrUrl) => TryParse(liveIdOrUrl) ?? throw new InvalidDataException($"无效的视频id或者url {liveIdOrUrl}");

        public static implicit operator LiveId(string liveIdOrUrl) => Parse(liveIdOrUrl);

        public static implicit operator string(LiveId liveId) => liveId.ToString();

        public static implicit operator int(LiveId liveId) => Convert.ToInt32(liveId.Value);
    }

    public partial struct LiveId : IEquatable<LiveId>
    {
        public bool Equals(LiveId other) => StringComparer.Ordinal.Equals(Value, other.Value);
        public override bool Equals(object? obj) => obj is LiveId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

        public static bool operator ==(LiveId left, LiveId right) => left.Equals(right);
        public static bool operator !=(LiveId left, LiveId right) => !(left == right);
    }
}
