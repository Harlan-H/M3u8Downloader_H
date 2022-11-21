using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using M3u8Downloader_H.M3U8.AttributeReader.Utils;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.M3U8.AttributeReader.AttributeReaders
{
    [M3U8Reader("#EXT-X-MEDIA-SEQUENCE", typeof(SequenceAttributeReader))]
    internal class SequenceAttributeReader : IAttributeReader
    {
        public bool ShouldTerminate => false;

        public void Write(M3UFileInfo fileInfo,string value, IEnumerator<string> reader, Uri baseUri)
        {
            fileInfo.MediaSequence = To.Value<int>(value);
        }
    }
}