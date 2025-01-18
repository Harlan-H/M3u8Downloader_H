using System;
using System.Collections.Generic;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;

namespace M3u8Downloader_H.Models
{
    public class M3u8DownloadParams : IM3u8DownloadParam
    {
        public Uri RequestUrl { get; } = default!;

        public string Method { get;  } = default!;

        public string? Key { get;  } = default!;

        public string? Iv { get;  } = default!;

        public string VideoName { get; set; } = default!;

        public string VideoFullName { get; set; } = default!;

        public string SavePath { get; set; } = default!;

        public IDictionary<string, string> Headers { get; set; } = default!;

        public M3u8DownloadParams(SettingsService settingsService,Uri url,string? videoname)
        {
            RequestUrl = url;
            VideoName = PathEx.GenerateFileNameWithoutExtension(url, videoname);
            SavePath = settingsService.SavePath;
        }

        public M3u8DownloadParams(SettingsService settingsService,Uri url, string? videoname,string method,string? key,string? iv)
        {
            SavePath = settingsService.SavePath;
            RequestUrl = url;
            VideoName = PathEx.GenerateFileNameWithoutExtension(url, videoname);
            Method = method;
            Key = key;
            Iv = iv;
        }

        public M3u8DownloadParams(SettingsService settingsService,IM3u8DownloadParam downloadParamBase)
        {
            RequestUrl = downloadParamBase.RequestUrl;
            Method = downloadParamBase.Method;
            Key = downloadParamBase.Key;
            Iv = downloadParamBase.Iv;
            VideoName = PathEx.GenerateFileNameWithoutExtension(downloadParamBase.RequestUrl, downloadParamBase.VideoName);
            VideoFullName = downloadParamBase.VideoFullName;
            SavePath = downloadParamBase.SavePath ?? settingsService.SavePath;
            Headers = downloadParamBase.Headers!;
        }

        public M3u8DownloadParams(SettingsService settingsService,Uri requestUrl, IDownloadParamBase downloadParamBase)
        {
            VideoFullName = downloadParamBase.VideoFullName;
            SavePath = downloadParamBase.SavePath ?? settingsService.SavePath;
            Headers = downloadParamBase.Headers!;
            VideoName = PathEx.GenerateFileNameWithoutExtension(requestUrl, downloadParamBase.VideoName);
        }

        public void SetVideoFullName(string videoName)
        {
            if (string.IsNullOrWhiteSpace(videoName))
                return;

            VideoFullName = videoName;
        }

        public M3UKeyInfo? GetM3uKeyInfo()
        {
            if (!string.IsNullOrWhiteSpace(Key))
                return new M3UKeyInfo(Method!, Key!, Iv!);
            return null;
        }
    }
}
