using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Demo.Plugins.FrameWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Demo.Plugins.ViewModel
{
    public partial class MainWindowViewModel(IWindowContext windowContext)  :IPluginViewModelBase
    {
        [ObservableProperty]
        public partial string ShowText { get; set; } = "this is plugin text";

        [RelayCommand]
        private void TestClick()
        {
            ShowText = "this is test click output text";
            windowContext.SnackbarMaranger.Notify("plugin send");
        }
    }
}
