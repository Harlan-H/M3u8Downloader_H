using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace M3u8Downloader_H.Abstractions.Plugins
{
    public interface IAttributeReaderCollection:
        IDictionary<string, IAttributeReader>,
        ICollection<KeyValuePair<string, IAttributeReader>>,
        IEnumerable<KeyValuePair<string, IAttributeReader>>,
        IEnumerable
    {
        
    }
}
