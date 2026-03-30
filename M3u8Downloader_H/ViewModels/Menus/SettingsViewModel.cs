using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.FrameWork;
using M3u8Downloader_H.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels.Menus
{
    public partial class SettingsViewModel(SettingsService settingService) : ViewModelBase
    {
        public SnackbarManager Notifications { get; } = new SnackbarManager("SettingsHost", TimeSpan.FromSeconds(5));

        [ObservableProperty]
        public partial bool IsActived { get; private set; }

        [ObservableProperty]
        public partial SettingsService SettingsServiceClone { get; private set; } = settingService.Clone<SettingsService>();

        [ObservableProperty]
        public partial string[] Formats { get; private set; } = { "mp4" };


        [RelayCommand]
        private void SubmitSettingInfo(SettingsService obj)
        {
            try
            {
                settingService.CopyFrom(obj);
                //settingService.UpdateAll();
                Notifications.Notify("设置已经保存！！！");
            }
            catch (Exception e)
            {
                Notifications.Notify($"提交失败,错误信息:{e.Message}");
            }
        }

        [RelayCommand]
        private void ResetSettingInfo()
        {
            SettingsServiceClone = settingService.Clone<SettingsService>();
            Notifications.Notify("设置已经重置！");
        }

        public bool CanTryConnectProxy => !IsActived;

        public async void TryConnectProxy(string proxy)
        {
            if (string.IsNullOrWhiteSpace(proxy))
            {
                Notifications.Notify("请输入代理地址后,再次点击");
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

                Notifications.Notify(statu ? "测试成功,代理正常" : "测试失败,代理不可用");
            }
            catch (Exception e)
            {
                Notifications.Notify($"链接失败,{e.Message}");
            }
            finally
            {
                IsActived = false;
            }
        }
    }
}
