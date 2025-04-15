using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using M3u8Downloader_H.ViewModels.Menus;

namespace M3u8Downloader_H.ViewModels
{
    public class DashboardViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public string VersionString => $"v{App.VersionString}";
        public DashboardViewModel()
        {
            
        }

        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            await NavigateTo("mainwindow");
            await base.OnInitializeAsync(cancellationToken);
        }

        public async Task NavigateTo(string viewmodel) 
        {
            if (string.IsNullOrEmpty(viewmodel))
                return;

            Screen sreen = default!;
            switch (viewmodel)
            {
                case "mainwindow":
                    sreen = IoC.Get<MainWindowViewModel>();
                    break;
                case "converterwindow":
                    sreen = IoC.Get<ConverterViewModel>();
                    break;
                case "settingwindow":
                    sreen = IoC.Get<SettingsViewModel>();
                    break;
                case "sponsorwindow":
                    sreen = IoC.Get<SponsorViewModel>();
                    break;
                case "aboutwindow":
                    sreen = IoC.Get<AboutViewModel>();
                    break;
            }
            await ActivateItemAsync(sreen);
        }
    }
}
