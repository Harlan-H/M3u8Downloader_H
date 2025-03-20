using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.Models
{
    public class DownloadParamsBase : IDownloadParamBase
    {
        public string CachePath => "./caches";

        public string VideoName { get; protected set; } = default!;

        public string VideoFullName { get; protected set; } = default!;

        public string SavePath { get; } = default!;

        public IDictionary<string, string>? Headers { get; } = default!;

        public DownloadParamsBase(string videoName, string savePath, IDictionary<string, string>? headers)
        {
            VideoName = videoName;
            SavePath = savePath;
            Headers = headers;
        }

        public DownloadParamsBase(string savePath, IDictionary<string, string>? headers)
        {
            SavePath = savePath;
            Headers = headers;
        }

        public void SetVideoFullName(string videoName)
        {
            if (string.IsNullOrWhiteSpace(videoName))
                return;

            VideoFullName = videoName;
        }
    }
}
