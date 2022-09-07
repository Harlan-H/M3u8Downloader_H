using System;

namespace M3u8Downloader_H.M3U8.Infos
{
    public partial class M3UKeyInfo
    {
        public string Method { get; set; } = default!;

        public Uri Uri { get; set; } = default!;

        public byte[] BKey { get; set; } = default!;

        public byte[] IV { get; set; } = default!;
    }
    
    public partial class M3UKeyInfo :IEquatable<M3UKeyInfo>
    {
        public bool Equals(M3UKeyInfo? other) => Object.ReferenceEquals(this, other);

        public override bool Equals(object? obj) => obj is M3UKeyInfo && Equals(obj);

        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode();

        public static bool operator ==(M3UKeyInfo M3UKeyInfo1, M3UKeyInfo M3UKeyInfo2) => M3UKeyInfo1.Equals(M3UKeyInfo2);
        public static bool operator !=(M3UKeyInfo M3UKeyInfo1, M3UKeyInfo M3UKeyInfo2) => !(M3UKeyInfo1 == M3UKeyInfo2);
    } 
}