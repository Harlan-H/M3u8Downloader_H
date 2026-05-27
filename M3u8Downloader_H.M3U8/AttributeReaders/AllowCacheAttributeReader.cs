
using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using System.Collections.Generic;
using M3u8Downloader_H.M3U8.Utilities;
using System;
using System.Threading.Tasks;
using M3u8Downloader_H.M3U8.Models;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-ALLOW-CACHE", typeof(AllowCacheAttributeReader))]
    internal class AllowCacheAttributeReader : AttributeReader
    {
        public override Task WriteAsync(M3UFileInfo fileInfo, string value, IAsyncEnumerator<string> reader, Uri baseUri)
        {
            fileInfo.AllowCache = To.Value<bool>(value);
            return Task.CompletedTask;
        }
    }
}