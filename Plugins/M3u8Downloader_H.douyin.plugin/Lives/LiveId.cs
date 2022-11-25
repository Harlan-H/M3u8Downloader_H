using M3u8Downloader_H.Common.Extensions;
using System.Text.RegularExpressions;

namespace M3u8Downloader_H.douyin.plugin.Lives
{
    public readonly partial struct LiveId
    {
        private string Value { get; }

        public string Url => $"https://live.douyin.com/{Value}";

        private LiveId(string value) => Value = value;

        public override string ToString() => Value;
    }

    public readonly partial struct LiveId
    {
        private static readonly Regex regex = new(@"live\.douyin.*?/(\d+)(?:\?|$)", RegexOptions.Compiled);
        private static string? TryNormalize(string? liveIdOrUrl)
        {
            if (string.IsNullOrWhiteSpace(liveIdOrUrl))
                return null;

            var regularMatch = regex.Match(liveIdOrUrl).Groups[1].Value;
            if (!string.IsNullOrWhiteSpace(regularMatch))
                return regularMatch;

            return null;
        }

        public static LiveId? TryParse(string? url) => TryNormalize(url)?.Pipe(r => new LiveId(r));

        public static LiveId Parse(string url) => TryParse(url) ?? throw new InvalidDataException("不能解析得地址,必须是抖音网页版地址，例如https://live.douyin.com/969591501540");

        public static implicit operator LiveId(Uri liveIdOrUrl) => Parse(liveIdOrUrl.OriginalString);

        public static implicit operator Uri(LiveId liveId) => new(liveId.Url);
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
