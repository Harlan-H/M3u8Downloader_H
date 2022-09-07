using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.M3U8.Infos;
using System.Linq;

namespace M3u8Downloader_H.Services
{
    public class HttpListenService
    {
        private readonly HttpListen httpListen = new();
        private readonly DownloadService downloadService;
        private Action<Uri, string?, string?, string?, string?, string?, IEnumerable<KeyValuePair<string, string>>?> DownloadByUrlAction = default!;
        private Action<string, Uri?, string?, string?, IEnumerable<KeyValuePair<string, string>>?> DownloadByContentAction = default!;
        private Action<M3UFileInfo, string?, string?, IEnumerable<KeyValuePair<string, string>>?> DownloadByM3uFileInfoAction = default!;
        private readonly string[] methods = { "AES-128", "AES-192", "AES-256" };

        public HttpListenService(DownloadService downloadService)
        {
            this.downloadService = downloadService;
            httpListen.RegisterService("downloadbyurl", DownloadByUrl);
            httpListen.RegisterService("downloadbycontent", DownloadByContent);
            httpListen.RegisterService("downloadbyjsoncontent", DownloadByJsonContent);
            httpListen.RegisterService("getm3u8data", GetM3u8FileInfo);
        }

        public void Initialization(
            Action<Uri, string?,string?,string?,string?, string?, IEnumerable<KeyValuePair<string, string>>?> downloadByUrl,
            Action<string, Uri?, string?, string?, IEnumerable<KeyValuePair<string, string>>?> downloadByContent,
            Action<M3UFileInfo, string?, string?, IEnumerable<KeyValuePair<string, string>>?> downloadByM3uFileInfo)
        {
            DownloadByUrlAction = downloadByUrl;
            DownloadByContentAction = downloadByContent;
            DownloadByM3uFileInfoAction = downloadByM3uFileInfo;
        }

        public void Run(string port)
        {
            httpListen.Run(port);
        }

        private void DownloadByUrl(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string text = request.ReadText();
                JObject jObj = JObject.Parse(text);
                string? url = (string?)jObj.SelectToken("url");
                if (string.IsNullOrEmpty(url))
                {
                    response.Json(new Response(1, "url不能为空"));
                    return;
                }

                string? videoName = (string?)jObj.SelectToken("name");
                string method = ((string?)jObj.SelectToken("method"))?.ToUpper() ?? "AES-128";
                if (method is not null && !methods.Contains(method))
                {
                    response.Json(new Response(1, "不可用的key方法，必须是AES-128,AES-192,AES-256其中之一"));
                    return;
                }

                string? key = (string?)jObj.SelectToken("key");
                string? iv = (string?)jObj.SelectToken("iv");
                string? savePath = (string?)jObj.SelectToken("savepath");
                Dictionary<string, string>? headers = jObj.SelectToken("headers")?.ToObject<Dictionary<string, string>>();

                Uri uri = new(url!, UriKind.Absolute);
                DownloadByUrlAction(uri, videoName,method,key, iv, savePath, headers);

                response.Json(Response.Success());
            }
            catch (Exception e)
            {
                response.Json(Response.Error($"请求失败,{e.Message}"));
            }
        }

        private void DownloadByContent(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string text = request.ReadText();
                JObject jObj = JObject.Parse(text);
                string? content = (string?)jObj.SelectToken("content");
                if (content == null)
                {
                    response.Json(Response.Error("content不能为空"));
                    return;
                }

                string? url = (string?)jObj.SelectToken("baseurl");
                Uri uri = default!;
                if (url != null)
                {
                    url = Path.EndsInDirectorySeparator(url) ? url : url + Path.DirectorySeparatorChar;
                    uri = new Uri(url, UriKind.Absolute);
                }

                string? videoname = (string?)jObj.SelectToken("name");
                string? savePath = (string?)jObj.SelectToken("savepath");
                Dictionary<string, string>? headers = jObj.SelectToken("headers")?.ToObject<Dictionary<string, string>>();

                DownloadByContentAction(content, uri, videoname, savePath, headers);

                response.Json(Response.Success());
            }
            catch(Exception e)
            {
                response.Json(Response.Error($"请求失败,{e.Message}"));
            }

        }

        //视频地址 必须是http开头 或者磁盘根路径
        private void DownloadByJsonContent(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string text = request.ReadText();
                JObject jObj = JObject.Parse(text);
                M3UFileInfo? m3UFileInfo = jObj.SelectToken("content")?.ToObject<M3UFileInfo>();
                if (m3UFileInfo == null)
                {
                    response.Json(Response.Error("m3UFileInfo解析失败"));
                    return;
                }

                string? videoName = (string?)jObj.SelectToken("name");
                string? savePath = (string?)jObj.SelectToken("savepath");
                Dictionary<string, string>? headers = jObj.SelectToken("headers")?.ToObject<Dictionary<string, string>>();

                DownloadByM3uFileInfoAction(m3UFileInfo, videoName, savePath, headers);

                response.Json(Response.Success());
            }
            catch (Exception e)
            {
                response.Json(Response.Error($"请求失败,{e.Message}"));
            }
        }


        private void GetM3u8FileInfo(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string text = request.ReadText();
                JObject jObj = JObject.Parse(text);
                string? content = (string?)jObj.SelectToken("content");
                if (content == null)
                {
                    response.Json(Response.Error("content不能为空"));
                    return;
                }

                string? url = (string?)jObj.SelectToken("baseurl");
                Uri uri = default!;
                if (url != null)
                {
                    url = Path.EndsInDirectorySeparator(url) ? url : url + Path.DirectorySeparatorChar;
                    uri = new Uri(url, UriKind.Absolute);
                }

                M3UFileInfo m3UFileInfo = downloadService.GetM3U8FileInfo(content, uri!);
                var r = new Response<M3UFileInfo>(0, "解析成功", m3UFileInfo);
                response.Json(r);
            }
            catch (Exception ex)
            {
                response.Json(Response.Error($"解析失败,{ex.Message}"));
            }
        }
    }
}
