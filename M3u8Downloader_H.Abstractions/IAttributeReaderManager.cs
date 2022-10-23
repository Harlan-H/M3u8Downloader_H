using M3u8Downloader_H.M3U8.AttributeReaders;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.AttributeReaderManager
{
    public interface IAttributeReaderManager
    {
        IAttributeReader this[string key] { get; set; }
        void Add(string key, IAttributeReader value);
        bool TryGetValue(string key, [MaybeNullWhen(false)] out IAttributeReader value);
    }
}
