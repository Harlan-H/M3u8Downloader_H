using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.ViewModels.FrameWork;

namespace M3u8Downloader_H.ViewModels
{
    public class SettingsViewModel : DialogScreen
    {
        private readonly SettingsService settingService;
        public bool IsActive { get; private set; }
        public bool? Status { get; set; } = default!;

        public SettingsService SettingsServiceClone { get; set; }

        public string[] Formats { get; } = { "默认", "mp4" };

        public IEnumerable<string> PluginKeys { get; set; } = Array.Empty<string>();

        public SettingsViewModel(SettingsService settingService)
        {
            this.settingService = settingService;
            SettingsServiceClone = (SettingsService)settingService.Clone();
        }

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
            }catch(Exception)
            {
                Status = false;
            }
        }

        public bool CanTryConnectProxy => !IsActive;

        public async void TryConnectProxy(string proxy)
        {
            if (Status is not null)
            {
                Status = null;
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

                Status = await httpclient.GetConnectStatus(new Uri("https://www.google.com"));
            }
            catch (Exception)
            {
                Status = false;
            }
            finally
            {
                IsActive = false;
            }
        }

    }
}
