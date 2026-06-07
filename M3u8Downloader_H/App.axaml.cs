using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using M3u8Downloader_H.FrameWork;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels;
using M3u8Downloader_H.ViewModels.Menus;
using M3u8Downloader_H.ViewModels.Windows;
using M3u8Downloader_H.Views;
using M3u8Downloader_H.Views.Menus;
using M3u8Downloader_H.Views.Windows;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.InteropServices;

namespace M3u8Downloader_H
{
    public partial class App : Application
    {
        private readonly SettingsService settingsService;
        private readonly PluginManager pluginManager;

        private Size? _restoreSize;
        private PosixSignalRegistration? _sigtermRegistration;
        private readonly ServiceProvider _serviceProvider = default!;


        public App()
        {
            var services = new ServiceCollection();

            services.AddSingleton<SettingsService>();
            services.AddSingleton(new PluginManager(() => Http.Instance.GetClient()));
            services.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));

            services.AddSingleton<DashboardWindowViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<PluginManagerViewModel>();
            services.AddSingleton<AdvancedViewModel>();
            services.AddSingleton<LocalPluginViewModel>();
            services.AddSingleton<OnlinePluginViewModel>();
            services.AddSingleton<SponsorViewModel>();
            services.AddSingleton<AboutViewModel>();
            services.AddSingleton<SettingsViewModel>();

            services.AddSingleton<MainWindowView>();
            services.AddSingleton<AdvancedView>();
            services.AddSingleton<PluginManagerView>();
            services.AddSingleton<LocalPluginView>();
            services.AddSingleton<OnlinePluginView>();
            services.AddSingleton<AboutView>();
            services.AddSingleton<SettingsView>();
            services.AddSingleton<SponsorView>();
            services.AddSingleton<M3u8WindowView>();
            services.AddSingleton<MediaWindowView>();

            _serviceProvider = services.BuildServiceProvider();

            pluginManager = _serviceProvider.GetRequiredService<PluginManager>();
            settingsService = _serviceProvider.GetRequiredService<SettingsService>();

        }
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var viewModel = _serviceProvider.GetRequiredService<DashboardWindowViewModel>();
                var dashboardWindow = new DashboardWindow
                {
                    DataContext = viewModel
                };
                desktop.MainWindow = dashboardWindow;
                dashboardWindow.Loaded += async (_, _) => await viewModel.InitializeAsync();

                var env = Environment.GetEnvironmentVariable("APP_MODE");
                if (string.IsNullOrWhiteSpace(env))
                {
                    Desktop_Initialize(dashboardWindow);
                    dashboardWindow.Resized += (_, resizeEvent) => DashboardWindow_Resized(dashboardWindow, resizeEvent);
                    desktop.Exit +=  (_,_) => Desktop_Exit(dashboardWindow);
                }
                else if (!string.IsNullOrWhiteSpace(env) && env.Equals("Docker"))
                {
                    Desktop_Initialize();
                    _sigtermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, async context =>
                    {
                        Desktop_Exit();
                    });
                }
            }

            DataTemplates.Add(new ViewLocator(_serviceProvider));
            
            base.OnFrameworkInitializationCompleted();
        }

        private void DashboardWindow_Resized(Window window, WindowResizedEventArgs e)
        {
            if (window.WindowState != WindowState.Normal)
                return;

            if (e.Reason != WindowResizeReason.User)
                return;

            _restoreSize = e.ClientSize;
        }

        private void Desktop_Initialize(Window window)
        {
            Desktop_Initialize();
            if (settingsService.WindowSettings is not null)
            {
                window.Width = settingsService.WindowSettings.Width;
                window.Height = settingsService.WindowSettings.Height;
                window.WindowState = settingsService.WindowSettings.State;
            }
        }

        private void Desktop_Initialize()
        {
            settingsService.Load();
            pluginManager.Load();
        }

 
        private void Desktop_Exit(Window window)
        {
            if(_restoreSize is Size restoreSize)
            {
                settingsService.WindowSettings = new WindowSettings(restoreSize.Width, restoreSize.Height, window.WindowState);
            }
            Desktop_Exit();
        }

        private void Desktop_Exit()
        {
            settingsService.Save();
            pluginManager.Save();
        }
    }
}