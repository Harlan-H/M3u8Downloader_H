using M3u8Downloader_H.M3U8.AttributeReader.AttributeReaders;
using M3u8Downloader_H.Plugin;
using System.Collections.Generic;

namespace M3u8Downloader_H.M3U8.M3UFileReaders
{
    internal class M3UFileReaderFactory
    {
        public static M3UFileReaderWithStream CreateM3UFileReader(IM3uFileReader? m3UFileReader , IDictionary<string, IAttributeReader>? attributeReaders)
        {
            if (m3UFileReader is not null)
                return new M3UFileReaderWithPlugin(m3UFileReader);
            else
                return new M3UFileReaderWithStream(attributeReaders);
        }
    }
}
