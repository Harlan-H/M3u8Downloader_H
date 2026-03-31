using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.Models;
using M3u8Downloader_H.Core;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace M3u8Downloader_H.ViewModels.Downloads
{
    public partial class DownloadViewModel(IDownloadParamBase DownloadParam) : ViewModelBase
    {
        private readonly ThrottlingSemaphore throttlingSemaphore = ThrottlingSemaphore.Instance;
        private CancellationTokenSource? cancellationTokenSource;
        protected DownloadProgress? downloadProgress;

        public DownloaderCoreClient downloaderCoreClient = default!;

        [ObservableProperty]
        public partial MyLog Log { get; set; } = new();

        [ObservableProperty]
        public partial Uri RequestUrl { get; set; } 
        [ObservableProperty]
        public partial string VideoName { get; set; }
        [ObservableProperty]
        public partial double ProgressNum { get; set; }
        [ObservableProperty]
        public partial long DownloadRateBytes { get; set; } = -1;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsProgressIndeterminate))]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand), nameof(RestartCommand))]
        public partial bool IsActive { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsProgressIndeterminate))]
        [NotifyCanExecuteChangedFor(nameof(ShowFileCommand), nameof(CancelCommand), nameof(RestartCommand))]
        public partial DownloadStatus Status { get; set; }


        public  bool IsProgressIndeterminate => IsActive && Status < DownloadStatus.StartedVod;


        public async void Start()
        {
            if (IsActive)
                return;

            IsActive = true;

            try
            {
                Status = DownloadStatus.Enqueued;
                cancellationTokenSource = new CancellationTokenSource();
                using var semaphore = await throttlingSemaphore.AcquireAsync(cancellationTokenSource.Token);

                downloadProgress ??= new(this);
                await downloaderCoreClient.Downloader.StartDownload(s => Status = (DownloadStatus)s, downloadProgress, cancellationTokenSource.Token);
                Status = DownloadStatus.Completed;
            }
            catch (OperationCanceledException) when (cancellationTokenSource!.IsCancellationRequested)
            {
                Status = DownloadStatus.Canceled;
                Log.Info("已经停止下载");
            }
            catch (Exception e)
            {
                Status = DownloadStatus.Failed;
                Log.Error(e);
            }
            finally
            {
                IsActive = false;
                downloadProgress?.Clear();
                cancellationTokenSource?.Dispose();
            }

        }

        private bool CanShowFile => Status == DownloadStatus.Completed;

        [RelayCommand(CanExecute = nameof(CanShowFile))]
        private void ShowFile()
        {
            try
            {
                Process.Start("explorer", $"/select, \"{DownloadParam.VideoFullName}\"");
            }
            catch (Exception)
            {
            }
        }


        public bool CanCancel => IsActive && Status != DownloadStatus.Canceled;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel()
        {
            cancellationTokenSource?.Cancel();
        }

        private bool CanRestart => !IsActive && Status != DownloadStatus.Completed;

        [RelayCommand(CanExecute = nameof(CanRestart))]
        private void Restart() => Start();

        
        public void DeleteCache(bool isDelete = true, bool isShowLog = false)
        {
            if (!isDelete)
                return;

            DirectoryInfo directory = new(DownloadParam.CachePath);
            if (!directory.Exists) return;

            directory.Delete(true);

            if (isShowLog)
                Log.Info("删除{0}目录成功", directory);
        }

    }

    public partial class DownloadViewModel : IEquatable<DownloadViewModel>
    {

        public bool Equals(DownloadViewModel? other) => GetHashCode() == other?.GetHashCode();
        public override bool Equals(object? obj) => obj is DownloadViewModel downloadviewmodel && Equals(downloadviewmodel);
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
//         public static DownloadViewModel CreateDownloadViewModel(
//          string requestUrl,
//          string videoName)
//         {
//             var downloadViewModel = new DownloadViewModel(default!)
//             {
//                 RequestUrl =new Uri(requestUrl),
//                 VideoName = videoName
//             };
//             return downloadViewModel;
//         }
    
        // 
        //         public static DownloadViewModel CreateDownloadViewModel(
        //             IM3uFileInfo m3UFileInfo,
        //             IDownloadParamBase m3U8DownloadParam,
        //             Type? pluginType)
        //         {
        //             DownloadViewModel viewModel = null!;
        //             viewModel.DownloadParam = m3U8DownloadParam;
        //             viewModel.VideoName = m3U8DownloadParam.VideoName;
        // 
        //             viewModel.downloaderCoreClient = new(Http.Client, m3U8DownloadParam, viewModel.settingsService.Clone(), viewModel.Log, pluginType, m3UFileInfo);
        //             return viewModel;
        //         }
        // 
        //         public static DownloadViewModel CreateDownloadViewModel(
        //              IMediaDownloadParam m3U8DownloadParam)
        //         {
        //             DownloadViewModel viewModel = null!;
        //             viewModel.DownloadParam = m3U8DownloadParam;
        //             viewModel.RequestUrl = m3U8DownloadParam.Medias[0].Url;
        //             viewModel.VideoName = m3U8DownloadParam.VideoName;
        // 
        //             viewModel.downloaderCoreClient = new DownloaderCoreClient(Http.Client, m3U8DownloadParam, viewModel.settingsService.Clone(), viewModel.Log);
        //             return viewModel;
        //         }
             }

    }

