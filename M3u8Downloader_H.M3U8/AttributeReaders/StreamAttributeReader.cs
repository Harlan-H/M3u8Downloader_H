using M3u8Downloader_H.M3U8.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using M3u8Downloader_H.M3U8.Attributes;
using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-STREAM-INF", typeof(StreamAttributeReader))]
    internal class StreamAttributeReader : IAttributeReader
    {
        public bool ShouldTerminate => false;

        public void Write(M3UFileInfo fileInfo, string value, IEnumerator<string> reader, Uri baseUri)
        {
            var source = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(e => KV.Parse(e, '='))
                              .ToList();

            fileInfo.Streams ??= new List<M3UStreamInfo>();

            var m3UstreamInfo = new M3UStreamInfo();
            foreach (var keyValuePair in source)
            {
                switch (keyValuePair.Key)
                {
                    case "BANDWIDTH":
                        m3UstreamInfo.Bandwidth = To.Value<int>(keyValuePair.Value);
                        break;
                    case "PROGRAM-ID":
                        m3UstreamInfo.ProgramId = To.Value<int>(keyValuePair.Value);
                        break;
                    case "CODECS":
                        m3UstreamInfo.Codecs = keyValuePair.Value;
                        break;
                    case "RESOLUTION":
                        m3UstreamInfo.Resolution = keyValuePair.Value;
                        break;
                }
            }

            if (!reader.MoveNext())
                throw new InvalidDataException("Invalid M3U file. Missing a stream URI.");

            if (reader.Current != null)
            {
                var relativeUri = new Uri(reader.Current.Trim(), UriKind.RelativeOrAbsolute);
                if (!relativeUri.IsAbsoluteUri && !relativeUri.IsWellFormedOriginalString())
                {
                    if ((reader.Current?.StartsWith("#EXT", StringComparison.CurrentCulture)) == true)
                    {
                        return;
                    }
                    else
                    {
                        throw new InvalidDataException("Invalid M3U file. Include a invalid stream URI.", new UriFormatException(reader.Current));
                    }
                }

                m3UstreamInfo.Uri = !relativeUri.IsAbsoluteUri ? new Uri(baseUri, relativeUri) : relativeUri;
            }

            fileInfo.Streams?.Add(m3UstreamInfo);
        }
    }
}