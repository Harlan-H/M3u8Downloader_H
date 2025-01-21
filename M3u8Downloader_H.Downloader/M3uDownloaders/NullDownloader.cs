using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.Downloader.M3uDownloaders
{
    internal class NullDownloader(HttpClient httpClient) : DownloaderBase(httpClient)
    {
        public override Task DownloadAsync(M3UFileInfo m3UFileInfo, CancellationToken cancellationToken = default)
        {
            base.DownloadAsync(m3UFileInfo, cancellationToken);

            return Task.CompletedTask;
        }
    }
}
