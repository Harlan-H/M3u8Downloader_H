using M3u8Downloader_H.M3U8.AttributeReaders;
using M3u8Downloader_H.M3U8.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace M3u8Downloader_H.M3U8.Utilities
{
    internal class AttributeReaderRoot
    {
        private static readonly Lazy<AttributeReaderRoot> attributeReaderRootLazy = new(() => new AttributeReaderRoot());
        public static AttributeReaderRoot Instance => attributeReaderRootLazy.Value;

        private readonly IReadOnlyDictionary<string, IAttributeReader> attributeReaders;

        public IDictionary<string, IAttributeReader> AttributeReaders
        {
            get => new Dictionary<string, IAttributeReader>(attributeReaders);
        }

        public AttributeReaderRoot()
        {
            attributeReaders = InitAttributeReaders();
        }

        private static IReadOnlyDictionary<string, IAttributeReader> InitAttributeReaders()
        {
            Assembly asm = typeof(M3U8ReaderAttribute).Assembly;
            return asm.GetTypes()
                .Where(t => t.IsDefined(typeof(M3U8ReaderAttribute), false))
                .Select(t => (M3U8ReaderAttribute)t.GetCustomAttribute(typeof(M3U8ReaderAttribute), false)!)
                .ToDictionary(x => x.Key!, x => (IAttributeReader)Activator.CreateInstance(x.Type)!);
        }

    }
}
