using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.ViewModels.FrameWork;

namespace M3u8Downloader_H.ViewModels
{
    public class SettingsViewModel : DialogScreen
    {
        private readonly SettingsService settingService;
        public SettingsService SettingsServiceClone { get; set; }

        public string[] Formats { get; } = { "默认", "mp4" };

        public IReadOnlyList<PluginItem> PluginItems { get; set; } = Array.Empty<PluginItem>();

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
            obj.ServiceUpdate();
            settingService.CopyFrom(obj);
            Close(true);
        }

    }
}
