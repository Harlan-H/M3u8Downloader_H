using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Abstractions.M3u8;
using System.Collections.Generic;
using System;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    internal abstract class AttributeReader : IAttributeReader
    {
        public virtual bool ShouldTerminate => false;

        public abstract Task WriteAsync(M3UFileInfo m3UFileInfo, string value, IAsyncEnumerator<string> reader, Uri baseUri);

        public async Task WriteAsync(IM3uFileInfo m3UFileInfo, string value, IAsyncEnumerator<string> reader, Uri baseUri)
        {
            await WriteAsync((M3UFileInfo)m3UFileInfo, value, reader, baseUri);
        }
    }
}