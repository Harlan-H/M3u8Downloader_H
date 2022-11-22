using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.AttributeReader.Attributes;

namespace M3u8Downloader_H.M3U8.AttributeReader.AttributeReaders
{
    [M3U8Reader("#EXT-X-BYTERANGE", typeof(ByteRangeAttributeReader))]
    internal class ByteRangeAttributeReader : DiscontinuityAttributeReader
    {
    }
}
