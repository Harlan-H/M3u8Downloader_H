using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.Abstractions.Common
{
    public interface IM3u8DownloadParam : IDownloadParamBase
    {
        Uri RequestUrl { get; }
        public string Method { get;  } 
        public string? Key { get;  }
        public string? Iv { get; }

    }
}
