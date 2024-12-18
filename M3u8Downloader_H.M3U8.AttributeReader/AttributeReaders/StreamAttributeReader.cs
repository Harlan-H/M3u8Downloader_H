using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.M3U8.AttributeReader.AttributeReaders
{
    [M3U8Reader("#EXT-X-STREAM-INF", typeof(StreamAttributeReader))]
    internal class StreamAttributeReader : IAttributeReader
    {
        public bool ShouldTerminate => false;

        public void Write(M3UFileInfo fileInfo, string value, IEnumerator<string> reader, Uri baseUri)
        {
            var source = value.Split([','], StringSplitOptions.RemoveEmptyEntries)
                              .Select(e => KV.Parse(e, '='))
                              .ToList();

            fileInfo.Streams ??= [];

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