using M3u8Downloader_H.M3U8.Core;
using System.Collections.Generic;
using M3u8Downloader_H.M3U8.Infos;
using System;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    internal interface IAttributeReader
    {
        bool Read(M3UFileInfo m3UFileInfo, LineReader reader, KeyValuePair<string, string> keyValuePair, Uri baseUri);
    }
}