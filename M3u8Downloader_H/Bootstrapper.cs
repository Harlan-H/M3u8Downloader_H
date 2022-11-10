using Stylet;
using StyletIoC;
using System.Net;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.ViewModels;
using M3u8Downloader_H.ViewModels.FrameWork;
using System.Windows.Threading;
using System.Windows;
using System.Net.Http;

namespace M3u8Downloader_H
{
    public class Bootstrapper : Bootstrapper<MainWindowViewModel>
    {
        protected override void OnStart()
        {
            base.OnStart();

            ServicePointManager.DefaultConnectionLimit = 2000;
            //HttpClient.DefaultProxy = new WebProxy();
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            base.ConfigureIoC(builder);

            builder.Bind<SettingsService>().ToSelf().InSingletonScope();
            builder.Bind<DownloadService>().ToSelf().InSingletonScope();
            builder.Bind<SoundService>().ToSelf().InSingletonScope();
            builder.Bind<PluginService>().ToSelf().InSingletonScope();

            builder.Bind<IVIewModelFactory>().ToAbstractFactory();
        }



#if !DEBUG
        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(e);

            MessageBox.Show(e.Exception.Message, "错误详情", MessageBoxButton.OK, MessageBoxImage.Error);
        }
#endif

    }
}
