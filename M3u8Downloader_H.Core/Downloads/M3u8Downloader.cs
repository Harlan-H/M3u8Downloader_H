using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Downloader;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Abstractions.Settings;
using M3u8Downloader_H.Combiners;
using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Common.Models;
using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.Downloader;
using M3u8Downloader_H.M3U8;
using M3u8Downloader_H.M3U8.Extensions;

namespace M3u8Downloader_H.Core.Downloads
{
    public partial class M3u8Downloader(IDownloadContext context) : IDownloader
    {
        private M3u8FileInfoClient m3U8FileInfoClient = default!;
        private DownloaderClient m3UDownloaderClient = default!;
        private M3uCombinerClient m3UCombinerClient = default!;
        private IM3uFileInfo M3U8FileInfo = default!;
        private IM3uKeyInfo? M3UKeyInfo;

        private bool _isParsed = false;
        private bool _isDownloaded = false;

        public async ValueTask StartDownload(Action<int> StateAction,IDialogProgress dialogProgress, CancellationToken cancellationToken)
        {
            StateAction.Invoke((int)DownloadStatus.Parsed);
            await GetM3U8FileInfo(cancellationToken);

            using var acquire = dialogProgress.Acquire();
            await DownloadAsync(dialogProgress, cancellationToken);

            await MergeAsync(dialogProgress, cancellationToken);

            IMergeSetting mergeSetting = (IMergeSetting)context.DownloaderSetting;
            if (mergeSetting.IsCleanUp)
                DirectoryEx.DeleteCache(context.DownloadParam.CachePath);
        }

        private async ValueTask GetM3U8FileInfo(CancellationToken cancellationToken)
        {
            if (_isParsed)
                return;


            if (M3U8FileInfo is not null && !M3U8FileInfo.IsEmpty)
            {
                context.Log.Info("获取视频流{0}个", M3U8FileInfo.MediaFiles.Count);
                _isParsed = true;
                return;
            }

            IM3u8DownloadParam m3u8DownloadParam = (IM3u8DownloadParam)context.DownloadParam;
            if (m3u8DownloadParam.RequestUrl.IsFile)
                M3U8FileInfo = m3U8FileInfoClient.M3u8FileReader.GetM3u8FileInfo(m3u8DownloadParam.RequestUrl);
            else
                M3U8FileInfo = await m3U8FileInfoClient.M3UFileReadManager.GetM3u8FileInfo(cancellationToken);

            if (M3U8FileInfo.IsEmpty)
                throw new InvalidDataException("没有包含任何数据");

            context.Log.Info("获取视频流{0}个", M3U8FileInfo.MediaFiles.Count);
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

            await m3UDownloaderClient.M3u8Downloader.Initialization(cancellationToken);
            await m3UDownloaderClient.M3u8Downloader.StartDownload(M3U8FileInfo, cancellationToken);
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
              IDownloadContext context,
              Type? pluginType
            )
        {
            M3u8Downloader m3U8Downloader = new(context);
            IPluginEntry? pluginEntry = null; /*PluginManger.CreatePluginMangaer(pluginType, context.HttpClient, context.Loglogger);*/
            m3U8Downloader.m3U8FileInfoClient = new M3u8FileInfoClient(context, pluginEntry);
            m3U8Downloader.m3UDownloaderClient = new DownloaderClient(context, pluginEntry)
            {
                GetLiveFileInfoFunc = m3U8Downloader.m3U8FileInfoClient.M3UFileReadManager.GetM3u8FileInfo
            };

            m3U8Downloader.m3UCombinerClient = new M3uCombinerClient(context.Log, context.DownloadParam, (IMergeSetting)context.DownloaderSetting);

            IM3u8DownloadParam m3U8DownloadParam = (IM3u8DownloadParam)context.DownloadParam;
            m3U8Downloader.M3UKeyInfo = m3U8DownloadParam.M3UKeyInfo;
            return m3U8Downloader;
        }

        //通过接口创建
        public static M3u8Downloader CreateM3u8Downloader(
            IDownloadContext context,
            Type? pluginType,
            IM3uFileInfo m3UFileInfo
            )
        {
            M3u8Downloader m3U8Downloader = new(context);
            IPluginEntry? pluginEntry =  null; /* PluginManger.CreatePluginMangaer(pluginType, httpClient, logger);*/
            m3U8Downloader.m3UDownloaderClient = new DownloaderClient(context, pluginEntry);
            m3U8Downloader.m3UCombinerClient = new M3uCombinerClient(context.Log, context.DownloadParam, (IMergeSetting)context.DownloaderSetting);

            m3U8Downloader.M3U8FileInfo = m3UFileInfo;
            context.Log.Info("通过接口传入m3u8文件的视频流有{0}个,将跳过获取操作开始直接下载", m3UFileInfo.MediaFiles.Count);
            m3U8Downloader._isParsed = true;
            return m3U8Downloader;
        }
    }
}
