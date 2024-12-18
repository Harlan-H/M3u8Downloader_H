using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using M3u8Downloader_H.Plugin;
using System.Reflection;

namespace M3u8Downloader_H.M3U8.AttributeReader
{
    public partial class AttributeReaderRoot
    {
        private readonly IReadOnlyDictionary<string, Type> attributeReaders;
        public IDictionary<string, IAttributeReader> AttributeReaders
        {
            get => attributeReaders.ToDictionary(x => x.Key, x => (IAttributeReader)Activator.CreateInstance(x.Value)!);
        }

        public AttributeReaderRoot()
        {
            attributeReaders = InitAttributeReaders();
        }

        private static Dictionary<string, Type> InitAttributeReaders()
        {
            Assembly asm = typeof(M3U8ReaderAttribute).Assembly;
            return asm.GetTypes()
                .Where(t => t.IsDefined(typeof(M3U8ReaderAttribute), false))
                .Select(t => (M3U8ReaderAttribute)t.GetCustomAttribute(typeof(M3U8ReaderAttribute), false)!)
                .ToDictionary(x => x.Key!, x => x.Type!);
        }

    }

    public partial class AttributeReaderRoot
    {
        private static readonly Lazy<AttributeReaderRoot> attributeReaderRootLazy = new(() => new AttributeReaderRoot());
        public static AttributeReaderRoot Instance => attributeReaderRootLazy.Value;
    }
}
