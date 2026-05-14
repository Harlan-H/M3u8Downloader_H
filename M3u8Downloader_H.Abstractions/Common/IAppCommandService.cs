using M3u8Downloader_H.Abstractions.M3u8;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Abstractions.Common
{
    public interface IAppCommandService
    {
        void DownloadByM3uFileInfo(IDownloadParamBase downloadParamBase, IM3uFileInfo m3UFileInfo, string? pluginKey);
        void DownloadByUrl(IM3u8DownloadParam m3U8DownloadParam, string? pluginKey);
        void DownloadMedia(IMediaDownloadParam mediaDownloadParam);
    }
}
