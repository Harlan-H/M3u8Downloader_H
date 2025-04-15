using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Utils;

namespace M3u8Downloader_H.Common.DownloadPrams
{
    public class DownloadParamsBase : IDownloadParamBase
    {
        protected static readonly string _defaultCachePath =
#if DEBUG
           "E:\\desktop\\download\\Caches";
#else
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Caches");
#endif

        protected readonly string _cacheName;
        protected string _videoName = string.Empty;

        public string CachePath =>  Path.Combine(_defaultCachePath, _cacheName);

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

        public DownloadParamsBase(Uri uri, string? videoName, string savePath, string selectFormat, IDictionary<string, string>? headers)
        {
            _cacheName = PathEx.GenerateFileNameWithoutExtension(uri);
            VideoName = videoName!;
            SelectFormats = selectFormat;
            SavePath = savePath;
            Headers = headers;
        }

    }
}
