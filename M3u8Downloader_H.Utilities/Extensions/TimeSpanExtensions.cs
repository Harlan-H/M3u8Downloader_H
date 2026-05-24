using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Common.Extensions;

public static  class TimeSpanExtensions
{
    extension(TimeSpan)
    {
        public static bool operator >(TimeSpan timeSpan ,double second) 
            => timeSpan.TotalSeconds > second;

        public static bool operator <(TimeSpan timeSpan, double second)
            => timeSpan.TotalSeconds < second;

        public static bool operator >(double second, TimeSpan timeSpan)
            => second > timeSpan.TotalSeconds;

        public static bool operator <(double second, TimeSpan timeSpan)
            => second < timeSpan.TotalSeconds;
    }
}
