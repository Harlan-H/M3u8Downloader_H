using M3u8Downloader_H.Models;
using M3u8Downloader_H.ViewModels.Components;
using Material.Styles.Themes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public class OnlinePluginViewModel : ViewModelBase
    {
        public ObservableCollection<PluginOnlineItem> PluginOnlineItems { get; } = [];
        public LoadState State { get; set; } = LoadState.NotLoaded;

        public async Task EnsureLoadedAsync()
        {
            if (State != LoadState.NotLoaded)
                return;

            State = LoadState.Loading;

            try
            {
                 await Task.Delay(1000);/* _service.GetOnlineAsync();*/

//                 foreach (var item in data)
//                     PluginOnlineItems.Add(item);

                State = LoadState.Loaded;
            }
            catch
            {
                State = LoadState.Failed;
            }
        }
    }
}
