using M3u8Downloader_H.M3U8.Attributes;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-DISCONTINUITY-SEQUENCE", typeof(DiscontinuitySequnceAttributeReader))]
    internal class DiscontinuitySequnceAttributeReader : DiscontinuityAttributeReader
    {
    }
}
