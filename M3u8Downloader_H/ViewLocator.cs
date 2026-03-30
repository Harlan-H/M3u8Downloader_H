using Avalonia.Controls;
using Avalonia.Controls.Templates;
using M3u8Downloader_H.ViewModels;
using M3u8Downloader_H.ViewModels.Dialogs;
using M3u8Downloader_H.ViewModels.Downloads;
using M3u8Downloader_H.ViewModels.Menus;
using M3u8Downloader_H.ViewModels.Windows;
using M3u8Downloader_H.Views.Dialogs;
using M3u8Downloader_H.Views.Downloads;
using M3u8Downloader_H.Views.Menus;
using M3u8Downloader_H.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace M3u8Downloader_H
{
    /// <summary>
    /// Given a view model, returns the corresponding view if possible.
    /// </summary>
    public class ViewLocator(IServiceProvider serviceProvider) : IDataTemplate
    {
        public Control? Build(object? param)
        {
            return param switch
            {
                MainWindowViewModel => serviceProvider.GetRequiredService<MainWindowView>(),
                AboutViewModel => serviceProvider.GetRequiredService<AboutView>(),
                ConverterViewModel => serviceProvider.GetRequiredService<ConverterView>(),
                SettingsViewModel => new SettingsView(),
                SponsorViewModel => serviceProvider.GetRequiredService<SponsorView>(),
                M3u8WindowViewModel => serviceProvider.GetRequiredService<M3u8WindowView>(),
                MediaWindowViewModel => serviceProvider.GetRequiredService<MediaWindowView>(),
                M3u8ConverterViewModel => new M3u8ConverterView(),
                MediaConverterViewModel => new MediaConverterView(),
                DownloadViewModel =>  new DownloadView(),
                DeleteDialogViewModel => new DeleteDialogView(),
                _ => new TextBlock { Text = $"No view for {param?.GetType().Name}" }
            };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
