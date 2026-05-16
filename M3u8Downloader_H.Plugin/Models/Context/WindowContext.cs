using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Models;


namespace M3u8Downloader_H.Plugin.Models.Context
{
    public class WindowContext(
        IHttpFactory apiFactory,
        INotificationService snackbarMaranger)
        : IWindowContext
    {
        public IHttpFactory ApiFactory => apiFactory;

        public INotificationService NotificationService { get; set; } = snackbarMaranger;
        public IAppCommandService AppCommandService { get; set; } = default!;

        public IPluginStorage PluginStorageService { get; set; } = default!;

        public WindowContext Clone()
        {
            return new WindowContext(this.ApiFactory, this.NotificationService)
            {
                AppCommandService = AppCommandService,
            };
        }
    }
}
