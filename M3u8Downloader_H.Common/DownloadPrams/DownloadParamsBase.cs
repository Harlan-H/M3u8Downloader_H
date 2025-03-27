using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Utils;

namespace M3u8Downloader_H.Common.DownloadPrams
{
    public class DownloadParamsBase : IDownloadParamBase
    {
        private static readonly string _cachePath =
#if DEBUG
           "E:\\desktop\\download\\Caches";
#else
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Caches");
#endif

        private readonly string _cacheName;
        private string _videoName = string.Empty;

        public string CachePath { get;private set; }

        public string SelectFormats { get;  set; } = "mp4";

        public string VideoFullName => Path.Combine(SavePath,  VideoName + "." + SelectFormats);

        public string SavePath { get;  set; } = default!;

        public IDictionary<string, string>? Headers { get; protected set; } = default!;

        public string VideoName
        {
            get => _videoName;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _videoName = _cacheName;
                    return;
                }

                if ( string.IsNullOrEmpty(_videoName) || _videoName != value)
                {
                    _videoName = PathEx.EscapeFileName(value);
                    return;
                }
            }
        }

        public DownloadParamsBase(Uri uri, string? videoName, string? cachePath, string savePath, string selectFormat, IDictionary<string, string>? headers)
        {
            _cacheName = PathEx.GenerateFileNameWithoutExtension(uri);

            if (!string.IsNullOrWhiteSpace(cachePath))
                CachePath = cachePath;
            else
                CachePath = Path.Combine(_cachePath, _cacheName);

            VideoName = videoName!;
            SelectFormats = selectFormat;
            SavePath = savePath;
            Headers = headers;
        }

    }
}
