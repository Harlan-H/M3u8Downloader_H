using M3u8Downloader_H.RestServer.Extensions;
using System;
using System.Collections.Generic;
using System.Net;

namespace M3u8Downloader_H.RestServer.Utils
{
    public class HttpListen
    {
        private HttpListener _httpListener = default!;
        private readonly Dictionary<string, Action<HttpListenerRequest, HttpListenerResponse>> callbackPostDict = new();
        public HttpListen()
        {

        }

        public void RegisterService(string name, Action<HttpListenerRequest, HttpListenerResponse> callback)
        {
            callbackPostDict.Add(name, callback);
        }

        public void Run(string port)
        {
            _httpListener = new();
            _httpListener.Prefixes.Add(port);
            _httpListener.Start();
            _httpListener.BeginGetContext(Result, null);
        }

        private void Result(IAsyncResult ar)
        {
            var context = _httpListener.EndGetContext(ar);
            var request = context.Request;
            var response = context.Response;

            var url = request.RawUrl?.Trim('/');
            if(string.IsNullOrEmpty(url))
            {
                response.Text("不能是空方法");
                _httpListener.BeginGetContext(Result, null);
                return;
            }
            try
            {
                if (request.HttpMethod == "POST" && callbackPostDict.TryGetValue(url, out Action<HttpListenerRequest, HttpListenerResponse>? callbackPost))
                {
                    if (callbackPost == null)
                    {
                        response.Text($"没有【{url}】方法");
                    }
                    else
                    {
                        callbackPost(request, response);
                    }
                }
                else
                {
                    response.Text($"没有找到【{request.RawUrl}】方法");
                }
            }
            catch (Exception ex)
            {
                response.Text(ex.Message);
            }
            _httpListener.BeginGetContext(Result, null);
        }
    }
}
