using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using M3u8Downloader_H.Exceptions;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels.FrameWork;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.RestServer;
using Caliburn.Micro;
using System.Threading;
using PropertyChanged;
using System.Security.Principal;

namespace M3u8Downloader_H.ViewModels
{
    public class MainWindowViewModel(SettingsService settingsService, PluginService pluginService) : Screen
    {
        private readonly SettingsService settingsService = settingsService;
        private readonly HttpListenService httpListenService = HttpListenService.Instance;
        private readonly PluginService pluginService = pluginService;

        public ISnackbarMessageQueue Notifications { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));
        public BindableCollection<DownloadViewModel> Downloads { get; } = [];

        [DoNotNotify]
        public IList<DownloadViewModel> SelectedDownloads { get; set; } = Array.Empty<DownloadViewModel>();
        public VideoDownloadInfo VideoDownloadInfo { get; } = new VideoDownloadInfo();

        public string VersionString => $"v{App.VersionString}";

        public int? HttpServicePort { get; set; }

        public bool IsBusy { get; private set; }

        public bool IsShowDialog { get; private set; }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            DisplayName = $"m3u8视频下载器 by:Harlan";

            _ = Task.Run(() =>
            {
                settingsService.Load();
                settingsService.Validate();

                pluginService.Load();
                WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
                WindowsPrincipal windowsPrincipal = new(windowsIdentity);
                if(windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    for (int i = 65432; i > 1024; i--)
                    {
                        try
                        {
                            httpListenService.Run($"http://+:{i}/");
                            httpListenService.Initialization(ProcessDownload, ProcessDownload);
                            HttpServicePort = i;
                            break;
                        }
                        catch (HttpListenerException)
                        {
                            continue;
                        }
                    }
                }
            }, cancellationToken);
            return base.OnInitializeAsync(cancellationToken);
        }


        public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in Downloads)
                item.OnCancel();

            settingsService.Headers = default!;
            settingsService.Save();
            return base.CanCloseAsync(cancellationToken);
        }


        private void EnqueueDownload(DownloadViewModel download)
        {
            var existingDownloads = Downloads.Where(d =>  d.VideoName == download.VideoName && d.SavePath == download.SavePath ).FirstOrDefault();
            if (existingDownloads is not null)
            {
                Notifications.Enqueue($"{download.VideoName} 已经在下载列表中!");
                return;
            }
           
            download.OnStart();
            Downloads.Insert(0, download);
        }


        public bool CanProcessDownload => !IsBusy;
        public void ProcessDownload(VideoDownloadInfo obj)
        {
            IsBusy = true;   
            try
            {
                (Uri uri,string ext) = obj.Validate();
                if(ext == "txt")
                    HandleTxt(uri);
                else
                    ProcessDownload(uri, obj.VideoName, obj.Method, obj.Key, obj.Iv);
                
                //只有操作成功才会清空
                obj.Reset(settingsService.IsResetAddress, settingsService.IsResetName);
            }
            catch (Exception e)
            {
                Notifications.Enqueue(e.ToString());
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
                    ProcessDownload(new Uri(result[0], UriKind.Absolute), result.Length > 1 ? result[1] : null, null, null,null);
                }
                catch (UriFormatException)
                {
                    Notifications.Enqueue($"{result[0]} 不是正确的地址");
                }
                catch (FileExistsException e)
                {
                    Notifications.Enqueue(e);
                }
            }
        }


        private void ProcessDownload(Uri uri, string? name,string? method,string? key,string? iv, string? savePath = default, string? pluginKey = default!, IEnumerable<KeyValuePair<string,string>>? headers = default)
        {
            string tmpVideoName = PathEx.GenerateFileNameWithoutExtension(uri,name);
            string fileFullPath = Path.Combine(savePath ?? settingsService.SavePath, tmpVideoName);
            FileEx.EnsureFileNotExist(fileFullPath);

            string tmpPluginKey = pluginKey is not null
                                ? pluginKey
                                : string.IsNullOrWhiteSpace(settingsService.PluginKey)
                                ? uri.GetHostName()
                                : settingsService.PluginKey;
            DownloadViewModel download = DownloadViewModel.CreateDownloadViewModel(uri, tmpVideoName,method,key,iv, headers ?? settingsService.Headers, fileFullPath, pluginService[tmpPluginKey]);
            if (download is null) return;

            EnqueueDownload(download);
        }

        private void ProcessDownload(M3UFileInfo m3UFileInfo, string? name, string? savePath = default!, string? pluginKey = default!, IEnumerable<KeyValuePair<string, string>>? headers = default!)
        {
            if (!m3UFileInfo.MediaFiles.Any())
                throw new ArgumentException("m3u8的数据不能为空");

            string tmpVideoName = PathEx.GenerateFileNameWithoutExtension(m3UFileInfo.MediaFiles[0].Uri, name);
            string fileFullPath = Path.Combine(savePath ?? settingsService.SavePath, tmpVideoName);
            FileEx.EnsureFileNotExist(fileFullPath);

            //这里因为不可能有url所以直接通过设置来判别使用某个插件
            DownloadViewModel download = DownloadViewModel.CreateDownloadViewModel(m3UFileInfo, headers, tmpVideoName, fileFullPath, pluginService[pluginKey ?? settingsService.PluginKey]);
            if (download is null) return;

            EnqueueDownload(download);
        }

        public void RestartDownloads()
        {
            foreach (var item in SelectedDownloads)
            {
                item.OnRestart();
            }
        }

        public void StopDownloads()
        {
            foreach (var item in SelectedDownloads)
            {
                item.OnCancel();
            }
        }

        public async void RemoveDownloads()
        {
            DeleteDialogViewModel deleteDialogViewModel = DeleteDialogViewModel.CreateDeleteDialogViewModel(SelectedDownloads.Count);
            DeleteDialogResult? deleteDialogResult = await DialogManager.ShowDialogAsync(deleteDialogViewModel);
            if (deleteDialogResult is null)
                return;

            if (deleteDialogResult.DialogResult == false)
                return;

            foreach (var download in SelectedDownloads)
            {
                download.OnCancel();
                Downloads.Remove(download);
                if (deleteDialogResult.IsDeleteCache)
                     download.DeleteCache();
            }
        }
     

        public void RemoveInactiveDownloads()
        {
            var inactiveDownloads = Downloads.Where(c => !c.IsActive).ToArray();
            Downloads.RemoveRange(inactiveDownloads);
        }

        public void RemoveSuccessDownloads()
        {
            var successDownloads = Downloads.Where(c => c.Status == DownloadStatus.Completed).ToArray();
            Downloads.RemoveRange(successDownloads);
        }

        public void RemoveFailedDownloads()
        {
            var failedDownloads = Downloads.Where(c => c.Status == DownloadStatus.Failed).ToArray();
            Downloads.RemoveRange(failedDownloads);
        }

        public void RestartFailedDownloads()
        {
            var failedDownloads = Downloads.Where(c => c.Status == DownloadStatus.Failed);
            foreach (var failedDownload in failedDownloads)
                failedDownload.OnRestart();
        }

        public void CopyUrl(DownloadViewModel download) => Clipboard.SetText(download.RequestUrl.OriginalString);

        public void CopyTitle(DownloadViewModel download) => Clipboard.SetText(download.VideoName);

        public void CopyFailReason(DownloadViewModel download) => Clipboard.SetText(download.FailReason);

        public void CopyLogs(DownloadViewModel download) => Clipboard.SetText(download.CopyLog());


        public bool CanShowSettings => !IsShowDialog;

        public async void ShowSettings()
        {
            IsShowDialog = true;
            try
            {
                var dialog = new SettingsViewModel(settingsService);
                dialog.PluginKeys.AddRange(pluginService.Keys);
                await DialogManager.ShowDialogAsync(dialog);
            }
            finally
            {
                IsShowDialog = false;
            }
        }

        public async void ShowAbout()
        {
            IsShowDialog = true;
            try
            {
                var dialog = new AboutViewModel();
                await DialogManager.ShowDialogAsync(dialog);
            }
            finally
            {
                IsShowDialog = false;
            }
        }

        public async void ShowSponsor()
        {
            IsShowDialog = true;
            try
            {
                var dialog = new SponsorViewModel();
                await DialogManager.ShowDialogAsync(dialog);
            }
            finally
            {
                IsShowDialog = false;
            }
        }
    }
}
