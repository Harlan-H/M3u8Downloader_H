using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.ViewModels.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public partial class LocalPluginViewModel : ViewModelBase
    {
        private readonly PluginService pluginService;

        public ObservableCollection<PluginLocalItem> PluginItems { get; } = [];

        public LocalPluginViewModel(PluginService pluginService)
        {
            this.pluginService = pluginService;
            InitPluginItem();
        }

        public void InitPluginItem()
        {
            foreach (var item in pluginService.GetAllPlugins)
            {
                PluginItems.Add(new PluginLocalItem(pluginService, item));
            }
        }


        [RelayCommand]
        private void DelItem(PluginLocalItem item)
        {
            PluginItems.Remove(item);
        }
    }
}
