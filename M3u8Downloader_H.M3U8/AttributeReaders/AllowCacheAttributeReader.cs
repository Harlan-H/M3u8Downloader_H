﻿using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using System.Collections.Generic;
using M3u8Downloader_H.M3U8.Utilities;
using System;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-ALLOW-CACHE", typeof(AllowCacheAttributeReader))]
    internal class AllowCacheAttributeReader : AttributeReader
    {
        public override void Write(M3UFileInfo fileInfo, string value, IEnumerator<string> reader, Uri baseUri)
        {
            fileInfo.AllowCache = To.Value<bool>(value);
        }
    }
}