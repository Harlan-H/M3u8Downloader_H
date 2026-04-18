using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Plugin.Services;
using System;

namespace M3u8Downloader_H.ViewModels.Components
{
    public partial class PluginNavItem(PluginHandle pluginHandle) : IDisposable
    {
        private Control? _view;
        private bool disposedValue;

        public string Title => pluginHandle.PluginManifest.Title;

        public Control GetView(IWindowContext windowContext)
        {
            if (_view != null) 
                return _view;

            var uiInstance = pluginHandle.LoadUI();
            uiInstance.InitializeWindow(windowContext);
            _view = uiInstance.CreateMainView();
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
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public static class PluginNavItemExtension
    {
        extension(PluginNavItem)
        {
            public static bool operator ==(PluginNavItem pluginNavItem, PluginHandle pluginHandle) => pluginNavItem.Title.Equals(pluginHandle.PluginManifest.Title);
            public static bool operator !=(PluginNavItem pluginNavItem, PluginHandle pluginHandle) => !(pluginNavItem == pluginHandle);
            public static bool operator ==(PluginHandle pluginHandle, PluginNavItem pluginNavItem) => pluginHandle.PluginManifest.Title.Equals(pluginNavItem.Title);
            public static bool operator !=(PluginHandle pluginHandle, PluginNavItem pluginNavItem) => !(pluginHandle == pluginNavItem);
            public static bool operator ==(PluginNavItem pluginNavItem, PluginNavItem pluginNavItem2) => ReferenceEquals(pluginNavItem,pluginNavItem2);
            public static bool operator !=(PluginNavItem pluginNavItem, PluginNavItem pluginNavItem2) => !(pluginNavItem == pluginNavItem2);
        }
    
    }
      
}
