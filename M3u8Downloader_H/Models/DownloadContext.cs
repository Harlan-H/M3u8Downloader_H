using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace M3u8Downloader_H.Models
{
    internal class DownloadContext(HttpClient httpClient,
        ILog log,
        IDownloadParamBase downloadParamBase,
        IDownloaderSetting downloaderSetting
            ) : IDownloadContext
    {
        public HttpClient HttpClient { get; private set; } = httpClient;

        public ILog Log { get; private set; } = log;

        public IDownloadParamBase DownloadParam { get; private set; } = downloadParamBase;

        public IDownloaderSetting DownloaderSetting { get; private set; } = downloaderSetting;
    }
}
