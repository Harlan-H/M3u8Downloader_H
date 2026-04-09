using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace M3u8Downloader_H.ViewModels.Menus
{
    public partial class AboutViewModel : ViewModelBase
    {
        [RelayCommand]
        private void Explore(string str)
        {

            ProcessStartInfo processStartInfo = new(str)
            {
                UseShellExecute = true
            };
            Process.Start(processStartInfo);
        }
    }
}
