using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Services;

namespace M3u8Downloader_H.ViewModels
{
    internal class MediaDownloadViewModel(SettingsService settingsService, SoundService soundService) : DownloadViewModel(settingsService, soundService)
    {
        protected override Task StartDownload(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
