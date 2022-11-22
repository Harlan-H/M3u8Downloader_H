using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.M3U8.AttributeReader.AttributeReaders
{
    [M3U8Reader("#EXT-X-ENDLIST", typeof(EndListAttributeReader))]
    internal class EndListAttributeReader : IAttributeReader
    {
        public bool ShouldTerminate => true;

        public void Write(M3UFileInfo m3UFileInfo,string value, IEnumerator<string> reader, Uri baseUri)
        {
            m3UFileInfo.PlaylistType = "VOD";
        }
    }
}