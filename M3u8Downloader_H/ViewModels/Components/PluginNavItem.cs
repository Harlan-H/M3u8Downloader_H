using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Plugin.Services;
using System;

namespace M3u8Downloader_H.ViewModels.Components
{
    public partial class PluginNavItem(PluginHandle pluginHandle) : ObservableObject
    {
        private Control? _view;

        [ObservableProperty]
        public partial string Title { get; set; } = pluginHandle.PluginManifest.Title;

        public Control GetView(IWindowContext windowContext)
        {
            if (_view != null) 
                return _view;

            var uiInstance = pluginHandle.LoadUI();
            uiInstance.InitializeWindow(windowContext);
            _view = uiInstance.CreateMainView();
            return _view;
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
