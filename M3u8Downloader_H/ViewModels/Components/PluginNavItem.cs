using Avalonia.Controls;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Plugin.Models.Context;
using M3u8Downloader_H.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace M3u8Downloader_H.ViewModels.Components
{
    public partial class PluginNavItem(PluginHandle pluginHandle , WindowContext windowContext) : IDisposable
    {
        private Control? _view;
        private bool disposedValue;

        public string Key => pluginHandle.PluginManifest.Key;

        public string Title => pluginHandle.PluginManifest.BasicInfo.Title;

        public Control GetView()
        {
            if (_view != null) 
                return _view;

            ServiceCollection serviceDescriptors = new();
            serviceDescriptors.AddSingleton(windowContext.NotificationService);
            serviceDescriptors.AddSingleton(windowContext.HttpFactory);
            serviceDescriptors.AddSingleton(windowContext.AppCommandService);
            var instance = pluginHandle.LoadUI(serviceDescriptors, windowContext.MemoryCacheSercie);

            instance.ConfigureServices(serviceDescriptors);
            var serviceProvider = serviceDescriptors.BuildServiceProvider();
            var view = Activator.CreateInstance(instance.MainWindowViewType);
            if (view is not UserControl control)
                throw new InvalidOperationException($"ui接口继承有误 不是{nameof(UserControl)}类型");

            control.DataContext = serviceProvider.GetRequiredService(instance.MainWindowViewModelType); 

            _view = control;
            return _view;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    _view = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public static class PluginNavItemExtension
    {
        extension(PluginNavItem)
        {
            public static bool operator ==(PluginNavItem pluginNavItem, PluginHandle pluginHandle) => pluginNavItem.Key.Equals(pluginHandle.PluginManifest.Key);
            public static bool operator !=(PluginNavItem pluginNavItem, PluginHandle pluginHandle) => !(pluginNavItem == pluginHandle);
            public static bool operator ==(PluginHandle pluginHandle, PluginNavItem pluginNavItem) => pluginHandle.PluginManifest.Key.Equals(pluginNavItem.Key);
            public static bool operator !=(PluginHandle pluginHandle, PluginNavItem pluginNavItem) => !(pluginHandle == pluginNavItem);
            public static bool operator ==(PluginNavItem pluginNavItem, PluginNavItem pluginNavItem2) => ReferenceEquals(pluginNavItem,pluginNavItem2);
            public static bool operator !=(PluginNavItem pluginNavItem, PluginNavItem pluginNavItem2) => !(pluginNavItem == pluginNavItem2);
        }
    
    }
      
}
