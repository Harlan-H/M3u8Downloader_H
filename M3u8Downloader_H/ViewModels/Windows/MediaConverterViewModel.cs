using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.Core;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public partial class MediaConverterViewModel : ViewModelBase
    {
        private CancellationTokenSource cancellationTokenSource = default!;
        private readonly SettingsService settingsService;
        private readonly DialogProgress _dialogProgress = default!;
        private MediaDownloadParams _downloadParams = default!;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ProcessCommand),nameof(CancelCommand),nameof(ResetCommand))]
        public partial bool IsStart { get; private set; } = false;

        [ObservableProperty]
        public partial string VideoFileUrl { get; set; } = default!;

        [ObservableProperty]
        public partial string AudioFileUrl { get; set; } = default!;

        [ObservableProperty]
        public partial string VideoName { get; set; } = default!;

        [ObservableProperty]
        public partial double Progress { get; set; } = default!;

        public MyLog Log { get; } = new();

        public MediaConverterViewModel(SettingsService settingsService)
        {
            this.settingsService = settingsService;
            _dialogProgress = new(d => Progress = d);
        }


        private MediaDownloadParams GetMediaDownloadParams()
        {
            if (string.IsNullOrWhiteSpace(VideoFileUrl))
            {
                throw new ArgumentNullException("视频地址不能为空");
            }

            FileInfo fileInfo = new(VideoFileUrl);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("{0}文件不存在", fileInfo.Name);
            }

            Uri? audioUri = null;
            if (!string.IsNullOrEmpty(AudioFileUrl))
            {
                FileInfo audiofileInfo = new(AudioFileUrl);
                if (!audiofileInfo.Exists)
                {
                    throw new FileNotFoundException("{0}文件不存在", audiofileInfo.Name);
                }
                audioUri = new Uri(AudioFileUrl);
            }

            _downloadParams = new(settingsService.SavePath, new Uri(VideoFileUrl), audioUri, VideoName, null);
            VideoName = _downloadParams.VideoName;
            return _downloadParams;
        }


        private bool CanProcess => !IsStart;

        [RelayCommand(CanExecute = nameof(CanProcess))]
        private void Process()
        {

            if (IsStart)
                return;

            IsStart = true;

            _ = Task.Run(async () =>
            {

                try
                {
                    if (_downloadParams is not null)
                    {
                        FileInfo fileInfo = new(_downloadParams.VideoFullName);
                        if (fileInfo.Exists)
                        {
                            Log.Info("{0}文件已经存在", _downloadParams.VideoFullName);
                            return;
                        }
                    }

                    cancellationTokenSource = new();
                    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(20));
                    MediaDownloadParams mediaDownloadParams = GetMediaDownloadParams();

                    DownloaderCoreClient downloaderCoreClient = new(mediaDownloadParams, settingsService, Log);
                    await downloaderCoreClient.Converter.StartMerge(_dialogProgress, cancellationTokenSource.Token);

                }
                catch (OperationCanceledException) when (cancellationTokenSource!.IsCancellationRequested)
                {
                    Log.Info("已经停止转码");
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                finally
                {
                    IsStart = false;
                    cancellationTokenSource.Dispose();
                }
            });

        }

        private bool CanCancel => IsStart;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel()
        {
            cancellationTokenSource?.Cancel();
        }

        private bool CanReset => !IsStart;

        [RelayCommand(CanExecute = nameof(CanReset))]
        private void Reset()
        {
            VideoFileUrl = string.Empty;
            AudioFileUrl = string.Empty;
            VideoName = string.Empty;
            Progress = 0;
        }

        [RelayCommand]
        private async Task CopyLogs()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.Clipboard is not { } provider)
                return;

            await provider.SetTextAsync(Log.CopyLog());
        }
    }
}
