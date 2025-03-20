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
using M3u8Downloader_H.Abstractions.Extensions;
using Caliburn.Micro;
using M3u8Downloader_H.ViewModels.Utils;

namespace M3u8Downloader_H.ViewModels
{
    public abstract  partial  class DownloadViewModel(SettingsService settingsService, SoundService soundService) : PropertyChangedBase
    {
        private readonly ThrottlingSemaphore throttlingSemaphore = ThrottlingSemaphore.Instance;
        private CancellationTokenSource? cancellationTokenSource;
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

        protected abstract Task StartDownload(CancellationToken cancellationToken);

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

                    await StartDownload(cancellationTokenSource.Token);
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
                Process.Start("explorer", $"/select, \"{DownloadParam.GetVideoFullPath()}\"");
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

            DirectoryInfo directory = new(DownloadParam.GetCachePath());
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
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(DownloadParam.GetCachePath());
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
        }
    }

   

}
