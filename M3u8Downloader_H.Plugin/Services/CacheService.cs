using M3u8Downloader_H.Abstractions.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Plugin.Services
{
    public class CacheService(IMemoryCache memoryCache, string PluginKey) : ICacheService
    {
        public T? GetOrCreate<T>(object key, Func<ICacheEntry, T> factory)
            => memoryCache.GetOrCreate<T>($"{PluginKey}-{key}", factory);

        public Task<T?> GetOrCreateAsync<T>(object key, Func<ICacheEntry, Task<T>> factory)
            => memoryCache.GetOrCreateAsync($"{PluginKey}-{key}", factory);
    }
}
