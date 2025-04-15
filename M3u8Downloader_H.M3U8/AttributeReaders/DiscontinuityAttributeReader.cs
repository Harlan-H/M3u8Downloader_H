using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using M3u8Downloader_H.Common.M3u8Infos;
using System.Collections.Generic;
using System;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-DISCONTINUITY", typeof(DiscontinuityAttributeReader))]
    internal class DiscontinuityAttributeReader : AttributeReader
    {
        public override void Write(M3UFileInfo m3UFileInfo, string value, IEnumerator<string> reader, Uri baseUri)
        {
        }
    }
}
