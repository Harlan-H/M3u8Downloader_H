using System.Runtime.Intrinsics.Arm;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Common.M3u8;

namespace M3u8Downloader_H.Common.DownloadPrams
{
    public class M3u8DownloadParams : DownloadParamsBase, IM3u8DownloadParam
    {
        public Uri RequestUrl { get; } = default!;
        public IM3uKeyInfo? M3UKeyInfo { get; private set;}

        public M3u8DownloadParams(Uri url, string? videoname,string? cachePath, string savePath, string selectFormat, IDictionary<string, string>? headers)
            : base(url, videoname, cachePath,savePath, selectFormat, headers)
        {
            RequestUrl = url;
        }

        public M3u8DownloadParams(Uri url, string? videoname, string? cachePath, string savePath, string selectFormat, IDictionary<string, string>? headers, string method, string? key, string? iv)
            : base(url, videoname, cachePath, savePath, selectFormat, headers)
        {
            RequestUrl = url;
            if (key != null)
                M3UKeyInfo = M3uKeyInfoHelper.GetKeyInfoInstance(method, key!, iv!);
        }

        public void UpdateKeyInfo(string method, string? key, string? iv)
        {
            if (key != null)
                M3UKeyInfo = M3uKeyInfoHelper.GetKeyInfoInstance(method, key!, iv!);
        }
    }
}
