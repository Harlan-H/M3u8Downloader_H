using Caliburn.Micro;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.PluginManager.Models;
using M3u8Downloader_H.PluginManager.Utils;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Diagnostics;

namespace M3u8Downloader_H.PluginManager.ViewModels
{
    public class MainWindowViewModel : Screen
    {
        private readonly HttpClient _httpClient = Http.Client;
        public string? Description { get; set; } = default!;
        public BindableCollection<DownloadViewModel> PluginItems { get; } = new BindableCollection<DownloadViewModel>();

        public MainWindowViewModel()
        {
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            DisplayName = "m3u8视频下载器插件管理系统 by:Harlan";
            OnStart(); 
            return base.OnInitializeAsync(cancellationToken);
        }

        private async void OnStart()
        {
            CancellationTokenSource cancellationTokenSource = new();
            var text = await _httpClient.GetStringAsync(GlobalData.DataSetUrl, cancellationTokenSource.Token);
            PluginManagerInfo? pluginManagerInfo = JsonConvert.DeserializeObject<PluginManagerInfo>(text);
            if (pluginManagerInfo is null)
            {
                Description = "获取插件数据失败";
                return;
            }

            var appVersion = FileVersionInfo.GetVersionInfo(GlobalData.AppPath);
            foreach (var item in pluginManagerInfo.Plugins)
            {
                var pluginitem = DownloadViewModel.Create(item, appVersion.FileVersion!);
                pluginitem.OnStart();
                PluginItems.Add(pluginitem);
            }
        }
    }
}
