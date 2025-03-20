﻿using Caliburn.Micro;
using M3u8Downloader_H.Common.M3u8Infos;
using System;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Downloader;
using M3u8Downloader_H.M3U8;
using M3u8Downloader_H.Plugin.PluginManagers;
using M3u8Downloader_H.Utils;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Combiners;

namespace M3u8Downloader_H.ViewModels
{
    public partial class M3u8DownloadViewModel(SettingsService settingsService, SoundService soundService) : DownloadViewModel(settingsService, soundService)
    {
        private readonly SettingsService settingsService = settingsService;
        private M3u8FileInfoClient m3U8FileInfoClient = default!;
        private DownloaderClient m3UDownloaderClient = default!;
        private M3uCombinerClient m3UCombinerClient = default!;
        private M3UFileInfo M3U8FileInfo = default!;
        private M3UKeyInfo? M3UKeyInfo;

        private bool _isParsed = false;
        private bool _isDownloaded = false;

        protected override async Task StartDownload(CancellationToken cancellationToken)
        {
            Status = DownloadStatus.Parsed;
            await GetM3U8FileInfo(cancellationToken);

            Status = DownloadStatus.Enqueued;
            downloadProgress ??= new(this);
            using var acquire = downloadProgress.Acquire();
            await DownloadAsync(downloadProgress,cancellationToken);

            await MergeAsync(downloadProgress, cancellationToken);

            DeleteCache(settingsService.IsCleanUp,true);
        }


        private async Task GetM3U8FileInfo(CancellationToken cancellationToken)
        {
            if (_isParsed)
                return;

            if (M3U8FileInfo is not null)
            {
                Log.Info("获取视频流{0}个", M3U8FileInfo.MediaFiles.Count);
                DownloadParam.SetVideoFullName($"{DownloadParam.VideoName}.{settingsService.SelectedFormat}");
                _isParsed = true;
                return;
            }

            m3U8FileInfoClient.DownloaderSetting = settingsService;
            if (RequestUrl.IsFile)
            {
                string ext = Path.GetExtension(RequestUrl.OriginalString).Trim('.');
                M3U8FileInfo = m3U8FileInfoClient.M3UFileReadManager.GetM3u8FileInfo(ext, RequestUrl);
            }
            else
            {
                M3U8FileInfo = await m3U8FileInfoClient.M3UFileReadManager.GetM3u8FileInfo(cancellationToken);
            }
            Log.Info("获取视频流{0}个", M3U8FileInfo.MediaFiles.Count);

            if (M3UKeyInfo is not null)
                M3U8FileInfo.Key = M3UKeyInfo;

            DownloadParam.SetVideoFullName($"{DownloadParam.VideoName}.{settingsService.SelectedFormat}");
            _isParsed = true;
        }


        private async Task DownloadAsync(IDialogProgress downloadProgress,CancellationToken cancellationToken)
        {
            if (_isDownloaded)
                return;

            m3UDownloaderClient.DownloaderSetting = settingsService;
            m3UDownloaderClient.M3UFileInfo = M3U8FileInfo;
            m3UDownloaderClient.DialogProgress = downloadProgress;

            //当使用m3u8数据接口传入时 m3U8FileInfoClient是不创建的 所以需要加判断
            if (m3U8FileInfoClient is not null)
                m3UDownloaderClient.GetLiveFileInfoFunc = m3U8FileInfoClient.M3UFileReadManager.GetM3u8FileInfo;

            await m3UDownloaderClient.M3u8Downloader.DownloadMapInfoAsync(M3U8FileInfo.Map, cancellationToken);
            await m3UDownloaderClient.M3u8Downloader.DownloadAsync(M3U8FileInfo, cancellationToken);
            _isDownloaded = true;
        }

        private async Task MergeAsync(IDialogProgress progress,CancellationToken cancellationToken)
        {
            m3UCombinerClient.Settings = settingsService;
            m3UCombinerClient.DialogProgress = progress;

            if(M3U8FileInfo.Map is not null)
            {
                m3UCombinerClient.M3u8FileMerger.Initialize(M3U8FileInfo);
                await m3UCombinerClient.M3u8FileMerger.MegerVideoHeader(M3U8FileInfo.Map, cancellationToken);
                await m3UCombinerClient.M3u8FileMerger.StartMerging(M3U8FileInfo, cancellationToken);
            }else
                await m3UCombinerClient.FFmpeg.ConvertToMp4(M3U8FileInfo, cancellationToken);
        }

    }

    public partial class M3u8DownloadViewModel
    {
        public static DownloadViewModel CreateDownloadViewModel(
            M3u8DownloadParams m3U8DownloadParam,
            Type? pluginType)
        {
            M3u8DownloadViewModel viewModel = IoC.Get<M3u8DownloadViewModel>();
            viewModel.DownloadParam = m3U8DownloadParam;
            viewModel.RequestUrl = m3U8DownloadParam.RequestUrl;
            viewModel.VideoName = m3U8DownloadParam.VideoName;

            PluginManger? pluginManger = PluginManger.CreatePluginMangaer(pluginType, Http.Client, viewModel.Log);
            viewModel.m3U8FileInfoClient = new M3u8FileInfoClient(Http.Client, pluginManger, viewModel.Log, m3U8DownloadParam);
            viewModel.m3UDownloaderClient = new DownloaderClient(Http.Client, pluginManger, viewModel.Log, m3U8DownloadParam);
            viewModel.m3UCombinerClient = new M3uCombinerClient(viewModel.Log, m3U8DownloadParam);

            viewModel.M3UKeyInfo = m3U8DownloadParam.GetM3uKeyInfo();
            
            return viewModel;
        }


        //当使用接口传入m3u8数据时一定不是直播所以不需要创建M3u8FileInfoClient
        public static DownloadViewModel CreateDownloadViewModel(
            M3UFileInfo m3UFileInfo,
            M3u8DownloadParams m3U8DownloadParam,
            Type? pluginType)
        {
            M3u8DownloadViewModel viewModel = IoC.Get<M3u8DownloadViewModel>();
            viewModel.DownloadParam = m3U8DownloadParam;
            viewModel.M3U8FileInfo = m3UFileInfo;
            viewModel.VideoName = m3U8DownloadParam.VideoName;

            PluginManger? pluginManger = PluginManger.CreatePluginMangaer(pluginType, Http.Client, viewModel.Log);
            viewModel.m3UDownloaderClient = new DownloaderClient(Http.Client, pluginManger, viewModel.Log, m3U8DownloadParam);
            viewModel.m3UCombinerClient = new M3uCombinerClient(viewModel.Log, m3U8DownloadParam);
            return viewModel;
        }
    }
}
