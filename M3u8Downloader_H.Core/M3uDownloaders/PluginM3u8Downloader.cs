using System.IO;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Core.Utils.Extensions;

namespace M3u8Downloader_H.Core.M3uDownloaders
{
    internal class PluginM3u8Downloader : M3u8Downloader
    {
        private readonly IDownloadService _pluginDownload = default!;
        private readonly M3UFileInfo m3UFileInfo = default!;

        public PluginM3u8Downloader(IDownloadService downloadService,  M3UFileInfo m3UFileInfo) : base()
        {
            this.m3UFileInfo = m3UFileInfo;
            _pluginDownload = downloadService;
        }

        public override async ValueTask Initialization(CancellationToken cancellationToken)
        {
            using var tokenSource = cancellationToken.CancelTimeOut(TimeOut);
            await _pluginDownload.Initialize(HttpClient,Headers, m3UFileInfo, tokenSource.Token);
        }

        protected override Stream DownloadAfter(Stream stream, string contentType, CancellationToken cancellationToken)
        {
            return _pluginDownload.HandleData(stream, cancellationToken);
        }

    }
}
