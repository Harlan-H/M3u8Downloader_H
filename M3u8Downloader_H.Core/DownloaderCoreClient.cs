using System;
using System.Net.Http;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Converter;
using M3u8Downloader_H.Abstractions.Downloader;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Settings;
using M3u8Downloader_H.Core.Converters;
using M3u8Downloader_H.Core.Downloads;

namespace M3u8Downloader_H.Core
{
    public class DownloaderCoreClient
    {
        public IDownloader Downloader { get; } = default!;

        public IConverter Converter { get; } = default!;

        public DownloaderCoreClient(HttpClient httpClient,
              IM3u8DownloadParam m3U8DownloadParam,
              IDownloaderSetting downloaderSetting,
              ILog logger,
              Type? pluginType)
        {
            Downloader = M3u8Downloader.CreateM3u8Downloader(httpClient, m3U8DownloadParam, downloaderSetting, logger, pluginType);
        }

        public DownloaderCoreClient(HttpClient httpClient,
              IDownloadParamBase m3U8DownloadParam,
              IDownloaderSetting downloaderSetting,
              ILog logger,
              Type? pluginType,
              IM3uFileInfo m3UFileInfo)
        {
            Downloader = M3u8Downloader.CreateM3u8Downloader(httpClient, m3U8DownloadParam, downloaderSetting, logger, pluginType, m3UFileInfo);
        }

        public DownloaderCoreClient(HttpClient httpClient,
            IMediaDownloadParam mediaDownloadParam,
            IDownloaderSetting downloaderSetting,
            ILog logger)
        {
            Downloader = MediaDownloader.CreateMediaDownloader(httpClient, mediaDownloadParam, downloaderSetting, logger);
        }

        public DownloaderCoreClient(
            IM3uFileInfo m3UFileInfo,
            IM3u8DownloadParam m3U8DownloadParam,
            IMergeSetting mergeSetting,
            ILog logger
            )
        {
            Converter = M3u8Converter.CreateM3u8Converter(m3UFileInfo, m3U8DownloadParam, mergeSetting, logger);
        }

        public DownloaderCoreClient(
            IMediaDownloadParam mediaDownloadParam,
            IMergeSetting mergeSetting,
            ILog logger
            )
        {
            Converter = MediaConverter.CreateMediaConverter(mediaDownloadParam, mergeSetting, logger);
        }
    }
}
