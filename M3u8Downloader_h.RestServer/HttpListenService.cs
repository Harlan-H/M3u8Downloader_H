using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.M3U8;
using M3u8Downloader_H.M3U8.Extensions;
using M3u8Downloader_H.RestServer.Extensions;
using M3u8Downloader_H.RestServer.Models;
using M3u8Downloader_H.RestServer.Utils;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace M3u8Downloader_H.RestServer
{
    public class HttpListenService
    {
        private readonly char _DirectorySeparatorChar = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? '/' : '\\';
        private readonly HttpListen httpListen = new();
        private IAppCommandService AppCommandService = default!;

       // private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly static HttpListenService instance = new();
        
        public static HttpListenService Instance => instance;
        private HttpListenService()
        {
            //jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            httpListen.RegisterService("CatCatch", DownloadByCatCatch);
            httpListen.RegisterService("downloadmedias", DownloadMedias);
            httpListen.RegisterService("downloadbyurl", DownloadByUrl);
            httpListen.RegisterService("downloadbycontent", DownloadByContent);
            httpListen.RegisterService("downloadbyjsoncontent", DownloadByJsonContent);
            httpListen.RegisterService("getm3u8data", GetM3u8FileInfo);
        }

        public void Initialization(IAppCommandService appCommandService)
        {
            AppCommandService = appCommandService;
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


        private async void DownloadByCatCatch(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                RequestWithCatch? requestWithCatch = JsonSerializer.Deserialize(request.InputStream, RequestWithCatchContext.Default.RequestWithCatch);
                if (requestWithCatch is null)
                {
                    response.Json(Response.Error("序列化失败"));
                    return;
                }

                if(!requestWithCatch.Action.Equals("catch"))
                    return;

                string ext;
                if(requestWithCatch.Data.Count == 1)
                    ext = requestWithCatch.Data.Single().Ext;
                else if (requestWithCatch.Data.Count > 1)
                    ext = requestWithCatch.Data[0].Ext;
                else
                {
                    response.Json(Response.Error("data不能为空"));
                    return;
                }


                if(ext.Equals("m3u8"))
                {
                    var downloadParam = RequestWithCatch.ToM3u8DownloadParams(requestWithCatch.Data[0]);
                    AppCommandService.DownloadByUrl(null, downloadParam, null);
                }
                else
                {
                    var downloadParam = RequestWithCatch.ToMediaDownloadParams(requestWithCatch.Data);
                    AppCommandService.DownloadMedia(null, downloadParam);
                }

                response.Json(Response.Success());
            }
            catch (Exception e)
            {
                response.Json(Response.Error($"请求失败,{e.Message}"));
            }
        }


        private void DownloadMedias(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                RequestWithMediaUri? requestWithMediaUri = JsonSerializer.Deserialize(request.InputStream, RequestWithMediaUriContext.Default.RequestWithMediaUri);
                if (requestWithMediaUri is null)
                {
                    response.Json(Response.Error("序列化失败"));
                    return;
                }

                requestWithMediaUri.Validate();
                if (!string.IsNullOrWhiteSpace(requestWithMediaUri.SavePath))
                    requestWithMediaUri.SavePath = requestWithMediaUri.SavePath.Replace(_DirectorySeparatorChar, Path.DirectorySeparatorChar);
                AppCommandService.DownloadMedia(null, requestWithMediaUri.ToMediaDownloadParams());

                response.Json(Response.Success());
            }
            catch (Exception e)
            {
                response.Json(Response.Error($"请求失败,{e.Message}"));
            }
        }

        private void DownloadByUrl(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                RequestWithURI? requestWithURI = JsonSerializer.Deserialize(request.InputStream, RequestWithURIContext.Default.RequestWithURI);
                if(requestWithURI is null)
                {
                    response.Json(Response.Error("序列化失败"));
                    return;
                }

                requestWithURI.Validate();
                if (!string.IsNullOrWhiteSpace(requestWithURI.SavePath))
                    requestWithURI.SavePath = requestWithURI.SavePath.Replace(_DirectorySeparatorChar, Path.DirectorySeparatorChar);
                AppCommandService.DownloadByUrl(null,requestWithURI.ToM3u8DownloadParams(),null);

                response.Json(Response.Success());
            }
            catch (Exception e)
            {
                response.Json(Response.Error($"请求失败,{e.Message}"));
            }
        }

        private async void DownloadByContent(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                RequestWithContent? requestWithContent = JsonSerializer.Deserialize(request.InputStream, RequestWithContentContext.Default.RequestWithContent);
                if (requestWithContent is null)
                {
                    response.Json(Response.Error("序列化失败"));
                    return;
                }


                IM3uFileInfo? m3UFileInfo = await M3u8FileInfoClient.CreateM3uFileReader().GetM3u8FileInfo(request.Url!,requestWithContent.Content);
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
                    SavePath = !string.IsNullOrWhiteSpace(requestWithContent.SavePath)? requestWithContent.SavePath.Replace(_DirectorySeparatorChar, Path.DirectorySeparatorChar) : requestWithContent.SavePath,
                    Headers = requestWithContent.Headers,
                };
                AppCommandService.DownloadByM3uFileInfo(null, requestWithM3U8FileInfo.ToDownloadParam(), requestWithM3U8FileInfo.M3UFileInfos, null);

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
                RequestWithM3u8FileInfo? requestWithM3U8FileInfo = JsonSerializer.Deserialize(request.InputStream, RequestWithM3u8FileInfoContext.Default.RequestWithM3u8FileInfo);
                if (requestWithM3U8FileInfo is null)
                {
                    response.Json(Response.Error("序列化失败"));
                    return;
                }

                requestWithM3U8FileInfo.Validate();
                requestWithM3U8FileInfo.M3UFileInfos.PlaylistType = "VOD";
                if (!string.IsNullOrWhiteSpace(requestWithM3U8FileInfo.SavePath))
                    requestWithM3U8FileInfo.SavePath = requestWithM3U8FileInfo.SavePath.Replace(_DirectorySeparatorChar, Path.DirectorySeparatorChar);
                AppCommandService.DownloadByM3uFileInfo(null, requestWithM3U8FileInfo.ToDownloadParam(), requestWithM3U8FileInfo.M3UFileInfos, null);

                response.Json(Response.Success());
            }
            catch (Exception e)
            {
                response.Json(Response.Error($"请求失败,{e.Message}"));
            }
        }


        private async void GetM3u8FileInfo(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                RequestWithGetM3u8FileInfo? requestWIthGetM3U8FileInfo = JsonSerializer.Deserialize(request.InputStream, RequestWithGetM3u8FileInfoContext.Default.RequestWithGetM3u8FileInfo);
                if (requestWIthGetM3U8FileInfo is null)
                {
                    response.Json(Response.Error("序列化失败"));
                    return;
                }

                requestWIthGetM3U8FileInfo.Validate();
                IM3uFileInfo m3UFileInfo = await M3u8FileInfoClient.CreateM3uFileReader().GetM3u8FileInfo(requestWIthGetM3U8FileInfo.Url!,requestWIthGetM3U8FileInfo.Content);
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