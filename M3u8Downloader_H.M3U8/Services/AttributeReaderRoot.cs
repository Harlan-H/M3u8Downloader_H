using M3u8Downloader_H.M3U8.AttributeReaders;
using M3u8Downloader_H.M3U8.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.Services
{
    public class AttributeReaderRoot : IAttributeReaderRoot
    {
        private static readonly Lazy<IAttributeReaderRoot> attributeReaderRootLazy = new(() => new AttributeReaderRoot());
        public static IAttributeReaderRoot Instance => attributeReaderRootLazy.Value;

        private readonly IReadOnlyDictionary<string, IAttributeReader> attributeReaders;
        IDictionary<string, IAttributeReader> IAttributeReaderRoot.AttributeReaders
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
