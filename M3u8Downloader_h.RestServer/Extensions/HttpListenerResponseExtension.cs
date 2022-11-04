using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.RestServer.Extensions
{
    internal static class HttpListenerResponseExtension
    {
        internal static void Json(this HttpListenerResponse response, byte[] message)
        {
            response.StatusCode = 200;
            response.ContentType = "application/json;charset=UTF-8";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = message.Length;

            using var stream = response.OutputStream;
            stream.Write(message, 0, message.Length);
            response.Close();
        }

        internal static void Text(this HttpListenerResponse response, string message)
        {
            response.StatusCode = 200;
            response.ContentType = "text/plain;charset=utf-8";
            response.ContentEncoding = Encoding.UTF8;
            var retText = Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = retText.Length;

            using var stream = response.OutputStream;
            stream.Write(retText, 0, retText.Length);
            response.Close();
        }
    }
}
