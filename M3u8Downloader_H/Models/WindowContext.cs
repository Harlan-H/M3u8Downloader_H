using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Utils;
using System.Net.Http;

namespace M3u8Downloader_H.Models
{
    public class WindowContext(
        ISnackbarMaranger snackbarMaranger)
        : IWindowContext
    {
        public HttpClient HttpClient { get; set; } = Http.Client;
        public ISnackbarMaranger SnackbarMaranger { get; set; } = snackbarMaranger;
        public IAppCommandService AppCommandService { get; set; } = default!;
    }
}
