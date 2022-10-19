using M3u8Downloader_H.M3U8.Attributes;
using M3u8Downloader_H.M3U8.Core;
using M3u8Downloader_H.M3U8.Utilities;
using M3u8Downloader_H.M3U8.Infos;
using System;
using System.Collections.Generic;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-TARGETDURATION", typeof(TargetDurationAttritubeReader))]
    internal class TargetDurationAttritubeReader : IAttributeReader
    {
        public bool ShouldTerminate => false;

        public void Write(M3UFileInfo fileInfo,string value, IEnumerator<string> reader, Uri baseUri)
        {
            fileInfo.TargetDuration = To.Value<int>(value);
        }
    }
}
