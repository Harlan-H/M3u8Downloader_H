using System;
using System.Collections.Generic;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using M3u8Downloader_H.M3U8.Utilities;
using System.IO;
using System.Linq;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-MAP", typeof(MapAttributeReader))]
    internal class MapAttributeReader : AttributeReader
    {

        public override void Write(M3UFileInfo m3UFileInfo, string value, IEnumerator<string> reader, Uri baseUri)
        {
            var result =
                value.Split([','], StringSplitOptions.RemoveEmptyEntries)
                   .Select(e => KV.Parse(e, '='))
                    .ToList();

            M3UMediaInfo m3UMapInfo = new();
            foreach (var keyValuePair in result)
            {
                if (keyValuePair.Key == "URI")
                {
                    if (string.IsNullOrWhiteSpace(keyValuePair.Value))
                        throw new InvalidDataException("Invalid M3U file. Include a invalid MAP URI.",
                                new UriFormatException(reader.Current));

                    if (Uri.TryCreate(keyValuePair.Value, UriKind.RelativeOrAbsolute, out Uri? relativeUri))
                    {
                        m3UMapInfo.Uri = relativeUri.IsAbsoluteUri ? relativeUri : new Uri(baseUri, relativeUri);
                        m3UMapInfo.Title = "header" + Path.GetExtension(keyValuePair.Value) ?? ".mp4";
                    }
                }
                else if (keyValuePair.Key == "BYTERANGE")
                {
                    m3UMapInfo.RangeValue = HandleRangeValue(keyValuePair.Value);
                }

            }
            m3UFileInfo.Map = m3UMapInfo;
        }

        private static System.Net.Http.Headers.RangeHeaderValue HandleRangeValue(string CurrentValue)
        {
            var ByteRangeContent = CurrentValue.Split('@', 2, StringSplitOptions.RemoveEmptyEntries);
            if (ByteRangeContent.Length < 2)
                throw new InvalidCastException("无效的BYTERANGE字段");

            long? from = To.Value<long>(ByteRangeContent[1]);
            long to = To.Value<long>(ByteRangeContent[0]) ?? 0;
            return new System.Net.Http.Headers.RangeHeaderValue(from, from + to - 1);
        }
    }
}
