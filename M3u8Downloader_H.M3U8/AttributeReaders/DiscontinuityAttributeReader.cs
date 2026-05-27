using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using M3u8Downloader_H.M3U8.Models;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-DISCONTINUITY", typeof(DiscontinuityAttributeReader))]
    internal class DiscontinuityAttributeReader : AttributeReader
    {
        public override Task WriteAsync(M3UFileInfo m3UFileInfo, string value, IAsyncEnumerator<string> reader, Uri baseUri)
        {
            return Task.CompletedTask;
        }
    }
}
