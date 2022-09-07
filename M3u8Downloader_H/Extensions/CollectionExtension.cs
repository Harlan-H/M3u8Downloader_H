using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M3u8Downloader_H.Extensions
{
    internal static class CollectionExtension
    {
        public static void RemoveWhere<T>(this ICollection<T> source,Predicate<T> perdicate)
        {
            foreach (var item in source.ToArray())
            {
                if (perdicate(item))
                    source.Remove(item);
            }
        }
    }
}
