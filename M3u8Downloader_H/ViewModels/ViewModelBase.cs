using CommunityToolkit.Mvvm.ComponentModel;

namespace M3u8Downloader_H.ViewModels
{
    public  abstract partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        public partial string Title { get; set; } = string.Empty;
    }
}
