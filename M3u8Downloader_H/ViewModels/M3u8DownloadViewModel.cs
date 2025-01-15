using Caliburn.Micro;
using M3u8Downloader_H.Common.M3u8Infos;
using System.Collections.Generic;
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
        private M3uDownloaderClient m3UDownloaderClient = default!;
        private M3uCombinerClient m3UCombinerClient = default!;
        private M3UFileInfo M3U8FileInfo = default!;
        private M3UKeyInfo M3UKeyInfo = default!;
        private bool _theFirstTime = true;

        protected override async Task StartDownload(CancellationToken cancellationToken)
        {
            Status = DownloadStatus.Parsed;
            await GetM3U8FileInfo(cancellationToken);

            Status = DownloadStatus.Enqueued;
            using DownloadProgress downloadProgress = new(this);
            using var acquire = downloadProgress.Acquire();
            await DownloadAsync(downloadProgress,cancellationToken);

            await MergeAsync(downloadProgress, cancellationToken);
        }


        private async Task GetM3U8FileInfo(CancellationToken cancellationToken)
        {
            m3U8FileInfoClient.DownloaderSetting = settingsService;
            if (!_theFirstTime)
                return;

            if (M3U8FileInfo is not null)
            {
                Info("获取视频流{0}个", M3U8FileInfo.MediaFiles.Count);
                VideoFullName = M3U8FileInfo.Map is not null ? Path.GetExtension(M3U8FileInfo.Map?.Title!) : ".ts";
                _theFirstTime = false;
                return;
            }

            if (RequestUrl.IsFile)
            {
                string ext = Path.GetExtension(RequestUrl.OriginalString).Trim('.');
                M3U8FileInfo = m3U8FileInfoClient.M3UFileReadManager.GetM3u8FileInfo(ext, RequestUrl);
            }
            else
            {
                M3U8FileInfo = await m3U8FileInfoClient.M3UFileReadManager.GetM3u8FileInfo(cancellationToken);
            }
            Info("获取视频流{0}个", M3U8FileInfo.MediaFiles.Count);

            if (M3UKeyInfo is not null)
                M3U8FileInfo.Key = M3UKeyInfo;

            VideoFullName = M3U8FileInfo.Map is not null ? Path.GetExtension(M3U8FileInfo.Map?.Title!) : ".ts";
            _theFirstTime = false;
        }


        private async Task DownloadAsync(IDialogProgress downloadProgress,CancellationToken cancellationToken)
        {
            m3UDownloaderClient.DownloaderSetting = settingsService;
            m3UDownloaderClient.M3UFileInfo = M3U8FileInfo;
            m3UDownloaderClient.DialogProgress = downloadProgress;

            await m3UDownloaderClient.M3u8Downloader.DownloadMapInfoAsync(M3U8FileInfo.Map, cancellationToken);
            await m3UDownloaderClient.M3u8Downloader.DownloadAsync(M3U8FileInfo, cancellationToken);
        }

        private async Task MergeAsync(IDialogProgress progress,CancellationToken cancellationToken)
        {
            m3UCombinerClient.M3UFileInfo = M3U8FileInfo;
            m3UCombinerClient.Settings = settingsService;
            m3UCombinerClient.DialogProgress = progress;

            await m3UCombinerClient.Converter(false, cancellationToken);
        }
    }

    public partial class M3u8DownloadViewModel
    {
        public static DownloadViewModel CreateDownloadViewModel(
            Uri requesturl,
            string videoname,
            string? method,
            string? key,
            string? iv,
            IDictionary<string, string>? headers,
            string cachePath,
            Type? pluginType)
        {
            M3u8DownloadViewModel viewModel = IoC.Get<M3u8DownloadViewModel>();
            viewModel.RequestUrl = requesturl;
            viewModel.Headers = headers;
            viewModel.VideoName = videoname;
            viewModel.SavePath = cachePath;

            PluginManger? pluginManger = PluginManger.CreatePluginMangaer(pluginType, Http.Client, viewModel);
            viewModel.m3U8FileInfoClient = new M3u8FileInfoClient(Http.Client, pluginManger, viewModel);
            viewModel.m3UDownloaderClient = new M3uDownloaderClient(Http.Client, pluginManger, viewModel);
            viewModel.m3UCombinerClient = new M3uCombinerClient(viewModel);

            if (!string.IsNullOrWhiteSpace(key))
            {
                viewModel.M3UKeyInfo = new M3UKeyInfo(method!, key, iv!);
            }

            return viewModel;
        }


        //当传入的是M3UFileInfo 此时因为他不是文件或者http地址 没有办法判断具体的缓存目录
        public static DownloadViewModel CreateDownloadViewModel(
            M3UFileInfo m3UFileInfo,
            IDictionary<string, string>? headers,
            string videoname,
            string cachePath,
            Type? pluginType)
        {
            M3u8DownloadViewModel viewModel = IoC.Get<M3u8DownloadViewModel>();
            viewModel.M3U8FileInfo = m3UFileInfo;
            viewModel.Headers = headers;
            viewModel.VideoName = videoname;
            viewModel.SavePath = cachePath;

            PluginManger? pluginManger = PluginManger.CreatePluginMangaer(pluginType, Http.Client, viewModel);
            viewModel.m3U8FileInfoClient = new M3u8FileInfoClient(Http.Client, pluginManger, viewModel);
            viewModel.m3UDownloaderClient = new M3uDownloaderClient(Http.Client, pluginManger, viewModel);
            viewModel.m3UCombinerClient = new M3uCombinerClient(viewModel);
            return viewModel;
        }
    }
}
