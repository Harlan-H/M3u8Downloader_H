using M3u8Downloader_H.M3U8.Core;
using M3u8Downloader_H.M3U8.Utilities;
using System;
using M3u8Downloader_H.M3U8.Attributes;
using M3u8Downloader_H.M3U8.Infos;
using System.Collections.Generic;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-PROGRAM-DATE-TIME", typeof(ProgramDateTimeAttributeReader))]
    internal class ProgramDateTimeAttributeReader : IAttributeReader
    {
        public bool ShouldTerminate => false;

        public void Write(M3UFileInfo fileInfo, string value, IEnumerator<string> reader, Uri baseUri)
        {
            fileInfo.ProgramDateTime = To.Value<DateTime>(value);
        }
    }
}