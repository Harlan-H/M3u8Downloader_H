using M3u8Downloader_H.Abstractions.Models;
using Refit;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;

namespace M3u8Downloader_H.Utils
{
    internal partial class Http : IApiFactory
    {
        private readonly ConcurrentDictionary<string, HttpClient> _clients = new();
        private readonly ConcurrentDictionary<(Type, string, string), object> _apicache = new();
        private WebProxy? _webProxy = null;

        public event Action? ProxyChanged;

        private HttpClient CreateClient()
        {
            HttpClientHandler handler = new()
            {
                UseCookies = false,
            };

            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (_webProxy is not null)
            {
                handler.Proxy = _webProxy;
                handler.UseProxy = true;
            }

            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36");
            return client;
        }

        public void UpdateProxy(string? address)
        {
            _webProxy = string.IsNullOrEmpty(address) ? null:  new WebProxy(address);
            Client = GetClient();
            ProxyChanged?.Invoke();
        }

        private HttpClient GetClient()
        {
            var key = _webProxy?.Address?.ToString() ?? "direct";
            return _clients.GetOrAdd(key, _ => CreateClient());
        }

        public T Create<T>(string baseUrl, RefitSettings? refitSettings) where T : class
        {
            var proxyKey = _webProxy?.Address?.ToString() ?? "direct";

            var key = (typeof(T), baseUrl, proxyKey);

            return (T)_apicache.GetOrAdd(key, _ =>
            {
                var client = CreateClient();
                client.BaseAddress = new Uri(baseUrl);
                return RestService.For<T>(client, refitSettings);
            });
        }

        public HttpClient Client { get; private set; }

        private Http()
        {
            Client = GetClient();
        }
    }

    internal partial class Http
    {
        private static readonly Lazy<Http> httpLazy = new(() => new Http());
        public static Http Instance => httpLazy.Value;
    }

}
