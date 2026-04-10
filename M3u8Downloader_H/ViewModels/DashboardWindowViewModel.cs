using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.ViewModels.Menus;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels
{
    
    internal partial class DashboardWindowViewModel : ViewModelBase
    {
        private readonly IServiceProvider serviceProvider;
        private readonly MainWindowViewModel mainWindowViewModel ;
        public string Version { get; } = Program.VersionString;

        [ObservableProperty]
        public partial ViewModelBase CurrentViewModel { get; set; } = default!;

        public DashboardWindowViewModel(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            mainWindowViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
            CurrentViewModel = mainWindowViewModel;
        }


        [RelayCommand]
        private async Task InitializeAsync()
        {
            Debug.WriteLine("InitializeAsync");
            await mainWindowViewModel.InitializeAsync();
        }

        [RelayCommand]
        private  void Closed()
        {
            Debug.WriteLine("ClosedAsync");
        }


        [RelayCommand]
        private void NavigateTo(Type type)
        {
            if (CurrentViewModel.GetType() == type)
                return;

            CurrentViewModel = (ViewModelBase)serviceProvider.GetRequiredService(type);
        }


    }
}
