using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace M3u8Downloader_H.Utils
{
    internal class Http
    {
        private static HttpClientHandler GetHandler()
        {
            HttpClientHandler handler = new()
            {
                UseCookies = false,
            };

            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return handler;
        }

        public static HttpClient Client { get; }

        static Http()
        {
            Client = new(GetHandler());
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36");
        }
    }
}
