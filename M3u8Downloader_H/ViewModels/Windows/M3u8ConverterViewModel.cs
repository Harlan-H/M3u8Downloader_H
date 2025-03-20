using System;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.ViewModels.Utils;
using PropertyChanged;
using System.Windows.Input;
using System.Diagnostics;
using M3u8Downloader_H.Downloader;
using M3u8Downloader_H.Models;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Combiners;
using M3u8Downloader_H.Services;
using System.Windows;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public class M3u8ConverterViewModel : Screen
    {
        private readonly M3u8FileInfoLocal m3U8FileInfoLocal;
        private readonly SettingsService settingsService;
        private M3UFileInfo _fileInfo = default!;
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
            m3U8FileInfoLocal = new M3u8FileInfoLocal(Log);
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
                m3u8Uri = new(newValue);
                var ext = Path.GetExtension(newValue).Trim('.');
                _fileInfo = m3U8FileInfoLocal.M3UFileReadManager.GetM3u8FileInfo(ext, m3u8Uri);
                Log.Info("读取到{0}个文件数据", _fileInfo.MediaFiles.Count);

                _downloadParams = new M3u8DownloadParams(settingsService, m3u8Uri, VideoName);
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
                    if (!_fileInfo.MediaFiles.Any(m => m.Uri.IsFile))
                    {
                        Log.Warn("ts文件不是本地路径");
                        return;
                    }

                    //解密
                    if (_fileInfo.Key is not null)
                    {
                        DownloaderLocal downloaderLocal = new(Log, _downloadParams)
                        {
                            DialogProgress = _dialogProgress
                        };

                        cancellationTokenSource = new();
                        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));

                        downloaderLocal.M3u8Downloader.Initialization(_fileInfo);
                        await downloaderLocal.M3u8Downloader.DownloadAsync(_fileInfo, cancellationTokenSource.Token);
                    }


                    M3uCombinerClient m3UCombinerClient = new(Log, _downloadParams)
                    {
                        Settings = settingsService,
                        DialogProgress = _dialogProgress
                    };
                    //合并
                    if (_fileInfo.Map is not null)
                    {
                        m3UCombinerClient.M3u8FileMerger.Initialize(_fileInfo);
                        await m3UCombinerClient.M3u8FileMerger.MegerVideoHeader(_fileInfo.Map, cancellationTokenSource.Token);
                        await m3UCombinerClient.M3u8FileMerger.StartMerging(_fileInfo, cancellationTokenSource.Token);
                    }
                    else
                    {
                        await m3UCombinerClient.FFmpeg.ConvertToMp4(_fileInfo, cancellationTokenSource.Token);
                    }

                }
                catch (OperationCanceledException) when (cancellationTokenSource!.IsCancellationRequested)
                {
                    Log.Info("已经停止转码");
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


        public void CopyLogs() => Clipboard.SetText(Log.CopyLog());
    }
}
