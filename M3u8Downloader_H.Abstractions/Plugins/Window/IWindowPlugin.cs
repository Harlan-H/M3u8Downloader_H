using M3u8Downloader_H.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;


namespace M3u8Downloader_H.Abstractions.Plugins.Window
{
    public interface IWindowPlugin
    {
        Type MainWindowViewType { get; }

        Type MainWindowViewModelType { get;  }

        void ConfigureServices(IServiceCollection services);
    }
}
