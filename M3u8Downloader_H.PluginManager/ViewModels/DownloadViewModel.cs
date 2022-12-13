using Caliburn.Micro;
using System.Net.Http;
using System;
using M3u8Downloader_H.PluginManager.Utils;
using M3u8Downloader_H.PluginManager.Services;
using M3u8Downloader_H.PluginManager.Models;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace M3u8Downloader_H.PluginManager.ViewModels
{
    public partial class DownloadViewModel : PropertyChangedBase
    {
        private readonly PluginService pluginService;
        private CancellationTokenSource? _cancellationTokenSource;
        public string Title { get; set; } = default!;
        public string Version { get; set; } = default!;
        public string AppVersion { get; set; } = default!;
        public string RequiredVersion { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public DownloadStatus Status { get; set; }

        public bool IsActive { get; private set; }

        private string localVersion = string.Empty;
        public string LocalVersion
        {
            get => localVersion;

            set 
            {
                try
                {
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(GlobalData.PluginDirectory, value + ".dll"));
                    localVersion = fileVersionInfo.FileVersion ?? string.Empty;
                }
                catch (FileNotFoundException)
                {
                }
                
            }
        }

        public DownloadViewModel(PluginService pluginService)
        {
            this.pluginService = pluginService;
        }


        public void OnStart()
        {
            if (FileEx.VersionCompareTo(RequiredVersion, AppVersion) > 0)
            {
                Status = DownloadStatus.AppVersionIsLow;
                return;
            }

            if (string.IsNullOrWhiteSpace(LocalVersion))
            {
                Status = DownloadStatus.PluginVersionIsLow;
                return;
            }

            if (FileEx.VersionCompareTo(Version, LocalVersion)  > 0)
            {
                Status = DownloadStatus.PluginVersionIsLow;
                return;
            }

            Status = DownloadStatus.Ok;
        }

        public bool CanOnDownload => !IsActive;

        public async void OnDownload()
        {
            if (IsActive)
                return;
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                IsActive = true;
                Uri uri = new(GlobalData.DownloadBaseUrl, FileName + ".zip");

                Status = DownloadStatus.Downloading;
                var tempPath = Path.GetTempPath();
                var fileFullPath = Path.Combine(tempPath, FileName);
                await pluginService.DownloadAsync(uri, fileFullPath, _cancellationTokenSource.Token);

                ZipFile.ExtractToDirectory(fileFullPath, GlobalData.PluginDirectory, true);
                File.Delete(fileFullPath);

                LocalVersion = FileName;
                Status = DownloadStatus.Ok;
            }
            catch (HttpRequestException)
            {
                Status = DownloadStatus.HttpRequestError;
            }
            catch (DirectoryNotFoundException)
            {
                Status = DownloadStatus.DirectoryNotFoundError;
            }
            catch (Exception)
            {
                Status = DownloadStatus.Error;
            }finally
            {
                IsActive = false;
                _cancellationTokenSource?.Dispose();
            }
        }

        public bool CanOnCancel => IsActive;
        public void OnCancel()
        {
            _cancellationTokenSource?.Dispose();
        }

    }

    public partial class DownloadViewModel
    {
        public static DownloadViewModel Create(PluginInfo pluginInfo,string appVersion)
        {
            DownloadViewModel downloadViewModel = IoC.Get<DownloadViewModel>();
            downloadViewModel.Title = pluginInfo.Title;
            downloadViewModel.Version = pluginInfo.Version;
            downloadViewModel.LocalVersion = pluginInfo.FileName;
            downloadViewModel.RequiredVersion = pluginInfo.RequiredVersion;
            downloadViewModel.FileName = pluginInfo.FileName;
            downloadViewModel.AppVersion = appVersion;

            return downloadViewModel;
        }
    }
}
