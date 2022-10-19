using M3u8Downloader_H.M3U8.Core;
using M3u8Downloader_H.M3U8.Attributes;
using M3u8Downloader_H.M3U8.Infos;
using System;
using System.Collections.Generic;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-DISCONTINUITY", typeof(DiscontinuityAttributeReader))]
    internal class DiscontinuityAttributeReader : IAttributeReader
    {
        public bool ShouldTerminate => false;

        public void Write(M3UFileInfo m3UFileInfo, string value, IEnumerator<string> reader, Uri baseUri)
        {
        }
    }
}
