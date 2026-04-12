using System;
using System.Net.Http;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Downloader;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Core.Downloads;

namespace M3u8Downloader_H.Core
{
    public class DownloaderCoreClient
    {
        public IDownloader Downloader { get; } = default!;

        public DownloaderCoreClient(IDownloadContext context,
              Type? pluginType)
        {
            Downloader = M3u8Downloader.CreateM3u8Downloader(context, pluginType);
        }

        public DownloaderCoreClient(IDownloadContext context,
              Type? pluginType,
              IM3uFileInfo m3UFileInfo)
        {
            Downloader = M3u8Downloader.CreateM3u8Downloader(context, pluginType, m3UFileInfo);
        }

        public DownloaderCoreClient(IDownloadContext context)
        {
            Downloader = MediaDownloader.CreateMediaDownloader(context);
        }

    }
}
