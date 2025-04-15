using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.M3U8.AttributeReaders;
using System.Diagnostics.CodeAnalysis;

namespace M3u8Downloader_H.Plugin.AttributeReaderManagers
{
    internal class AttributeReaderManager : IAttributeReaderManager
    {
        public IDictionary<string, IAttributeReader> AttributeReaders { get; }
        internal AttributeReaderManager()
        {
            AttributeReaders = AttributeReaderRoot.Instance.AttributeReaders;
        }

        public IAttributeReader this[string key]
        {
            get => AttributeReaders[key];
            set => AttributeReaders[key] = value;
        }

        public void Add(string key, IAttributeReader value)
        {
            AttributeReaders.Add(key, value);
        }

        public void Set(string key, IAttributeReader value)
        {
            this[key] = value;
        }

        public bool ContainsKey(string key)
        {
            return AttributeReaders.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out IAttributeReader value)
        {
            return AttributeReaders.TryGetValue(key, out value);
        }

    }
}
