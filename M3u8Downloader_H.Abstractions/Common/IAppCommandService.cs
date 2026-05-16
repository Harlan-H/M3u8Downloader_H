using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Abstractions.Common
{
    public interface IAppCommandService
    {
        void DownloadByM3uFileInfo(HttpClient? httpClient, IDownloadParamBase downloadParamBase, IM3uFileInfo m3UFileInfo, IDownloadPlugin? downloadPlugin);
        void DownloadByUrl(HttpClient? httpClient, IM3u8DownloadParam m3U8DownloadParam, IDownloadPlugin? downloadPlugin);
        void DownloadMedia(HttpClient? httpClient, IMediaDownloadParam mediaDownloadParam);
    }
}
