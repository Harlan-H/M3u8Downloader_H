using M3u8Downloader_H.RestServer.Models;
using M3u8Downloader_H.RestServer.Extensions;
using M3u8Downloader_H.RestServer.Utils;
using System.Net;
using System.Text.Json;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.M3UFileReaders;
using M3u8Downloader_H.M3U8.Extensions;
using System;

namespace M3u8Downloader_H.RestServer
{
    public class HttpListenService
    {
        private readonly HttpListen httpListen = new();
        private Action<Uri, string?, string?, string?, string?, string?, string?, IEnumerable<KeyValuePair<string, string>>?> DownloadByUrlAction = default!;
        private Action<string, Uri?, string?, string?, string?, IEnumerable<KeyValuePair<string, string>>?> DownloadByContentAction = default!;
        private Action<M3UFileInfo, string?, string?, string?, IEnumerable<KeyValuePair<string, string>>?> DownloadByM3uFileInfoAction = default!;

        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly static HttpListenService instance = new();
        public static HttpListenService Instance => instance;

        private HttpListenService()
        {
            jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            httpListen.RegisterService("downloadbyurl", DownloadByUrl);
            httpListen.RegisterService("downloadbycontent", DownloadByContent);
            httpListen.RegisterService("downloadbyjsoncontent", DownloadByJsonContent);
            httpListen.RegisterService("getm3u8data", GetM3u8FileInfo);
        }

        public void Initialization(
            Action<Uri, string?, string?, string?, string?, string?,string?,IEnumerable<KeyValuePair<string, string>>?> downloadByUrl,
            Action<string, Uri?, string?, string?, string?,IEnumerable<KeyValuePair<string, string>>?> downloadByContent,
            Action<M3UFileInfo, string?, string?, string?, IEnumerable<KeyValuePair<string, string>>?> downloadByM3uFileInfo)
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
                RequestWithURI? requestWithURI = JsonSerializer.Deserialize<RequestWithURI>(request.InputStream, jsonSerializerOptions);
                if(requestWithURI is null)
                {
                    response.Json(Response.Error("序列化失败"));
                    return;
                }

                requestWithURI.Validate();
                DownloadByUrlAction(requestWithURI.Url, requestWithURI.VideoName, requestWithURI.Method, requestWithURI.Key, requestWithURI.Iv, requestWithURI.SavePath, requestWithURI.PluginKey, requestWithURI.Headers);

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
                RequestWithContent? requestWithContent = JsonSerializer.Deserialize<RequestWithContent>(request.InputStream, jsonSerializerOptions);
                if (requestWithContent is null)
                {
                    response.Json(Response.Error("序列化失败"));
                    return;
                }

                requestWithContent.Validate();
                DownloadByContentAction(requestWithContent.Content, requestWithContent.Url, requestWithContent.VideoName, requestWithContent.SavePath, requestWithContent.PluginKey, requestWithContent.Headers);

                response.Json(Response.Success());
            }
            catch (Exception e)
            {
                response.Json(Response.Error($"请求失败,{e.Message}"));
            }

        }

        //视频地址 必须是http开头 或者磁盘根路径
        private void DownloadByJsonContent(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                RequestWithM3u8FileInfo? requestWithM3U8FileInfo = JsonSerializer.Deserialize<RequestWithM3u8FileInfo>(request.InputStream, jsonSerializerOptions);
                if (requestWithM3U8FileInfo is null)
                {
                    response.Json(Response.Error("序列化失败"));
                    return;
                }
               
                requestWithM3U8FileInfo.Validate();
                DownloadByM3uFileInfoAction(requestWithM3U8FileInfo.M3u8FileInfo, requestWithM3U8FileInfo.VideoName, requestWithM3U8FileInfo.SavePath, requestWithM3U8FileInfo.PluginKey, requestWithM3U8FileInfo.Headers);

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
                RequestWithGetM3u8FileInfo? requestWIthGetM3U8FileInfo = JsonSerializer.Deserialize<RequestWithGetM3u8FileInfo>(request.InputStream, jsonSerializerOptions);
                if (requestWIthGetM3U8FileInfo is null)
                {
                    response.Json(Response.Error("序列化失败"));
                    return;
                }

                requestWIthGetM3U8FileInfo.Validate();
                M3UFileInfo m3UFileInfo = new M3UFileReaderWithStream().GetM3u8FileInfo(requestWIthGetM3U8FileInfo.Url!, requestWIthGetM3U8FileInfo.Content);
                Response<M3UFileInfo> r = m3UFileInfo.MediaFiles != null && m3UFileInfo.MediaFiles.Any()
                                        ? new Response<M3UFileInfo>(0, "解析成功", m3UFileInfo)
                                        : new Response<M3UFileInfo>(1, "没有包含任何数据", null);
                response.Json(r);
            }
            catch (Exception ex)
            {
                response.Json(Response.Error($"解析失败,{ex.Message}"));
            }
        }
    }
}