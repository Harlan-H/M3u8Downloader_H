using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Abstractions.Models
{
    public interface IApiFactory
    {
        event Action? ProxyChanged;
        HttpClient Client { get; }
        T Create<T>(string baseUrl, RefitSettings? refitSettings) where T : class;
    }
}
