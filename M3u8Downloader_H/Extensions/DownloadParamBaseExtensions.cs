using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.Services;
using System.IO;
using System.Linq;

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
                    var savePath = Path.GetInvalidFileNameChars().Append('.').Aggregate(downloadParamBase.SavePath.TrimStart('/'), (current, invalidChar) => current.Replace(invalidChar, '_'));
                    downloadParamBase.SavePath = Path.Combine(settingsService.SavePath, savePath);
                }
                downloadParamBase.Headers ??= settingsService.Headers;
            }
        }
    }
}
