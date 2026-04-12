using M3u8Downloader_H.M3U8.Core;
using System;
using System.Collections.Generic;
using System.IO;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.M3U8.Utilities;
using M3u8Downloader_H.M3U8.AttributeReaders;

namespace M3u8Downloader_H.M3U8.M3UFileReaders
{
    public class M3UFileReaderWithStream(Uri baseUri) : IM3uFileReader
    {
        public IAttributeReaderCollection attributeReaders = default!;

        public void InitAttributeReade(IAttributeReaderCollection readers)
        {
            attributeReaders = readers;
        }

        public IM3uFileInfo GetM3u8FileInfo(Stream stream)
        {
            M3UFileInfo m3UFileInfo = new();
            using var reader = new LineReader(stream);
            if (!reader.MoveNext())
                throw new InvalidDataException("无效得m3u8文件");
            if (!string.Equals(reader.Current?.Trim(), "#EXTM3U", StringComparison.Ordinal))
                throw new InvalidDataException("无效得m3u8文件头部");

            while (reader.MoveNext())
            {
                var text = reader.Current?.Trim();
                if (string.IsNullOrEmpty(text))
                    continue;

                var keyValuePair = KV.Parse(text);
                var CompareKey = keyValuePair.Key;

                if (!attributeReaders.TryGetValue(CompareKey ?? text, out IAttributeReader? attributeReader))
                    attributeReaders.TryGetValue("#EXT-X-DISCONTINUITY", out attributeReader);

                if (attributeReader is null)
                    throw new InvalidDataException($"{text} 无法识别的标签,可能是非标准的标签，你可以删除此行，然后拖拽m3u8文件到请求地址，再次尝试下载");

                attributeReader.Write(m3UFileInfo, keyValuePair.Value, reader, baseUri);
                if(attributeReader.ShouldTerminate) break;
            }

            stream?.Dispose();
            return m3UFileInfo;
        }

 
    }
}