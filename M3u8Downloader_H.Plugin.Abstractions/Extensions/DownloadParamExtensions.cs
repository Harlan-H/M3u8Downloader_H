using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.Abstractions.Extensions
{
    public static class DownloadParamExtensions
    {
        public static string GetCachePath<T>(this T obj) where T : IDownloadParamBase
        {
            if(string.IsNullOrWhiteSpace(obj.SavePath))
                return string.Empty;

            if(string.IsNullOrWhiteSpace(obj.VideoName)) 
                return string.Empty;

            return Path.Combine(obj.SavePath,obj.VideoName);
        }

        public static string GetVideoFullPath<T>(this T obj) where T : IDownloadParamBase
        {
            if (string.IsNullOrWhiteSpace(obj.SavePath))
                return string.Empty;

            if (string.IsNullOrWhiteSpace(obj.VideoFullName))
                return string.Empty;

            return Path.Combine(obj.SavePath, obj.VideoFullName);
        }
    }
}
