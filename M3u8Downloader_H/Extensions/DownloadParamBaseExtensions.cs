using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.Services;

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
                downloadParamBase.Headers ??= settingsService.Headers;
            }
            
        }
    }
}
