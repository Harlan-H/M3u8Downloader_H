using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Core;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Progress.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels.Downloads
{
    public partial class DownloadViewModel : ViewModelBase
    {
        private readonly ThrottlingSemaphore throttlingSemaphore = ThrottlingSemaphore.Instance;
        private readonly IDownloadParamBase downloadParam;
        private CancellationTokenSource? cancellationTokenSource;

        public DownloaderCoreClient downloaderCoreClient = default!;

        public MyLog Log { get; } 

        public ObservableCollection<LogParams> Logs { get;  } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasMultiMedia))]
        public partial MultiMediaSetupViewModel? MultiMediaSetupViewModel { get; private set; }

        public bool HasMultiMedia => MultiMediaSetupViewModel is not null;

        [ObservableProperty]
        public partial Uri RequestUrl { get; set; } = default!;
        [ObservableProperty]
        public partial string VideoName { get; set; } = default!;

        public ProgressManager ProgressManager { get; set; } 

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsProgressIndeterminate))]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand), nameof(RestartCommand))]
        public partial bool IsActive { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsProgressIndeterminate))]
        [NotifyCanExecuteChangedFor(nameof(ShowFileCommand), nameof(CancelCommand), nameof(RestartCommand))]
        public partial DownloadStatus Status { get; set; }

        public bool IsProgressIndeterminate => IsActive && Status < DownloadStatus.StartedVod;

        public DownloadViewModel(IDownloadParamBase DownloadParam)
        {
            downloadParam = DownloadParam;
            Log = new(Logs);
            ProgressManager = new ProgressManager(status => Status = status);
        }

        public async Task<IList<IM3uFileInfoSource>> GetM3uFileInfoState(IM3uFileInfo m3UFileInfo)
        {
            var audios = m3UFileInfo.Medias?.Where(m => m.Type.ToUpper().Equals("AUDIO")).ToList();
            var subtitles = m3UFileInfo.Medias?.Where(s => s.Type.ToUpper().Equals("SUBTITLES")).ToList();
            MultiMediaSetupViewModel = new MultiMediaSetupViewModel(m3UFileInfo.Streams, audios, subtitles);
            var result = await MultiMediaSetupViewModel.GetM3uFileInfoState();
            MultiMediaSetupViewModel = null;
            return result;
        }

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

                if (downloadParam is IM3u8DownloadParam)
                    Status = DownloadStatus.Parsed;
                
                await downloaderCoreClient.Downloader.StartDownload(ProgressManager, cancellationTokenSource.Token);
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
                ProgressManager.Clear();
                cancellationTokenSource?.Dispose();
            }
        }

        private bool CanShowFile => Status == DownloadStatus.Completed;

        [RelayCommand(CanExecute = nameof(CanShowFile))]
        private void ShowFile()
        {
            try
            {
                Process.ShowFile(downloadParam.VideoFullName);
            }
            catch (Exception)
            {
            }
        }


        public bool CanCancel => IsActive && Status != DownloadStatus.Canceled;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel()
        {
            if (!CanCancel)
                return;

            cancellationTokenSource?.Cancel();
        }

        private bool CanRestart => !IsActive && Status != DownloadStatus.Completed;

        [RelayCommand(CanExecute = nameof(CanRestart))]
        private void Restart() => Start();

        
        public void DeleteCache(bool isDelete = true, bool isShowLog = false)
        {
            if (!isDelete)
                return;

            DirectoryInfo directory = new(downloadParam.CachePath);
            if (!directory.Exists) return;

            directory.Delete(true);

            if (isShowLog)
                Log.Info("删除{0}目录成功", directory);
        }


        public virtual string CopyLog()
        {
            StringBuilder sb = new();
            foreach (var log in Logs)
            {
                sb.Append(log.Time.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.Append(' ');
                sb.Append(log.Message);
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

    }

    public partial class DownloadViewModel : IEquatable<DownloadViewModel>
    {

        public bool Equals(DownloadViewModel? other) => GetHashCode() == other?.GetHashCode();
        public override bool Equals(object? obj) => obj is DownloadViewModel downloadviewmodel && Equals(downloadviewmodel);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(downloadParam.CachePath);
        public static bool operator ==(DownloadViewModel downloadviewmode, DownloadViewModel downloadviewmode1) => downloadviewmode.Equals(downloadviewmode1);
        public static bool operator !=(DownloadViewModel downloadviewmode, DownloadViewModel downloadviewmode1) => !(downloadviewmode == downloadviewmode1);
    }
}

