using System;
using System.Windows;
using System.Collections.Generic;
using Caliburn.Micro;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.ViewModels;
using M3u8Downloader_H.ViewModels.Menus;
#if !DEBUG
using System.Windows.Threading;
#endif

namespace M3u8Downloader_H
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer simpleContainer = new();
      //  private readonly Func<Type, DependencyObject, object, Type> defaultLocator = ViewLocator.LocateTypeForModelType;
        public Bootstrapper()
        {
            Initialize();
           // ViewLocator.LocateTypeForModelType = MyLocateTypeForModelType;
        }

        protected override void Configure()
        {
            simpleContainer
                .Singleton<IWindowManager, WindowManager>();

            simpleContainer
                .Singleton<SettingsService>()
                // .Singleton<DownloadService>()
                .Singleton<SoundService>()
                .Singleton<PluginService>()
                .Singleton<DashboardViewModel>()
                .Singleton<SponsorViewModel>()
                .Singleton<AboutViewModel>()
                .Singleton<SettingsViewModel>()
                .Singleton<MainWindowViewModel>()
                .Singleton<ConverterViewModel>()
                .PerRequest<DownloadViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return simpleContainer.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return simpleContainer.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            simpleContainer.BuildUp(instance);
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync<DashboardViewModel>();
        }

        /*private Type MyLocateTypeForModelType(Type modelType,DependencyObject displayLocation,object context)
        {
            var viewType = defaultLocator(modelType, displayLocation, context);
            if (viewType == null && modelType != typeof(object))
            {
                modelType = modelType!.BaseType!;
                viewType = defaultLocator(modelType, displayLocation, context);
            }
            return viewType!;
        }*/


#if !DEBUG
        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(sender, e);

            MessageBox.Show(e.Exception.ToString(), "错误详情", MessageBoxButton.OK, MessageBoxImage.Error);
        }
#endif

    }
}
