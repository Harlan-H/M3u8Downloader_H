using System;
using System.Collections.Generic;
using M3u8Downloader_H.M3U8.Attributes;
using M3u8Downloader_H.M3U8.Core;
using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-ENDLIST", typeof(EndListAttributeReader))]
    internal class EndListAttributeReader : IAttributeReader
    {
        public bool ShouldTerminate => true;

        public void Write(M3UFileInfo m3UFileInfo,string value, IEnumerator<string> reader, Uri baseUri)
        {
            m3UFileInfo.PlaylistType = "VOD";
        }
    }
}