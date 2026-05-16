
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using System;

namespace M3u8Downloader_H.Plugin.Services
{
    using DownloadByM3uFileInfoActionType = Action<HttpClient?,IDownloadParamBase, IM3uFileInfo, IDownloadPlugin?>;
    using DownloadByUrlActionType = Action<HttpClient?, IM3u8DownloadParam, IDownloadPlugin?>;
    using DownloadMediaActionType = Action<HttpClient?, IMediaDownloadParam>;

    public class AppCommandService(
            DownloadByUrlActionType downloadByUrl,
            DownloadByM3uFileInfoActionType downloadByM3uFileInfo,
            DownloadMediaActionType downloadMedia) : IAppCommandService
    {

        public void DownloadByM3uFileInfo(HttpClient? httpClient, IDownloadParamBase downloadParamBase, IM3uFileInfo m3UFileInfo, IDownloadPlugin? downloadPlugin)
        {
            downloadByM3uFileInfo(httpClient,downloadParamBase, m3UFileInfo, downloadPlugin);
        }


        public void DownloadByUrl(HttpClient? httpClient, IM3u8DownloadParam m3U8DownloadParam, IDownloadPlugin? downloadPlugin)
        {
            downloadByUrl(httpClient,m3U8DownloadParam, downloadPlugin);
        }


        public void DownloadMedia(HttpClient? httpClient, IMediaDownloadParam mediaDownloadParam)
        {
            downloadMedia(httpClient,mediaDownloadParam);
        }
    }
}
