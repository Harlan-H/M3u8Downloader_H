using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Downloader.Extensions
{
    internal static class EnumerableExtension
    {
        public static int IndexOf<T>(this IEnumerable<T> source,Func<T,bool> predicate)
        {
            var i = 0;
            foreach (var element in source)
            {
                if (predicate(element))
                    return i;

                i++;
            }

            return -1;
        }

        /// <summary>
        /// 跳过表达式为真的元素（不包含自身）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">元素集合</param>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        public static IEnumerable<T> Skip<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            int pos = source.IndexOf(predicate);
            if (pos == -1)
                return source;

            return source.Skip(pos + 1);
        }
    }
}
