using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.Core;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using PropertyChanged;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public class MediaConverterViewModel : Screen
    {
        private CancellationTokenSource cancellationTokenSource = default!;
        private readonly SettingsService settingsService;
        private readonly DialogProgress _dialogProgress = default!;
        private MediaDownloadParams _downloadParams = default!;

        public bool IsStart { get; private set; } = false;

        public string VideoFileUrl { get; set; } = default!;

        public string AudioFileUrl { get; set; } = default!;

        public string VideoName { get; set; } = default!;

        public double Progress { get; set; } = default!;
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
            if(!string.IsNullOrEmpty(AudioFileUrl))
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


        public bool CanOnProcess => !IsStart;
        public void OnProcess()
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

        public bool CanOnCancel => IsStart;
        public void OnCancel()
        {
            if (!IsStart)
                return;

            cancellationTokenSource?.Cancel();
        }

        public bool CanOnReset => !IsStart;
        public void OnReset()
        {
            if(IsStart)
                return;

            VideoFileUrl = string.Empty;
            AudioFileUrl = string.Empty;
            VideoName = string.Empty;
            Progress = 0;
        }

    }
}
