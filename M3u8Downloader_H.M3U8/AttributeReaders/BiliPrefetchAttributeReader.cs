using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M3u8Downloader_H.M3U8.Attributes;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-BILI-PREFETCH", typeof(BiliPrefetchAttributeReader))]
    internal class BiliPrefetchAttributeReader : DiscontinuityAttributeReader
    {
    }
}
