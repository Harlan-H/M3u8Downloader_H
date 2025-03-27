using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.Services;
using MaterialDesignThemes.Wpf;

namespace M3u8Downloader_H.ViewModels.Menus
{
    public class SettingsViewModel(SettingsService settingService) : Screen
    {
        public ISnackbarMessageQueue MyMessageQueue { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        public bool IsActived { get; private set; }

        public SettingsService SettingsServiceClone { get;private set; } = default!;

        public string[] Formats { get; } = { "默认", "mp4" };

        public BindableCollection<string> PluginKeys { get; } = [];

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            SettingsServiceClone = (SettingsService)settingService.Clone();
            return base.OnActivateAsync(cancellationToken);
        }

        public void OnSubmitSettingInfo(SettingsService obj)
        {
            try
            {
                obj.Validate();
                settingService.CopyFrom(obj);
                settingService.UpdateConcurrentDownloadCount();
                MyMessageQueue.Enqueue("设置已经保存！！！");
            }
            catch(Exception e)
            {
                MyMessageQueue.Enqueue($"提交失败,错误信息:{e.Message}");
            }
        }

        public void OnResetSettingInfo()
        {
            SettingsServiceClone = (SettingsService)settingService.Clone();
            MyMessageQueue.Enqueue("设置已经重置！");
        }

        public bool CanTryConnectProxy => !IsActived;

        public async void TryConnectProxy(string proxy)
        {
            if(string.IsNullOrWhiteSpace(proxy))
            {
                MyMessageQueue.Enqueue("请输入代理地址后,再次点击");
                return;
            }

            IsActived = true;
            try
            {
                using HttpClientHandler clientHandler = new()
                {
                    Proxy = new WebProxy(proxy)
                };
                using HttpClient httpclient = new(clientHandler)
                {
                    Timeout = TimeSpan.FromSeconds(5)
                };

                var statu = await httpclient.GetConnectStatus(new Uri("https://www.google.com"));
               
                MyMessageQueue.Enqueue(statu ? "测试成功,代理正常":"测试失败,代理不可用");
            }
            catch (Exception e)
            {
                MyMessageQueue.Enqueue($"链接失败,{e.Message}");
            }
            finally
            {
                IsActived = false;
            }
        }

    }
}
