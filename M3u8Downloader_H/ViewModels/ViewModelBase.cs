using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels
{
    public  abstract partial class ViewModelBase : ObservableObject
    {
        protected bool _initialized = false;

        [ObservableProperty]
        public partial string Title { get; set; } = string.Empty;

        public virtual Task InitializeAsync() => Task.CompletedTask;

    }
}
