using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Caliburn.Micro;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.ViewModels.FrameWork;
using MaterialDesignThemes.Wpf;

namespace M3u8Downloader_H.ViewModels
{
    public class SettingsViewModel(SettingsService settingService) : DialogScreen
    {
        private readonly SettingsService settingService = settingService;
        public ISnackbarMessageQueue MyMessageQueue { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));
        public bool IsActive { get; private set; }
        public bool? Status { get; set; } = default!;

        public SettingsService SettingsServiceClone { get; set; } = (SettingsService)settingService.Clone();

        public string[] Formats { get; } = { "默认", "mp4" };

        public BindableCollection<string> PluginKeys { get; } = [];

        public void OnCloseDialog()
        {
            Close(false);
        }

        public void OnSubmitSettingInfo(SettingsService obj)
        {
            try
            {
                obj.Validate();
                settingService.CopyFrom(obj);
                Close(true);
            }catch(Exception e)
            {
                MyMessageQueue.Enqueue($"提交失败,错误信息:{e.Message}");
            }
        }

        public bool CanTryConnectProxy => !IsActive;

        public async void TryConnectProxy(string proxy)
        {
            if(string.IsNullOrWhiteSpace(proxy))
            {
                MyMessageQueue.Enqueue("请输入代理地址后,再次点击");
                return;
            }    

            IsActive = true;
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
                IsActive = false;
            }
        }

    }
}
