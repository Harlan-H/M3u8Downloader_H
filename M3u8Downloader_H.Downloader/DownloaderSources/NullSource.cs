using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Downloader.DownloaderSources
{
    internal class NullSource : DownloaderSource
    {
        public NullSource(HttpClient httpClient, IDownloadService? downloadService) : base(downloadService)
        {
        }

        public override Task DownloadAsync(Action<bool> IsLiveAction, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
