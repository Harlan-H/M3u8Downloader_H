using System;
using System.IO;
using M3u8Downloader_H.M3U8.Attributes;
using M3u8Downloader_H.M3U8.Core;
using M3u8Downloader_H.M3U8.Infos;
using M3u8Downloader_H.M3U8.Utilities;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-MAP", typeof(MapAttributeReader))]
    internal class MapAttributeReader : AttributeReader
    {
        protected override void Write(M3UFileInfo m3UFileInfo, string value, LineReader reader, Uri baseUri)
        {
            var result = KV.Parse(value, '=');

            if(result.Key.ToUpper() != "URI")
                throw new InvalidDataException("Invalid M3U file. Include a invalid MAP URI.",
                        new UriFormatException(reader.Current));

            M3UMediaInfo m3UMapInfo = new();
            if (Uri.TryCreate(result.Value, UriKind.RelativeOrAbsolute, out Uri? relativeUri))
            {
                m3UMapInfo.Uri = relativeUri.IsAbsoluteUri ? relativeUri : new Uri(baseUri, relativeUri);
                m3UMapInfo.Title = "header" + Path.GetExtension(result.Value) ?? ".mp4";
            }
            m3UFileInfo.Map = m3UMapInfo;
        }
    }
}
