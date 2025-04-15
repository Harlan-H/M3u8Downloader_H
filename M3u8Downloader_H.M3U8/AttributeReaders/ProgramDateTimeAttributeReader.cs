using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using System.Collections.Generic;
using System;
using M3u8Downloader_H.M3U8.Utilities;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-PROGRAM-DATE-TIME", typeof(ProgramDateTimeAttributeReader))]
    internal class ProgramDateTimeAttributeReader : AttributeReader
    {

        public override void Write(M3UFileInfo fileInfo, string value, IEnumerator<string> reader, Uri baseUri)
        {
            fileInfo.ProgramDateTime = To.Value<DateTime>(value);
        }
    }
}