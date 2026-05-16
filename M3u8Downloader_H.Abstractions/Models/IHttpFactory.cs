using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Abstractions.Models
{
    public interface IHttpFactory
    {
        event Action? ProxyChanged;
        HttpClient GetClient(string name);

        void Configure(string name, Action<HttpClient, HttpClientHandler> configure);

        void Remove(string name);
    }
}
