using M3u8Downloader_H.M3U8.Attributes;
using M3u8Downloader_H.M3U8.AttributeReaders;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-INDEPENDENT-SEGMENTS", typeof(IndependentSegments))]
    internal class IndependentSegments : DiscontinuityAttributeReader
    {
    }
}
