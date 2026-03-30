using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.ViewModels.Menus;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace M3u8Downloader_H.ViewModels
{
    
    internal partial class DashboardWindowViewModel : ViewModelBase
    {
        private readonly IServiceProvider serviceProvider;

        public string Version { get; } = Program.VersionString;

        [ObservableProperty]
        private ViewModelBase _currentViewModel = default!;

        public DashboardWindowViewModel(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            CurrentViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
        }


        [RelayCommand]
        private void InitializeAsync()
        {
            Debug.WriteLine("InitializeAsync");
        }

        [RelayCommand]
        private void ClosedAsync()
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
