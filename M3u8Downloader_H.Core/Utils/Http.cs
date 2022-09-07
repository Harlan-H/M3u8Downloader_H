using System;
using System.Net.Http;
using System.Net;

namespace M3u8Downloader_H.Core.Utils
{
    public class Http
    {
        private static readonly Lazy<HttpClient> httpClientLazy = new(() =>
        {
            HttpClientHandler handler = new()
            {
                UseCookies = false
            };

            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            HttpClient http = new(handler, true);
            Client.Timeout = TimeSpan.FromSeconds(10);
            http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36");
            return http;
        });

        public static HttpClient Client => httpClientLazy.Value;
    }
}
