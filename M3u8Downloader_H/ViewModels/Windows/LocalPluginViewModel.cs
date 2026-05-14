using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.ViewModels.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public partial class LocalPluginViewModel : ViewModelBase
    {
        private readonly PluginManager pluginManager;

        public ObservableCollection<PluginLocalItem> PluginItems { get; } = [];

        public LocalPluginViewModel(PluginManager pluginManager)
        {
            this.pluginManager = pluginManager;
            InitPluginItem();
        }

        public void InitPluginItem()
        {
            foreach (var item in pluginManager.AllPlugins)
            {
                PluginItems.Add(new PluginLocalItem(pluginManager, item));
            }
        }


        [RelayCommand]
        private void DelItem(PluginLocalItem item)
        {
            PluginItems.Remove(item);
            item.DeletePlugin();
        }
    }
}
