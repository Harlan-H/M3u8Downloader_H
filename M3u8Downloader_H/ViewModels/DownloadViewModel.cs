using Stylet;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.M3U8.Infos;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels.FrameWork;
using System.Linq;
using System.Collections.Generic;
using M3u8Downloader_H.Core.Utils.Extensions;
using M3u8Downloader_H.Models;
using System.Text;
using M3u8Downloader_H.Extensions;

namespace M3u8Downloader_H.ViewModels
{
    public class DownloadViewModel : PropertyChangedBase
    {
        private readonly DownloadService downloadService;
        private readonly SettingsService settingsService;
        private readonly SoundService soundService;
        private CancellationTokenSource? cancellationTokenSource;

        private bool IsFirst = true;
        private bool? IsLive;

        public M3UKeyInfo? KeyInfos;
        public string M3u8Content = default!;
        public M3UFileInfo? M3uFileInfo;
        public string VideoFullPath = default!;
        public string VideoFullName = default!;
        public IEnumerable<KeyValuePair<string, string>>? Headers;

        public Uri RequestUrl { get; set; } = default!;

        public string VideoName { get; set; } = default!;

        public double ProgressNum { get; private set; }

        public double RecordDuration { get; private set; }

        public bool IsActive { get; private set; }

        public DownloadStatus Status { get; private set; }

        public bool IsProgressIndeterminate => IsActive && Status < DownloadStatus.StartedVod;

        public string? FailReason { get; private set; } = string.Empty;

        public DownloadViewModel(DownloadService downloadService, SettingsService settingsService, SoundService soundService)
        {
            this.downloadService = downloadService;
            this.settingsService = settingsService;
            this.soundService = soundService;
        }

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
                    cancellationTokenSource = new CancellationTokenSource();

                    Status = DownloadStatus.Parsed;
                    if (IsLive is not null and true)
                    {
                        //当islive不是null同时为真 说明重新开始了  直播只要重新开始 就重新获取数据
                        M3uFileInfo = await DownloadService.GetM3U8FileInfo(RequestUrl, Headers, null, KeyInfos, cancellationTokenSource.Token);
                    }
                    else
                    {
                        M3uFileInfo ??= await DownloadService.GetM3U8FileInfo(RequestUrl, Headers, M3u8Content, KeyInfos, cancellationTokenSource.Token);
                    }

                    VideoFullName = VideoFullPath + (M3uFileInfo.Map is not null ? Path.GetExtension(M3uFileInfo.Map?.Title) : ".ts");
                    Status = DownloadStatus.Enqueued;

                    await DownloadAsync(M3uFileInfo, cancellationTokenSource.Token);

                    await downloadService.ConvertToMp4(VideoFullName, VideoFullName, new Progress<double>(d => ProgressNum = d), cancellationTokenSource.Token);

                    soundService.PlaySuccess();
                    Status = DownloadStatus.Completed;
                }
                catch (OperationCanceledException)
                {
                    Status = DownloadStatus.Canceled;
                }
                catch (Exception e)
                {
                    soundService.PlayError();
                    Status = DownloadStatus.Failed;
                    FailReason = e.ToString();
                }
                finally
                {
                    IsActive = false;
                    cancellationTokenSource?.Dispose();
                }

            });
        }


        private async Task DownloadAsync(M3UFileInfo m3UFileInfo, CancellationToken cancellationToken)
        {
            //判断如果M3UMediaInfo中第一个项的请求地址不是是文件 那么则创建缓存目录
            //当uri是相对路劲的时候 这里会报错
            bool isFile = m3UFileInfo.MediaFiles.Any(m => m.Uri.IsFile);
            if (IsFirst)
            {
                //当第一次合并文件的时候 没有进行过任何的下载操作 设置中的保存路径可能是不存在的
                //所以需要创建
                if (isFile)
                    PathEx.CreateDirectory(settingsService.SavePath, true);
                else
                    PathEx.CreateDirectory(VideoFullPath, settingsService.SkipDirectoryExist);
                IsFirst = false;
            }

            IsLive ??= !m3UFileInfo.IsVod();
            if (!isFile && (bool)IsLive)
            {
                Progress<double> GetProgress()
                {
                    Status = DownloadStatus.StartedLive;
                    return new(d => RecordDuration = d);
                }
                await downloadService.LiveDownloadAsync(RequestUrl, m3UFileInfo, Headers, VideoFullPath, VideoFullName, GetProgress, cancellationToken);
            }
            else
            {
                if (!isFile)
                {
                    Progress<double> GetProgress()
                    {
                        Status = DownloadStatus.StartedVod;
                        return new(d => ProgressNum = d);
                    }
                    await downloadService.DownloadAsync(m3UFileInfo, Headers, VideoFullPath, GetProgress, cancellationToken);
                }

                await downloadService.VideoMerge(m3UFileInfo, VideoFullPath, VideoFullName, isFile);
            }
        }




        public bool CanOnShowFile => Status == DownloadStatus.Completed;
        public void OnShowFile()
        {
            if (!CanOnShowFile)
                return;

            try
            {
                string videoFullName = settingsService.SelectedFormat != "mp4" ? VideoFullName : VideoFullPath + ".mp4";
                Process.Start("explorer", $"/select, \"{videoFullName}\"");
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


        public void DeleteCache()
        {
            DirectoryInfo directory = new(VideoFullPath);
            if (directory.Exists)
                directory.Delete(true);
        }

    }

    public static class DownloadViewModelExtensions
    {
        public static DownloadViewModel CreateDownloadViewModel(
            this IVIewModelFactory factory,
            Uri requesturl,
            string videoname,
            string? method,
            string? key,
            string? iv,
            IEnumerable<KeyValuePair<string, string>>? headers,
            string cachePath)
        {

            DownloadViewModel viewModel = factory.CreateDownloadViewModel();
            viewModel.RequestUrl = requesturl;
            viewModel.Headers = headers;
            viewModel.VideoName = videoname;
            viewModel.VideoFullPath = cachePath;

            if(!string.IsNullOrWhiteSpace(key))
            {
                viewModel.KeyInfos = new M3UKeyInfo()
                {
                    Method = method!,
                    BKey = Encoding.UTF8.GetBytes(key),
                    IV = iv?.ToHex()!,
                };
            }

            return viewModel;
        }


        public static DownloadViewModel CreateDownloadViewModel(
            this IVIewModelFactory factory,
            Uri? requesturl,
            string content,
            IEnumerable<KeyValuePair<string, string>>? headers,
            string cachePath,
            string videoname)
        {
            DownloadViewModel viewModel = factory.CreateDownloadViewModel();
            viewModel.RequestUrl = requesturl!;
            viewModel.M3u8Content = content;
            viewModel.Headers = headers;
            viewModel.VideoName = videoname;
            viewModel.VideoFullPath = cachePath;

            return viewModel;
        }


        //当传入的是M3UFileInfo 此时因为他不是文件或者http地址 没有办法判断具体的缓存目录
        public static DownloadViewModel CreateDownloadViewModel(
            this IVIewModelFactory factory,
            M3UFileInfo m3UFileInfo,
            IEnumerable<KeyValuePair<string, string>>? headers,
            string videoname,
            string videoPath)
        {
            DownloadViewModel viewModel = factory.CreateDownloadViewModel();

            viewModel.M3uFileInfo = m3UFileInfo;
            viewModel.Headers = headers;
            viewModel.VideoName = videoname;
            viewModel.VideoFullPath = videoPath;

            return viewModel;
        }
    }

}
