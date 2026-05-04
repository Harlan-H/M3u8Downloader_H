using M3u8Downloader_H.Abstractions.Models;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Net.Http;

namespace M3u8Downloader_H.Utils
{
    internal partial class Http : IApiFactory
    {
        private class ClientEntry
        {
            public Action<HttpClient, HttpClientHandler>? Configure;
            public HttpClient? Client;
        }

        private readonly ConcurrentDictionary<string, ClientEntry> _clients = new();
        private WebProxy? _webProxy = null;

        public event Action? ProxyChanged;

        private HttpClientHandler CreateHandler()
        {
            var handler = new HttpClientHandler
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate

            };

            if (_webProxy is not null)
            {
                handler.Proxy = _webProxy;
                handler.UseProxy = true;
            }

            return handler;
        }

        public void UpdateProxy(string? address)
        {
            _webProxy = string.IsNullOrEmpty(address) ? null:  new WebProxy(address);
            foreach (var (key, clientEntry) in _clients)
            {
                if (key == "m3u8downloader_h")
                {
                    clientEntry.Client = null;
                    continue;
                }
                clientEntry.Client?.Dispose();
            }
            
            ProxyChanged?.Invoke();
        }

        public HttpClient GetClient(string name = "m3u8downloader_h")
        {
            var entry = _clients.GetOrAdd(name, _ => new ClientEntry());

            if (entry.Client != null)
                return entry.Client;

            var handler = CreateHandler();
            var client = new HttpClient(handler)
            {
                Timeout = Timeout.InfiniteTimeSpan,
            };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36");

            entry.Configure?.Invoke(client, handler);

            entry.Client = client;

            return client;
        }

        public void Configure(string name, Action<HttpClient, HttpClientHandler> configure)
        {
            var entry = _clients.GetOrAdd(name, _ => new ClientEntry());

            entry.Configure = configure;
            entry.Client = null;
        }

        public void Remove(string name)
        {
            if(_clients.TryRemove(name,out ClientEntry? clientEntry))
                clientEntry.Client?.Dispose();
        }

        private Http()
        {
        }
    }

    internal partial class Http
    {
        private static readonly Lazy<Http> httpLazy = new(() => new Http());
        public static Http Instance => httpLazy.Value;
    }

}
