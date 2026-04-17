using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.FrameWork;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels.Downloads;
using System;
using System.Diagnostics;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public partial class M3u8WindowViewModel(SettingsService settingsService, PluginService pluginService, ViewModelManager viewModelManager, SnackbarManager Notification) : ViewModelBase
    {
        public M3u8DownloadInfo VideoDownloadInfo { get; } = new M3u8DownloadInfo();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ProcessM3u8DownloadCommand))]
        public partial bool IsBusy { get; private set; }

        public Action<DownloadViewModel> EnqueueDownloadAction { get; set; } = default!;

        public bool CanProcessM3u8Download => !IsBusy;

        [RelayCommand(CanExecute = nameof(CanProcessM3u8Download))]
        private void ProcessM3u8Download(M3u8DownloadInfo obj)
        {
            IsBusy = true;
            try
            {
                Uri uri = obj.GetRequestUri();
                M3u8DownloadParams m3U8DownloadParams = new(uri, obj.VideoName, settingsService.SavePath, settingsService.SelectedFormat, settingsService.Headers, obj.Method, obj.Key, obj.Iv);
                ProcessM3u8Download(m3U8DownloadParams, null);

                //只有操作成功才会清空
                obj.Reset(settingsService.IsResetAddress, settingsService.IsResetName);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Notification.Notify(e.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }

        //处理软件界面来的请求
        public void ProcessM3u8Download(IM3u8DownloadParam m3U8DownloadParam, string? pluginKey = default!)
        {
            FileEx.EnsureFileNotExist(m3U8DownloadParam.VideoFullName);

            IDownloadPlugin? downloadPlugin = pluginService[pluginKey ?? m3U8DownloadParam.RequestUrl.GetHostName()];
            DownloadViewModel download = viewModelManager.CreateDownloadViewModel(m3U8DownloadParam, downloadPlugin);
            if (download is null) return;

            EnqueueDownloadAction(download);
        }

        //处理接口过来的请求
        public void ProcessM3u8Download(IDownloadParamBase m3U8DownloadParam, IM3uFileInfo m3UFileInfo, string? pluginKey = default!)
        {
            FileEx.EnsureFileNotExist(m3U8DownloadParam.VideoFullName);

            //这里因为不可能有url所以直接通过设置来判别使用某个插件
            DownloadViewModel download = viewModelManager.CreateDownloadViewModel(m3UFileInfo, m3U8DownloadParam, pluginService[pluginKey]);
            if (download is null) return;

            EnqueueDownloadAction(download);
        }

    }
}
