using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Abstractions.Settings;
using M3u8Downloader_H.Combiners;
using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.M3U8.Models;
using M3u8Downloader_H.Common.Models;
using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.Downloader;
using M3u8Downloader_H.M3U8;
using M3u8Downloader_H.M3U8.Extensions;
using System.Linq;
using System.Collections.Generic;
using M3u8Downloader_H.Core.Interfaces;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.M3U8.M3UFileReaderManangers;

namespace M3u8Downloader_H.Core.Downloads
{
    public record M3uFileInfoState(IM3uFileInfoSource M3UFileInfoSource)
    {
        public bool IsDownloaded { get; set; }
        public bool M3uFileInfoIsEmpty => M3UFileInfoSource.M3uFile is null;
        public M3UFileReaderManager? M3uReader { get; set; }
        public IDownloadService? DownloadService { get; set; }
        public override string ToString() => M3UFileInfoSource.Type.GetDescription();
    }

    public partial class M3u8Downloader(IDownloadContext context,
        Func<IM3uFileInfo, Task<IList<IM3uFileInfoSource>>> m3UFileInfoSourcesFactory,
        bool skipParse) : IDownloader
    {
        private readonly M3u8DownloadParams m3U8DownloadParams = (M3u8DownloadParams)context.DownloadParam;
        private M3u8FileInfoClient m3U8FileInfoClient = default!;
        private DownloaderClient m3UDownloaderClient = default!;
        private M3uCombinerClient m3UCombinerClient = default!;
        private readonly List<M3uFileInfoState> m3UFileInfoStates = [];


        private bool _isParsed = skipParse;


        public async Task StartDownload(Action<int> StateAction,IDialogProgress dialogProgress, CancellationToken cancellationToken)
        {
            StateAction.Invoke((int)DownloadStatus.Parsed);
            await GetM3U8FileInfo(cancellationToken);

            using var acquire = dialogProgress.Acquire();
            await DownloadAsync(dialogProgress, cancellationToken);

            await GenerateM3u8(cancellationToken);

            await MergeAsync(dialogProgress, cancellationToken);

            IMergeSetting mergeSetting = (IMergeSetting)context.DownloaderSetting;
            if (mergeSetting.IsCleanUp)
                DirectoryEx.DeleteCache(context.DownloadParam.CachePath);
        }


        private async Task GetM3U8FileInfo(CancellationToken cancellationToken)
        {
            if (_isParsed)
                return;

            context.Log.Info("开始获取m3u8数据,请等待..");
            IM3u8DownloadParam m3u8DownloadParam = (IM3u8DownloadParam)context.DownloadParam;
            if (m3u8DownloadParam.RequestUrl.IsFile)
            {
                var m3UFileInfo = await m3U8FileInfoClient.M3u8FileReader.GetM3u8FileInfo(m3u8DownloadParam.RequestUrl);
                m3UFileInfoStates.Add(new M3uFileInfoState(new M3uFileInfoSource(m3u8DownloadParam.RequestUrl, m3UFileInfo)));
            }
            else
            {
                if (m3U8DownloadParams.M3UFileInfoSources is null)
                {
                    var reader = m3U8FileInfoClient.M3UFileReadManager;
                    var m3UFileInfo = await reader.GetM3u8FileInfo(cancellationToken);
                    m3U8DownloadParams.M3UFileInfoSources = reader.AutoHandleM3uFileInfo(m3UFileInfo)
                                                ?? await m3UFileInfoSourcesFactory(m3UFileInfo);
                }

                foreach (var item in m3U8DownloadParams.M3UFileInfoSources)
                {
                    if(item.M3uFile is null)
                    {
                        var m3UFileState = new M3uFileInfoState(item);
                        m3UFileState.M3uReader ??= m3U8FileInfoClient.M3UFileReadManager;
                        item.M3uFile = await m3UFileState.M3uReader.GetM3u8FileInfo(item.RequestUrl, cancellationToken);
                        if (item.M3uFile is null || item.M3uFile.IsEmpty)
                        {
                            throw new InvalidDataException("获取的m3u8数据为空");
                        }

                        context.Log.Info("获取到{0},有 {1} 个", m3UFileState, item.M3uFile.MediaFiles.Count);
                        m3UFileInfoStates.Add(m3UFileState);
                    }
                }
            }

            if (m3UFileInfoStates.Count == 1)
            {
                var m3uinfoState = m3UFileInfoStates.Single();
                if (m3uinfoState.M3uFileInfoIsEmpty)
                { 
                    M3UFileInfo m3UFileInfo = (M3UFileInfo)m3uinfoState.M3UFileInfoSource.M3uFile!;
                    m3UFileInfo.Key = m3u8DownloadParam.M3UKeyInfo!;
                }
            }

            _isParsed = true;
        }


        private async Task DownloadAsync(IDialogProgress downloadProgress, CancellationToken cancellationToken)
        {
            m3UDownloaderClient.DialogProgress = downloadProgress;

            foreach (var stateItem in m3UFileInfoStates)
            {
                if (stateItem.IsDownloaded)
                    continue;

                context.Log.Info("开始下载{0}数据", stateItem);
                stateItem.DownloadService ??= m3UDownloaderClient.CreateM3u8Downloader(stateItem.M3UFileInfoSource);
                await stateItem.DownloadService.Initialization(stateItem.M3UFileInfoSource, cancellationToken);
                await stateItem.DownloadService.StartDownload(stateItem.M3UFileInfoSource, cancellationToken);
                stateItem.IsDownloaded = true;
            }
        }

        private async Task GenerateM3u8(CancellationToken cancellationToken)
        {
            var m3u8FileInfoStates = m3UFileInfoStates.Where(m => m.M3UFileInfoSource.Type != M3uType.SUBTITLE)
                                    .ToList() ?? throw new InvalidOperationException("获取M3UFileInfoSource list失败");

            foreach (var stateItem in m3u8FileInfoStates)
            {
                var filePath = Path.Combine(m3U8DownloadParams.CachePath,stateItem.M3UFileInfoSource.CachePath, "index.m3u8");
                if (File.Exists(filePath))
                    continue;

                var cachePath = Path.Combine(m3U8DownloadParams.CachePath, stateItem.M3UFileInfoSource.CachePath);
                await stateItem.M3UFileInfoSource.M3uFile!.GenerateM3U8StreamToFile(filePath, cachePath, cancellationToken);
            }
        }

        private async Task MergeAsync(IDialogProgress progress, CancellationToken cancellationToken)
        {
            var m3UFileInfos = m3UFileInfoStates.Where(m => m.M3UFileInfoSource.Type != M3uType.SUBTITLE)
                                                .Select(m => m.M3UFileInfoSource)
                                                .ToList() ?? throw new InvalidOperationException("获取M3UFileInfoSource list失败");

            await m3UCombinerClient.FFmpeg.ConvertToMp4(m3UFileInfos, progress, cancellationToken);

            var subtitle = m3UFileInfoStates.FirstOrDefault(m => m.M3UFileInfoSource.Type == M3uType.SUBTITLE);
            if(subtitle is not null && subtitle.M3UFileInfoSource.M3uFile is not null)
            {
                var filePath = Path.Combine(m3U8DownloadParams.SavePath, m3U8DownloadParams.VideoName + subtitle.M3UFileInfoSource.Extension);
                if (File.Exists(filePath))
                    return;

                await using var file = File.Create(filePath);
                foreach (var item in subtitle.M3UFileInfoSource.M3uFile.MediaFiles)
                {
                    var cachePath = Path.Combine(m3U8DownloadParams.CachePath, subtitle.M3UFileInfoSource.CachePath, item.Title);
                    await using var reader =  File.OpenRead(cachePath);
                    await reader.CopyToAsync(file, cancellationToken);
                }
            }
        }
    }


    public partial class M3u8Downloader
    {
        //通过软件界面创建
        public static M3u8Downloader CreateM3u8Downloader(
              Func<IM3uFileInfo, Task<IList<IM3uFileInfoSource>>> m3UFileInfoSourcesFactory,
              IDownloadContext context,
              IDownloadPlugin? downloadPlugin
            )
        {
            M3u8Downloader m3U8Downloader = new(context, m3UFileInfoSourcesFactory, false)
            {
                m3U8FileInfoClient = new M3u8FileInfoClient(context, downloadPlugin)
            };
            m3U8Downloader.m3UDownloaderClient = new DownloaderClient(context, downloadPlugin)
            {
                GetLiveFileInfoFunc = m3U8Downloader.m3U8FileInfoClient.M3UFileReadManager.GetM3u8FileInfo
            };

            m3U8Downloader.m3UCombinerClient = new M3uCombinerClient(context.Log, context.DownloadParam, (IMergeSetting)context.DownloaderSetting);
            return m3U8Downloader;
        }

        //通过接口创建
        public static M3u8Downloader CreateM3u8Downloader(
            IDownloadContext context,
            IDownloadPlugin? downloadPlugin,
            IM3uFileInfo m3UFileInfo
            )
        {
            M3u8Downloader m3U8Downloader = new(context,null!,true)
            {
                m3UDownloaderClient = new DownloaderClient(context, downloadPlugin),
                m3UCombinerClient = new M3uCombinerClient(context.Log, context.DownloadParam, (IMergeSetting)context.DownloaderSetting),
            };

            M3u8DownloadParams m3U8DownloadParams = (M3u8DownloadParams)context.DownloadParam;
            m3U8Downloader.m3UFileInfoStates.Add(new M3uFileInfoState(new M3uFileInfoSource(m3U8DownloadParams.RequestUrl, m3UFileInfo)));
            context.Log.Info("通过接口传入m3u8文件的视频流有{0}个,将跳过获取操作开始直接下载", m3UFileInfo.MediaFiles.Count);
            return m3U8Downloader;
        }
    }
}
