using Stylet;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.M3U8.Infos;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels.FrameWork;
using System.Collections.Generic;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Core.DownloaderManagers;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.Core.DownloaderPluginManagers;
using M3u8Downloader_H.Extensions;

namespace M3u8Downloader_H.ViewModels
{
    public class DownloadViewModel : PropertyChangedBase
    {
        private readonly DownloadService downloadService;
        private readonly SoundService soundService;
        private CancellationTokenSource? cancellationTokenSource;
        public IDownloadManager _downloadManager = default!;

        public Uri RequestUrl { get; set; } = default!;

        public string VideoName { get; set; } = default!;

        public double ProgressNum { get; set; }

        public double RecordDuration { get; set; }

        public bool IsActive { get; private set; }

        public DownloadStatus Status { get; set; }

        public bool IsProgressIndeterminate => IsActive && Status < DownloadStatus.StartedVod;

        public string? FailReason { get; private set; } = string.Empty;

        public DownloadViewModel(DownloadService downloadService, SoundService soundService)
        {
            this.downloadService = downloadService;
            this.soundService = soundService;
        }

        public bool CanOnStart => !IsActive;

        public void OnStart()
        {
            if (!CanOnStart)
                return;

            IsActive = true;

            _ = Task.Run(async () =>
            {
                try
                {
                    cancellationTokenSource = new CancellationTokenSource();

                    Status = DownloadStatus.Parsed;
                    await _downloadManager.GetM3U8FileInfo(cancellationTokenSource.Token);

                    Status = DownloadStatus.Enqueued;
                    await downloadService.DownloadAsync(_downloadManager.Build(), cancellationTokenSource.Token);

                    soundService.PlaySuccess();
                    Status = DownloadStatus.Completed;
                }
                catch (OperationCanceledException)
                {
                    Status = DownloadStatus.Canceled;
                }
                catch (Exception e)
                {
                    soundService.PlayError();
                    Status = DownloadStatus.Failed;
                    FailReason = e.ToString();
                }
                finally
                {
                    IsActive = false;
                    cancellationTokenSource?.Dispose();
                }

            });
        }

        public bool CanOnShowFile => Status == DownloadStatus.Completed;
        public void OnShowFile()
        {
            if (!CanOnShowFile)
                return;

            try
            {
                Process.Start("explorer", $"/select, \"{_downloadManager.VideoFullName}\"");
            }
            catch (Exception)
            {
            }
        }


        public bool CanOnCancel => IsActive && Status != DownloadStatus.Canceled;
        public void OnCancel()
        {
            if (!CanOnCancel)
                return;

            cancellationTokenSource?.Cancel();
        }

        public bool CanOnRestart => CanOnStart && Status != DownloadStatus.Completed;
        public void OnRestart() => OnStart();


        public void DeleteCache()
        {
            DirectoryInfo directory = new(_downloadManager.VideoFullPath);
            if (directory.Exists)
                directory.Delete(true);
        }

    }

    public static class DownloadViewModelExtensions
    {
        public static DownloadViewModel CreateDownloadViewModel(
            this IVIewModelFactory factory,
            Uri requesturl,
            string videoname,
            string? method,
            string? key,
            string? iv,
            IEnumerable<KeyValuePair<string, string>>? headers,
            string cachePath,
            string? pluginKey = default!)
        {
            DownloadViewModel viewModel = factory.CreateDownloadViewModel();
            viewModel.RequestUrl = requesturl;
            viewModel.VideoName = videoname;

            PluginManger? pluginManger = default!;

            SettingsService settingService = Container.Ioc.Get<SettingsService>();
            string tmpPluginKey = pluginKey is not null
                                ? pluginKey
                                : string.IsNullOrWhiteSpace(settingService.PluginKey)
                                ? requesturl.GetHostName()
                                : settingService.PluginKey;

            IPluginBuilder? pluginBuilder = Container.Ioc.Get<PluginService>()[tmpPluginKey];
            if(pluginBuilder is not null)
            {
                pluginManger = new PluginManger(pluginBuilder);
                pluginManger.Build();
            }
            IDownloadManager downloadManager = new DownloadManager(Http.Client, requesturl, headers, cachePath, pluginManger)
                                                  .WithLiveProgress(new Progress<double>(d => viewModel.RecordDuration = d))
                                                  .WithVodProgress(new Progress<double>(d => viewModel.ProgressNum = d))
                                                  .WithStatusAction(s => viewModel.Status = (DownloadStatus)s);

            if (!string.IsNullOrWhiteSpace(key))
            {
                downloadManager.WithKeyInfo(method!, key, iv!);
            }

            viewModel._downloadManager = downloadManager;
            return viewModel;
        }


        public static DownloadViewModel CreateDownloadViewModel(
            this IVIewModelFactory factory,
            Uri? requesturl,
            string content,
            IEnumerable<KeyValuePair<string, string>>? headers,
            string cachePath,
            string videoname,
            string? pluginKey)
        {
            DownloadViewModel viewModel = factory.CreateDownloadViewModel();
            viewModel.RequestUrl = requesturl!;
            viewModel.VideoName = videoname;

            SettingsService settingService = Container.Ioc.Get<SettingsService>();
            string? tmpPluginKey = pluginKey is not null
                                 ? pluginKey
                                 : string.IsNullOrWhiteSpace(settingService.PluginKey)
                                 ? requesturl?.GetHostName() 
                                 : settingService.PluginKey;

            PluginManger? pluginManger = default!;
            if (!string.IsNullOrWhiteSpace(tmpPluginKey))
            {
                IPluginBuilder? pluginBuilder = Container.Ioc.Get<PluginService>()[tmpPluginKey];
                if (pluginBuilder is not null)
                {
                    pluginManger = new PluginManger(pluginBuilder);
                    pluginManger.Build();
                }
            }
            viewModel._downloadManager = new DownloadManager(Http.Client, requesturl!, headers, cachePath, pluginManger)
                                                  .WithLiveProgress(new Progress<double>(d => viewModel.RecordDuration = d))
                                                  .WithVodProgress(new Progress<double>(d => viewModel.ProgressNum = d))
                                                  .WithStatusAction(s => viewModel.Status = (DownloadStatus)s)
                                                  .WithM3u8Content(content);

            return viewModel;
        }


        //当传入的是M3UFileInfo 此时因为他不是文件或者http地址 没有办法判断具体的缓存目录
        public static DownloadViewModel CreateDownloadViewModel(
            this IVIewModelFactory factory,
            M3UFileInfo m3UFileInfo,
            IEnumerable<KeyValuePair<string, string>>? headers,
            string videoname,
            string videoPath,
            string? pluginKey)
        {
            DownloadViewModel viewModel = factory.CreateDownloadViewModel();
            viewModel.VideoName = videoname;

            //这里因为不可能有url所以直接通过设置来判别使用某个插件
            PluginManger? pluginManger = default!;
            SettingsService settingService = Container.Ioc.Get<SettingsService>();

            string? tmpPluginKey = pluginKey ?? settingService.PluginKey;
            if (!string.IsNullOrWhiteSpace(tmpPluginKey))
            {
                IPluginBuilder? pluginBuilder = Container.Ioc.Get<PluginService>()[tmpPluginKey];
                if (pluginBuilder is not null)
                {
                    pluginManger = new PluginManger(pluginBuilder);
                    pluginManger.Build();
                }
            }
            viewModel._downloadManager = new DownloadManager(Http.Client, default!, headers, videoPath, pluginManger)
                                      .WithLiveProgress(new Progress<double>(d => viewModel.RecordDuration = d))
                                      .WithVodProgress(new Progress<double>(d => viewModel.ProgressNum = d))
                                      .WithStatusAction(s => viewModel.Status = (DownloadStatus)s)
                                      .WithM3u8FileInfo(m3UFileInfo);

            return viewModel;
        }
    }

}
