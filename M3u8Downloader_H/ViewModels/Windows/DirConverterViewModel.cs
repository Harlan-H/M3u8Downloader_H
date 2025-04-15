using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Core;
using M3u8Downloader_H.M3U8;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using PropertyChanged;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public class DirConverterViewModel : Screen
    {
        private CancellationTokenSource? cancellationTokenSource = default!;
        private readonly M3u8FileInfoClient m3U8FileInfoClient;
        private M3u8DownloadParams _downloadParams = default!;
        private readonly DialogProgress _dialogProgress = default!;
        private readonly SettingsService settingsService;
        private IM3uFileInfo _m3u8FileInfo = default!;

        public bool IsStart { get; private set; } = false;

        [OnChangedMethod(nameof(OnM3u8DirUrlChanged))]
        public string M3u8DirUrl { get; set; } = default!;
        public string VideoName { get; set; } = default!;
        public string Method { get; set; } = default!;
        public string Key { get; set; } = default!;
        public string Iv { get; set; } = default!;
        public double Progress { get; set; } = default!;
        public MyLog Log { get; } = new();

        public BindableCollection<IM3uMediaInfo> MediaItems { get; set; } = [];

        public DirConverterViewModel(SettingsService settingsService)
        {
            m3U8FileInfoClient = new M3u8FileInfoClient(Log);
            _dialogProgress = new(d => Progress = d);
            this.settingsService = settingsService;
        }

        private void OnM3u8DirUrlChanged(string oldValue, string newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue))
                return;

            if (oldValue == newValue)
            {
                Log.Warn("本次传入的文件和上次传入的一致");
                return;
            }

            DirectoryInfo directoryInfo = new(newValue);
            if (!directoryInfo.Exists)
            {
                Log.Warn("{0}文件不存在", directoryInfo.Name);
                return;
            }

            
            Uri m3u8Uri;
            try
            {
                ResetInternal();

                m3u8Uri = new(newValue);
                var ext = Path.GetExtension(newValue).Trim('.');
                _m3u8FileInfo = m3U8FileInfoClient.DefaultM3uFileReadManager.GetM3u8FileInfo(ext, m3u8Uri);
                MediaItems.AddRange(_m3u8FileInfo.MediaFiles);
                Log.Info("读取到{0}个文件数据", _m3u8FileInfo.MediaFiles.Count);

                _downloadParams = new M3u8DownloadParams(m3u8Uri, VideoName,  settingsService.SavePath,"mp4",null);
                VideoName = _downloadParams.VideoName;
                Log.Info("生成视频名称:{0}", VideoName);

                if (_m3u8FileInfo.Key is not null)
                {
                    Method = _m3u8FileInfo.Key.Method;
                    Log.Info("文件加密方式:{0}", Method);
                    Iv = _m3u8FileInfo.Key.IV.Length == 0 ? "" : Convert.ToHexString(_m3u8FileInfo.Key.IV);
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

        public bool CanOnProcess => !IsStart;
        public void OnProcess()
        {
            if (IsStart)
            {
                return;
            }

            IsStart = true;

            _ = Task.Run(async () =>
            {
                try
                {
                    _downloadParams.VideoName = VideoName;
                    if (!string.IsNullOrEmpty(Key))
                        _downloadParams.UpdateKeyInfo(Method, Key, Iv);

                    M3UFileInfo m3UFileInfo = (M3UFileInfo)_m3u8FileInfo;
                    m3UFileInfo.MediaFiles = MediaItems;

                    cancellationTokenSource = new();
                    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(20));

                    DownloaderCoreClient downloaderCoreClient = new(m3UFileInfo, _downloadParams, settingsService, Log);
                    await downloaderCoreClient.Converter.StartMerge(_dialogProgress, cancellationTokenSource.Token);
                    _dialogProgress.Report(1.0);
                }
                catch (OperationCanceledException) when (cancellationTokenSource!.IsCancellationRequested)
                {
                    Log.Info("已经停止转码");
                }
                finally
                {
                    IsStart = false;
                    cancellationTokenSource?.Dispose();
                }
            });
        }


        public bool CanOnCancel => IsStart;
        public void OnCancel()
        {
            if (!CanOnCancel)
                return;

            cancellationTokenSource?.Cancel();
        }


        public void OnDelete(IM3uMediaInfo m3UMediaInfo)
        {
            MediaItems.Remove(m3UMediaInfo);
        }

        public bool CanOnReset => !IsStart;
        public void OnReset()
        {
            M3u8DirUrl = string.Empty;
            ResetInternal();
        }

        public void ResetInternal()
        {
            VideoName = string.Empty;
            Method = "AES-128";
            Key = string.Empty;
            Iv = string.Empty;
            Progress = 0;
            MediaItems.Clear();
        }
    }
}
