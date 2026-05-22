using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.Services;
using System;
using System.IO;

namespace M3u8Downloader_H.Extensions;

internal static class DownloadParamBaseExtensions
{
    extension<T>(T downloadbase)
        where T : IDownloadParamBase
    {
        public void CompleteAttribute(SettingsService settingsService)
        {
            if(downloadbase is DownloadParamsBase downloadParamBase)
            {
                if (string.IsNullOrEmpty(downloadbase.SavePath))
                {
                    downloadParamBase.SavePath = settingsService.SavePath;
                }
                else
                {
                    var savePath = Path.GetFullPath(Path.Combine(settingsService.SavePath, downloadParamBase.SavePath));
                    var rootSavePath = Path.GetFullPath(settingsService.SavePath);
                    if (!savePath.StartsWith(rootSavePath))
                        throw new InvalidOperationException("路径非法");
                    downloadParamBase.SavePath = savePath;
                }
                downloadParamBase.Headers ??= settingsService.Headers;
            }
        }
    }
}
