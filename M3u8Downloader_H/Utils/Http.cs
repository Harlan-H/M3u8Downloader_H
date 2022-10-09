using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace M3u8Downloader_H.Utils
{
    public class Http
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
            Client = new(GetHandler())
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1");
        }
    }
}
