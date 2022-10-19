using M3u8Downloader_H.M3U8.AttributeReaders;
using System.Collections.Generic;

namespace M3u8Downloader_H.M3U8.Services
{
    public interface IAttributeReaderRoot
    {
        IDictionary<string,IAttributeReader> AttributeReaders { get; }
    }
}
