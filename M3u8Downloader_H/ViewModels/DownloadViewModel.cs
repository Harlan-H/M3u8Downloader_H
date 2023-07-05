using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Caliburn.Micro;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Core.DownloaderManagers;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.ViewModels
{
    public partial class DownloadViewModel : PropertyChangedBase
    {
        private readonly DownloadService downloadService;
        private readonly SoundService soundService;
        private CancellationTokenSource? cancellationTokenSource;
        public IDownloadManager _downloadManager = default!;

        public Uri RequestUrl { get; set; } = default!;

        public string VideoName { get; set; } = default!;

        public double ProgressNum { get; set; }

        public double RecordDuration { get; set; }

        public long DownloadRateBytes { get; set; } = -1;

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
                    await downloadService.GetM3u8FileInfo(_downloadManager, cancellationTokenSource.Token);

                    Status = DownloadStatus.Enqueued;
                    using DownloadRateSource downloadRate = new(s => DownloadRateBytes = s);
                    await downloadService.DownloadAsync(_downloadManager.Build(), downloadRate, cancellationTokenSource.Token);

                    soundService.PlaySuccess();
                    Status = DownloadStatus.Completed;
                }
                catch (OperationCanceledException) when (cancellationTokenSource!.IsCancellationRequested)
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

    public partial class DownloadViewModel
    {
        public static DownloadViewModel CreateDownloadViewModel(
            Uri requesturl,
            string videoname,
            string? method,
            string? key,
            string? iv,
            IEnumerable<KeyValuePair<string, string>>? headers,
            string cachePath,
            IPluginBuilder? pluginBuilder)
        {
            DownloadViewModel viewModel = IoC.Get<DownloadViewModel>();
            viewModel.RequestUrl = requesturl;
            viewModel.VideoName = videoname;

            IDownloadManager downloadManager = new DownloadManager(Http.Client, requesturl, headers, cachePath, pluginBuilder)
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
            Uri? requesturl,
            string content,
            IEnumerable<KeyValuePair<string, string>>? headers,
            string cachePath,
            string videoname,
            IPluginBuilder? pluginBuilder)
        {
            DownloadViewModel viewModel = IoC.Get<DownloadViewModel>();
            viewModel.RequestUrl = requesturl!;
            viewModel.VideoName = videoname;

            viewModel._downloadManager = new DownloadManager(Http.Client, requesturl!, headers, cachePath, pluginBuilder)
                                                  .WithLiveProgress(new Progress<double>(d => viewModel.RecordDuration = d))
                                                  .WithVodProgress(new Progress<double>(d => viewModel.ProgressNum = d))
                                                  .WithStatusAction(s => viewModel.Status = (DownloadStatus)s)
                                                  .WithM3u8Content(content);

            return viewModel;
        }


        //当传入的是M3UFileInfo 此时因为他不是文件或者http地址 没有办法判断具体的缓存目录
        public static DownloadViewModel CreateDownloadViewModel(
            M3UFileInfo m3UFileInfo,
            IEnumerable<KeyValuePair<string, string>>? headers,
            string videoname,
            string videoPath,
            IPluginBuilder? pluginBuilder)
        {
            DownloadViewModel viewModel = IoC.Get<DownloadViewModel>();
            viewModel.VideoName = videoname;

            viewModel._downloadManager = new DownloadManager(Http.Client, default!, headers, videoPath, pluginBuilder)
                                      .WithLiveProgress(new Progress<double>(d => viewModel.RecordDuration = d))
                                      .WithVodProgress(new Progress<double>(d => viewModel.ProgressNum = d))
                                      .WithStatusAction(s => viewModel.Status = (DownloadStatus)s)
                                      .WithM3u8FileInfo(m3UFileInfo);

            return viewModel;
        }
    }

}
