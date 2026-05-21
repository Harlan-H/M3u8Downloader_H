using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Abstractions.Models
{
    public interface ICacheService
    {
        Task<T?> GetOrCreateAsync<T>(object key, Func<ICacheEntry, Task<T>> factory);

        T? GetOrCreate<T>(object key, Func<ICacheEntry, T> factory);
    }
}
