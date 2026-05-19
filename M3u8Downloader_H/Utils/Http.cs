using M3u8Downloader_H.Abstractions.Models;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Net.Http;
using M3u8Downloader_H.Services;

namespace M3u8Downloader_H.Utils
{
    internal partial class Http : IHttpFactory
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

        public void UpdateProxy(ProxyService proxy)
        {
            if(string.IsNullOrWhiteSpace(proxy.Address))
                _webProxy = null;
            else
            {
                var webproxy = new WebProxy(proxy.Address);
                if (!string.IsNullOrWhiteSpace(proxy.UserName))
                    webproxy.Credentials = new NetworkCredential(proxy.UserName, proxy.PassWord);

                _webProxy = webproxy;
            } 
            foreach (var clientEntry in _clients.Values)
                clientEntry.Client = null;

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


        public void CloseClient(string name = "m3u8downloader_h")
        {
            if (_clients.TryGetValue(name, out ClientEntry? clientEntry))
            {
                clientEntry?.Client?.Dispose();
                clientEntry?.Client = null;
            }
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
