using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace M3u8Downloader_H.Abstractions.Plugins.Download
{
    /// <summary>
    /// AttributeReader的容器，主要是为了更好的操作IAttributeReader集合
    /// </summary>
    public interface IAttributeReaderCollection:
        IDictionary<string, IAttributeReader>,
        ICollection<KeyValuePair<string, IAttributeReader>>,
        IEnumerable<KeyValuePair<string, IAttributeReader>>,
        IEnumerable
    {
        
    }
}
