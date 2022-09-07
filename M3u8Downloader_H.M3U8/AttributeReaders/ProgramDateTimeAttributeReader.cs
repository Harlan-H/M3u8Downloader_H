using M3u8Downloader_H.M3U8.Core;
using M3u8Downloader_H.M3U8.Utilities;
using System;
using M3u8Downloader_H.M3U8.Attributes;
using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-PROGRAM-DATE-TIME", typeof(ProgramDateTimeAttributeReader))]
    internal class ProgramDateTimeAttributeReader : AttributeReader
    {
        protected override void Write(M3UFileInfo fileInfo, string value, LineReader reader, Uri baseUri)
        {
            fileInfo.ProgramDateTime = To.Value<DateTime>(value);
        }
    }
}