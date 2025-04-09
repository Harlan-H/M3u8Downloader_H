using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Downloader;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Settings;
using M3u8Downloader_H.Combiners;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Common.Models;
using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.Downloader;
using M3u8Downloader_H.M3U8;
using M3u8Downloader_H.Plugin.PluginManagers;

namespace M3u8Downloader_H.Core.Downloads
{
    public partial class M3u8Downloader : IDownloader
    {
        private readonly IDownloadParamBase downloadParam;
        private readonly IDownloaderSetting downloaderSetting;
        private readonly ILog Log;
        private M3u8FileInfoClient m3U8FileInfoClient = default!;
        private DownloaderClient m3UDownloaderClient = default!;
        private M3uCombinerClient m3UCombinerClient = default!;
        private IM3uFileInfo M3U8FileInfo = default!;
        private IM3uKeyInfo? M3UKeyInfo;

        private bool _isParsed = false;
        private bool _isDownloaded = false;


        public M3u8Downloader(IDownloadParamBase m3U8DownloadParam, IDownloaderSetting downloaderSetting,ILog log)
        {
            this.downloadParam = m3U8DownloadParam;
            this.downloaderSetting = downloaderSetting;
            Log = log;
        }

        public async ValueTask StartDownload(Action<int> StateAction,IDialogProgress dialogProgress, CancellationToken cancellationToken)
        {
            StateAction.Invoke((int)DownloadStatus.Parsed);
            await GetM3U8FileInfo(cancellationToken);

            using var acquire = dialogProgress.Acquire();
            await DownloadAsync(dialogProgress, cancellationToken);

            await MergeAsync(dialogProgress, cancellationToken);

            IMergeSetting mergeSetting = (IMergeSetting)downloaderSetting;
            if (mergeSetting.IsCleanUp)
                DirectoryEx.DeleteCache(downloadParam.CachePath);
        }

        private async ValueTask GetM3U8FileInfo(CancellationToken cancellationToken)
        {
            if (_isParsed)
                return;

            IM3u8DownloadParam m3u8DownloadParam = (IM3u8DownloadParam)downloadParam;
            if (m3u8DownloadParam.RequestUrl.IsFile)
            {
                string ext = Path.GetExtension(m3u8DownloadParam.RequestUrl.OriginalString).Trim('.');
                M3U8FileInfo = m3U8FileInfoClient.M3UFileReadManager.GetM3u8FileInfo(ext, m3u8DownloadParam.RequestUrl);
            }
            else
            {
                M3U8FileInfo = await m3U8FileInfoClient.M3UFileReadManager.GetM3u8FileInfo(cancellationToken);
            }
            Log.Info("获取视频流{0}个", M3U8FileInfo.MediaFiles.Count);

            if (M3UKeyInfo is not null)
            {
                M3UFileInfo m3UFileInfo = (M3UFileInfo)M3U8FileInfo;
                m3UFileInfo.Key = m3u8DownloadParam.M3UKeyInfo!;
            }

            _isParsed = true;
        }


        private async ValueTask DownloadAsync(IDialogProgress downloadProgress, CancellationToken cancellationToken)
        {
            if (_isDownloaded)
                return;

            m3UDownloaderClient.M3UFileInfo = M3U8FileInfo;
            m3UDownloaderClient.DialogProgress = downloadProgress;

            await m3UDownloaderClient.M3u8Downloader.DownloadAsync(M3U8FileInfo, cancellationToken);
            _isDownloaded = true;
        }

        private async ValueTask MergeAsync(IDialogProgress progress, CancellationToken cancellationToken)
        {
            if (M3U8FileInfo.Map is not null)
            {
                m3UCombinerClient.M3u8FileMerger.Initialize(M3U8FileInfo);
                await m3UCombinerClient.M3u8FileMerger.StartMerging(M3U8FileInfo, progress, cancellationToken);
            }
            else
                await m3UCombinerClient.FFmpeg.ConvertToMp4(M3U8FileInfo, progress, cancellationToken);
        }
    }


    public partial class M3u8Downloader
    {
        //通过软件界面创建
        public static M3u8Downloader CreateM3u8Downloader(
              HttpClient httpClient,
              IM3u8DownloadParam m3U8DownloadParam,
              IDownloaderSetting downloaderSetting,
              ILog logger,
              Type? pluginType
            )
        {
            M3u8Downloader m3U8Downloader = new(m3U8DownloadParam, downloaderSetting, logger);
            PluginManger? pluginManger = PluginManger.CreatePluginMangaer(pluginType, httpClient, logger);
            m3U8Downloader.m3U8FileInfoClient = new M3u8FileInfoClient(httpClient, pluginManger, logger, m3U8DownloadParam, downloaderSetting);
            m3U8Downloader.m3UDownloaderClient = new DownloaderClient(httpClient, pluginManger, logger, m3U8DownloadParam, downloaderSetting)
            {
                GetLiveFileInfoFunc = m3U8Downloader.m3U8FileInfoClient.M3UFileReadManager.GetM3u8FileInfo
            };

            m3U8Downloader.m3UCombinerClient = new M3uCombinerClient(logger, m3U8DownloadParam, (IMergeSetting)downloaderSetting);

            m3U8Downloader.M3UKeyInfo = m3U8DownloadParam.M3UKeyInfo;
            return m3U8Downloader;
        }

        //通过接口创建
        public static M3u8Downloader CreateM3u8Downloader(
            HttpClient httpClient,
            IDownloadParamBase m3U8DownloadParam,
            IDownloaderSetting downloaderSetting,
            ILog logger,
            Type? pluginType,
            IM3uFileInfo m3UFileInfo
            )
        {
            M3u8Downloader m3U8Downloader = new(m3U8DownloadParam, downloaderSetting, logger);
            PluginManger? pluginManger = PluginManger.CreatePluginMangaer(pluginType, httpClient, logger);
            m3U8Downloader.m3UDownloaderClient = new DownloaderClient(httpClient, pluginManger, logger, m3U8DownloadParam,downloaderSetting);
            m3U8Downloader.m3UCombinerClient = new M3uCombinerClient(logger, m3U8DownloadParam, (IMergeSetting)downloaderSetting);

            m3U8Downloader.M3U8FileInfo = m3UFileInfo;
            logger.Info("通过接口传入m3u8文件的视频流有{0}个,将跳过获取操作开始直接下载", m3UFileInfo.MediaFiles.Count);
            m3U8Downloader._isParsed = true;
            return m3U8Downloader;
        }
    }
}
