using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Abstractions.M3u8;
using System.Collections.Generic;
using System;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    internal abstract class AttributeReader : IAttributeReader
    {
        public virtual bool ShouldTerminate => false;

        public abstract void Write(M3UFileInfo m3UFileInfo, string value, IEnumerator<string> reader, Uri baseUri);

        public bool Read(IM3uFileInfo m3UFileInfo, IEnumerator<string> reader, KeyValuePair<string, string> keyValuePair, Uri baseUri)
        {
            Write(m3UFileInfo, keyValuePair.Value, reader, baseUri);

            return ShouldTerminate;
        }

        public void Write(IM3uFileInfo m3UFileInfo, string value, IEnumerator<string> reader, Uri baseUri)
        {
            Write((M3UFileInfo)m3UFileInfo, value, reader, baseUri);
        }
    }
}