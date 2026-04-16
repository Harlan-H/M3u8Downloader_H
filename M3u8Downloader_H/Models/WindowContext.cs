using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace M3u8Downloader_H.Models
{
    public class WindowContext(HttpClient httpClient,
        ISnackbarMaranger snackbarMaranger)
        : IWindowContext
    {
        public HttpClient HttpClient { get; set; } = httpClient;
        public ISnackbarMaranger SnackbarMaranger { get; set; } = snackbarMaranger;
        public IAppCommandService AppCommandService { get; set; } = default!;
    }
}
