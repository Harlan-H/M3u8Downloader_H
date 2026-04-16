using M3u8Downloader_H.Abstractions.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Abstractions.Models
{
    public interface IWindowContext
    {
        HttpClient HttpClient { get; }
        ISnackbarMaranger SnackbarMaranger { get; }

        IAppCommandService AppCommandService { get; }
    }
}
