using Avalonia.Threading;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using System;

namespace M3u8Downloader_H.Services
{
    using DownloadByM3uFileInfoActionType = Action<IDownloadParamBase, IM3uFileInfo, string?>;
    using DownloadByUrlActionType = Action<IM3u8DownloadParam, string?>;
    using DownloadMediaActionType = Action<IMediaDownloadParam>;

    public class AppCommandService(
            DownloadByUrlActionType downloadByUrl,
            DownloadByM3uFileInfoActionType downloadByM3uFileInfo,
            DownloadMediaActionType downloadMedia) : IAppCommandService
    {
        /*
          (param, key) =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        m3U8WindowViewModel.ProcessM3u8Download(param, key);
                    });
                }*/
        public void DownloadByM3uFileInfo(IDownloadParamBase downloadParamBase, IM3uFileInfo m3UFileInfo, string? pluginKey)
        {
            downloadByM3uFileInfo(downloadParamBase, m3UFileInfo, pluginKey);
        }

        public void DownloadByUrl(IM3u8DownloadParam m3U8DownloadParam, string? pluginKey)
        {
            downloadByUrl(m3U8DownloadParam, pluginKey);
        }

        public void DownloadMedia(IMediaDownloadParam mediaDownloadParam)
        {
            downloadMedia(mediaDownloadParam);
        }   
    }
}
