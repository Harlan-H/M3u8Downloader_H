using M3u8Downloader_H.Abstractions.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Abstractions.Models
{
    public interface IWindowContext
    {
        IHttpFactory ApiFactory { get; }
        INotificationService NotificationService { get; }
        IAppCommandService AppCommandService { get; }
        IPluginStorage PluginStorageService { get; }
    }
}
