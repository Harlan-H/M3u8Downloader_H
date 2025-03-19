using System;
using System.Linq;
using Caliburn.Micro;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.Abstractions.Extensions;
using MaterialDesignThemes.Wpf;
using M3u8Downloader_H.Exceptions;
using System.IO;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public class M3u8WindowViewModel  : Screen
    {
        private readonly SettingsService settingsService;
        private readonly PluginService pluginService;
        private readonly ISnackbarMessageQueue notifications;

        public M3u8DownloadInfo VideoDownloadInfo { get; } = new M3u8DownloadInfo();
        public bool IsBusy { get; private set; }

        public Action<DownloadViewModel> EnqueueDownloadAction { get; set; } = default!;

        public M3u8WindowViewModel(SettingsService settingsService, PluginService pluginService, ISnackbarMessageQueue Notifications)
        {
            VideoDownloadInfo.HandleTextAction = HandleTxt;
            VideoDownloadInfo.NormalProcessDownloadAction = ProcessM3u8Download;
            this.settingsService = settingsService;
            this.pluginService = pluginService;
            notifications = Notifications;
        }

        public bool CanProcessM3u8Download => !IsBusy;
        public void ProcessM3u8Download(M3u8DownloadInfo obj)
        {
            IsBusy = true;
            try
            {
                obj.DoProcess(settingsService);

                //只有操作成功才会清空
                obj.Reset(settingsService.IsResetAddress, settingsService.IsResetName);
            }
            catch (Exception e)
            {
                notifications.Enqueue(e.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }


        public bool CanProcessMediaDownload => !IsBusy;
        public void ProcessMediaDownload(MediaDownloadInfo mediaDownloadInfo)
        {
            IsBusy = true;
            try
            {
                mediaDownloadInfo.DoProcess(settingsService);

                mediaDownloadInfo.Reset(settingsService.IsResetAddress, settingsService.IsResetName);
            }
            catch (Exception e)
            {
                notifications.Enqueue(e.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void HandleTxt(Uri uri)
        {
            foreach (var item in File.ReadLines(uri.OriginalString))
            {
                if (string.IsNullOrWhiteSpace(item)) continue;

                string[] result = item.Trim().Split(settingsService.Separator, 2);
                try
                {
                    M3u8DownloadParams m3U8DownloadParams = new(settingsService, new Uri(result[0], UriKind.Absolute), result.Length > 1 ? result[1] : null);
                    ProcessM3u8Download(m3U8DownloadParams);
                }
                catch (UriFormatException)
                {
                    notifications.Enqueue($"{result[0]} 不是正确的地址");
                    break;
                }
                catch (FileExistsException e)
                {
                    notifications.Enqueue(e);
                    break;
                }
            }
        }


        public void ProcessM3u8Download(IM3u8DownloadParam m3U8DownloadParam, string? pluginKey = default!)
        {
            ProcessM3u8Download(new M3u8DownloadParams(settingsService, m3U8DownloadParam), pluginKey);
        }


        private void ProcessM3u8Download(M3u8DownloadParams m3U8DownloadParam, string? pluginKey = default!)
        {
            FileEx.EnsureFileNotExist(m3U8DownloadParam.GetCachePath());

            string tmpPluginKey = pluginKey is not null
                                ? pluginKey
                                : string.IsNullOrWhiteSpace(settingsService.PluginKey)
                                ? m3U8DownloadParam.RequestUrl.GetHostName()
                                : settingsService.PluginKey;
            DownloadViewModel download = M3u8DownloadViewModel.CreateDownloadViewModel(m3U8DownloadParam, pluginService[tmpPluginKey]);
            if (download is null) return;

            EnqueueDownloadAction(download);
        }

        public void ProcessM3u8Download(IM3u8FileInfoDownloadParam m3U8DownloadParam, string? pluginKey = default!)
        {
            if (!m3U8DownloadParam.M3UFileInfos.MediaFiles.Any())
                throw new ArgumentException("m3u8的数据不能为空");

            M3u8DownloadParams m3U8DownloadParams = new(settingsService, m3U8DownloadParam.M3UFileInfos.MediaFiles[0].Uri, m3U8DownloadParam);
            FileEx.EnsureFileNotExist(m3U8DownloadParam.GetCachePath());

            //这里因为不可能有url所以直接通过设置来判别使用某个插件
            DownloadViewModel download = M3u8DownloadViewModel.CreateDownloadViewModel(m3U8DownloadParam.M3UFileInfos, m3U8DownloadParams, pluginService[pluginKey ?? settingsService.PluginKey]);
            if (download is null) return;

            EnqueueDownloadAction(download);
        }

    }
}
