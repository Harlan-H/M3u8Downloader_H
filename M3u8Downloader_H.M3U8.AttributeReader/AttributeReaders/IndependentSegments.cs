using M3u8Downloader_H.M3U8.AttributeReader.Attributes;

namespace M3u8Downloader_H.M3U8.AttributeReader.AttributeReaders
{
    [M3U8Reader("#EXT-X-INDEPENDENT-SEGMENTS", typeof(IndependentSegments))]
    internal class IndependentSegments : DiscontinuityAttributeReader
    {
    }
}
