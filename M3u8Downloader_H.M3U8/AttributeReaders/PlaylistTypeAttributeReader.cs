using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-PLAYLIST-TYPE", typeof(PlaylistTypeAttributeReader))]
    internal class PlaylistTypeAttributeReader : AttributeReader
    {
        public override Task WriteAsync(M3UFileInfo fileInfo, string value, IAsyncEnumerator<string> reader, Uri baseUri)
        {
            fileInfo.PlaylistType = value;
            return Task.CompletedTask;
        }
    }
}