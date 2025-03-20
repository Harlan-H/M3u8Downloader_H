using System;
using System.Collections.Generic;
using System.Security.Policy;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;

namespace M3u8Downloader_H.Models
{
    public class M3u8DownloadParams : DownloadParamsBase, IM3u8DownloadParam
    {
        public Uri RequestUrl { get; } = default!;

        public string Method { get;  } = default!;

        public string? Key { get;  } = default!;

        public string? Iv { get;  } = default!;


        public M3u8DownloadParams(SettingsService settingsService,Uri url,string? videoname)
            : base(PathEx.GenerateFileNameWithoutExtension(url, videoname),
                settingsService.SavePath,
                settingsService.Headers!)
        {
            RequestUrl = url;
        }

        public M3u8DownloadParams(SettingsService settingsService,Uri url, string? videoname,string method,string? key,string? iv)
            : base(PathEx.GenerateFileNameWithoutExtension(url, videoname),
                settingsService.SavePath,
                settingsService.Headers!)
        {
            RequestUrl = url;
            Method = method;
            if(key != null) 
                Key = key;
            if(iv != null) 
                Iv = iv;
        }

        public M3u8DownloadParams(SettingsService settingsService,IM3u8DownloadParam downloadParamBase)
             : base(PathEx.GenerateFileNameWithoutExtension(downloadParamBase.RequestUrl, downloadParamBase.VideoName),
                downloadParamBase.SavePath ?? settingsService.SavePath,
                downloadParamBase.Headers!)
        {
            RequestUrl = downloadParamBase.RequestUrl;
            Method = downloadParamBase.Method;
            Key = downloadParamBase.Key;
            Iv = downloadParamBase.Iv;
            VideoFullName = downloadParamBase.VideoFullName;
        }

        public M3u8DownloadParams(SettingsService settingsService,Uri requestUrl, IDownloadParamBase downloadParamBase)
             : base(PathEx.GenerateFileNameWithoutExtension(requestUrl, downloadParamBase.VideoName),
                downloadParamBase.SavePath ?? settingsService.SavePath,
                downloadParamBase.Headers!)
        {
            VideoFullName = downloadParamBase.VideoFullName;
        }


        public M3UKeyInfo? GetM3uKeyInfo()
        {
            if (!string.IsNullOrWhiteSpace(Key))
                return new M3UKeyInfo(Method!, Key!, Iv!);
            return null;
        }
    }
}
