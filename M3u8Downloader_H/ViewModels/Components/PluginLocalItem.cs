using CommunityToolkit.Mvvm.ComponentModel;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.Plugin.Services;
using M3u8Downloader_H.Utils;
using System;

namespace M3u8Downloader_H.ViewModels.Components
{
    public partial class PluginLocalItem(PluginManager pluginManager , PluginHandle pluginHandle) : ObservableObject
    {
        private readonly DebounceDispatcher debounceDispatcher = new();

        public string Title => pluginHandle.PluginManifest.BasicInfo.Title;
        public string Desc => pluginHandle.PluginManifest.BasicInfo.Description;
        public Version Version => pluginHandle.PluginManifest.Release.Version;

        [ObservableProperty]
        public partial string State {  get; set; }

        [ObservableProperty]
        public partial bool IsEnable { get; set; } = pluginManager.RegistryClient.IsEnable(pluginHandle.PluginManifest);


        partial void OnIsEnableChanged(bool value)
        {
            _ = debounceDispatcher.DebounceAsync(
                () =>
                {
                    try
                    {
                        if (value)
                        {
                            pluginManager.Enable(pluginHandle);
                            State = "启动成功";
                        }
                        else
                        {
                            pluginManager.Disable(pluginHandle);
                            State = "禁用成功";
                        }
                    }
                    catch (Exception ex)
                    {
                        State = ex.Message;
                    }

                }
                , 1000);
        }

        public void DeletePlugin()
        {
            pluginManager.DelPlugins(pluginHandle);
        }
    }
}
