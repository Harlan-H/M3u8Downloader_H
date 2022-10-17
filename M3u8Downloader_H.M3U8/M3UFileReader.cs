using M3u8Downloader_H.M3U8.Adapters;
using M3u8Downloader_H.M3U8.AttributeReaders;
using M3u8Downloader_H.M3U8.Core;
using M3u8Downloader_H.M3U8.Utilities;
using M3u8Downloader_H.M3U8.Infos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using M3u8Downloader_H.M3U8.Attributes;
using System.Linq;

namespace M3u8Downloader_H.M3U8
{
    public class M3UFileReader
    {
        private readonly IReadOnlyDictionary<string, AttributeReader> attributeReaders;

        public M3UFileReader()
        {
            attributeReaders = InitAttributeReaders();
        }

        private static IReadOnlyDictionary<string, AttributeReader> InitAttributeReaders()
        {
            Assembly asm = typeof(M3U8ReaderAttribute).Assembly;
            return asm.GetTypes()
                .Where(t => t.IsDefined(typeof(M3U8ReaderAttribute), false))
                .Select(t => (M3U8ReaderAttribute)t.GetCustomAttribute(typeof(M3U8ReaderAttribute), false)!)
                .ToDictionary(x => x.Key!, x => (AttributeReader)Activator.CreateInstance(x.Type)!);

            //             return (from t in asm.GetTypes()
            //                     where t.IsDefined(typeof(M3U8ReaderAttribute), false)
            //                     let attribute = (M3U8ReaderAttribute)t.GetCustomAttribute(typeof(M3U8ReaderAttribute), false)
            //                     select (attribute.Key, attribute.Type))
            //                    .ToDictionary(x => x.Key, x => (AttributeReader)Activator.CreateInstance(x.Type));
        }


        public M3UFileInfo Read(Uri baseUri, Stream stream) => Read(baseUri,new StreamAdapter(stream));
        public M3UFileInfo Read(Uri baseUri, string text) => Read(baseUri,new TextAdapter(text));
        public M3UFileInfo Read(Uri baseUri, FileInfo file) => Read(baseUri,new FileAdapter(file));

        private M3UFileInfo Read(Uri baseUri, IAdapter adapter)
        {
            M3UFileInfo m3UFileInfo = new();
            using var reader = new LineReader(adapter);
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

                if (!attributeReaders.TryGetValue(CompareKey ?? text, out AttributeReader? attributeReader))
                    throw new InvalidDataException($"{text} 无法识别的标签,可能是非标准的标签，你可以删除此行，然后拖拽m3u8文件到请求地址，再次尝试下载");

                if (attributeReader.Read(m3UFileInfo, reader, keyValuePair, baseUri))
                    break;
            }

            adapter?.Dispose();
            return m3UFileInfo;
        }
    }
}