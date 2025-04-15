using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.Models;
using System.Timers;
using M3u8Downloader_H.Abstractions.Common;
using Caliburn.Micro;
using M3u8Downloader_H.Common.Models;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Core;

namespace M3u8Downloader_H.ViewModels
{
    public  partial class DownloadViewModel(SettingsService settingsService, SoundService soundService) : PropertyChangedBase
    {
        private readonly ThrottlingSemaphore throttlingSemaphore = ThrottlingSemaphore.Instance;
        private readonly SettingsService settingsService = settingsService;
        private CancellationTokenSource? cancellationTokenSource;
        private DownloaderCoreClient downloaderCoreClient = default!;
        protected IDownloadParamBase DownloadParam = default!;
        protected DownloadProgress? downloadProgress;


        public MyLog Log { get; } = new();
        public Uri RequestUrl { get; set; } = default!;

        public string VideoName { get; set; } = default!;

        public double ProgressNum { get; set; }

        public long DownloadRateBytes { get; set; } = -1;

        public bool IsActive { get; private set; }

        public DownloadStatus Status { get; set; }

        public bool IsProgressIndeterminate => IsActive && Status < DownloadStatus.StartedVod;

        public string? FailReason { get; private set; } = string.Empty;

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
                    Status = DownloadStatus.Enqueued;
                    cancellationTokenSource = new CancellationTokenSource();
                    using var semaphore = await throttlingSemaphore.AcquireAsync(cancellationTokenSource.Token);

                    downloadProgress ??= new(this);
                    await downloaderCoreClient.Downloader.StartDownload(s => Status = (DownloadStatus)s, downloadProgress, cancellationTokenSource.Token);
                    soundService.PlaySuccess(settingsService.IsPlaySound);
                    Status = DownloadStatus.Completed;
                }
                catch (OperationCanceledException) when (cancellationTokenSource!.IsCancellationRequested)
                {
                    Status = DownloadStatus.Canceled;
                    Log.Info("已经停止下载");
                }
                catch (Exception e)
                {
                    soundService.PlayError(settingsService.IsPlaySound);
                    Status = DownloadStatus.Failed;
                    FailReason = e.ToString();
                    Log.Error(e);
                }
                finally
                {
                    IsActive = false;
                    downloadProgress?.Clear();
                    cancellationTokenSource?.Dispose();
                }

            });
        }

        public bool CanOnShowFile => Status == DownloadStatus.Completed;
        public virtual void OnShowFile()
        {
            if (!CanOnShowFile)
                return;

            try
            {
                Process.Start("explorer", $"/select, \"{DownloadParam.VideoFullName}\"");
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

        public void DeleteCache(bool isDelete = true,bool isShowLog = false)
        {
            if (!isDelete)
                return;

            DirectoryInfo directory = new(DownloadParam.CachePath);
            if (!directory.Exists) return ;
                
            directory.Delete(true);

            if(isShowLog)
                Log.Info("删除{0}目录成功", directory);
        }

    }

    public partial class DownloadViewModel : IEquatable<DownloadViewModel>
    {
        
        public bool Equals(DownloadViewModel? other) => GetHashCode() == other?.GetHashCode();
        public override bool Equals(object? obj) =>  obj is DownloadViewModel  downloadviewmodel && Equals(downloadviewmodel);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(DownloadParam.CachePath);
        public static bool operator ==(DownloadViewModel downloadviewmode, DownloadViewModel downloadviewmode1) => downloadviewmode.Equals(downloadviewmode1);
        public static bool operator !=(DownloadViewModel downloadviewmode, DownloadViewModel downloadviewmode1) => !(downloadviewmode == downloadviewmode1);
    }


    public partial class DownloadViewModel
    {
        protected class DownloadProgress(DownloadViewModel downloadViewModel) : IDialogProgress
        {
            private readonly TimerContainer timerContainer = TimerContainer.Instance;
            private long _lastBytes;
            private long _countBytes;
            private double _currentProgress;
            private bool _isIncProgressNum = false;

            public void Clear()
            {
                _countBytes = 0;
                _lastBytes = 0;
                _currentProgress = 0;
                downloadViewModel.DownloadRateBytes = -1;
            }

            private void OnTimedEvent(Object? source, ElapsedEventArgs e)
            {
                long tmpBytes = _countBytes;
                downloadViewModel.DownloadRateBytes = tmpBytes - _lastBytes;
                _lastBytes = tmpBytes;

                if (_isIncProgressNum)
                    downloadViewModel.ProgressNum = _currentProgress++;
                else
                    downloadViewModel.ProgressNum = _currentProgress;
            }

            public IDisposable Acquire() => timerContainer.AddTimerCallback(OnTimedEvent);

            public void Report(long value)
            {
                Interlocked.Add(ref _countBytes, value);
            }

            public void Report(double value)
            {
                _currentProgress = value;
            }

            public void SetDownloadStatus(bool IsLiveDownloading)
            {
                downloadViewModel.Status = IsLiveDownloading ? DownloadStatus.StartedLive : DownloadStatus.StartedVod;
            }

            public void IncProgressNum(bool isInc)
            {
                _isIncProgressNum = true;
                _currentProgress = 0;
            }
        }
    }

    public partial class DownloadViewModel
    {
        public static DownloadViewModel CreateDownloadViewModel(
            IM3u8DownloadParam m3U8DownloadParam,
            Type? pluginType)
        {
            DownloadViewModel viewModel = IoC.Get<DownloadViewModel>();
            viewModel.DownloadParam = m3U8DownloadParam;
            viewModel.RequestUrl = m3U8DownloadParam.RequestUrl;
            viewModel.VideoName = m3U8DownloadParam.VideoName;

            viewModel.downloaderCoreClient = new(Http.Client, m3U8DownloadParam, viewModel.settingsService.Clone(), viewModel.Log, pluginType); 
            return viewModel;
        }

        public static DownloadViewModel CreateDownloadViewModel(
            IM3uFileInfo m3UFileInfo,
            IDownloadParamBase m3U8DownloadParam,
            Type? pluginType)
        {
            DownloadViewModel viewModel = IoC.Get<DownloadViewModel>();
            viewModel.DownloadParam = m3U8DownloadParam;
            viewModel.VideoName = m3U8DownloadParam.VideoName;

            viewModel.downloaderCoreClient = new(Http.Client, m3U8DownloadParam, viewModel.settingsService.Clone(), viewModel.Log, pluginType, m3UFileInfo);
            return viewModel;
        }

        public static DownloadViewModel CreateDownloadViewModel(
             IMediaDownloadParam m3U8DownloadParam)
        {
            DownloadViewModel viewModel = IoC.Get<DownloadViewModel>();
            viewModel.DownloadParam = m3U8DownloadParam;
            viewModel.RequestUrl = m3U8DownloadParam.Medias[0].Url;
            viewModel.VideoName = m3U8DownloadParam.VideoName;

            viewModel.downloaderCoreClient = new DownloaderCoreClient(Http.Client, m3U8DownloadParam, viewModel.settingsService.Clone(), viewModel.Log);
            return viewModel;
        }
    }

}
