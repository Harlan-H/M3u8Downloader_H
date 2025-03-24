using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Utils;

namespace M3u8Downloader_H.Common.DownloadPrams
{
    public class DownloadParamsBase : IDownloadParamBase
    {
        private readonly string _cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "caches");
        private readonly string _cacheName;
        public string CachePath => Path.Combine(_cachePath, _cacheName);
        public string VideoName { get; protected set; } = default!;

        public string SelectFormats { get; protected set; } = "mp4";

        public string VideoFullName => VideoName + "." + SelectFormats;

        public string SavePath { get; protected set; } = default!;

        public IDictionary<string, string>? Headers { get; protected set; } = default!;

        public DownloadParamsBase(Uri uri, string? videoName, string savePath, string selectFormat, IDictionary<string, string>? headers)
        {
            _cacheName = PathEx.GenerateFileNameWithoutExtension(uri);
            VideoName = videoName ?? _cacheName;
            SelectFormats = selectFormat;
            SavePath = savePath;
            Headers = headers;
        }

        public void UpdateSavePath(string savePath)
        {
            if(string.IsNullOrWhiteSpace(SavePath))
                SavePath =  savePath;
        }
    }
}
