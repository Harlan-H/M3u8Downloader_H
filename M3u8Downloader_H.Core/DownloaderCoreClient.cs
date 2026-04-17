using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Downloader;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Core.Downloads;
using System;
using System.Net.Http;

namespace M3u8Downloader_H.Core
{
    public class DownloaderCoreClient
    {
        public IDownloader Downloader { get; } = default!;

        public DownloaderCoreClient(IDownloadContext context,
              IDownloadPlugin? downloadPlugin)
        {
            Downloader = M3u8Downloader.CreateM3u8Downloader(context, downloadPlugin);
        }

        public DownloaderCoreClient(IDownloadContext context,
              IM3uFileInfo m3UFileInfo,
              IDownloadPlugin? downloadPlugin)
        {
            Downloader = M3u8Downloader.CreateM3u8Downloader(context, downloadPlugin, m3UFileInfo);
        }

        public DownloaderCoreClient(IDownloadContext context)
        {
            Downloader = MediaDownloader.CreateMediaDownloader(context);
        }

    }
}
