using M3u8Downloader_H.Abstractions.Plugins.Download;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace M3u8Downloader_H.M3U8.Collection
{
    internal class AttributeReaderCollection(IDictionary<string, IAttributeReader> readers) : IAttributeReaderCollection
    {
        public AttributeReaderCollection() 
            :this (new Dictionary<string,IAttributeReader>())
        {
            
        }

        public IAttributeReader this[string key] 
        {
            get => readers[key];
            set => readers[key] = value;
        }

        public ICollection<string> Keys => readers.Keys;

        public ICollection<IAttributeReader> Values => readers.Values;

        public int Count => readers.Count;

        public bool IsReadOnly => readers.IsReadOnly;

        public void Add(string key, IAttributeReader value) 
            => readers.Add(key, value);
    
        public void Add(KeyValuePair<string, IAttributeReader> item)
            => readers[item.Key] = item.Value;

        public void Clear()
            => readers.Clear();

        public bool Contains(KeyValuePair<string, IAttributeReader> item)
            => readers.Contains(item);
 
        public bool ContainsKey(string key)
            => readers.ContainsKey(key);
 

        public void CopyTo(KeyValuePair<string, IAttributeReader>[] array, int arrayIndex)
            => readers.CopyTo(array, arrayIndex);


        public IEnumerator<KeyValuePair<string, IAttributeReader>> GetEnumerator()
            => readers.GetEnumerator();

        public bool Remove(string key)
            => readers.Remove(key);

        public bool Remove(KeyValuePair<string, IAttributeReader> item)
            => readers.Remove(item);

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out IAttributeReader value)
            => readers.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
