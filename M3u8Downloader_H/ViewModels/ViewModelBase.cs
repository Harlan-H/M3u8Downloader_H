using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace M3u8Downloader_H.ViewModels
{
    public  abstract partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        private string _title = string.Empty;

    }
}
