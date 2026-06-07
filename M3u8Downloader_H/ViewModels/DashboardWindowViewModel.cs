using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using M3u8Downloader_H.ViewModels.Menus;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels
{
    
    internal partial class DashboardWindowViewModel : ViewModelBase
    {
        private readonly IServiceProvider serviceProvider;
        private readonly MainWindowViewModel mainWindowViewModel;
        public string Version { get; } = Program.VersionString;

        [ObservableProperty]
        public partial ViewModelBase CurrentViewModel { get; set; } = default!;

        public DashboardWindowViewModel(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            mainWindowViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
            CurrentViewModel = mainWindowViewModel;
        }

        public override Task InitializeAsync()
        {
            if(_initialized)
                return Task.CompletedTask;

            NavigateTo(typeof(MainWindowViewModel));
            _initialized = true;
            return base.InitializeAsync();
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
