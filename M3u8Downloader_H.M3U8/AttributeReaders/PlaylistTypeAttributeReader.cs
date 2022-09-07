using System;
using M3u8Downloader_H.M3U8.Attributes;
using M3u8Downloader_H.M3U8.Core;
using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-PLAYLIST-TYPE", typeof(PlaylistTypeAttributeReader))]
    internal class PlaylistTypeAttributeReader : AttributeReader
    {
        protected override void Write(M3UFileInfo fileInfo, string value, LineReader reader, Uri baseUri)
        {
            fileInfo.PlaylistType = value;
        }
    }
}