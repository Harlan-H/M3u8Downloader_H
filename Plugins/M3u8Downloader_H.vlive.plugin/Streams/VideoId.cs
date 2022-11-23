using M3u8Downloader_H.Common.Extensions;
using System.Text.RegularExpressions;

namespace M3u8Downloader_H.vlive.plugin.Streams
{
    public readonly partial struct VideoId
    {
        private string Value { get; }

        private VideoId(string value) => Value = value;

        public override string ToString() => Value;
    }

    public readonly partial struct VideoId
    {
        private static readonly Regex videoRegex = new(@"(?:|www\.)vlive\.tv\/video/\d+", RegexOptions.Compiled);
        private static readonly Regex postRegex = new(@"(?:|www\.)vlive\.tv\/post\/\d+-\d+", RegexOptions.Compiled);
        private static string? TryNormalize(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            if (videoRegex.IsMatch(url) || postRegex.IsMatch(url))
                return url;

            return null;
        }

        public static VideoId? TryParse(string? url) => TryNormalize(url)?.Pipe(r => new VideoId(r));

        public static VideoId Parse(string url) => TryParse(url) ?? throw new InvalidDataException("不能解析得地址,必须类似 https://www.vlive.tv/video/294312 或者 https://www.vlive.tv/post/1-31149735");

        public static implicit operator VideoId(Uri url) => Parse(url.OriginalString);

        public static implicit operator Uri(VideoId videoid) => new(videoid.ToString());
    }

    public partial struct VideoId : IEquatable<VideoId>
    {
        public bool Equals(VideoId other) => StringComparer.Ordinal.Equals(Value, other.Value);
        public override bool Equals(object? obj) => obj is VideoId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

        public static bool operator ==(VideoId left, VideoId right) => left.Equals(right);
        public static bool operator !=(VideoId left, VideoId right) => !(left == right);
    }

}
