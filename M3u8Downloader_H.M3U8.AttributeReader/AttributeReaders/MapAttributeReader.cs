using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.M3U8.AttributeReader.AttributeReaders
{
    [M3U8Reader("#EXT-X-MAP", typeof(MapAttributeReader))]
    internal class MapAttributeReader : IAttributeReader
    {
        public bool ShouldTerminate => false;

        public void Write(M3UFileInfo m3UFileInfo, string value, IEnumerator<string> reader, Uri baseUri)
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
