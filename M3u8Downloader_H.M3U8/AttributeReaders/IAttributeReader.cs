using M3u8Downloader_H.M3U8.Core;
using System.Collections.Generic;
using M3u8Downloader_H.M3U8.Infos;
using System;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    public interface IAttributeReader
    {
        bool ShouldTerminate { get; }
        void Write(M3UFileInfo m3UFileInfo, string value, IEnumerator<string> reader, Uri baseUri);
    }
}