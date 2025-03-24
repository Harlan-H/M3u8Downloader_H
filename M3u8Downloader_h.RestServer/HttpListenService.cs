using M3u8Downloader_H.RestServer.Models;
using M3u8Downloader_H.RestServer.Extensions;
using M3u8Downloader_H.RestServer.Utils;
using System.Net;
using System.Text.Json;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.M3UFileReaders;
using M3u8Downloader_H.M3U8.Extensions;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.RestServer
{
    using DownloadByUrlActionType = Action<IM3u8DownloadParam, string?>;
    using DownloadByM3uFileInfoActionType = Action<IDownloadParamBase,IM3uFileInfo, string?>;

    public class HttpListenService
    {
        private readonly HttpListen httpListen = new();
        private DownloadByUrlActionType DownloadByUrlAction = default!;        
        private DownloadByM3uFileInfoActionType DownloadByM3uFileInfoAction = default!;

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
            DownloadByUrlActionType downloadByUrl,
            DownloadByM3uFileInfoActionType downloadByM3uFileInfo)
        {
            DownloadByUrlAction = downloadByUrl;
            DownloadByM3uFileInfoAction = downloadByM3uFileInfo;
        }

        public void Run(Action<int> SetPortAction)
        {
            for (int i = 65432; i > 65400; i--)
            {
                try
                {
                    httpListen.Run($"http://+:{i}/");
                    SetPortAction(i);
                    break;
                }
                catch (HttpListenerException)
                {
                    continue;
                }
            }
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
                if (!string.IsNullOrWhiteSpace(requestWithURI.SavePath))
                    requestWithURI.SavePath = requestWithURI.SavePath.Replace('/', Path.DirectorySeparatorChar);
                DownloadByUrlAction(requestWithURI.ToM3u8DownloadParams(),requestWithURI.PluginKey);

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

                IM3uFileInfo? m3UFileInfo = new M3UFileReaderWithStream().GetM3u8FileInfo(requestWithContent.Url!, requestWithContent.Content);
                if (m3UFileInfo is null)
                {
                    response.Json(Response.Error("m3u8内容读取失败,请检查传入的参数是否有误"));
                    return;
                }

                if(m3UFileInfo!.MediaFiles is null || !m3UFileInfo.MediaFiles.Any())
                {
                    response.Json(Response.Error("m3u8的ts列表为空"));
                    return;
                }
                requestWithContent.Validate();

                RequestWithM3u8FileInfo requestWithM3U8FileInfo = new()
                {
                    M3UFileInfos = m3UFileInfo,
                    VideoName = requestWithContent.VideoName,
                    SavePath = !string.IsNullOrWhiteSpace(requestWithContent.SavePath)? requestWithContent.SavePath.Replace('/', Path.DirectorySeparatorChar) : requestWithContent.SavePath,
                    Headers = requestWithContent.Headers,
                };
                DownloadByM3uFileInfoAction(requestWithM3U8FileInfo.ToDownloadParam(), requestWithM3U8FileInfo.M3UFileInfos, requestWithContent.PluginKey);

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
                requestWithM3U8FileInfo.M3UFileInfos.PlaylistType = "VOD";
                if (!string.IsNullOrWhiteSpace(requestWithM3U8FileInfo.SavePath))
                    requestWithM3U8FileInfo.SavePath = requestWithM3U8FileInfo.SavePath.Replace('/', Path.DirectorySeparatorChar);
                DownloadByM3uFileInfoAction(requestWithM3U8FileInfo.ToDownloadParam(), requestWithM3U8FileInfo.M3UFileInfos, requestWithM3U8FileInfo.PluginKey);

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
                IM3uFileInfo m3UFileInfo = new M3UFileReaderWithStream().GetM3u8FileInfo(requestWIthGetM3U8FileInfo.Url!, requestWIthGetM3U8FileInfo.Content);
                Response<IM3uFileInfo> r = m3UFileInfo.MediaFiles != null && m3UFileInfo.MediaFiles.Any()
                                        ? new Response<IM3uFileInfo>(0, "解析成功", m3UFileInfo)
                                        : new Response<IM3uFileInfo>(1, "没有包含任何数据", null);
                response.Json(r);
            }
            catch (Exception ex)
            {
                response.Json(Response.Error($"解析失败,{ex.Message}"));
            }
        }
    }
}