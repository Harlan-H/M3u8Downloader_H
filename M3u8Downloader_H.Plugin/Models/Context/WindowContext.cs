using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Models;
using Microsoft.Extensions.Caching.Memory;


namespace M3u8Downloader_H.Plugin.Models.Context
{
    public class WindowContext(
        IHttpFactory apiFactory,
        INotificationService snackbarMaranger)
    {
        public IHttpFactory HttpFactory => apiFactory;

        public INotificationService NotificationService { get; set; } = snackbarMaranger;
        public IAppCommandService AppCommandService { get; set; } = default!;

        public IPluginStorage PluginStorageService { get; set; } = default!;

        public IMemoryCache MemoryCacheSercie { get; set; } = default!;

        public WindowContext Clone()
        {
            return new WindowContext(this.HttpFactory, this.NotificationService)
            {
                AppCommandService = AppCommandService,
            };
        }
    }
}
