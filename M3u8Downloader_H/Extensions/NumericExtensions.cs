using System;
using System.Diagnostics.CodeAnalysis;

namespace M3u8Downloader_H.Extensions
{
    public static class NumericExtensions
    {
        public static T Range<T>([NotNull] this T value, [NotNull] T min, [NotNull] T max) where T : IComparable<T>
        {
            if (max.CompareTo(min) < 0)
                throw new ArgumentException("max必须大于或者等于min");

            if (value.CompareTo(min) <= 0)
                return min;

            if (value.CompareTo(max) >= 0)
                return max;

            return value;
        }
    }
}
