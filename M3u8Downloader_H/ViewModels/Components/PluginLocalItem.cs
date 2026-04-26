using CommunityToolkit.Mvvm.ComponentModel;
using M3u8Downloader_H.Plugin.Services;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels.Components
{
    public partial class PluginLocalItem(PluginService pluginService , PluginHandle pluginHandle) : ObservableObject
    {
        private readonly DebounceDispatcher debounceDispatcher = new();
        public string Title => pluginHandle.PluginManifest.BasicInfo.Title;
        public string Desc => pluginHandle.PluginManifest.BasicInfo.Descriptor;
        public Version Version => pluginHandle.PluginManifest.Release.Version;

        [ObservableProperty]
        public partial string State {  get; set; }

        [ObservableProperty]
        public partial bool IsEnable { get; set; } = pluginHandle.Enable;


        partial void OnIsEnableChanged(bool value)
        {
            _ = debounceDispatcher.DebounceAsync(
                () =>
                {
                    try
                    {
                        if (value)
                        {
                            pluginService.Enable(pluginHandle);
                            State = "启动成功";
                        }
                        else
                        {
                            pluginService.Disable(pluginHandle);
                            State = "禁用成功";
                        }
                    }
                    catch (Exception ex)
                    {
                        State = ex.Message;
                    }

                }
                , 2000);
        }
    }
}
