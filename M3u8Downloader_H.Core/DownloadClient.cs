using M3u8Downloader_H.Plugin;
using System.Collections.Generic;
using System.Net.Http;
using System;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Combiners.Interfaces;
using M3u8Downloader_H.Plugin.PluginManagers;
using System.Threading.Tasks;
using System.Threading;
using M3u8Downloader_H.M3U8;
using M3u8Downloader_H.M3U8.M3UFileReaderManangers;
using M3u8Downloader_H.Downloader.DownloaderSources;
using M3u8Downloader_H.Downloader;
using M3u8Downloader_H.Combiners;
using System.IO;
using M3u8Downloader_H.Settings.Models;
using M3u8Downloader_H.Core.M3u8UriManagers;
using M3u8Downloader_H.Common.Interfaces;

namespace M3u8Downloader_H.Core
{
    public class DownloadClient(HttpClient httpClient, Uri url, IEnumerable<KeyValuePair<string, string>>? header, ILog log, Type? pluginType)
    {
        private readonly HttpClient httpClient = httpClient;
        private Uri _url = url;
        private readonly IEnumerable<KeyValuePair<string, string>>? _header = header;
        private readonly PluginManger? pluginManager = PluginManger.CreatePluginMangaer(pluginType, httpClient, log);
        private readonly ILog _log = log;
        private M3u8FileInfoClient? m3U8FileInfoClient;
        private M3uDownloaderClient? m3UDownloaderClient;
        private M3uCombinerClient? m3UCombinerClient;
        private IM3u8UriManager? m3U8UriManager;
        private bool _theFirstTime = true;

        public string M3uContent { get; set; } = default!;
        public M3UFileInfo M3u8FileInfo { get; set; } = default!;
        public M3UKeyInfo M3UKeyInfo { get; set; } = default!;
        public IDownloadParams DownloadParams { get; set; } = default!;
        public ISettings Settings { get; set; } = default!;


        private IM3u8UriManager M3U8UriManager
        {
            get
            {
                m3U8UriManager ??= M3u8UriManagerFactory.CreateM3u8UriManager(pluginManager?.M3U8UriProvider,  _header);
                return m3U8UriManager;
            }

        }

        private IM3UFileInfoMananger M3uFileReader
        {
            get
            {
                m3U8FileInfoClient ??= new M3u8FileInfoClient(httpClient, pluginManager);
                m3U8FileInfoClient.M3UFileReader.TimeOuts = TimeSpan.FromSeconds(Settings.Timeouts);
                m3U8FileInfoClient.M3UFileReader.Log = _log;
                return m3U8FileInfoClient.M3UFileReader;
            }
        }

        public IDownloaderSource Downloader
        {
            get
            {
                if (m3UDownloaderClient is null)
                {
                    m3UDownloaderClient = new M3uDownloaderClient(httpClient, _url, M3u8FileInfo, pluginManager);
                    m3UDownloaderClient.Downloader.GetLiveFileInfoFunc = M3uFileReader.GetM3u8FileInfo;
                    m3UDownloaderClient.Downloader.M3UFileInfo = M3u8FileInfo;
                    m3UDownloaderClient.Downloader.Headers = _header;
                    m3UDownloaderClient.Downloader.DownloadParams = DownloadParams;
                    m3UDownloaderClient.Downloader.Log = _log;
                }

                return m3UDownloaderClient.Downloader;
            }
        }

        public M3uCombinerClient Merger 
        {
            get
            {
                m3UCombinerClient ??= new M3uCombinerClient(M3u8FileInfo)
                {
                    DownloadParams = DownloadParams,
                    Log = _log
                };
                m3UCombinerClient.Settings = Settings;
                return m3UCombinerClient;
            }
        }

        public async Task GetM3u8Uri(CancellationToken cancellationToken)
        {
            if(!M3U8UriManager.Completed)
                _url = await M3U8UriManager.GetM3u8UriAsync(_url,0,cancellationToken);
        }


        public async Task GetM3U8FileInfo(CancellationToken cancellationToken)
        {
            if (!_theFirstTime) 
                return;

            if (M3u8FileInfo is not null )
            {
                _log.Info("获取视频流{0}个", M3u8FileInfo.MediaFiles.Count);
                DownloadParams.VideoFullName = DownloadParams.VideoFullPath + (M3u8FileInfo.Map is not null ? Path.GetExtension(M3u8FileInfo.Map?.Title) : ".ts");
                _theFirstTime = false;
                return;
            }


            if (_url.IsFile)
            {
                string ext = Path.GetExtension(_url.OriginalString).Trim('.');
                M3u8FileInfo = M3uFileReader.GetM3u8FileInfo(ext, _url);
            }
            else
            {
                M3u8FileInfo =  await M3uFileReader.GetM3u8FileInfo(_url, _header, cancellationToken);
            }
            _log.Info("获取视频流{0}个", M3u8FileInfo.MediaFiles.Count);
            if (M3UKeyInfo is not null)
                M3u8FileInfo.Key = M3UKeyInfo;

            DownloadParams.VideoFullName = DownloadParams.VideoFullPath + (M3u8FileInfo.Map is not null ? Path.GetExtension(M3u8FileInfo.Map?.Title) : ".ts");
            _theFirstTime = false;
        }
    }
}
