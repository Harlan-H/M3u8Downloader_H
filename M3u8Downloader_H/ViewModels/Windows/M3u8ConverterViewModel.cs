using System;
using System.IO;
using Caliburn.Micro;
using M3u8Downloader_H.M3U8;
using PropertyChanged;
using M3u8Downloader_H.Models;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Core;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public class M3u8ConverterViewModel : Screen
    {
        private readonly M3u8FileInfoClient m3U8FileInfoClient;
        private readonly SettingsService settingsService;
        private IM3uFileInfo _fileInfo = default!;
        private M3u8DownloadParams _downloadParams = default!;
        private readonly DialogProgress _dialogProgress = default!;
        private CancellationTokenSource cancellationTokenSource = default!;

        public bool IsStart { get; private set; } = false;

        [OnChangedMethod(nameof(OnM3u8FileUrlChanged))]
        public string M3u8FileUrl { get; set; } = default!;
        public string VideoName { get; set; } = default!;
        public string Method { get; set; } = default!;
        public string Key {  get; set; } = default!;
        public string Iv { get; set; } = default!;
        public double Progress  { get; set; } = default!;
        public MyLog Log { get; } = new();

        public M3u8ConverterViewModel(SettingsService settingsService)
        {
            m3U8FileInfoClient = new M3u8FileInfoClient(Log);
            this.settingsService = settingsService;
            _dialogProgress = new(d => Progress = d);
        }

        private void OnM3u8FileUrlChanged(string oldValue, string newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue))
                return;

            if (oldValue == newValue )
            {
                Log.Warn("本次传入的文件和上次传入的一致");
                return;
            }

            FileInfo fileInfo = new(newValue);
            if (!fileInfo.Exists)
            {
                Log.Warn("{0}文件不存在", fileInfo.Name);
                return;
            }

            Uri m3u8Uri;
            try
            {
                Reset();
                Log.Info("开始解析m3u8文件");
                m3u8Uri = new(newValue);
                var ext = Path.GetExtension(newValue).Trim('.');
                _fileInfo = m3U8FileInfoClient.DefaultM3uFileReadManager.GetM3u8FileInfo(ext, m3u8Uri);
                Log.Info("读取到{0}个文件数据", _fileInfo.MediaFiles.Count);

                _downloadParams = new M3u8DownloadParams(m3u8Uri, VideoName, fileInfo.DirectoryName, settingsService.SavePath, "mp4", null);
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
            });

        }

        public bool CanOnCancel => IsStart;
        public void OnCancel() 
        {
            if (!CanOnCancel)
                return;

            cancellationTokenSource?.Cancel();
        }


        private void Reset()
        {
            VideoName = string.Empty;
            Method = "AES-128";
            Key = string.Empty;
            Iv = string.Empty;
            Progress = 0;
        }
    }
}
