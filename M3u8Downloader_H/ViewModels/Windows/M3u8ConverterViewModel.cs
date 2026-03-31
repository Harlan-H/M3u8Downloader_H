using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.Core;
using M3u8Downloader_H.M3U8;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public partial class M3u8ConverterViewModel : ViewModelBase
    {
        private readonly M3u8FileInfoClient m3U8FileInfoClient;
        private readonly SettingsService settingsService;
        private IM3uFileInfo _fileInfo = default!;
        private M3u8DownloadParams _downloadParams = default!;
        private readonly DialogProgress _dialogProgress = default!;
        private CancellationTokenSource cancellationTokenSource = default!;

        [ObservableProperty]
        public partial string[] MethodArr { get; private set; } = ["AES-128", "AES-196", "AES-256"];

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ProcessCommand),nameof(CancelCommand),nameof(ResetCommand))]
        public partial bool IsStart { get; private set; } = false;

        [ObservableProperty]
        public partial string M3u8FileUrl { get; set; } = default!;

        [ObservableProperty]
        public partial string VideoName { get; set; } = default!;

        [ObservableProperty]
        public partial string Method { get; set; } = default!;

        [ObservableProperty]
        public partial string Key { get; set; } = default!;

        [ObservableProperty]
        public partial string Iv { get; set; } = default!;

        [ObservableProperty]
        public partial double Progress { get; set; } = default!;

        public  MyLog Log { get; } = new();

        public M3u8ConverterViewModel(SettingsService settingsService)
        {
            m3U8FileInfoClient = new M3u8FileInfoClient(Log);
            this.settingsService = settingsService;
            _dialogProgress = new(d => Progress = d);
        }


        partial void OnM3u8FileUrlChanged(string oldValue, string newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue))
                return;

            if (oldValue == newValue)
            {
                Log.Warn("本次传入的文件和上次传入的一致");
                return;
            }

            FileAttributes fileAttributes = File.GetAttributes(newValue);
            if ((fileAttributes & FileAttributes.Directory) > 0 && !Directory.Exists(newValue))
            {
                Log.Warn("{0}文件夹不存在", newValue);
                return;
            }else if((fileAttributes & FileAttributes.Archive) > 0 && !File.Exists(newValue))
            {
                Log.Warn("{0}文件夹不存在", newValue);
                return;
            }

            Uri m3u8Uri;
            try
            {
                ResetInternal();
                Log.Info("开始解析m3u8文件");
                m3u8Uri = new(newValue);
                var ext = Path.GetExtension(newValue).Trim('.');
                _fileInfo = m3U8FileInfoClient.DefaultM3uFileReadManager.GetM3u8FileInfo(ext, m3u8Uri);
                Log.Info("读取到{0}个文件数据", _fileInfo.MediaFiles.Count);

                _downloadParams = new M3u8DownloadParams(m3u8Uri, VideoName, settingsService.SavePath, "mp4", null);
                VideoName = _downloadParams.VideoName;
                Log.Info("生成视频名称:{0}", VideoName);

                if (_fileInfo.IsCrypted)
                {
                    Method = _fileInfo.Key.Method;
                    Log.Info("文件加密方式:{0}", Method);
                    Iv = _fileInfo.Key.IV.Length == 0 ? "" : Convert.ToHexString(_fileInfo.Key.IV);
                    Log.Info("文件加密iv:{0}", string.IsNullOrWhiteSpace(Iv) ? "0" : Iv);
                    Log.Info("密钥无法读取,请自行填写,如果不清楚直接使用base64方式");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return;
            }
        }

 

        private bool CanProcess => !IsStart;

        [RelayCommand(CanExecute = nameof(CanProcess))]
        private async Task Process()
        {
            if (IsStart)
            {
                return;
            }

            IsStart = true;

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

                _downloadParams!.VideoName = VideoName;
                if (!string.IsNullOrEmpty(Key))
                    _downloadParams.UpdateKeyInfo(Method, Key, Iv);

                cancellationTokenSource = new();
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(20));

                DownloaderCoreClient downloaderCoreClient = new(_fileInfo, _downloadParams, settingsService, Log);
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
        }

        private bool CanCancel => IsStart;

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel()
        {
            if (!CanCancel)
                return;

            cancellationTokenSource?.Cancel();
        }

        private bool CanReset => !IsStart;

        [RelayCommand(CanExecute = nameof(CanReset))]
        private void Reset()
        {
            M3u8FileUrl = string.Empty;
            ResetInternal();
        }


        [RelayCommand]
        private async Task CopyLogs()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.Clipboard is not { } provider)
                return;

            await provider.SetTextAsync(Log.CopyLog());
        }

        public void ResetInternal()
        {
            VideoName = string.Empty;
            Method = string.Empty;
            Key = string.Empty;
            Iv = string.Empty;
            Progress = 0;
        }
    }
}
