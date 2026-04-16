using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using M3u8Downloader_H.FrameWork;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.ViewModels;
using M3u8Downloader_H.ViewModels.Menus;
using M3u8Downloader_H.ViewModels.Windows;
using M3u8Downloader_H.Views;
using M3u8Downloader_H.Views.Menus;
using M3u8Downloader_H.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace M3u8Downloader_H
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider = default!;

        public App()
        {
            var services = new ServiceCollection();

            services.AddSingleton<SettingsService>();
            services.AddSingleton<PluginService>();

            services.AddSingleton<DashboardWindowViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<AdvancedViewModel>();
            services.AddSingleton<SponsorViewModel>();
            services.AddSingleton<AboutViewModel>();
            services.AddSingleton<SettingsViewModel>();

            services.AddSingleton<MainWindowView>();
            services.AddSingleton<AdvancedView>();
            services.AddSingleton<AboutView>();
            services.AddSingleton<SponsorView>();
            services.AddSingleton<M3u8WindowView>();
            services.AddSingleton<MediaWindowView>();

            _serviceProvider = services.BuildServiceProvider();

        }
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = new DashboardWindow
                {
                    DataContext = _serviceProvider.GetRequiredService<DashboardWindowViewModel>()
                };
            }

            DataTemplates.Add(new ViewLocator(_serviceProvider));
            
            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}