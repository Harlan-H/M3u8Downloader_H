using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.Combiners.Extensions
{
    internal static class DownloadParamExtensions
    {
        public static string GetCachePath<T>(this T obj) where T : IDownloadParamBase
        {
            if(string.IsNullOrWhiteSpace(obj.SavePath))
                return string.Empty;

            if(string.IsNullOrWhiteSpace(obj.VideoName)) 
                return string.Empty;

            return Path.Combine(obj.SavePath,obj.VideoName);
        }

        public static void SetVideoName<T>(this T obj,string newVideoName) where T: IDownloadParamBase
        {
            if (string.IsNullOrWhiteSpace(newVideoName))
                return;

            obj.VideoFullName = newVideoName;
        }

        public static string GetVideoFullPath<T>(this T obj) where T : IDownloadParamBase
        {
            if (string.IsNullOrWhiteSpace(obj.SavePath))
                return string.Empty;

            if (string.IsNullOrWhiteSpace(obj.VideoName))
                return string.Empty;

            return Path.Combine(obj.SavePath, obj.VideoFullName);
        }
    }
}
