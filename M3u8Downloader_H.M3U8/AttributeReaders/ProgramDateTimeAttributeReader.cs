using M3u8Downloader_H.M3U8.Models;
using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using System.Collections.Generic;
using System;
using M3u8Downloader_H.M3U8.Utilities;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-PROGRAM-DATE-TIME", typeof(ProgramDateTimeAttributeReader))]
    internal class ProgramDateTimeAttributeReader : AttributeReader
    {

        public override Task WriteAsync(M3UFileInfo fileInfo, string value, IAsyncEnumerator<string> reader, Uri baseUri)
        {
            fileInfo.ProgramDateTime = To.Value<DateTime>(value);
            return Task.CompletedTask;
        }
    }
}