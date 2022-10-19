using M3u8Downloader_H.M3U8.Core;
using System.Collections.Generic;
using M3u8Downloader_H.M3U8.Infos;
using System;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    internal abstract class AttributeReader : IAttributeReader
    {
        public bool ShouldTerminate => false;

       // protected virtual bool ShouldTerminate() => false;

        public abstract void Write(M3UFileInfo m3UFileInfo, string value, IEnumerator<string> reader,Uri baseUri);

        public bool Read(M3UFileInfo m3UFileInfo, IEnumerator<string> reader, KeyValuePair<string, string> keyValuePair, Uri baseUri)
        {
            Write(m3UFileInfo, keyValuePair.Value, reader, baseUri);

            return ShouldTerminate;
        }
    }
}