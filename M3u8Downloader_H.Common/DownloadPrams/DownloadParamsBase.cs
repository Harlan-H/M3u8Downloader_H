using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Utils;

namespace M3u8Downloader_H.Common.DownloadPrams
{
    public class DownloadParamsBase : IDownloadParamBase
    {
        private readonly string _cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "caches");
        private readonly string _cacheName;
        private string _videoName = string.Empty;

        public string CachePath => Path.Combine(_cachePath, _cacheName);

        public string SelectFormats { get;  set; } = "mp4";

        public string VideoFullName => VideoName + "." + SelectFormats;

        public string SavePath { get;  set; } = default!;

        public IDictionary<string, string>? Headers { get; protected set; } = default!;

        public string VideoName
        {
            get => _videoName;
            set
            {
                if (string.IsNullOrEmpty(_videoName) || _videoName != value)
                {
                    _videoName = PathEx.EscapeFileName(value);
                    return;
                }
            }
        }

        public DownloadParamsBase(Uri uri, string? videoName, string savePath, string selectFormat, IDictionary<string, string>? headers)
        {
            _cacheName = PathEx.GenerateFileNameWithoutExtension(uri);
            VideoName = videoName ?? _cacheName;
            SelectFormats = selectFormat;
            SavePath = savePath;
            Headers = headers;
        }

    }
}
